using UnityEngine;
using System.Collections.Generic;

public class MSMS_Active_Chase : MonoStateMachineState {
	private UnityEngine.AI.NavMeshAgent m_Agent;
	private Animator m_Anim;
	private TargetSelector m_Target;
	private CharacterController m_Character;
	private Transform m_RaycastSource;
	private BoxCollider m_BoxCollider;
	public bool canMove = true;

    public float movementSpeed = 4.5f;

	[Header("Attack check parameters")]
	public float delayAfterAttack = 3.0f;
	private float m_LastAttackTime;
	public float attackRange = 4.5f;
	public float maxAngle = 60.0f;
	public float maxDifferenceY = 1.5f;

    [Header("Area Attack parameters")]
    public float areaAttackRange = 3.5f;
	public float maxAngleAreaAttack = 40f;
    public float delayAfterAreaAttack = 10.0f;
    private float m_LastAreaAttackTime;

    [Header("Charge parameters")]
    public float delayAfterChargeAttack = 10.0f;
    public float chargeRange = 10.0f;
    public float maxAngleCharge = 25.0f;
    private float m_LastChargeAttackTime;

	[Header("Throw rock parameters")]
	public float delayAfterThrow = 8.0f;
	private float m_LastThrowTime;
	public float throwMaxRange = 20.0f;
	public float throwMinRange = 6.0f;
	public float throwMaxAngle = 20.0f;

	[Header("Rotation parameters")]
	public float rotationSpeed = 5.0f;
	public float threshold = 0.5f;



	public override void StateUpdate() {
        bool l_Grounded = CheckGrounded();
        if (m_Agent.enabled == false && l_Grounded && canMove)
        {
            m_Agent.enabled = true;
        }else if (!l_Grounded)
        {
            m_Agent.enabled = false;
        }

		if (m_Agent.speed == 0.0f && canMove == true) {
			if (m_Character.isGrounded) {
				m_Agent.speed = movementSpeed;
			}
		}
		if (m_Target.target != null) {
			Vector3 target2D = m_Target.target.transform.position;
			target2D.y = 0f;
			Vector3 position2D = transform.position;
			position2D.y = 0f;
			float distance = Vector3.Distance(target2D, position2D);
			float angle = Vector3.Angle(transform.forward, (target2D - position2D));
			if (CheckAttackTime() && CheckAttack(angle, distance)) { // Caso de ataque
				ActionStop ();
				ActionAttack ();
				return;
            }
			else if (CheckAreaAttackTime() && CheckAreaAttack(distance, angle) && CheckAngle(angle))
            {
                ActionStop();
                ActionAreaAttack();
                return;
            } else if (CheckThrowTime() && CheckThrow(angle, distance)) { // Caso lanzar roca
				ActionStop ();
				ActionThrow();
			} else if (CheckStopDistance(distance) || !CheckGroundInFront() || !CheckCanMove()) { // Caso llega cerca
				ActionStop ();
				if (!CheckAngle(angle)) { // Caso no esta encarado
					ActionRotate();
				} else {
					ActionStopRotating();
				}
            }else if (CheckChargeTime() && CheckCharge(angle, distance))
            {
                ActionStop();
                ActionCharge();
            }else if (CheckCanMove()) { // Caso sigue persiguiendo y tiene nav agent
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
		m_Anim.SetBool ("rotating", false);
		m_Anim.speed = 1.0f;
	}
	
	public override void OnExit() {
	}
	
	public override void OnStart() {
		m_Anim = GetComponentInChildren<Animator> ();
		m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		m_Target = GetComponent<TargetSelector> ();
		m_Character = GetComponent<CharacterController> ();
		m_BoxCollider = GetComponent<BoxCollider> ();
		m_RaycastSource = GetComponentInChildren<RaycastObject> ().transform;
		if (canMove == false) {
			m_Agent.speed = 0.0f;
		}
		m_LastAttackTime = 0f;
        m_LastAreaAttackTime = 0f;
        m_LastChargeAttackTime = 0f;
		m_LastThrowTime = 0f;
	}

	// Checks
	bool CheckAttackTime() {
		return m_LastAttackTime + delayAfterAttack < Time.time;
	}

    bool CheckChargeTime()
    {
        //return false;
        return m_LastChargeAttackTime + delayAfterChargeAttack < Time.time;
    }

    bool CheckCharge(float angle,float distance )
    {
        return angle <= maxAngleCharge && distance <= chargeRange && ((Mathf.Abs(m_Target.target.transform.position.y - transform.position.y) <= maxDifferenceY) || maxDifferenceY == 0.0f);
    }

	bool CheckAttack(float angle, float distance) {
		return angle <= maxAngle && distance <= attackRange && ((Mathf.Abs (m_Target.target.transform.position.y - transform.position.y) <= maxDifferenceY) || maxDifferenceY == 0.0f);
	}

    bool CheckAreaAttackTime()
    {
        return m_LastAreaAttackTime + delayAfterAreaAttack < Time.time;
    }

	bool CheckAreaAttack(float distance, float angle)
    {
		return (distance <= areaAttackRange && ((Mathf.Abs(m_Target.target.transform.position.y - transform.position.y) <= maxDifferenceY) || maxDifferenceY == 0.0f)) && angle <= maxAngleAreaAttack;
    }

	bool CheckThrowTime() {
		return m_LastThrowTime + delayAfterThrow < Time.time;
	}

	bool CheckThrow(float angle, float distance) {
		return distance <= throwMaxRange && distance >= throwMinRange && angle <= throwMaxAngle;
	}

	bool CheckStopDistance(float distance) {
		return distance <= m_Agent.stoppingDistance;
	}

	bool CheckAngle(float angle) {
		return angle <= threshold;
	}

	bool CheckGroundInFront() {
		return Physics.Raycast (m_RaycastSource.position, -Vector3.up, 0.5f);
	}

    bool CheckGrounded()
    {
        Vector3 lastPos = transform.position;
        Vector3 up = transform.up;
        Ray ray = new Ray(lastPos, -up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f))
        {
            if (hit.collider.gameObject.layer != gameObject.layer)
            {
                return true;
            }
        }
		if (m_BoxCollider != null) {
			Bounds b = m_BoxCollider.bounds;
			Vector3 p1 = b.min;
			Vector3 p2 = b.max;
			p2.y = p1.y;
			if (Physics.Raycast(p1, -up, out hit, 2f)) {
				if (hit.collider.gameObject.layer != gameObject.layer)
				{
					return true;
				}
			}
			if (Physics.Raycast(p2, -up, out hit, 2f)) {
				if (hit.collider.gameObject.layer != gameObject.layer)
				{
					return true;
				}
			}
		}
        return false;
    }

	bool CheckCanMove() {
		return (m_Agent.speed > 0.0f) && canMove;
	}

	// Actions
	void ActionStop() {
		if (CheckCanMove ()) {
            if (m_Agent.enabled)
            {
                m_Agent.ResetPath();
                m_Agent.isStopped = true;
            }
			m_Anim.SetBool ("walking", false);
		}
	}

	void ActionMove() {
		m_Anim.SetBool ("rotating", false);
		if (CheckCanMove ()) {
			m_Anim.SetBool ("walking", true);
            m_Agent.isStopped = false;
			m_Agent.SetDestination (m_Target.target.transform.position);
		}
	}
	
	void ActionAttack() {
		m_Anim.SetBool ("rotating", false);
		m_LastAttackTime = Time.time;
		m_Parent.ChangeState("Active_Attack");
	}

    void ActionCharge()
    {
        m_Anim.SetBool("rotating", false);
        m_LastChargeAttackTime = Time.time;
        m_Parent.ChangeState("Active_Charge");
    }

    void ActionAreaAttack()
    {
        m_Anim.SetBool("rotating", false);
        m_LastAreaAttackTime = Time.time;
        m_Parent.ChangeState("Active_AreaAttack");
    }

	void ActionThrow() {
		//m_Anim.SetBool ("rotating", false);
		m_LastThrowTime = Time.time;
		m_Parent.ChangeState ("Active_Throw");
	}

	void ActionRotate() {
		Quaternion lookRotation;
		Vector3 direction;
		m_Anim.SetBool ("rotating", true);
		direction = (m_Target.target.transform.position - transform.position).normalized;
		direction.y = transform.forward.y;
		lookRotation = Quaternion.LookRotation (direction);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
	}

	void ActionStopRotating() {
		m_Anim.SetBool ("rotating", false);
	}
}
