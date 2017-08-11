using UnityEngine;
using System.Collections.Generic;

public class MSMS_Active : MonoStateMachineState {
	private Animator l_Anim;

	public override void StateUpdate() {
	}
	
	public override void OnEnter() {
		ChangeState ("Active_Chase");
	}
	
	public override void OnExit() {
	}
	
	public override void OnStart() {
	}

	public override void OnFirstEnter() {
	}

}
