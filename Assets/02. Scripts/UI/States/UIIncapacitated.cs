using UnityEngine;
using System.Collections;

public class UIIncapacitated : UIState {

	// Use this for initialization
	protected  override void Start () {
	
	}

	protected override void UpdateUI() {
		Health health = character.GetComponent<Health> ();
		if(health.GetHealth() == 0) {
			SetUIActive(true);
		} else {
			SetUIActive(false);
		}
	}

}
