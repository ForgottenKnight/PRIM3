using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebuffController : MonoBehaviour {
	private float m_initialSpeed;
	private NavMeshAgent m_agent;
	private IEnumerator l_coroutine;
	// Use this for initialization
	void Start () {
		m_agent = GetComponent<NavMeshAgent> ();
		if (m_agent) {
			m_initialSpeed = m_agent.speed;	
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Slow(float aSlow, float aDuration) {
        MSMS_Active l_MSMS = GetComponent<MSMS_Active>();
        bool l_slow = true;
        if(l_MSMS)
        {
            if (l_MSMS.m_CurrentSubState && l_MSMS.m_CurrentSubState.stateName == "Active_Charge")
            {
                l_slow = false;
            }
        }

        if (l_slow)
        {
            if (l_coroutine != null)
            {
                StopCoroutine(l_coroutine);
            }
            l_coroutine = SlowCoroutine(aSlow, aDuration);
            StartCoroutine(l_coroutine);
        }
	}

    public void StopSlowCoroutine()
    {
        if (l_coroutine != null)
        {
            StopCoroutine(l_coroutine);
        }

        if (m_agent && m_agent.speed < m_initialSpeed)
        {
            m_agent.speed = m_initialSpeed;
        }
    }

	IEnumerator SlowCoroutine(float aSlow, float aDuration) {
		float l_interpolation = 0.0f;
		float l_timer = 0.0f;
		float l_minSpeed =  m_initialSpeed * (1.0f - aSlow);

		m_agent.speed = l_minSpeed;

		while (m_agent.speed < m_initialSpeed) {
			l_timer += Time.deltaTime;
			l_interpolation = l_timer / aDuration;
			m_agent.speed = Mathf.Lerp (l_minSpeed, m_initialSpeed, l_interpolation);
			yield return null;
		}
	}
}
