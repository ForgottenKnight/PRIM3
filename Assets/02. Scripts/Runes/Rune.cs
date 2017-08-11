using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Rune : MonoBehaviour {
	public delegate void SimpleDelegate ();
	UnityEvent onActivateEvents;
	UnityEvent onDeactivateEvents;

	public bool needStay = false;
	public bool moveRune = false;
	public float runeActivationTime = 4.0f;
	protected  float currentTimer = 0.0f;
	protected bool activated = false;
	public Color initColor = new Color(0.03f,0.25f,0.0f);
	public Color FinalColor = new Color(0.15f,1.0f,0.0f);
	private Vector3 m_InitialPosition;
	private Vector3 m_FinalPosition;
	public Transform objectToMove;
	public Transform finalTransform;
    public ParticleSystem partycles;
    public int m_numPlayersStay = 0;


	[HideInInspector]
	public bool finished = false;
	public Image runeImage;

	Material mat;

	// Use this for initialization
	void Start () {
		if (onActivateEvents == null) {
			onActivateEvents = new UnityEvent ();
		}
		mat = GetComponent<Renderer> ().material;
		//mat.color = lightColor;
		mat.SetColor ("_EmissionColor", initColor);
		runeImage.fillAmount = 0;

		if (objectToMove == null) {
			objectToMove = transform;
		}
		m_InitialPosition = objectToMove.position;
		if (finalTransform != null) {
			m_FinalPosition = finalTransform.position;
		} else {
			m_FinalPosition = m_InitialPosition;
		}
	}

	public void RegisterForEvent(SimpleDelegate method) {
		if (onActivateEvents == null) {
			onActivateEvents = new UnityEvent ();
		}
		onActivateEvents.AddListener (new UnityAction(method));
	}

	public void RegisterForEventDeactivate(SimpleDelegate method) {
		if (onDeactivateEvents == null) {
			onDeactivateEvents = new UnityEvent ();
		}
		onDeactivateEvents.AddListener (new UnityAction(method));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" || col.tag == "JumpingPlayer")
        {
            ++m_numPlayersStay;
        }
    }

	void OnTriggerStay(Collider col) 
	{
		if (col.tag == "Player") {
			if (currentTimer >= runeActivationTime) {
				if (!activated) {
					activated = true;
					onActivate();
                    if (partycles)
                    {
                        partycles.Stop();
                    }
				}
			} else {
				currentTimer += Time.deltaTime;
				float l_T = currentTimer / runeActivationTime;
				Color c = Color.Lerp (initColor, FinalColor, l_T);
				if(moveRune)
				{
					objectToMove.position = Vector3.Lerp (m_InitialPosition, m_FinalPosition, l_T);
				}

				//mat.color = c;
				float fill = Mathf.Lerp(0f, 1f, l_T);
				runeImage.fillAmount = fill;
				mat.SetColor ("_EmissionColor", c);
			}
		}
	}

	void onActivate() {
		onActivateEvents.Invoke ();
	}

	void onDeactivate() {
		onDeactivateEvents.Invoke ();
	}

	void OnTriggerExit(Collider col) 
	{
        if (col.tag == "Player" || col.tag == "JumpingPlayer")
        {
            --m_numPlayersStay;
            if (!finished && m_numPlayersStay <= 0)
            {
                if (activated && needStay)
                {
                    activated = false;
                    onDeactivate();
                    currentTimer = 0.0f;
                    runeImage.fillAmount = 0;
                    //mat.color = initColor;
                    mat.SetColor("_EmissionColor", initColor);
                }
                if (!activated || needStay)
                {
                    currentTimer = 0.0f;
                    runeImage.fillAmount = 0;
                    //mat.color = initColor;
                    mat.SetColor("_EmissionColor", initColor);
                }
            }
        }
	}



}
