using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GeneralPlayerController))]
[AddComponentMenu("PRIM3/CharacterScripts/Core/Player Manager")] 
public class PlayerManager : MovementManager, IPausable {
	public bool stoppedOffLimits = false;
	Camera mc;
	Vector3 lastPosition;

	private bool m_Stopped;
	private int m_PauseCount = 0;
    [HideInInspector]
    public Bounds eventBoundingBox;
    [HideInInspector]
    public bool insideBoundingBox;

	private Jump m_Jump; // Componente jump, del cual siempre se ejecuta el update para poder caer.

	// Use this for initialization
	protected override void Start () {
		allBehaviours.Clear ();
		blockedBehaviours.Clear ();
		executedBehaviours.Clear ();
		base.Start();
		mc = Camera.main;
		m_Stopped = false;
		m_Jump = GetComponent<Jump> ();

        insideBoundingBox = false;
	}
	
	// Update is called once per frame
	protected override void Update() {
		base.Update();
	}

	protected override void DoManagement() {
		if (!noInput) {
			for (int i = 0; i < executedBehaviours.Count; i++) {
				executedBehaviours [i].GetInput ();
			}
		}
		if (!noUpdate) {
			for (int i = 0; i < executedBehaviours.Count; i++) {
				executedBehaviours [i].ActionUpdate ();
			}
		} else {
			m_Jump.ActionUpdate();
		}
		lastPosition = transform.position;
        if (insideBoundingBox == true)
        {
            if (eventBoundingBox.Contains(transform.position + movement))
            {
                cc.Move(movement);
            }
        }
        else
        {
            cc.Move(movement);
        }
		movement = Vector3.zero;
		if (!noInput && !noUpdate) {
			m_Stopped = false;
			if (!mc.GetComponent<PrimeCamera> ().CheckPlayerOk (transform)) {
				if (stoppedOffLimits) {
					transform.position = lastPosition;
				}
			}
		} else {
			if (!m_Stopped) {
				Animator anim = GetComponentInChildren<Animator>();
				if (anim) {
					anim.SetBool("Moving", false);
				}

				m_Stopped = true;
			}
		}
	}

	#region IPausable implementation

	public void Pause ()
	{
		noInput = true;
		noUpdate = true;
		m_PauseCount++;
	}

	public void Unpause ()
	{
		m_PauseCount--;
		if (m_PauseCount <= 0) {
			noInput = false;
			noUpdate = false;
			m_PauseCount = 0;
		}
	}

	#endregion
}
