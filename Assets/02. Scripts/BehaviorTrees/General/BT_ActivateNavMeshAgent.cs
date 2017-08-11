
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Activa o desactiva el NavMeshAgent.")]
[TaskCategory("Enemies")]
public class BT_ActivateNavMeshAgent : Action {
	public bool enabled;

	UnityEngine.AI.NavMeshAgent nma;

	public override void OnAwake ()
	{
		nma = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}

	public override void OnStart ()
	{
		nma.enabled = enabled;
	}

	public override TaskStatus OnUpdate ()
	{
		return TaskStatus.Success;
	}
}
