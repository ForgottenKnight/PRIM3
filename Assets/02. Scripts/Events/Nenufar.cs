using UnityEngine;
using System.Collections;

public class Nenufar : MonoBehaviour {
	public float speed = 10.0f;
	public Transform targetTransform;
	public Nenufar targetNenufar;
	public bool activated = false;
	public bool onePlayerMode = false;

	Vector3 initPos;
	Vector3 targetPos;

	int framesToStop = 3;
	int frames;


	// Use this for initialization
	void Start () {
		initPos = transform.position;
		if (targetTransform != null) {
			targetPos = targetTransform.position;
		} else {
			targetPos = Vector3.zero;
		}
		frames = framesToStop;
	}
	
	// Update is called once per frame
	void Update () {
		if (targetTransform != null) {
			if (activated) {
				frames = framesToStop;
				if (Vector3.Distance(transform.position, targetPos) > 0.1f) {
					Vector3 pos = Vector3.MoveTowards (transform.position, targetPos, speed * Time.deltaTime);
					transform.position = pos;
				}
			} else {
				if (frames == 0) {
					if (Vector3.Distance(transform.position, initPos) > 0.1f) {
						Vector3 pos = Vector3.MoveTowards (transform.position, initPos, speed * Time.deltaTime);
						transform.position = pos;
					}
				} else {
					frames--;
				}
			}
			if (!onePlayerMode)
				activated = false;
		}
	}

	void OnTriggerStay(Collider col) {
		if (col.tag == "Player" && targetNenufar != null) {
			targetNenufar.activated = true;
		}
	}
}
