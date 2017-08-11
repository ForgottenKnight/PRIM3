using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Calcula el angulo hacia el objetivo desde tu forward y lo compara con el angulo requerido")]
[TaskCategory("Enemies")]
public class BT_AngleToTarget : Conditional {
	public float maxAngle = 30.0f;

	public SharedGameObject target;

	public override TaskStatus OnUpdate ()
	{
		Vector3 self2D = transform.position;
		self2D.y = 0f;
		Vector3 target2D = target.Value.transform.position;
		target2D.y = 0f;
		float angle = Vector3.Angle (transform.forward, target2D - self2D);
		if (angle <= maxAngle) {
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}
}
