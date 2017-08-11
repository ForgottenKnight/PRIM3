using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSMS_Active_Attack : MonoStateMachineState {
	[Header("Attack parameters")]
	private Animator m_Anim;
	private bool m_Attacked;
	private bool m_CoroutineFinished;
	[Tooltip("Tiempo antes de ejecutar el ataque")]
	public float preWaitTime = 1.5f;
	[Tooltip("Tiempo para cambiar de estado tras ejecutar el ataque")]
	public float postWaitTime = 1.15f;
	public float attackLength = 5.0f;
	public float attackHeight = 2.0f;
	public int attackForce = 10;
	public float attackAngle = 30.0f;

	public override void StateUpdate() {
		if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Golpe") && !m_Anim.IsInTransition(0) && m_Attacked == false && m_CoroutineFinished == true) {
			m_Attacked = true;
			StartCoroutine(ChangeStateWaitTime());
		}
	}
	
	public override void OnEnter() {
		m_Attacked = false;
		m_CoroutineFinished = false;
		m_Anim.SetTrigger ("attackTrigger");
		StartCoroutine (AttackWaitTime ());
	}

	IEnumerator AttackWaitTime() {
		yield return new WaitForSeconds (preWaitTime);
		m_CoroutineFinished = true;
		DoAttack ();
	}

	IEnumerator ChangeStateWaitTime() {
		yield return new WaitForSeconds (postWaitTime);
		m_Parent.ChangeState ("Active_Chase");
	}

	void DoAttack() {
		List<GameObject> players = CustomTagManager.GetObjectsByTag ("Player");
		for (int i = 0; i < players.Count; ++i) {
			staticAttackCheck.checkAttack (players[i].transform, transform, attackLength, attackAngle, attackHeight, (float)attackForce);
		}
		GameObject saltShield = GameObject.FindGameObjectWithTag ("SaltShield");
		if (saltShield) {
			staticAttackCheck.checkAttack (saltShield.transform, transform, attackLength, attackAngle, attackHeight, (float)attackForce);
		}
	}
	
	public override void OnExit() {
		StopAllCoroutines ();
		m_Attacked = true;
	}
	
	public override void OnStart() {
		m_Anim = GetComponentInChildren<Animator> ();
	}
}
