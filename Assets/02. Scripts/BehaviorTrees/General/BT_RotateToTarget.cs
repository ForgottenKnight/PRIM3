using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Rota hacia un objetivo con una velocidad determinada. Si ya esta encarado, lanza failure.")]
[TaskCategory("Enemies")]
public class BT_RotateToTarget : Action {
	public float rotationSpeed = 5.0f;
	public SharedGameObject target;

	public float threshold = 0.5f;

	Quaternion lookRotation;
	Vector3 direction;

	public override TaskStatus OnUpdate ()
	{
		direction = (target.Value.transform.position - transform.position).normalized;
		direction.y = transform.forward.y;
		if (Vector3.Angle (transform.forward, direction) <= threshold) {
			return TaskStatus.Success;
		}
		lookRotation = Quaternion.LookRotation (direction);
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
		return TaskStatus.Success;
	}

}
