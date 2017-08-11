using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Comprueba si hay suelo en frente.")]
[TaskCategory("Enemies")]
public class BT_GroundInFront : Conditional {
	private Transform m_RaycastFront;

	public override void OnAwake ()
	{
		m_RaycastFront = transform.Find("RaycastFront");
	}


	public override TaskStatus OnUpdate ()
	{
		if (m_RaycastFront == null) {
			return TaskStatus.Success;
		}
		if (Physics.Raycast (m_RaycastFront.position, -Vector3.up, 0.5f)) {
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}
}
