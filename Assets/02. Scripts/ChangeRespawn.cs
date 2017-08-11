using UnityEngine;
using System.Collections;

public class ChangeRespawn : MonoBehaviour {

	public RespawnPoint rp;
	public Transform newSpawnPoint;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangePoint()
	{
		rp.spawnPoint = newSpawnPoint;
	}
}
