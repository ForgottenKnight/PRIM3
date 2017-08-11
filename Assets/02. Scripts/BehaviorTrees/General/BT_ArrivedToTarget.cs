using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskDescription("Comprueba si ha llegado al objetivo dado un threshold de distancia")]
[TaskCategory("Enemies")]
public class BT_ArrivedToTarget : Conditional {
	public SharedFloat distance;
	public float maxDistance;

	public override TaskStatus OnUpdate ()
	{
		return distance.Value <= maxDistance ? TaskStatus.Success : TaskStatus.Failure;
	}
}
