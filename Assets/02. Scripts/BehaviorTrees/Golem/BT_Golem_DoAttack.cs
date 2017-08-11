using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskDescription("Realiza un ataque melee si esta a la distancia adecuada. Accion.")]
[TaskCategory("Enemies/Golem")]
public class BT_Golem_DoAttack : Action {
	public SharedFloat attackLength;
	public float attackHeight = 1.0f;
	public int attackForce = 10;
	public float attackAngle = 30.0f;
	public Transform players;

	Animator anim;

	public override void OnAwake ()
	{
		anim = gameObject.GetComponentInChildren<Animator> ();
		DebugDrawer.AddAngle (0.0f, 0.0f, attackAngle, attackLength.Value, Color.red, gameObject);
		players = GameObject.FindGameObjectWithTag ("PlayerContainer").transform;
	}

	public override TaskStatus OnUpdate ()
	{
		DoAttack ();
		return TaskStatus.Success;
	}

	public void DoAttack() {
		anim.SetBool ("walking", false);
		//GameObject[] players = GameObject.FindGameObjectsWithTag ("Player"); // TODO: Game object que gestione lista de players
		for (int i = 0; i < players.childCount; ++i) {
			staticAttackCheck.checkAttack (players.GetChild(i), transform, attackLength.Value, attackAngle, attackHeight, attackForce);
		}
		GameObject saltShield = GameObject.FindGameObjectWithTag ("SaltShield");
		if (saltShield) {
			staticAttackCheck.checkAttack (saltShield.transform, transform, attackLength.Value, attackAngle, attackHeight, attackForce);
		}
	}
}
