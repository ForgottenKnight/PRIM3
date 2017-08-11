using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskDescription("Hace emerger al golem.")]
[TaskCategory("Enemies/Golem")]
public class BT_Golem_Emerge : Action {
	public SharedBool emerged;
	public SharedGameObject target;
	
	Animator anim;
	float timer;
	
	public override void OnAwake ()
	{
		anim = gameObject.GetComponentInChildren<Animator> ();
		anim.speed = 0f;
	}
	
	public override TaskStatus OnUpdate ()
	{
		if (emerged.Value == true) {
			return TaskStatus.Success;
		} else {
			anim.speed = 1f;
			if (anim.IsInTransition (0) || !anim.GetCurrentAnimatorStateInfo(0).IsName("Emerger")) {
				emerged.Value = true;
				if (target.Value) {
					return TaskStatus.Success;
				} else {
					return TaskStatus.Failure;
				}
			} else {
				return TaskStatus.Running;
			}
		}
	}
	
}
