using UnityEngine;
using System.Collections;

public class UIInvulnerable : UIState {

	// Use this for initialization
	protected  override void Start () {
	
	}

	protected override void UpdateUI() {
//		Debug.Log ("Checked");
		Health health = character.GetComponent<Health> ();
		if(health.invincible && health.GetHealth() > 0) {
			SetUIActive(true);
		} else {
			SetUIActive(false);
		}
	}
}
