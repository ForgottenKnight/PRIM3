using UnityEngine;
using System.Collections;

public class DisappearIfPlayers : MonoBehaviour {
	public int maxPlayers = 1;
	public bool disableInstead = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		if (players.Length > maxPlayers) {
			if (disableInstead) {
				gameObject.SetActive (false);
			} else {
				Destroy (gameObject);
			}
		}
	}
}
