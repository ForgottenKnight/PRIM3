using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskDescription("Lanza una roca al target si esta a la distancia adecuada. Animacion.")]
[TaskCategory("Enemies/Golem")]
public class BT_Golem_ThrowRock : Action {
	public SharedGameObject target;
	public SharedFloat distance;
	public SharedVector3 aim;

	Animator anim;

	public SharedFloat minRange = 6.0f;
	public SharedFloat maxRange = 20.0f;
	public SharedFloat delayAfterAttack = 5.0f;
	public float rockRadius = 1.0f;
	public float rockSpeed = 1.0f;
	public float animationSpeed = 1.0f;
	public float rockLifeTime = 5.0f;
	public int rockDamage = 20;
	public float rockOffset = 2.0f;
	
	public GameObject rockPrefab;
	
	[Header("Parametros de la parabola")]
	public bool parable = true;
	public bool calcBySpeed = true;
	public float parableTime = 2.0f;
	public float parableXSpeed = 10.0f;
	
	float timer;


	public override void OnAwake ()
	{
		anim = gameObject.GetComponentInChildren<Animator> ();
		timer = delayAfterAttack.Value;
		DebugDrawer.AddOval (0f, 0f, maxRange.Value, maxRange.Value, Color.cyan, gameObject);
	}

	public override TaskStatus OnUpdate ()
	{
		timer += Time.deltaTime;
		if (timer >= delayAfterAttack.Value && distance.Value <= maxRange.Value && distance.Value >= minRange.Value) {
			//DoThrow();
			anim.speed = animationSpeed;
			anim.SetTrigger("throwTrigger");
			aim.Value = target.Value.transform.position;
			timer = 0.0f;

			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}


}