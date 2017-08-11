using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Comprueba si esta en el suelo.")]
[TaskCategory("Enemies")]
public class BT_Grounded : Conditional {
	public float maxDistance;
	public override TaskStatus OnUpdate ()
	{
		/*ApplyGravity l_Gravity = GetComponent<ApplyGravity> ();
		if (l_Gravity != null) {
			return l_Gravity.IsFalling() ? TaskStatus.Failure : TaskStatus.Success;
		} else {*/
			if (Physics.Raycast (transform.position, -Vector3.up, maxDistance)) {
				return TaskStatus.Success;
			}
		//}
		return TaskStatus.Failure;
	}
}
