using UnityEngine;
using System.Collections;

public class UISlowed : UIState
{

	// Use this for initialization
	protected  override void Start () {
        	
	}

	protected override void UpdateUI() {
//		Debug.Log ("Checked");
        Movement l_Movement = character.GetComponent<Movement>();
        if (l_Movement.slowDown)
        {
			SetUIActive(true);
		} else {
			SetUIActive(false);
		}
	}
}
