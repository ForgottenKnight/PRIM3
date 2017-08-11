using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class SimpleEvent : MonoBehaviour
{
    [TextArea(1, 10)]
    public string description; // Este parametro solo se usa en el inspector como metodo informativo.
    public UnityEvent ev;
    public enum EventCondition { KillEnemies, ActivateRune, ExternalTrigger, TimePassed, AnyPlayerInArea, AllPlayersInArea }
    public static Dictionary<string, SimpleEvent> eventsDictionary;
    [Header("General parameters")]
    public EventCondition eventCondition = EventCondition.ExternalTrigger;
    public bool destroyEventOnTrigger = true;
    public bool destroyGameObjectOnTrigger = true;
    public string eventName = "id";
    public int resolvedQuantity = 0;
    public int quantityToResolve = 0;
    public int playersInArea = 0;
    private List<int> playersList;

    [Header("Kill enemies parameters")]
    public Damageable[] enemiesList;
    int quantity;
    [Header("Activate runes")]
    public Rune[] runes;
    [Header("Time passed")]
    public float passedTime = 1.0f;
    bool active = false;
    public bool needActivation = true;
    public float timer = 0.0f;


    // Use this for initialization
    void Start()
    {
        playersList = new List<int>();
        if (eventName == "id")
        {
            Debug.LogWarning("No se recomienda dejar el nombre por defecto \"id\" para los eventos. Objeto: " + gameObject.name);
        }
        if (SimpleEvent.eventsDictionary == null)
        {
            SimpleEvent.eventsDictionary = new Dictionary<string, SimpleEvent>();
        }
        if (SimpleEvent.eventsDictionary.ContainsKey(eventName))
        {
            if (!SimpleEvent.eventsDictionary[eventName])
            {
                Debug.LogWarning("Ya existia un evento con el nombre " + eventName + ". El objeto no existia y ha sido borrado del map.");
                SimpleEvent.eventsDictionary.Remove(eventName);
                SimpleEvent.eventsDictionary.Add(eventName, this);
            }
            else
            {
                Debug.LogWarning("Ya existe un evento con el nombre " + eventName + ". Esta en el objeto " + SimpleEvent.eventsDictionary[eventName].gameObject.name);
            }
        }
        else
        {
            SimpleEvent.eventsDictionary.Add(eventName, this);
        }
        if (eventCondition == EventCondition.KillEnemies)
        {
            quantity = enemiesList.Length;
            for (int i = 0; i < quantity; ++i)
            {
                enemiesList[i].RegisterOnDie(OnMonsterKilled);
            }
        }
        if (eventCondition == EventCondition.ActivateRune)
        {
            quantity = runes.Length;
            for (int i = 0; i < quantity; ++i)
            {
                if (runes[i])
                {
                    runes[i].RegisterForEvent(OnRuneActivated);
                    runes[i].RegisterForEventDeactivate(OnRuneDeactivated);
                }
            }
        }
        if (eventCondition == EventCondition.ActivateRune || eventCondition == EventCondition.KillEnemies)
        {
            if (quantityToResolve > 0)
            {
                resolvedQuantity = quantity - quantityToResolve;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (eventCondition == EventCondition.TimePassed)
        {
            if (active || !needActivation)
            {
                timer += Time.deltaTime;
                if (timer >= passedTime)
                {
                    OnEventTriggered();
                    timer = 0.0f;
                    active = false;
                }
            }
        }
    }

    public void OnMonsterKilled()
    {
        resolvedQuantity++;
        CheckQuantity();
    }

    public void OnRuneActivated()
    {
        resolvedQuantity++;
        CheckQuantity();
    }

    public void OnRuneDeactivated()
    {
        resolvedQuantity--;
    }

    void CheckQuantity()
    {
        if (resolvedQuantity >= quantity && (eventCondition == EventCondition.ActivateRune || eventCondition == EventCondition.KillEnemies))
        {
            OnEventTriggered();
            if (eventCondition == EventCondition.ActivateRune)
            {
                for (int i = 0; i < quantity; ++i)
                {
                    runes[i].finished = true;
                }
            }
        }
    }

    public void ExternalTriggerFunction()
    {
        if (eventCondition == EventCondition.ExternalTrigger)
        {
            OnEventTriggered();
        }
    }

    public void ExternalDebugFunction()
    {
        OnEventTriggered();
    }

    public void ActivateTimer()
    {
        active = true;
    }

    void OnEventTriggered()
    {
        if (this != null)
        {
            ev.Invoke();
            if (destroyGameObjectOnTrigger || destroyEventOnTrigger)
            {
                SimpleEvent.eventsDictionary.Remove(eventName);
            }
            if (destroyGameObjectOnTrigger)
            {
                if (gameObject != null)
                {
                    Destroy(gameObject);
                }
            }
            else if (destroyEventOnTrigger)
            {
                Destroy(this);
            }
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Player")
        {
            if (eventCondition == EventCondition.AnyPlayerInArea)
            {
                OnEventTriggered();
            }
            else if (eventCondition == EventCondition.AllPlayersInArea)
            {
                AddPlayerToList(c.gameObject.GetComponent<GeneralPlayerController>().player);
                if (playersInArea == StaticParemeters.numPlayers)
                {
                    OnEventTriggered();
                }

            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Player" && eventCondition == EventCondition.AllPlayersInArea)
        {
            DeletePlayerFromList(c.gameObject.GetComponent<GeneralPlayerController>().player);
        }
    }


    public static void ClearDictionary()
    {
        SimpleEvent.eventsDictionary.Clear();
    }

    bool PlayerInList(int player)
    {
        foreach (int p in playersList)
        {
            if (p == player)
                return true;
        }
        return false;
    }

    void AddPlayerToList(int player)
    {
        bool exists = PlayerInList(player);
        if (!exists)
        {
            playersInArea++;
            playersList.Add(player);
        }
    }

    public void DeletePlayerFromList(int player, bool playerChanged = false)
    {
        bool exists = PlayerInList(player);
        if (exists)
        {
            playersInArea--;
            playersList.Remove(player);
        }
    }

    public static void CallEvent(string aEventName)
    {
        if (SimpleEvent.eventsDictionary.ContainsKey(aEventName))
        {
            SimpleEvent.eventsDictionary[aEventName].ExternalTriggerFunction();
        }
    }

    public static void CallEventBypass(string aEventName)
    {
        if (SimpleEvent.eventsDictionary.ContainsKey(aEventName))
        {
            SimpleEvent.eventsDictionary[aEventName].ExternalDebugFunction();
        }
    }


}