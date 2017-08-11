using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Enemies")]
public class BT_SetAnimator : Action {
	public SharedGameObject animGO;
	// Use this for initialization
	public override void OnStart ()
	{
		animGO.Value = gameObject.GetComponentInChildren<Animator> ().gameObject;
	}
	
	public override TaskStatus OnUpdate ()
	{
		return TaskStatus.Success;
	}
}
