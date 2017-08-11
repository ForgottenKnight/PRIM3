using UnityEngine;
using System.Collections;

public class DroppedItem : MonoBehaviour {

	public float HealAmmount = 20;

	private Transform m_PlayerContainer;

    [HideInInspector]
    public float m_timer = 0.0f;

	// Use this for initialization
	void Start () {
		m_PlayerContainer = GameObject.FindGameObjectWithTag("PlayerContainer").transform;
	}
	
	// Update is called once per frame
	void Update () {
        m_timer += Time.deltaTime;	
	}

	void OnTriggerEnter(Collider col) 
	{
        if (m_timer >= 1.0f)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                for (int i = 0; i < m_PlayerContainer.childCount; ++i)
                {
                    Health l_PlayerHealth = m_PlayerContainer.GetChild(i).GetComponent<Health>();

                    if (l_PlayerHealth && l_PlayerHealth.health > 0)
                    {
                        l_PlayerHealth.Heal(HealAmmount);
                    }
                }
                Destroy(transform.parent.gameObject);
            }
        }
	}


    void OnTriggerStay(Collider col)
    {
        if (m_timer >= 1.0f)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                for (int i = 0; i < m_PlayerContainer.childCount; ++i)
                {
                    Health l_PlayerHealth = m_PlayerContainer.GetChild(i).GetComponent<Health>();

                    if (l_PlayerHealth && l_PlayerHealth.health > 0)
                    {
                        l_PlayerHealth.Heal(HealAmmount);
                    }
                }
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
