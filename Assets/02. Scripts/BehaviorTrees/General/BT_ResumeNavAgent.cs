
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Establece un destino para el nav agent.")]
[TaskCategory("Enemies")]
public class BT_ResumeNavAgent : Action {
	private UnityEngine.AI.NavMeshAgent m_Agent;

	public override void OnAwake ()
	{
		m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	public override TaskStatus OnUpdate ()
	{
		if (m_Agent != null) {
			if (m_Agent.isActiveAndEnabled && m_Agent.isOnNavMesh) {
                m_Agent.isStopped = false;
				return TaskStatus.Success;
			}
		}
		return TaskStatus.Failure;
	}
}
