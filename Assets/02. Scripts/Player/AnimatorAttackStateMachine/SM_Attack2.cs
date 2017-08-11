using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class SM_Attack2 : StateMachineBehaviour {
     
    [HideInInspector]
    public GeneralPlayerController m_gc;
    [HideInInspector]
    public Movement m_Movement;

	private bool m_attackPressed = false;
    private bool m_firstTrigger = false;
    private float m_InitialSpeed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("OnStateEnter");
     //   m_InitialSpeed = m_Movement.movementSpeed;
    //    m_Movement.movementSpeed = m_InitialSpeed / 2.0f;
      //  m_Movement.SlowDown(m_Movement.movementSpeed / 2.0f, 50f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       // Debug.Log("OnStateUpdate");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //  Debug.Log("OnStateExit");   
        
    //    m_Movement.movementSpeed = m_InitialSpeed;       
        m_firstTrigger = false;
        m_attackPressed = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!m_attackPressed)
        {
            if (m_gc.player == 0 && StaticParemeters.useKeyboard)
            {
                m_attackPressed = Input.GetButtonDown("Attack0"); 
            }
            else
            {
                m_attackPressed = XCI.GetButtonDown(XboxButton.X, m_gc.controller);//Input.GetButtonDown("Attack" + m_gc.player);
            }
        }
        else
        {
            if(!m_firstTrigger)
            {
                m_firstTrigger = true;
                animator.SetTrigger("AttackTrigger1");
            }
        }
      //  Debug.Log("OnStateMove2, m_AttackPressed: " + m_attackPressed);
    }

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      //  Debug.Log("OnStateIK");
    }
}

