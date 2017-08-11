using UnityEngine;
using System.Collections;

public class ApplyGravity : MonoBehaviour {
	private CharacterController m_CharacterController;
	public float gravity = 20.0f;
	public float fallSpeedLimit = 40.0f;
	public float fallingSpeedThreshold = 1.5f;
	public float timeToFall = 0.1f;
    public float corrutineTimer = 1f;
    public LayerMask raycastMask;
    public bool fallAnimation = true;
    public Animator animatorController;

	private float m_Speed;
	private float m_FallTime;
	private bool m_Falling;

    public bool fallingDebug;


	// Use this for initialization
	void Start () {
		m_Falling = false;
		m_Speed = 0f;
        m_FallTime = 0f;
		m_CharacterController = GetComponent<CharacterController> ();
	}

    void OnEnable()
    {
        if (fallAnimation)
        {
            StartCoroutine("CheckFalling");
        }
    }

    void OnDisable()
    {
        if (fallAnimation)
        {
            StopAllCoroutines();
        }
    }

    IEnumerator CheckFalling()
    {
        while (true)
        {
            Vector3 lastPos = gameObject.transform.position;
            Vector3 up = transform.up;
            Ray ray = new Ray(lastPos, -up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f, raycastMask))
            {
                if (hit.collider.gameObject.layer != gameObject.layer)
                {
                    if(FallingAnimation())
                    {
                        animatorController.SetTrigger("Land");
                        StopCoroutine("CheckFalling");
                    }
                }
            }
            else
            {
                if (!FallingAnimation())
                {
                    animatorController.SetTrigger("Fall");
                    if(animatorController.GetBool("Stun"))
                    {
                        animatorController.SetBool("Stun", false);
                    }
                }
            }
            yield return new WaitForSeconds(corrutineTimer);
        }
    }
	
    public bool FallingAnimation()
    {
        bool l_Active = animatorController.GetAnimatorTransitionInfo(0).IsUserName("BeginFallTransition");
        l_Active = l_Active || animatorController.GetCurrentAnimatorStateInfo(0).IsName("BeginFall");
        l_Active = l_Active || animatorController.GetAnimatorTransitionInfo(0).IsUserName("BLT");
        l_Active = l_Active || animatorController.GetAnimatorTransitionInfo(0).IsUserName("FallTransition");
        l_Active = l_Active || animatorController.GetCurrentAnimatorStateInfo(0).IsName("LoopFall");

        return l_Active;      
    }


	// Update is called once per frame
	void Update () {
		if (m_Falling) {
            m_FallTime += Time.deltaTime;
		} else {
            m_FallTime = 0f;
		}
        // Aplicamos gravedad
		if (m_CharacterController.isGrounded) {
			m_Speed = 0.0f;
			m_Falling = false;
		} else {
			m_Speed += gravity * Time.deltaTime;
			m_Speed = Mathf.Clamp (m_Speed, 0.0f, fallSpeedLimit);
		}
		if (m_Speed > fallingSpeedThreshold) {
			m_Falling = true;
		}
        if (m_CharacterController.enabled == true && IsFalling())
        {
            m_CharacterController.Move(-Vector3.up * m_Speed * Time.deltaTime);
        }
	}

	public bool IsFalling() {
        fallingDebug = m_FallTime >= timeToFall;

        return m_FallTime >= timeToFall;
	}
}
