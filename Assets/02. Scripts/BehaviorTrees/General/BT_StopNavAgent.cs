using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Detiene el NavAgent por completo.")]
[TaskCategory("Enemies")]
public class BT_StopNavAgent : Action {
	private UnityEngine.AI.NavMeshAgent m_Agent;

	public override void OnAwake ()
	{
		m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	public override TaskStatus OnUpdate ()
	{
		if (m_Agent != null) {
			if (m_Agent.isActiveAndEnabled && m_Agent.isOnNavMesh) {
				m_Agent.velocity = Vector3.zero;
				m_Agent.ResetPath();
                m_Agent.isStopped = true;
			}
		}
		return TaskStatus.Success;
	}
}
