using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SickDev.CommandSystem;
using DevConsole;

public class DevConsoleHelper : MonoBehaviour {
	[System.Serializable]
	public struct Place {
		public string name;
		public Transform position;
	}
	
	public Transform[] resetPositions;
	public Place[] places;
	[HideInInspector]
	public Dictionary<string, Transform> gotoPlaces;

	// Use this for initialization
	void Start () {
		gotoPlaces = new Dictionary<string, Transform> ();
		for (int i = 0; i < places.Length; ++i) {
			gotoPlaces.Add (places[i].name, places[i].position);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
