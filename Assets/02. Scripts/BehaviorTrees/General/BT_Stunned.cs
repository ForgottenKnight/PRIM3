using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskDescription("Gestiona el stun. Recive un evento llamado PushEvent con el tiempo del stun. Pasado ese tiempo detiene el stun. Interrumpe el resto del BT al recibir el mensaje.")]
[TaskCategory("Enemies")]
public class BT_Stunned : Conditional {
	public Interrupt interruptTasks;
	private bool m_BeingPushed;
	private BehaviorTree m_BehaviorTree;
	private Animator m_Animator;

	public override void OnAwake ()
	{
		m_BehaviorTree = gameObject.GetComponent<BehaviorTree> ();
		m_BeingPushed = false;
		m_BehaviorTree.RegisterEvent< object >("PushEvent", PushEvent);
		m_Animator = gameObject.GetComponentInChildren<Animator> ();
	}

	public override TaskStatus OnUpdate ()
	{
		return m_BeingPushed ? TaskStatus.Failure : TaskStatus.Success;
	}

	public void PushEvent(object arg1)
	{
		StopCoroutine (StopPush ((float)arg1));
		m_BeingPushed = true;
		StartCoroutine(StopPush ((float) arg1));
		interruptTasks.DoInterrupt (TaskStatus.Failure);
		StopAllAnimations ();
	}

	void StopAllAnimations() {
		m_Animator.SetBool ("walking", false);
		m_Animator.Play ("Idle");
	}

	public IEnumerator StopPush(float time) {
		yield return new WaitForSeconds(time);
		m_BeingPushed = false;
	}
}
