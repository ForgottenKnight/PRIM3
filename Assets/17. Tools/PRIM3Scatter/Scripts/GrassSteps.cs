using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSteps : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		List<GameObject> players = CustomTagManager.GetObjectsByTag ("Player");
		Vector4 pos = new Vector4 (players [0].transform.position.x, players [0].transform.position.y, players [0].transform.position.z, 1.0f);
		Shader.SetGlobalVector ("_G_PlayerPosition0", pos);
	}
}
