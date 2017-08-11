using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RockSpawn : MonoBehaviour {
	public List<ParticleSystem> particles = new List<ParticleSystem>();
	public bool useParticles;
	public Transform targetPosition;
	Vector3 targetPos;
	public bool activated = false;
	public float movSpeed = 1.0f;
	protected bool finishSpawn = false;
    protected Vector3 m_InitPosition;
    protected bool returning = false;
    protected bool finishReturn = false;

	// Use this for initialization
	void Start () {
        m_InitPosition = transform.position;
        if (targetPosition)
        {
            targetPos = targetPosition.position;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(activated && ! finishSpawn)
		{
			
			Vector3 pos = Vector3.MoveTowards(transform.position, targetPos, movSpeed * Time.deltaTime);
			transform.position = pos;
			if (pos == targetPos) {
				finishSpawn = true;
				ActivateParticles (false);
			}
		}

        if(returning && !finishReturn)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, m_InitPosition, movSpeed * Time.deltaTime);
            transform.position = pos;
			if (pos == m_InitPosition) {
				finishReturn = true;
				ActivateParticles (false);
			}
        }
	}

	public void Activate() {
		activated = true;
		if (useParticles) {
			ActivateParticles (true);
		}
	}

	public void ActivateParticles(bool aActive) {
		if (aActive) {
			for (int i = 0; i < particles.Count; i++) {
				particles [i].Play ();
			}
		} else {
			for (int i = 0; i < particles.Count; i++) {
				particles [i].Stop ();
			}
		}
	}

    public void Return()
    {
        returning = true;
		if (useParticles) {
			ActivateParticles (true);
		}
    }
}
