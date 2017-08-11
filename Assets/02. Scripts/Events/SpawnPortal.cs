using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPortal : MonoBehaviour {

	public List<GameObject> test;
	public GameObject PortalGO;

	//private bool portal = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//CheckPortalSpawn();	
	}

	/*private void CheckPortalSpawn()
	{
		if(!portal)
		{
			for(int i = 0;i<test.Count;++i)
			{
				if(test[i] == null)
				{
					test.RemoveAt(i);
					if(test.Count == 0)
					{
						portal = true;
					}
				}		
			}
			
			if(portal)
			{
				StartPortal();
			}
		}
	}*/

	public void StartPortal()
	{
		PortalGO.SetActive(true);
	}
}
