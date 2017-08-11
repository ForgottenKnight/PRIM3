using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SuperAbilityStone : MonoBehaviour {

	public float velocity = 100f;
	public float damageDone = 150f;
	public float radius = 5f;

	[HideInInspector]
	public Vector3 destiny = default(Vector3);
	[HideInInspector]
	public Transform enemies;
	[HideInInspector]
	public Transform damagableObjects;
    public GameObject explosion;
    public float explosionDistance = 0.25f;

    [HideInInspector]
    public float zoomOffset;
    [HideInInspector]
    public float minZoom;
    [HideInInspector]
    public float maxZoom;
    [HideInInspector]
    public PrimeCamera m_PrimeCamera;

	//private Vector3 startPos;
	private float timer = 0.0f;
	// Use this for initialization
	void Start () {
		//startPos = transform.position;		
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(transform.position, destiny) > explosionDistance)
		{
			timer += Time.deltaTime;
			//transform.localPosition = Vector3.Lerp(startPos,destiny,Time.deltaTime*velocity);
            transform.position = Vector3.MoveTowards(transform.position, destiny, Time.deltaTime * velocity);
		}else{
            //m_PrimeCamera.zoom /= zoomOffset;
            //m_PrimeCamera.zoomConstraint = false;
            m_PrimeCamera.minZoom = minZoom;
            m_PrimeCamera.maxZoom = maxZoom;

			//int childCount = enemies.childCount;
			List<GameObject> enemiesList = CustomTagManager.GetObjectsByTag ("Enemy");
			for (int i = 0; i < enemiesList.Count; ++i)
			{
				staticAttackCheck.checkAttack(enemiesList[i].transform, transform, radius, 360f, radius, damageDone);

			}

			//if(damagableObjects)
			//{
				//childCount = damagableObjects.childCount;
				List<GameObject> destructibleObjects = CustomTagManager.GetObjectsByTag("Destructible");
				for (int i = 0; i < destructibleObjects.Count; ++i)
				{
					staticAttackCheck.checkAttack(destructibleObjects[i].transform, transform, radius, 360f, radius, damageDone);
				}
			//}
            if (explosion != null)
            {
                Instantiate(explosion, transform.position, transform.rotation);
            }
            SuperAbilityStatic.Deactivate();

			Destroy(transform.gameObject);
		}
	}
}
