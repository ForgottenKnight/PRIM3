using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaltShieldChecker : MonoBehaviour {
	List<Health> hs;
	// Use this for initialization
	void Start () {
		hs = new List<Health>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" || other.tag == "Roll") {
			Health h = other.GetComponent<Health>();
			if (h) {
				h.invincible = true;
				hs.Add(h);
			}
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (other.tag == "Player" || other.tag == "Roll") {
			Health h = other.GetComponent<Health>();
			if (h) {
				h.invincible = false;
				hs.Remove(h);
			}
		}
	}

	void OnDestroy() {
		foreach (Health h in hs) {
			h.invincible = false;
		}
	}


}
