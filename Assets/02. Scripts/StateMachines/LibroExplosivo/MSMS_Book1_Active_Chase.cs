using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSMS_Book1_Active_Chase : MonoStateMachineState {

	private UnityEngine.AI.NavMeshAgent m_Agent;
	private Animator m_Anim;
	private TargetSelector m_Target;
    private Health m_Health;
	private CharacterController m_Character;
	private Transform m_RaycastSource;
	public bool canMove = true;
    public float stopDistance = 10f;

    [Header("Explode parameters")]
    public float healthPercentToExplode = 0.3f;
	public float distanceToExplode = 3.0f;

    [Header("Lightning parameters")]
    public float delayAfterLightning = 3.0f;
    public float lightningDistance = 10.0f;
    private float m_LastLightningTime;

    [Header("Shot parameters")]
    public float fireMinRange = 3.5f;
    public float fireMaxRange = 30f;
    public float delayAfterShot = 5f;
    private float m_LastShotTime;

	[Header("Rotation parameters")]
	public float rotationSpeed = 5.0f;
	public float threshold = 0.5f;



	public override void StateUpdate() {
		if (m_Agent.enabled == false) {
			if (m_Character.isGrounded) {
				m_Agent.enabled = true;
			}
		}
		if (m_Target.target != null) {
			Vector3 target2D = m_Target.target.transform.position;
			target2D.y = 0f;
			Vector3 position2D = transform.position;
			position2D.y = 0f;
			float distance = Vector3.Distance(target2D, position2D);
			float angle = Vector3.Angle(transform.forward, (target2D - position2D));
			if (CheckExplode (distance) && CheckExplosionHealth()) { // Caso empieza a explotar
				ActionStop();
				ActionExplode();
				return;
            }else if(CheckFireTime() && CheckAngle(angle) && CheckFireDistance(distance))
            {
                ActionStop();
                ActionFire();
            }
            else if (CheckLightningTime() && CheckLightningDistance(distance))
            {
                ActionStop();
                ActionLightning();
			} else if (CheckStopDistance(distance) || !CheckGroundInFront()) { // Caso llega cerca
				ActionStop ();
				if (!CheckAngle(angle)) { // Caso no esta encarado
					ActionRotate();
				}
			} else if (CheckCanMove()) { // Caso sigue persiguiendo y tiene nav agent
				ActionMove ();
			}
		} else {
			ActionStop ();
			// Change state to wander
		}
	}

	public override void OnEnter() {
		//m_LastAttackTime = Time.time;
		//m_LastThrowTime = Time.time;
		m_Anim.SetBool("walking", false);
		m_Anim.speed = 1.0f;
	}

	public override void OnExit() {
	}

	public override void OnStart() {
		m_Anim = GetComponentInChildren<Animator> ();
		m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		m_Target = GetComponent<TargetSelector> ();
		m_Character = GetComponent<CharacterController> ();
		m_RaycastSource = GetComponentInChildren<RaycastObject> ().transform;
        m_Health = GetComponent<Health>();
        m_LastLightningTime = Time.time - Random.Range(0f, 1f) * delayAfterLightning;
        m_LastShotTime = Time.time - Random.Range(0f, 1f) * delayAfterShot;
	}

	// Checks
	bool CheckExplode(float distance) {
		return distance <= distanceToExplode;
	}

    bool CheckExplosionHealth() {
        return m_Health.GetHealthAsUnit() <= healthPercentToExplode;
    }

    bool CheckLightningTime() {
        return m_LastLightningTime + delayAfterLightning < Time.time;
    }

    bool CheckLightningDistance(float distance) {
        return distance <= lightningDistance;
    }

    bool CheckFireDistance(float distance)
    {
        return distance <= fireMaxRange && distance >= fireMinRange;
    }

    bool CheckFireTime()
    {
        return m_LastShotTime + delayAfterShot < Time.time;
    }

	bool CheckStopDistance(float distance) {
        return distance <= stopDistance;
	}

	bool CheckAngle(float angle) {
		return angle <= threshold;
	}

	bool CheckGroundInFront() {
        return true;
		//return Physics.Raycast (m_RaycastSource.position, -Vector3.up, 2.0f);
	}

	bool CheckCanMove() {
		return m_Agent.enabled && canMove;
	}

	// Actions
	void ActionExplode() {
		m_Parent.ChangeState ("Active_Explode");
	}

    void ActionFire()
    {
        m_LastShotTime = Time.time;
        m_Parent.ChangeState("Active_Fire");
    }

	void ActionStop() {
		if (CheckCanMove ()) {
			m_Agent.ResetPath ();
			m_Agent.isStopped = true;
			m_Anim.SetBool ("walking", false);
		}
	}

	void ActionMove() {
		if (CheckCanMove ()) {
			m_Anim.SetBool ("walking", true);
			m_Agent.isStopped = false;
			m_Agent.SetDestination (m_Target.target.transform.position);
		}
	}

    void ActionLightning() {
        m_LastLightningTime = Time.time;
        m_Parent.ChangeState("Active_Lightning");
    }

	void ActionRotate() {
		Quaternion lookRotation;
		Vector3 direction;
		direction = (m_Target.target.transform.position - transform.position).normalized;
		direction.y = transform.forward.y;
		lookRotation = Quaternion.LookRotation (direction);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
	}
}
