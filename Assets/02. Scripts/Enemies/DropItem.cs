using UnityEngine;
using System.Collections;

public class DropItem : MonoBehaviour {

	public GameObject prefabFauto;
	public float DropChance;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Drop()
	{
		float l_DropChance = Random.Range(0.0f, 100.0f);

		if(l_DropChance <= DropChance)
		{
			Instantiate(prefabFauto, transform.position + Vector3.up, transform.rotation);
		}
	}
}
