using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSMS_Active_Charge : MonoStateMachineState
{
	[Header("Charge parameters")]
    private Animator m_Anim;
    private TargetSelector m_Target;
    private UnityEngine.AI.NavMeshAgent m_Agent;
    private Vector3  m_FinalPosition;
    public GameObject chargeCollider;
    private ChargeCollider m_ChargeColliderScript;

    public float damage = 10.0f;
    public float pushSpeed = 5.0f;
    public float pushTime = 1.0f;
    public float maxAngle = 75.0f;
    public float angularSpeed = 5f;
    public float stopDistance = 2.5f;
    public float chargeSpeed = 5f;
    public float maxChargingTime = 10.0f;
    public float maxDifferenceY = 3.5f;
    public bool chasePlayer = false;
    private float m_InitialSpeed; 
    private float m_InitialAngularSpeed;
    private float m_Timer;
    private bool onExit = false;
    private DebuffController m_debuffController;


    

	public override void StateUpdate() {
        float l_Distance = Vector3.Distance(transform.position, m_FinalPosition);
        m_Timer += Time.deltaTime;

        if (m_Timer >= maxChargingTime && chargeCollider.activeSelf)
        {
            m_Parent.ChangeToPreviousState();
        }

        if (!CheckDistanceY() && chargeCollider.activeSelf)
        {
            m_Parent.ChangeToPreviousState();
        }

        if ( l_Distance <= stopDistance && chargeCollider.activeSelf)
        {
			m_Parent.ChangeToPreviousState();
        }
        else
        {
            if (chasePlayer && CheckRotation())
            {
                m_FinalPosition = m_Target.target.transform.position;
                m_FinalPosition.y = transform.position.y;
                m_Agent.ResetPath();
                m_Agent.SetDestination(m_FinalPosition);
            }
        }
	}

    private bool CheckRotation()
    {
        if (m_Target.target)
        {
            float angle = Vector3.Angle(m_Target.target.transform.position - transform.position, transform.forward);
            return angle <= maxAngle / 2.0f;
        }

        return false;
    }

    private bool CheckDistanceY()
    {
        if (m_Target.target != null)
        {
            return Mathf.Abs(m_Target.target.transform.position.y - transform.position.y) <= maxDifferenceY;
        }

        return false;
    }
	
	public override void OnEnter() {
        m_Anim.SetBool("Charge", true);
        m_FinalPosition = m_Target.target.transform.position;
        m_FinalPosition.y = transform.position.y;
        m_Agent.ResetPath();
        m_Agent.SetDestination(m_FinalPosition);
        m_debuffController = GetComponent<DebuffController>();
        onExit = false;

        if (m_debuffController != null)
        {
            m_debuffController.StopSlowCoroutine();
        }

        m_InitialSpeed = m_Agent.speed;
        m_InitialAngularSpeed = m_Agent.angularSpeed;
        m_Agent.speed = m_InitialSpeed + chargeSpeed;
        m_Agent.angularSpeed = 0.0f;//angularSpeed;
        m_Timer = 0.0f;
	}

    public void ActivateCollider()
    {
        if (onExit == false)
        {
            chargeCollider.SetActive(true);
        }
    }
	
    public bool IsColliderAcive()
    {
        return chargeCollider.activeSelf;
    }

	public override void OnExit() {
        Debug.Log("Exit");
        m_Anim.SetBool("Charge", false);
        m_Agent.speed = m_InitialSpeed;
        m_Agent.angularSpeed = m_InitialAngularSpeed;
        chargeCollider.SetActive(false);
        onExit = true;
	}
	
	public override void OnStart() {
		m_Anim = GetComponentInChildren<Animator> ();
        m_Target = GetComponent<TargetSelector>();
        m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_ChargeColliderScript = chargeCollider.GetComponent<ChargeCollider>();
        m_ChargeColliderScript.damage = damage;
        m_ChargeColliderScript.pushSpeed = pushSpeed ;
        m_ChargeColliderScript.pushTime = pushTime;
	}
}
