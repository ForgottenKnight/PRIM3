using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSMS_Buried : MonoStateMachineState {
	[Header("Buried parameters")]
	private Animator l_Anim;
	private UnityEngine.AI.NavMeshAgent m_Agent;
	private TargetSelector m_Target;
	private Health m_Health;
	private Target m_TargetIndicator;
	private MSMS_Active_Chase m_ChaseState;
	//public float distanceToEmerge = 5.0f;
	private bool m_Emerging;
    private BoxCollider m_Collider;
    private CharacterController m_CharController;
	public bool emergeByPlayerProximity = true;
	public bool invulnerableBeforeEmerging = true;
	public string layerBeforeEmerging = "Event";
	public string layerAfterEmerging = "Enemy";
	public float emergeSpeed = 1.0f;

	public override void StateUpdate() {
		if (!m_Emerging) {
			if (m_Target.target != null && emergeByPlayerProximity == true) {
				StartEmerging (0.0f);
			}
		}
	}

	IEnumerator FinishEmergeAnimation() {
		while (l_Anim.GetCurrentAnimatorStateInfo (0).normalizedTime < 1f && l_Anim.GetCurrentAnimatorStateInfo(0).IsName("Emerger")) {
			yield return null;
		}
		//m_TargetIndicator.ActivateTarget ();
		l_Anim.speed = 1.0f;
		m_Parent.ChangeState ("Active");
		yield return null;
	}
	
	public override void OnEnter() {
		gameObject.layer = LayerMask.NameToLayer(layerBeforeEmerging);
	}
	
	public override void OnExit() {

	}
	
	public override void OnStart() {
		l_Anim = GetComponentInChildren<Animator> ();
		m_Target = GetComponent<TargetSelector> ();
		m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		m_Health = GetComponent<Health> ();
		m_TargetIndicator = GetComponent<Target> ();
        m_CharController = GetComponent<CharacterController>();
        m_Collider = GetComponent<BoxCollider>();
		m_ChaseState = GetComponent<MSMS_Active_Chase> ();
		//m_TargetIndicator.DeactivateTarget ();
		l_Anim.speed = 0.0f;
		l_Anim.Play ("Emerger");
		m_Emerging = false;
		m_Agent.enabled = false;
        m_CharController.enabled = false;
        m_Collider.enabled = false;
		MakeInvulnerable ();
	}

	public void Emerge(float aWaitForSeconds) {
		StartEmerging (aWaitForSeconds);
	}

	private void StartEmerging(float aWaitForSeconds) {
		StartCoroutine (EmergeAfterSeconds (aWaitForSeconds));
	}

	IEnumerator EmergeAfterSeconds(float aWaitForSeconds) {
		yield return new WaitForSecondsRealtime (aWaitForSeconds);
		m_Emerging = true;
		l_Anim.speed = emergeSpeed;
		gameObject.layer = LayerMask.NameToLayer(layerAfterEmerging);
		if (m_ChaseState.canMove)
		    m_Agent.enabled = true;
        m_CharController.enabled = true;
        m_Collider.enabled = true;
		MakeVulnerable ();
		StartCoroutine(FinishEmergeAnimation());
		yield return null;
	}

	private void MakeInvulnerable() {
		if (invulnerableBeforeEmerging) {
			m_Health.invincible = true;
		}
	}

	private void MakeVulnerable() {
		if (invulnerableBeforeEmerging) {
			m_Health.invincible = false;
		}
	}
}
