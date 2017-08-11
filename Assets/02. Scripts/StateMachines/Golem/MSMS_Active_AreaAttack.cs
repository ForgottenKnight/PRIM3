using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSMS_Active_AreaAttack : MonoStateMachineState {

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
	public int attackForce = 30;
	public float attackAngle = 360.0f;
    public float raycastDistance = 10.0f;

	[Header("Coroutines")]
	private Coroutine AttackWaitTime;
	private Coroutine ChangeStateWaitTime;
	private Coroutine CreateFieldEffect;

	[Header("Stomp Effect")]
	public GameObject stompRockPrefab;
	public GameObject stompPrefab;
	public float distanceBetweenRocks;
	public float distance;
	public float delayBetweenRocks;

	public override void StateUpdate() {
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("GolpeSuelo") && !m_Anim.IsInTransition(0) && m_Attacked == false && m_CoroutineFinished == true)
        {
			m_Attacked = true;
			ChangeStateWaitTime = StartCoroutine (ChangeStateWaitTimeCoroutine());
		}
	}
	
	public override void OnEnter() {
		m_Attacked = false;
		m_CoroutineFinished = false;
		m_Anim.SetTrigger ("areaAttackTrigger");
		//AttackWaitTime = StartCoroutine (AttackWaitTimeCoroutine());
	}

	IEnumerator AttackWaitTimeCoroutine() {
		yield return new WaitForSeconds (preWaitTime);
		m_CoroutineFinished = true;
		//DoAttack ();
	}

	IEnumerator ChangeStateWaitTimeCoroutine() {
		yield return new WaitForSeconds (postWaitTime);
		m_Parent.ChangeState ("Active_Chase");
	}

	public void DoAttack() {
		List<GameObject> players = CustomTagManager.GetObjectsByTag ("Player");
		for (int i = 0; i < players.Count; ++i) {
			staticAttackCheck.checkAttack (players[i].transform, transform, attackLength, attackAngle, attackHeight, (float)attackForce);
		}
        GameObject saltShield = CustomTagManager.GetObjectByTag("SaltShield");
		if (saltShield) {
			staticAttackCheck.checkAttack (saltShield.transform, transform, attackLength, attackAngle, attackHeight, (float)attackForce);
		}
		CreateFieldEffect = StartCoroutine (CreateFieldEffectCoroutine());
		if (stompPrefab != null) {
			GameObject l_particles = Instantiate (stompPrefab, transform.position, Quaternion.AngleAxis(90.0f, Vector3.right));
			l_particles.GetComponent<ParticleSystem>().Play ();
		}
	}
	
	public override void OnExit() {
		if (AttackWaitTime != null)
			StopCoroutine (AttackWaitTime);
		if (ChangeStateWaitTime != null)
			StopCoroutine (ChangeStateWaitTime);
		m_Attacked = true;
	}
	
	public override void OnStart() {
		m_Anim = GetComponentInChildren<Animator> ();
	}

    public void ChangeState()
    {
        m_Parent.ChangeState("Active_Chase");
    }


	IEnumerator CreateFieldEffectCoroutine() {
		Vector3 l_direction = transform.forward;
		Vector3 l_initialPosition  = transform.position + new Vector3(0.0f, 50.0f, 0.0f);

		int l_steps = (int)(distance / distanceBetweenRocks);
		if (distance % (distanceBetweenRocks) != 0) {
			l_steps++;
		}
		float l_distanceBetweenColliders = distance / l_steps;
		l_direction = l_direction.normalized;

		for (int i = 1; i < l_steps; i++) {
			GameObject l_stompRockEffect = Instantiate (stompRockPrefab);
			l_stompRockEffect.transform.forward = l_direction;
	
			RaycastHit l_hit;
			int l_layerMask = 1;


            if (Physics.Raycast((l_initialPosition + l_direction * i * l_distanceBetweenColliders), -Vector3.up, out l_hit, raycastDistance, l_layerMask))
            {
				//l_stompRockEffect.transform.eulerAngles = new Vector3(Vector3.Angle(l_hit.normal, l_stompRockEffect.transform.up), l_stompRockEffect.transform.eulerAngles.y,  l_stompRockEffect.transform.eulerAngles.z);
				l_stompRockEffect.transform.position = new Vector3 (l_hit.point.x, l_hit.point.y/* + l_stompRockEffect.GetComponent<GolemStomp>().initialEffectSize.y * 0.5f*/, l_hit.point.z);
            }
            else
            {
                if (CreateFieldEffect != null)
                {
                    StopCoroutine(CreateFieldEffect);
                }
                break;
            }
			yield return new WaitForSeconds (delayBetweenRocks);
		}
	}
}
