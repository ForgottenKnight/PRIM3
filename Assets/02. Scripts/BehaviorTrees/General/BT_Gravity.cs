using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Aplica gravedad al enemigo.")]
[TaskCategory("Enemies")]
public class BT_Gravity : Action {
	CharacterController cc;
	public float fallSpeed = 20.0f;

	public override void OnAwake ()
	{
		cc = GetComponent<CharacterController> ();
	}

	public override TaskStatus OnUpdate ()
	{
		cc.Move (-Vector3.up * fallSpeed * Time.deltaTime);
		return TaskStatus.Success;
	}
}
