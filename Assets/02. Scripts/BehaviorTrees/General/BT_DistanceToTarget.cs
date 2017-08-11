using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskDescription("Calcula la distancia 2D al objetivo")]
[TaskCategory("Enemies")]
public class BT_DistanceToTarget : Action {
	public SharedGameObject target;
	public SharedFloat distance;
	public SharedVector3 targetPosition;

	public override TaskStatus OnUpdate ()
	{
		Vector3 pos1 = transform.position;
		targetPosition.Value = target.Value.transform.position;
		pos1.y = targetPosition.Value.y;
		distance.Value = Vector3.Distance (pos1, targetPosition.Value);
		return TaskStatus.Success;
	}
}
