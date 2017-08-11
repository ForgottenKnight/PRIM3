using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSMS_Book1_Active_Fire : MonoStateMachineState
{
    [Header("Fire parameters")]
    private Animator m_Anim;
    [Tooltip("Tiempo antes de ejecutar el ataque")]
    public float preWaitTime = 1.5f;
    [Tooltip("Tiempo para cambiar de estado tras ejecutar el ataque")]
    public float postWaitTime = 1.15f;

    public float animatorSpeed = 2f;
    public float shotSpeed = 30f;
    public float shotLifeTime = 15.0f;
    public int shotDamage = 20;
    public float shotOffsetForward = 2f;
    public float shotOffsetUpwards = 2f;
    public float slow = 2f;
    public float slowTimer = 1.5f;

    private TargetSelector m_Target;
    private Vector3 m_Aim;

    public GameObject shotPrefab;
	public GameObject shotParticles;

    [Header("Rotation parameters")]
    public float rotationSpeed = 5.0f;
    public float threshold = 0.5f;

    public override void StateUpdate()
    {
    }

    public override void OnEnter()
    {
        m_Anim.speed = animatorSpeed;
        m_Anim.SetTrigger("shotTrigger");
        m_Target = GetComponent<TargetSelector>();
    }


    public void DoAttack()
    {
        Quaternion rotation;
        //WIP
        if (m_Target.target != null)
        {
            Movement mov = m_Target.target.GetComponent<Movement>();
            float l_TargetSpeed = 1;
            if (mov)
            {
                l_TargetSpeed = mov.currentSpeed / 2.0f;
            }
            m_Aim = m_Target.target.transform.position + m_Target.target.transform.forward * l_TargetSpeed + Vector3.up;

            Vector3 pos = m_Aim - (gameObject.transform.position + gameObject.transform.forward * shotOffsetForward + Vector3.up * shotOffsetUpwards);
            rotation = Quaternion.LookRotation(pos);
        }
        else
        {
            rotation = transform.rotation;
        }

		Instantiate (shotParticles, gameObject.transform.position + gameObject.transform.forward * shotOffsetForward + Vector3.up * shotOffsetUpwards, rotation);
        GameObject projectile = Instantiate(shotPrefab, gameObject.transform.position + gameObject.transform.forward * shotOffsetForward + Vector3.up * shotOffsetUpwards, rotation) as GameObject;
        BookProjectileController l_BPC = projectile.GetComponent<BookProjectileController>();
        if(l_BPC)
        {
            l_BPC.damage = shotDamage;
            l_BPC.speed = shotSpeed;
            l_BPC.slow = slow;
            l_BPC.slowTimer = slowTimer;
        }
        ChangeState();
    }

    public void ChangeState()
    {
        m_Parent.ChangeState("Active_Chase");
    }
    
    public override void OnExit()
    {
        StopAllCoroutines();
    }

    public override void OnStart()
    {
        m_Anim = GetComponentInChildren<Animator>();
    }

}
