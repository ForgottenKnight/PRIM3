using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskDescription("Rellena el gauge de explosion y devuelve success cuando esta lleno.")]
[TaskCategory("Enemies/ExplosiveBook")]
public class BT_EBook_ExplodeCheck : Action {
	public SharedGameObject target;
	public SharedFloat distance;

	public float range;

	Animator anim;
	Material mat;
	float gauge;
	public float gaugeTime = 2.0f;
	public float decreaseRatio = 0.5f;
	public Color finalColor = Color.red;
	public Color initColor = Color.white;
	public float maxAnimSpeed = 3.0f;
	Color originalColor;

	public override void OnAwake ()
	{
		anim = GetComponent<Animator> ();
		mat = gameObject.GetComponentInChildren<Renderer> ().material;
		originalColor = mat.color;
		gauge = 0.0f;
	}

	public override TaskStatus OnUpdate ()
	{
		if (distance.Value <= range) {
			gauge += Time.deltaTime;
			if (gauge >= gaugeTime) {
				return TaskStatus.Success;
			}
		} else {
			gauge -= Time.deltaTime * decreaseRatio;
			Mathf.Clamp(gauge, 0.0f, gaugeTime);
		}
		mat.color = Color.Lerp(initColor * originalColor, finalColor, (gauge / gaugeTime));
		Animate ();
		return TaskStatus.Failure;
	}

	void Animate() {
		if (anim) {
			if (gauge > 0.0f) {
				anim.SetBool("vibrating", true);
				anim.speed = (gauge/gaugeTime) * maxAnimSpeed;
			} else if (gauge <= 0.0f) {
				anim.SetBool("vibrating", false);
			}
		}
	}

}
