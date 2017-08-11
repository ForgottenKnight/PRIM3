using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskDescription("Realiza un ataque melee si esta a la distancia adecuada. Animacion.")]
[TaskCategory("Enemies/Golem")]
public class BT_Golem_Attack : Action {
	public SharedGameObject target;
	public SharedFloat distance;

	public SharedFloat range;
	public float delayAfterAttack = 2.0f;	

	public float attackSpeed = 1.0f;
	public float maxDifferenceY = 1.5f;

	Animator anim;
	float timer;

	public override void OnAwake ()
	{
		anim = gameObject.GetComponentInChildren<Animator> ();
		timer = delayAfterAttack;
	}

	public override TaskStatus OnUpdate ()
	{
		timer += Time.deltaTime;
		if (timer >= delayAfterAttack) {
			float lessRange = range.Value * 0.9f;
			if (distance.Value <= lessRange && ((Mathf.Abs (target.Value.transform.position.y - transform.position.y) <= maxDifferenceY) || maxDifferenceY == 0.0f)) {
				anim.speed = attackSpeed;
				anim.SetTrigger("attackTrigger");
				timer = 0.0f;
				return TaskStatus.Success;
			}
		}
		return TaskStatus.Failure;
	}

}
