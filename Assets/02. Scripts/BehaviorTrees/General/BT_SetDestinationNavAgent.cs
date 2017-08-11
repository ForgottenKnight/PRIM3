
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Establece un destino para el nav agent.")]
[TaskCategory("Enemies")]
public class BT_SetDestinationNavAgent : Action {
	private UnityEngine.AI.NavMeshAgent m_Agent;
	public SharedVector3 target;

	public override void OnAwake ()
	{
		m_Agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	public override TaskStatus OnUpdate ()
	{
		if (m_Agent != null) {
			if (m_Agent.isActiveAndEnabled && m_Agent.isOnNavMesh) {
				m_Agent.SetDestination(target.Value);
				return TaskStatus.Success;
			}
		}
		return TaskStatus.Failure;
	}
}
