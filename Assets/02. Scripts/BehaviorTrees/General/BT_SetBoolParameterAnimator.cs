using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Modifica un valor bool del animator.")]
[TaskCategory("Enemies")]
public class BT_SetBoolParameterAnimator : Action {
	public string parameter;
	public bool value;
	private Animator m_Animator;

	public override void OnAwake ()
	{
		m_Animator = gameObject.GetComponentInChildren<Animator> ();
	}

	public override TaskStatus OnUpdate ()
	{
		m_Animator.SetBool (parameter, value);
		return TaskStatus.Success;
	}
}
