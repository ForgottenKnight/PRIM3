using UnityEngine;
using System.Collections;

using BehaviorDesigner.Runtime;

public class Pullable : MonoBehaviour, IPullable {
	CharacterController m_CharacterController;
	BehaviorTree m_BehaviorTree;
	UnityEngine.AI.NavMeshAgent m_Agent;
	public bool keepFacing = true;
	bool m_BeingPulled;
	float m_Speed;
	Vector3 m_Direction;
	Vector3 m_Center;
	float m_StopDistance;

	public bool stopsBehavior = true;

	// Use this for initialization
	void Start () {
		m_BeingPulled = false;
		m_CharacterController = GetComponent<CharacterController> ();
		m_BehaviorTree = GetComponent<BehaviorTree> ();
		m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_BeingPulled) {
			float l_Distance = Vector3.Distance(m_Center, transform.position);
			if (l_Distance > m_StopDistance) {
				Vector3 l_Movement = -m_Direction * m_Speed * Time.deltaTime;
				m_CharacterController.Move(l_Movement);
			}
		}
	}

	#region IPushable implementation
	public void Pull (float aSpeed, float aTime, Vector3 aSource, float aStopDistance)
	{
		Quaternion l_OldDir = transform.rotation;
		m_BeingPulled = true;
		m_Speed = aSpeed;
		aSource.y = transform.position.y;
		transform.LookAt (aSource);
		m_Direction = -transform.forward;
		m_Center = aSource;
		m_StopDistance = aStopDistance;
		if (keepFacing) {
			transform.rotation = l_OldDir;
		}
		CancelInvoke ("NoPull");
		Invoke ("NoPull", aTime);
		if (stopsBehavior) {
			m_BehaviorTree.enabled = false;
		}
		m_Agent.enabled = false;
	}
	#endregion

	public void NoPull() {
		m_BeingPulled = false;
		if (stopsBehavior) {
			m_BehaviorTree.enabled = true;
		}
		//nma.enabled = true;
	}
}
