using UnityEngine;
using System.Collections;

using BehaviorDesigner.Runtime;

public class Pushable2 : MonoBehaviour, IPushable {
	CharacterController m_CharacterController;
	BehaviorTree m_BehaviorTree;
	UnityEngine.AI.NavMeshAgent m_Agent;
	public bool keepFacing = true;
	bool m_BeingPushed;
	float m_Speed;
	Vector3 m_Direction;

	// Use this for initialization
	void Start () {
		m_BeingPushed = false;
		m_CharacterController = GetComponent<CharacterController> ();
		m_BehaviorTree = GetComponent<BehaviorTree> ();
		m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_BeingPushed) {
			/*Vector3 l_Pos = transform.position;
			l_Pos -= m_Direction * m_Speed * Time.deltaTime;*/
			Vector3 movement = -m_Direction * m_Speed * Time.deltaTime;
			m_CharacterController.Move(movement);
			//transform.position = pos;
		}
	}

	#region IPushable implementation
	public void Push (float aSpeed, float aTime, Vector3 aSource, bool aChangeAnimation = true)
	{
		Quaternion l_OldDir = transform.rotation;
		m_BeingPushed = true;
		m_Speed = aSpeed;
		aSource.y = transform.position.y;
		transform.LookAt (aSource);
		m_Direction = transform.forward;
		if (keepFacing) {
			transform.rotation = l_OldDir;
		}
		CancelInvoke ("NoPush");
		Invoke ("NoPush", aTime);
		m_BehaviorTree.SendEvent<object> ("PushEvent", aTime);
		m_Agent.enabled = false;
	}
	#endregion

	public void NoPush() {
		m_BeingPushed = false;
		//nma.enabled = true;
	}
}
