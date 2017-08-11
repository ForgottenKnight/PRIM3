using UnityEngine;
using System.Collections;

public class RockManager : MonoBehaviour {
	[HideInInspector]
	public float speed;
	[HideInInspector]
	public float timeToDie;
	[HideInInspector]
	public int damage;
    [HideInInspector]
    public GameObject golem;
	[HideInInspector]
	bool movement = false;
	private Rigidbody rb;
	private Animator anim;

	float timer;

	[Header("Parabola")]
	public bool parable;
	public float parableTime;
	public Vector3 target;
	float gravedad = 9.8f;
	public float parableSpeedX;
	float parableSpeedY;

	[Header("Parametros push")]
	public bool push = true;
	public float pushTime = 0.5f;
	public float pushSpeed = 10.0f;

	protected Attack attk;


	// Use this for initialization
	void Start () {
		timer = 0.0f;
		parableTime = 0.0f;
		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		movement = true;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (movement) {
			if (parable) {
				UpdatePhysics ();
			} else {
				UpdateNoPhysics ();
			}
		}
		if (timer >= timeToDie) {
			Die();
		}
	}

	public void CalcSpeed() {
		float distance2D = Vector2.Distance (new Vector2 (transform.position.x, transform.position.z), new Vector2 (target.x, target.z));
		if (parableTime != 0.0f) {
			parableSpeedX = distance2D / parableTime;
		} else {
			parableTime = distance2D / parableSpeedX;
		}
		parableSpeedY = gravedad * (parableTime / 2);
	}

	void UpdateNoPhysics() {
		transform.position = transform.position + transform.forward * speed * Time.deltaTime;
	}

	void UpdatePhysics() {
		Vector3 pos = transform.position;
		pos += transform.up * parableSpeedY * Time.deltaTime;
		pos += transform.forward * parableSpeedX * Time.deltaTime;
		transform.position = pos;
		parableSpeedY -= gravedad * Time.deltaTime;
	}

	void Die() {
		/******************************************************TEMPORAL***********************************************/
		Destroy(gameObject.transform.parent.gameObject);
		//Destroy(gameObject);
		/******************************************************FIN_TEMPORAL***********************************************/
	}

	void OnTriggerEnter(Collider col) {
		movement = false;
		rb.isKinematic = false;
		rb.useGravity = true;
		anim.enabled = true;

		if (col.gameObject.tag == "Player" || col.gameObject.tag == "SaltShield") {
			Damageable h = col.gameObject.GetComponent<Damageable>();
			if (h) {
				if (h.Damage(damage) > 0.0f) {
					if (push)
						PushBack(col.gameObject);					
					attk = col.transform.gameObject.GetComponent<Attack>();
					if(attk)
					{
						attk.currentCombo = 0;
						attk.currentPhase = 0;
						if(attk.chargeInterruptable)
						{
							attk.chargeTimer = 0.0f;
						}
					}
				}
			}
		}
		/*
        if (col.gameObject != golem)
        {
            Die();
        }*/

	}

	void PushBack(GameObject go) {
		IPushable p = go.GetComponent<IPushable> ();
		if (p != null) {
			p.Push(pushSpeed, pushTime, transform.position);
		}
	}
}
