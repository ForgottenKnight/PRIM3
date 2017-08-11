using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSMS_Active_Throw : MonoStateMachineState {
	[Header("Throw parameters")]
	private Animator m_Anim;
	private TargetSelector m_Target;
	private bool m_Throwed;
	private bool m_CoroutineFinished;
	[Tooltip("Tiempo antes de ejecutar el ataque")]
	public float preWaitTime = 1.5f;
	[Tooltip("Tiempo para cambiar de estado tras ejecutar el ataque")]
	public float postWaitTime = 1.15f;

	private Vector3 m_Aim;
	
	public float rockRadius = 1.0f;
	public float rockSpeed = 1.0f;
	public float rockLifeTime = 5.0f;
	public int rockDamage = 20;
	public float rockOffset = 2.0f;
	public float aimOffset = 1.0f;
	
	public GameObject rockPrefab;
	public GameObject groundRock;
	
	[Header("Parametros de la parabola")]
	public bool parable = true;
	public bool calcBySpeed = true;
	public float parableTime = 2.0f;
	public float parableXSpeed = 10.0f;

    [Header("Rotation parameters")]
    public float rotationSpeed = 5.0f;
    public float threshold = 0.5f;

	public override void StateUpdate() {
		//if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Lanzamiento") && !m_Anim.IsInTransition(0) && m_Throwed == false /*&& m_CoroutineFinished == true*/) {
		//	m_Throwed = true;
		//	StartCoroutine(ChangeStateWaitTime());
		//}

        if(m_Anim.GetBool("rotating") == true)
        {
            ActionRotate();
        }
        else
        {
            ActionStopRotating();
        }
	}
	
	public override void OnEnter() {
		m_Throwed = false;
		m_CoroutineFinished = false;
		m_Anim.SetTrigger ("throwTrigger");
		m_Aim = m_Target.target.transform.position + Vector3.up * aimOffset;
		//StartCoroutine (AttackWaitTime ());

        Vector3 target2D = m_Aim;
        target2D.y = 0f;
        Vector3 position2D = transform.position;
        position2D.y = 0f;
        float angle = Vector3.Angle(transform.forward, (target2D - position2D));

        if (!CheckAngle(angle))
        { // Caso no esta encarado
            ActionRotate();
        }
        else
        {
            ActionStopRotating();
        }


        if (m_Anim.GetBool("rotating") == true)
        {
            ActionRotate();
        }



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

    public void ChangeState()
    {
        m_Parent.ChangeState("Active_Chase");
    }

	public void DoAttack() {
		if (parable) {
            DoThrowPrediction();
			//DoThrowPhysics();
		} else {
			DoThrowNoPhysics();
		}
	}
	
	public override void OnExit() {
		StopAllCoroutines ();
	}
	
	public override void OnStart() {
		m_Anim = GetComponentInChildren<Animator> ();
		m_Target = GetComponent<TargetSelector> ();
	}

    public void DoThrowPrediction()
    {
		if (m_Target.target != null) {
			Movement mov = m_Target.target.GetComponent<Movement> ();
			float l_TargetSpeed = 1;
			if (mov) {
				l_TargetSpeed = mov.currentSpeed / 2.0f;
			}
			m_Aim = m_Target.target.transform.position + m_Target.target.transform.forward * l_TargetSpeed + Vector3.up * aimOffset;
            Vector3 target2D = m_Aim;
            target2D.y = 0f;
            Vector3 position2D = transform.position;
            position2D.y = 0f;
            float angle = Vector3.Angle(transform.forward, (target2D - position2D));

            if (!CheckAngle(angle))
            { // Caso no esta encarado
                ActionRotate();
            }
            else
            {
                ActionStopRotating();
            }

			groundRock.SetActive(false);
			GameObject rock = GameObject.Instantiate (rockPrefab);
			/*Vector3 pos = rock.transform.position;
			pos = transform.position + transform.up * rockOffset;
			rock.transform.position = pos;*/
			rock.transform.position = groundRock.transform.position;
			rock.transform.rotation = groundRock.transform.rotation;
			rock.transform.LookAt (m_Aim);
			rock.transform.localScale = new Vector3 (rockRadius, rockRadius, rockRadius);

			/******************************************************TEMPORAL***********************************************/
			//RockManager rm = GetComponent<RockManager> ();
			RockManager rm = rock.GetComponentInChildren<RockManager> ();
			/******************************************************FIN TEMPORAL***********************************************/
			rm.parable = parable;
			if (calcBySpeed) {
				rm.parableSpeedX = parableXSpeed;
			} else {
				rm.parableTime = parableTime;
			}
            rm.golem = gameObject;
			rm.damage = rockDamage;
			rm.target = m_Aim;
			rm.timeToDie = rockLifeTime;
			rm.CalcSpeed ();
		}
    }

	public void DoThrowNoPhysics() {
		GameObject rock = GameObject.Instantiate (rockPrefab);
		Vector3 pos = rock.transform.position;
		pos = transform.position + transform.forward * rockOffset;
		//pos.y = aim.y;
		rock.transform.position = pos;
		rock.transform.LookAt (m_Aim);
		rock.transform.localScale = new Vector3(rockRadius, rockRadius, rockRadius);
		/******************************************************TEMPORAL***********************************************/
		//RockManager rm = rock.GetComponent<RockManager> ();
		RockManager rm = rock.GetComponentInChildren<RockManager> ();
		/******************************************************FIN TEMPORAL***********************************************/
		rm.speed = rockSpeed;
		rm.timeToDie = rockLifeTime;
		rm.damage = rockDamage;
	}
	
	public void DoThrowPhysics() {
		GameObject rock = GameObject.Instantiate (rockPrefab);
		Vector3 pos = rock.transform.position;
		pos = transform.position + transform.up * rockOffset;
		rock.transform.position = pos;
		rock.transform.LookAt (m_Aim);
		rock.transform.localScale = new Vector3(rockRadius, rockRadius, rockRadius);
		/******************************************************TEMPORAL***********************************************/
		//RockManager rm = rock.GetComponent<RockManager> ();
		RockManager rm = rock.GetComponentInChildren<RockManager> ();
		/******************************************************FIN TEMPORAL***********************************************/
		rm.parable = parable;
		if (calcBySpeed) {
			rm.parableSpeedX = parableXSpeed;
		} else {
			rm.parableTime = parableTime;
		}
		rm.damage = rockDamage;
		rm.target = m_Aim;
		rm.timeToDie = rockLifeTime;
		rm.CalcSpeed ();
	}

    bool CheckAngle(float angle)
    {
        return angle <= threshold;
    }

    void ActionRotate()
    {
        Quaternion lookRotation;
        Vector3 direction;
        m_Anim.SetBool("rotating", true);
        float l_TargetSpeed = 1;
        if (m_Target.target != null)
        {
            Movement mov = m_Target.target.GetComponent<Movement>();
            if (mov)
            {
                l_TargetSpeed = mov.currentSpeed / 2.0f;
            }
        }
        if (m_Target.target)
        {
            direction = (m_Target.target.transform.position + m_Target.target.transform.forward * l_TargetSpeed + Vector3.up * aimOffset) - transform.position;

            direction.y = transform.forward.y;
            lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void ActionStopRotating()
    {
        m_Anim.SetBool("rotating", false);
    }


}
