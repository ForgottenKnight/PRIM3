using UnityEngine;
using System.Collections;

public class SuperAbilityController : MonoBehaviour {


    public SuperAbility[] markers = new SuperAbility[3];//0->Salt, 1->Sulphur, 2->Merc
    public GameObject markerStone;
    public Vector3 spawnRockOffset = new Vector3(0f,100f,0f);
    public GameObject markerWater;//Este marker aun no existe
    public GameObject markerWind;//Este marker aun no existe
    public GameObject markerFire;//Este marker aun no existe
	public LineRenderer[] Connectors;//0->Salt-Merc, 1->Salt-Sulphur, 2->Sulphur-Merc
    public float zoomOffset = 1.5f;

	public bool[] activeMarkers;
	public GameObject StonePrefab;
	public Transform enemies;
	public Transform damagableObjects;
	public float stoneDamage = 50;
	public Camera mainCamera;
    public GameObject explosion;

	private Transform[] m_MarkersTransforms;//0->Salt, 1->Sulphur, 2->Merc
    private PrimeCamera m_PrimeCamera;

	private enum MercuryDirection
	{
		LEFT = 0,
		RIGHT,
		UP,
		DOWN
	}

	private MercuryDirection mercDir;

	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
		activeMarkers = new bool[3];
		activeMarkers[0] = false;
		activeMarkers[1] = false;
		activeMarkers[2] = false;
        SuperAbilityStatic.m_markerManager = this;
		//markers = new SuperAbility[3];
		m_MarkersTransforms = new Transform[3];
		enemies = GameObject.FindGameObjectWithTag ("EnemiesContainer").transform;
        m_PrimeCamera = mainCamera.GetComponent<PrimeCamera>();

		//damagableObjects = GameObject.FindGameObjectWithTag("").transform;// TO DO: transform objetos destruibles (p.e. Rocas lvl 2)
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool IsMarkerActive(int character)
	{
		return activeMarkers[character];
	}

	public void ReInit()
	{
		for(int i = 0; i<Connectors.Length;++i)
		{
			Connectors[i].enabled = false;
		}

		for(int i = 0; i<m_MarkersTransforms.Length;++i)
		{
			if(m_MarkersTransforms[i])
			{
				m_MarkersTransforms[i].gameObject.SetActive(false);
			}
		}
		for(int i = 0; i<activeMarkers.Length; ++i)
		{
			if(activeMarkers[i])
			{
				activeMarkers[i] = false;
			}
		}
	}

	public void SetConnector(int index, bool active)
	{//index: 0->Salt-Sulphur, 1->Salt-Merc, 2->Sulphur-Merc
		Connectors[index].enabled = active;
		if(active)
		{
			switch(index)
			{
			case 0://Salt-Sulphur
				Connectors[index].SetPosition(0, m_MarkersTransforms[0].position);
				Connectors[index].SetPosition(1, m_MarkersTransforms[1].position);
				break;
			case 1://Salt-Merc
				Connectors[index].SetPosition(0, m_MarkersTransforms[0].position);
				Connectors[index].SetPosition(1, m_MarkersTransforms[2].position);
				break;
			case 2://Sulphur-Merc
				Connectors[index].SetPosition(0, m_MarkersTransforms[1].position);
				Connectors[index].SetPosition(1, m_MarkersTransforms[2].position);
				break;
			}

		}
	}

	public void setMarker(int character, bool active, Transform markerTransform)
	{
		activeMarkers[character] = active;
		if(active)
		{
			m_MarkersTransforms[character] = markerTransform;
			int numActive = 0;
			for (int i = 0; i < activeMarkers.Length; ++i)
			{
				if(activeMarkers[i])
					++numActive;
			}
			if(numActive == activeMarkers.Length)
			{
				CheckMercuryPosition();
				switch(mercDir)
				{
				case MercuryDirection.LEFT:
					break;
				case MercuryDirection.RIGHT:
					break;
				case MercuryDirection.UP:
					break;
				case MercuryDirection.DOWN:
					//NOTA: El siguiente bucle se deberia hacer fuera antes del switch, 
					//si implerentaramos TODAS las habilidades
				//	for(int i = 0; i < markers.Length; ++i)
				//	{
				//			markers[i].useEnergy();
						//	activeMarkers[i] = false;
				//	}
					Vector3 centerPoint = CalculateCenter();
                    markerStone.transform.localPosition = centerPoint;
                    markerStone.SetActive(true);
					SpawnStoneRock(centerPoint);
                    SuperAbilityStatic.Activate();
					break;
				}
			}
		}
	}

    public void UseMarkersEnergy()
    {
        for (int i = 0; i < markers.Length; ++i)
        {
            markers[i].useEnergy();
            activeMarkers[i] = false;
        }
        markerStone.SetActive(false);
    }

	private void SpawnStoneRock(Vector3 centerPoint)
	{
     
        Vector3 l_SpawnLocation = mainCamera.transform.position + Vector3.Scale(mainCamera.transform.forward , spawnRockOffset) * -1f;


        GameObject go = Instantiate(StonePrefab, new Vector3(999,999,999), Quaternion.identity) as GameObject;
        go.transform.parent = mainCamera.transform;
        go.transform.localPosition = spawnRockOffset;

        //m_PrimeCamera.zoom *= zoomOffset;
      

      //  m_PrimeCamera.zoomConstraint = true;

		SuperAbilityStone sAS = go.AddComponent<SuperAbilityStone>();
        sAS.destiny = markerStone.transform.position;//centerPoint;
		if(enemies == null)
			enemies = GameObject.FindGameObjectWithTag ("EnemiesContainer").transform;
		sAS.enemies = enemies;
		sAS.damagableObjects = damagableObjects;
		sAS.damageDone = stoneDamage;
        sAS.explosion = explosion;
        sAS.zoomOffset = zoomOffset;
        sAS.maxZoom = m_PrimeCamera.maxZoom;
        sAS.minZoom = m_PrimeCamera.minZoom;
        sAS.m_PrimeCamera = m_PrimeCamera;

        m_PrimeCamera.maxZoom = m_PrimeCamera.zoom * zoomOffset;
        m_PrimeCamera.minZoom = m_PrimeCamera.zoom * zoomOffset;

	}

	private Vector3 CalculateCenter()
	{
		Vector3 center;

		Vector3 lSalt = transform.GetChild(0).localPosition;
		Vector3 lSulphur = transform.GetChild(1).localPosition;
		Vector3 lMercury = transform.GetChild(2).localPosition;

		center = lSalt + lSulphur + lMercury;
		center /= 3.0f;

		return center;

	}

	private void CheckMercuryPosition()
	{
		Vector3 lSalt = transform.GetChild(0).localPosition;
		Vector3 lSulphur = transform.GetChild(1).localPosition;
		Vector3 lMercury = transform.GetChild(2).localPosition;
		if(mainCamera == null)
			mainCamera = Camera.main;

		Vector3 forwardCamera = mainCamera.transform.forward;
		Vector3 rightCamera = mainCamera.transform.right;

		lSalt.y = lSulphur.y = lMercury.y = forwardCamera.y = rightCamera.y = 0.0f;


/*		forwardCamera.x = Mathf.Abs(forwardCamera.x);
		forwardCamera.z = Mathf.Abs(forwardCamera.z);
		rightCamera.x = Mathf.Abs(rightCamera.x);
		rightCamera.z = Mathf.Abs(rightCamera.z);*/



		if(Mathf.Abs(rightCamera.x) > Mathf.Abs(rightCamera.z))
		{
			if(rightCamera.x > 0)
			{
				if(lMercury.x > lSalt.x && lMercury.x > lSulphur.x)
				{
					mercDir = MercuryDirection.RIGHT;
				}
				if(lMercury.x < lSalt.x && lMercury.x < lSulphur.x)
				{
					mercDir = MercuryDirection.LEFT;
				}
			}else{
				if(lMercury.x < lSalt.x && lMercury.x < lSulphur.x)
				{
					mercDir = MercuryDirection.RIGHT;
				}
				if(lMercury.x > lSalt.x && lMercury.x > lSulphur.x)
				{
					mercDir = MercuryDirection.LEFT;
				}
			}
		}else{
			if(rightCamera.z > 0)
			{
				if(lMercury.z > lSalt.z && lMercury.z > lSulphur.z)
				{
					mercDir = MercuryDirection.RIGHT;
				}
				if(lMercury.z < lSalt.z && lMercury.z < lSulphur.z)
				{
					mercDir = MercuryDirection.LEFT;
				}
			}else{
				if(lMercury.z < lSalt.z && lMercury.z < lSulphur.z)
				{
					mercDir = MercuryDirection.RIGHT;
				}
				if(lMercury.z > lSalt.z && lMercury.z > lSulphur.z)
				{
					mercDir = MercuryDirection.LEFT;
				}
			}
		}

		if(Mathf.Abs(forwardCamera.x) > Mathf.Abs(forwardCamera.z))
		{
			if(forwardCamera.x > 0)
			{
				if(lMercury.x > lSalt.x && lMercury.x > lSulphur.x)
				{
					mercDir = MercuryDirection.UP;
				}
				if(lMercury.x < lSalt.x && lMercury.x < lSulphur.x)
				{
					mercDir = MercuryDirection.DOWN;
				}
			}else{
				if(lMercury.x < lSalt.x && lMercury.x < lSulphur.x)
				{
					mercDir = MercuryDirection.UP;
				}
				if(lMercury.x > lSalt.x && lMercury.x > lSulphur.x)
				{
					mercDir = MercuryDirection.DOWN;
				}
			}
		}else{
			if(forwardCamera.z > 0)
			{
				if(lMercury.z > lSalt.z && lMercury.z > lSulphur.z)
				{
					mercDir = MercuryDirection.UP;
				}
				if(lMercury.z < lSalt.z && lMercury.z < lSulphur.z)
				{
					mercDir = MercuryDirection.DOWN;
				}
			}else{
				if(lMercury.z < lSalt.z && lMercury.z < lSulphur.z)
				{
					mercDir = MercuryDirection.UP;
				}
				if(lMercury.z > lSalt.z && lMercury.z > lSulphur.z)
				{
					mercDir = MercuryDirection.DOWN;
				}
			}
		}

		/*if(changed && changed2)
		{
			Debug.Log(forwardCamera);
			Debug.Log(rightCamera);
			Debug.Log(lSalt);
			Debug.Log(lSulphur);
			Debug.Log(lMercury);
			Debug.Log ("PJ * Forward");
			Debug.Log(Vector3.Scale(lSalt, forwardCamera));
			Debug.Log(Vector3.Scale(lSulphur, forwardCamera));
			Debug.Log(Vector3.Scale(lMercury, forwardCamera));
			Debug.Log ("PJ * Right");			
			Debug.Log(Vector3.Scale(lSalt, rightCamera));
			Debug.Log(Vector3.Scale(lSulphur, rightCamera));
			Debug.Log(Vector3.Scale(lMercury, rightCamera));

		}*/

		/*Debug.Log(mercDir);
		Debug.Log(forwardCamera);
		Debug.Log(rightCamera);*/

	}
}
