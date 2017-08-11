using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskDescription("Lanza una roca al target si esta a la distancia adecuada. Accion")]
[TaskCategory("Enemies/Golem")]
public class BT_Golem_DoThrowRock : Action {
	public SharedVector3 aim;
	
	public float rockRadius = 1.0f;
	public float rockSpeed = 1.0f;
	public float rockLifeTime = 5.0f;
	public int rockDamage = 20;
	public float rockOffset = 2.0f;
	
	public GameObject rockPrefab;
	
	[Header("Parametros de la parabola")]
	public bool parable = true;
	public bool calcBySpeed = true;
	public float parableTime = 2.0f;
	public float parableXSpeed = 10.0f;

	Animator anim;

	public override void OnAwake ()
	{
		anim = gameObject.GetComponentInChildren<Animator> ();
	}

	public override TaskStatus OnUpdate ()
	{
		DoThrow ();
		return TaskStatus.Success;
	}

	public void DoThrow() {
		if (parable) {
			DoThrowPhysics();
		} else {
			DoThrowNoPhysics();
		}
		anim.SetBool ("walking", false);
	}
	
	public void DoThrowNoPhysics() {
		GameObject rock = GameObject.Instantiate (rockPrefab);
		Vector3 pos = rock.transform.position;
		pos = transform.position + transform.forward * rockOffset;
		//pos.y = aim.y;
		rock.transform.position = pos;
		rock.transform.LookAt (aim.Value);
		rock.transform.localScale = new Vector3(rockRadius, rockRadius, rockRadius);
		RockManager rm = rock.GetComponent<RockManager> ();
		rm.speed = rockSpeed;
		rm.timeToDie = rockLifeTime;
		rm.damage = rockDamage;
	}
	
	public void DoThrowPhysics() {
		GameObject rock = GameObject.Instantiate (rockPrefab);
		Vector3 pos = rock.transform.position;
		pos = transform.position + transform.up * rockOffset;
		rock.transform.position = pos;
		rock.transform.LookAt (aim.Value);
		rock.transform.localScale = new Vector3(rockRadius, rockRadius, rockRadius);
		RockManager rm = rock.GetComponent<RockManager> ();
		rm.parable = parable;
		if (calcBySpeed) {
			rm.parableSpeedX = parableXSpeed;
		} else {
			rm.parableTime = parableTime;
		}
		rm.damage = rockDamage;
		rm.target = aim.Value;
		rm.timeToDie = rockLifeTime;
		rm.CalcSpeed ();
	}
}
