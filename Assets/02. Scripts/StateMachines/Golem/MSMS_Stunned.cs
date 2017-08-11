using UnityEngine;
using System.Collections.Generic;

public class MSMS_Stunned : MonoStateMachineState, IPushable {
	[Header("Stun parameters")]
	public bool keepFacing;
	private Animator m_Anim;
	private UnityEngine.AI.NavMeshAgent m_Agent;
	private CharacterController m_CharacterController;
	private float m_StunTime;
	private float m_Timer;
	private float m_Speed;
	private Vector3 m_Direction;
    public LayerMask stunMask;
    public bool movable = true;

    ApplyGravity m_AG;


	public override void StateUpdate() {
		Vector3 movement = -m_Direction * m_Speed * Time.deltaTime;
		m_CharacterController.Move(movement);
		m_Timer += Time.deltaTime;
        if (m_Timer >= m_StunTime && !m_AG.FallingAnimation() && CheckGrounded())
        {
			m_Parent.ChangeToPreviousState();
		}
	}
	
	public override void OnEnter() {
		m_Anim.SetBool ("walking", false);
		m_Anim.SetBool ("rotating", false);
        m_Anim.SetBool("Stun", true);
		//m_Anim.Play ("Idle");
		m_Agent.enabled = false;
		m_Timer = 0f;
        if (movable)
            m_AG.enabled = true;

    }
	
	public override void OnExit() {
        m_Anim.SetBool("Stun", false);
        if (movable)
            m_AG.enabled = false;
    }
	
	public override void OnStart() {
		m_Anim = GetComponentInChildren<Animator> ();
		m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		m_CharacterController = GetComponent<CharacterController> ();
        m_AG = GetComponent<ApplyGravity>();

    }

    bool CheckGrounded()
    {
        Vector3 lastPos = transform.position;
        Vector3 up = transform.up;
        Ray ray = new Ray(lastPos, -up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f, stunMask))
        {
            if (hit.collider.gameObject.layer != gameObject.layer)
            {
                return true;
            }
        }
        return false;
    }

	#region IPushable implementation

	public void Push (float aSpeed, float aTime, Vector3 aSource, bool aChangeAnimation)
	{
		Quaternion l_OldDir = transform.rotation;
        aSource.y = transform.position.y;
		m_StunTime = aTime;
		m_Speed = aSpeed;
		aSource.y = transform.position.y;
		transform.LookAt (aSource);
		m_Direction = transform.forward;
		if (keepFacing) {
			transform.rotation = l_OldDir;
		}
		m_Parent.ChangeState (stateName);
	}

	#endregion
}
