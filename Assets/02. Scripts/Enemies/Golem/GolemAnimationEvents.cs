using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAnimationEvents : MonoBehaviour {

    public MSMS_Active_Throw stateThrow;
    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    private float m_InitialMovementSpeed = 4.5f;
    private Coroutine CreateFieldEffect;
    public MSMS_Active_AreaAttack stateAreaAttack;
    public MSMS_Active_Charge stateCharge;
	public GameObject groundRock;

	// Use this for initialization
	void Start () {
        stateThrow = transform.parent.GetComponent<MSMS_Active_Throw>();
        stateAreaAttack = transform.parent.GetComponent<MSMS_Active_AreaAttack>();
        stateCharge = transform.parent.GetComponent<MSMS_Active_Charge>();
	}
	
	public void ThrowRock()
    {
        if(stateThrow)
        {
            stateThrow.DoAttack();
        }
    }

	public void GrabRock()
	{
		if(groundRock)
		{
			groundRock.SetActive(true);
		}
	}


    public void ThrowRockEnd()
    {
        if(stateThrow)
        {
            stateThrow.ChangeState();
        }
    }

    public void StartMovevement()
    {
        navMeshAgent.speed = m_InitialMovementSpeed;
    }

    public void StopMovevement()
    {
        m_InitialMovementSpeed = navMeshAgent.speed;
        navMeshAgent.speed = 0.0f;
    }

    public void Stomp()
    {
        if (stateAreaAttack)
        {
            stateAreaAttack.DoAttack();
        }
    }

    public void StompEnd()
    {
        if (stateAreaAttack)
        {
            stateAreaAttack.ChangeState();
        }
    }

    public void ActiveChargeCollider()
    {
        if (stateCharge && !stateCharge.IsColliderAcive())
        {
            stateCharge.ActivateCollider();
        }
    }
}
