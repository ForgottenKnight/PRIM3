using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSMS_Book1_Active : MonoStateMachineState {
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
