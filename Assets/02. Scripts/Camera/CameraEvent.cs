using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine.Events;

[AddComponentMenu("PRIM3/Camera/Camera Event")]
[RequireComponent(typeof (PrimeCamera))]
public class CameraEvent : MonoBehaviour {
	[TextArea(1,10)]
	public string description; // Este parametro no se usa, es solo para el inspector
	public UnityEvent callback;
	private bool m_CallbackOnce = false;
	public enum TriggerType {AnyPlayer, AllPlayers, PlayersDistance, ExternalTrigger};
	public enum EventType {While, Trigger};
	[Header("Trigger configuration")]
	public EventType eventType;
	public TriggerType triggerType;

	[Header("PlayersDistance parameters")]
	public float maxDistanceBetweenPlayers = 5.0f;

	[Header("Extras")]
	public bool teleportPlayers = false;
	bool alreadyTeleported = false;
	public Transform teleportLocation;
	public bool stopPlayersUntilAll = false;
	bool alreadyStoppedPlayers = false;
	List<PlayerManager> stoppedPlayers;

	[Header("Parametros de la duracion")]
	public bool onlyOnce = false;
	[Tooltip("Un evento cinematico impide que los jugadores se muevan mientras dure")]
	public bool cinematicForPlayers = false;
	public bool cinematicForEnemies = false;
	public float cinematicDuration = 0.0f;

	[Header("Camera Settings")]
	public bool copyTransform = false;
	public bool copyFocus = false;
	public Transform transformToCopy;
	[Tooltip("Copia las opciones de reduccion de velocidad de los jugadores")]
	public bool copySlowDown = false;
	[Tooltip("Copia las velocidades de movimiento del evento a la camara")]
	public bool copySpeeds = true;
	[Tooltip("Copia el objective yaw del evento a la camara")]
	public bool copyYaw = true;
    [Tooltip("Copia el offset para el punto de focus de la cámara")]
    public bool copyOffset = true;
	[Tooltip("Copia los valores actuales de zoom, pitch, roll y yaw del evento a la camara")]
	public bool copyValues = false;
	[Tooltip("Copia las distancias minimas y maximas a considerar del evento a la camara")]
	public bool copyDistance = true;
	[Tooltip("Copia el zoom minimo y maximo del evento a la camara")]
	public bool copyZoom = true;
	[Tooltip("Copia el pitch minimo y maximo del evento a la camara")]
	public bool copyPitch = true;
	[Tooltip("Copia la rotacion de yaw entre dos puntos")]
	public bool copyYawRotation = true;
	[Tooltip("Copia la rotacion de pitch entre dos puntos")]
	public bool copyBeginEndPitchRotation = true;
	[Tooltip("Copia el constraint de rotación en yaw")]
	public bool copyYawConstraint = true;
	[Tooltip("Copia el constraint de rotación en pitch")]
	public bool copyPitchConstraint = true;
	[Tooltip("Copia el constraint de rotación en roll")]
	public bool copyRollConstraint = true;
	[Tooltip("Copia el constraint de movimiento")]
	public bool copyMovementConstraint = true;
	[Tooltip("Copia el constraint de zoom")]
	public bool copyZoomConstraint = true;
	[Tooltip("Copia el tipo de chequeo de posicion de jugadores")]
	public bool copyPlayerCheck = true;
	[Tooltip("Copia la configuracion de un solo jugador")]
	public bool copyOnePlayerLerp = true;
	[Tooltip("Copia las aceleraciones de la camara")]
	public bool copyAcceleration = false;

	Transform players;
    [Header("Opciones dialogo wait")]
    private IEnumerator waitCoroutine;
    private float timeBetweenDialogs = 10f;
    private bool coroutineStarted = false;


	private GameObject primeCamera;
	private int playersInArea = 0;
	private string PROXY_CAM_NAME = "ProxyCam";
	private List<int> playersList;

    WindowManager m_WindowManager;


	// Use this for initialization
	void Start () {
		playersList = new List<int> ();
		stoppedPlayers = new List<PlayerManager> ();
		primeCamera = Camera.main.gameObject;
		players = GameObject.FindGameObjectWithTag ("PlayerContainer").transform;
        m_WindowManager = GameObject.FindObjectOfType<WindowManager>();
        waitCoroutine = WaitDialog();

    }
	
	// Update is called once per frame
	void Update () {

	}

	void AddPlayerToList(int player) {
		bool exists = PlayerInList(player);
		if (!exists) {
			playersInArea++;
			playersList.Add (player);
		}
	}

	public void DeletePlayerFromList(int player, bool playerChanged = false) {
		bool exists = PlayerInList(player);
		if (exists) {
			playersInArea--;
			playersList.Remove (player);
			if (playerChanged && eventType == EventType.While) {
				DoProxy();
			}
		}
	}

	bool PlayerInList(int player) {
		foreach (int p in playersList) {
			if (p == player)
				return true;
		}
		return false;
	}

	bool CheckCondition() {
		bool condition = false;
		switch (triggerType) {
		case TriggerType.AllPlayers:
			if (eventType == EventType.Trigger) {
				condition = AllPlayers();
			} else if (eventType ==EventType.While) {
				condition = AllPlayersInArea();
			}
			break;
		case TriggerType.AnyPlayer:
			if (eventType ==EventType.Trigger) {
				condition = AnyPlayer();
			} else if (eventType ==EventType.While) {
				condition = AnyPlayerInArea();
			}
			break;
		case TriggerType.PlayersDistance:
			condition = PlayersInDistance();
			break;
		default:
			break;
		}
		return condition;
	}

	bool PlayersInDistance() {
		//GameObject [] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (Transform p1 in players) {
			foreach (Transform p2 in players) {
				if (p1.gameObject != p2.gameObject) {
					Vector3 pos1 = p1.position;
					Vector3 pos2 = p2.position;
					if (Vector3.Distance(pos1, pos2) > maxDistanceBetweenPlayers) {
						return false;
					}
				}
			}
		}
		return true;
	}

	bool AnyPlayerInArea() {
		return playersInArea > 0;
	}

	bool AnyPlayer() {
		return playersList.Capacity > 0;
	}

	bool AllPlayersInArea() {
		int maxPlayers = players.childCount;
		return playersInArea >= maxPlayers;
	}

	bool AllPlayers() {
		int maxPlayers = players.childCount;
		return playersList.Count >= maxPlayers;
	}

	bool AllPlayersStopped() {
		int maxPlayers = players.childCount;
		return stoppedPlayers.Count >= maxPlayers;
	}

    /*void CheckStoppedPlayers(GameObject go) {
		if (!alreadyStoppedPlayers && stopPlayersUntilAll) {
			IPausable[] l_Pausables = go.GetComponents<IPausable> ();
			for (int j = 0; j < l_Pausables.Length; ++j) {
				l_Pausables [j].Pause ();
			}
			PlayerManager pm = go.GetComponent<PlayerManager>();
			Movement playerMovement = go.GetComponent<Movement>();
			if (playerMovement) {
				playerMovement.ResetSpeed();
			}
			stoppedPlayers.Add(pm);
			if (AllPlayersStopped ()) {
				foreach (PlayerManager p in stoppedPlayers) {
					IPausable[] l_Pauseds = p.GetComponents<IPausable> ();
					for (int j = 0; j < l_Pausables.Length; ++j) {
						l_Pauseds [j].Unpause ();
					}
				}
				alreadyStoppedPlayers = true;
			} else if (stoppedPlayers.Count == 1) {
                //TextEventsLibrary.ShowTextEvent("StoppedPlayers");
                //GameObject l_windowManager = GameObject.Find("WindowManager");
                //l_windowManager.GetComponent<WindowManager>().OpenWindow("Dialog_StoppedPlayers");
                m_WindowManager.OpenWindow("Dialog_StoppedPlayers");

            }
		}
	}*/

    void CheckStoppedPlayers(GameObject go)
    {
        if (!alreadyStoppedPlayers && stopPlayersUntilAll)
        {
            PlayerManager pm = go.GetComponent<PlayerManager>();
            pm.eventBoundingBox = GetComponent<Collider>().bounds;
            pm.insideBoundingBox = true;
            Movement playerMovement = go.GetComponent<Movement>();
            if (playerMovement)
            {
                playerMovement.ResetSpeed();
            }
            if (!stoppedPlayers.Contains(pm))
            {
                stoppedPlayers.Add(pm);
                if (AllPlayersStopped())
                {
                    foreach (PlayerManager p in stoppedPlayers)
                    {
                        p.insideBoundingBox = false;
                    }
                    alreadyStoppedPlayers = true;
                    if (coroutineStarted == true)
                    {
                        StopCoroutine(waitCoroutine);
                    }
                }
                else if (stoppedPlayers.Count == 1)
                {
                    //TextEventsLibrary.ShowTextEvent("StoppedPlayers");
                    //GameObject l_windowManager = GameObject.Find("WindowManager");
                    //l_windowManager.GetComponent<WindowManager>().OpenWindow("Dialog_StoppedPlayers");
                    m_WindowManager.OpenWindow("Dialog_StoppedPlayers");

                }
                if (!AllPlayersStopped())
                {
                    StartCoroutine(waitCoroutine);
                }
            }
        }
    }

    IEnumerator WaitDialog()
    {
        coroutineStarted = true;
        while(true)
        {
            yield return new WaitForSeconds(timeBetweenDialogs);
            int index = Random.Range(0, stoppedPlayers.Count);
            IngameDialogManager.instance.ShowTriggerByCharacter("WAIT", stoppedPlayers[index].GetComponent<GeneralPlayerController>().character);
            yield return null;
        }
    }

    void Teleport() {
		if (teleportPlayers && !alreadyTeleported) {
			//GameObject [] players = GameObject.FindGameObjectsWithTag("Player");
			float offset = 0.0f;
			float plusoffset = 2.0f;
			foreach (Transform go in players) {
				go.position = teleportLocation.position + go.forward * offset + go.right * offset;
				offset += plusoffset;
			}
			alreadyTeleported = true;
		}
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag == "Player") {
			//playersInArea++;
			CheckStoppedPlayers(c.gameObject);
			AddPlayerToList(c.gameObject.GetComponent<GeneralPlayerController>().player);
			Teleport();
			if (CheckCondition()) {
                DoCameraEvent();
			}
		}
	}

    void DoCameraEvent()
    {
        PrimeCamera cc = GetComponent<PrimeCamera>();
        switch (eventType)
        {
            case EventType.Trigger:
                CopyCamera(primeCamera, cc);
                break;
            case EventType.While:
                GameObject proxy = NewProxyCam();
                if (proxy)
                    CopyCamera(proxy, primeCamera.GetComponent<PrimeCamera>(), false);
                CopyCamera(primeCamera, cc);
                break;
        }
        CheckCinematic();
        DoEvents();
        CheckDestroy();
    }

	void CheckDestroy() {
		if (onlyOnce) {
			Destroy (gameObject);
		}
	}

	void DestroyWithoutCheck() {
		Destroy (gameObject);
	}

	void CheckCinematic() {
		if (cinematicForEnemies) {
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject enemy in enemies) {
				MovementManager mm = enemy.GetComponent<MovementManager>();
				if (mm) {
					mm.noInput = true;
					mm.noUpdate = true;
				}
				BehaviorTree bt = enemy.GetComponent<BehaviorTree>();
				if (bt)
					bt.enabled = false;
			}
		}
		if (cinematicForPlayers) {
			//GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach (Transform player in players) {
				player.gameObject.GetComponent<MovementManager>().noInput = true;
				player.gameObject.GetComponent<MovementManager>().noUpdate = true;
			}
		}
		if (cinematicForPlayers || cinematicForEnemies) {
			Invoke ("ReverseCinematic", cinematicDuration);
			if (onlyOnce) {
				Invoke ("DestroyWithoutCheck", cinematicDuration);
				onlyOnce = false;
			}
		}
	}

	void ReverseCinematic() {
		if (cinematicForEnemies) {
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject enemy in enemies) {
				MovementManager mm = enemy.GetComponent<MovementManager>();
				if (mm) {
					mm.noInput = false;
					mm.noUpdate = false;
				}
				BehaviorTree bt = enemy.GetComponent<BehaviorTree>();
				if (bt)
					bt.enabled = true;
			}
		}
		if (cinematicForPlayers) {
			//GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach (Transform player in players) {
				player.gameObject.GetComponent<MovementManager>().noInput = false;
				player.gameObject.GetComponent<MovementManager>().noUpdate = false;
			}
		}
	}

	void OnTriggerStay(Collider c) {
		/*if (c.tag == "Player") {
		}*/
	}

	void OnTriggerExit(Collider c) {
		if (c.tag == "Player") {
			DeletePlayerFromList(c.gameObject.GetComponent<GeneralPlayerController>().player);
			//playersInArea--;
			switch (eventType) {
			case EventType.Trigger:
				break;
			case EventType.While:
				if (triggerType == TriggerType.AnyPlayer && AnyPlayerInArea()) {
				} else {
					GameObject proxy = GetProxyCam();
					CopyCamera(primeCamera, proxy.GetComponent<PrimeCamera>());
				}
				break;
			}
		}
	}

	public void DestroyCameraEvent() {
		if (CheckCondition()) {
			playersList.Clear ();
			GameObject proxy = GetProxyCam ();
			CopyCamera (primeCamera, proxy.GetComponent<PrimeCamera> ());
		}
		Destroy (gameObject);
	}

	void DoProxy() {
		GameObject proxy = GetProxyCam();
		CopyCamera(primeCamera, proxy.GetComponent<PrimeCamera>());
	}

	void DoEvents() {
		if (m_CallbackOnce == false) {
			callback.Invoke();
			m_CallbackOnce = true;
		}
	}

	GameObject NewProxyCam() {
		if (playersList.Count != 1 && playersInArea != 1)
			return null;
		if (GetProxyCam ())
			Destroy (GetProxyCam ());
		GameObject go = new GameObject ();
		go.name = PROXY_CAM_NAME;
		go.transform.parent = transform;
		return go;
	}

	GameObject GetProxyCam() {
		return GameObject.Find (PROXY_CAM_NAME);
	}

    public void ExternalTrigger()
    {
        if (triggerType == TriggerType.ExternalTrigger)
        {
            DoCameraEvent();
        }
    }

	PrimeCamera CopyCamera(GameObject go, PrimeCamera cc, bool active = true) {
		PrimeCamera pc = go.GetComponent<PrimeCamera> ();
		if (!pc) {
			go.AddComponent<PrimeCamera>();
			pc = go.GetComponent<PrimeCamera>();
		}
		if (copySlowDown) {
			pc.slowSpeed = cc.slowSpeed;
			pc.slowTime = cc.slowTime;
		}
		if (copySpeeds) {
			pc.pitchSpeed = cc.pitchSpeed;
			pc.rollSpeed = cc.rollSpeed;
			pc.speed = cc.speed;
			pc.yawSpeed = cc.yawSpeed;
			pc.zoomSpeed = cc.zoomSpeed;
		}
		if (copyYaw) {
			pc.objectiveYaw = cc.objectiveYaw;
		}
		if (copyValues) {
			pc.yaw = cc.yaw;
			pc.roll = cc.roll;
			pc.zoom = cc.zoom;
			pc.pitch = cc.pitch;
		}
		if (copyDistance) {
			pc.maxDistance = cc.maxDistance;
			pc.minDistance = cc.minDistance;
			pc.lerpType = cc.lerpType;
			pc.lerpCurve = cc.lerpCurve;
		}
		if (copyZoom) {
			pc.minZoom = cc.minZoom;
			pc.maxZoom = cc.maxZoom;
		}
		if (copyPitch) {
			pc.maxPitch = cc.maxPitch;
			pc.minPitch = cc.minPitch;
		}
		if (copyYawRotation) {
			pc.beginYaw = cc.beginYaw;
			pc.endYaw = cc.endYaw;
			pc.beginYawPos = cc.beginYawPos;
			pc.endYawPos = cc.endYawPos;
			pc.rotationYaw = cc.rotationYaw;
			pc.yawLerpType = cc.yawLerpType;
			pc.yawLerpCurve = cc.yawLerpCurve;
		}
		if (copyBeginEndPitchRotation) {
			pc.rotationPitch = cc.rotationPitch;
			pc.beginMinPitch = cc.beginMinPitch;
			pc.endMinPitch = cc.endMinPitch;
			pc.beginMaxPitch = cc.beginMaxPitch;
			pc.endMaxPitch = cc.endMaxPitch;
			pc.beginPitchPos = cc.beginPitchPos;
			pc.endPitchPos = cc.endPitchPos;
			pc.pitchLerpType = cc.pitchLerpType;
		}
		if (copyYawConstraint)
        {
            pc.yawConstraint = cc.yawConstraint;
            pc.rotateInstead = cc.rotateInstead;
        }
		if (copyPitchConstraint)
			pc.pitchConstraint = cc.pitchConstraint;
		if (copyRollConstraint)
			pc.rollConstraint = cc.rollConstraint;
		if (copyMovementConstraint)
			pc.movementConstraint = cc.movementConstraint;
		if (copyZoomConstraint)
			pc.zoomConstraint = cc.zoomConstraint;
		if (copyPlayerCheck)
			pc.playerCheck = cc.playerCheck;
		if (copyOnePlayerLerp)
			pc.onePlayerLerp = cc.onePlayerLerp;

		if (copyAcceleration) {
			pc.accelerated = cc.accelerated;
			pc.acceleration = cc.acceleration;
			pc.minSpeed = cc.minSpeed;
			pc.distanceToDeccel = cc.distanceToDeccel;
			pc.zoomAcceleration = cc.zoomAcceleration;
			pc.minZoomSpeed = cc.minZoomSpeed;
			pc.pitchAcceleration = cc.pitchAcceleration;
			pc.minPitchSpeed = cc.minPitchSpeed;
			pc.yawAcceleration = cc.yawAcceleration;
			pc.minYawSpeed = cc.minYawSpeed;
			pc.timeToDeccel = cc.timeToDeccel;
		}
		pc.followStoppedPlayers = cc.followStoppedPlayers;
		if (cinematicForPlayers) {
			cc.followStoppedPlayers = true;
		}
		if (copyTransform) {
			pc.transform.position = transformToCopy.position;
			pc.transform.rotation = transformToCopy.rotation;
		}
		if (copyFocus) {
			pc.customFocusPoint = cc.customFocusPoint;
        } else {
			pc.customFocusPoint = null;
		}
        if (copyOffset) {
            pc.focusOffset = cc.focusOffset;
        }
        
		pc.enabled = active;
		return pc;
	}
}
