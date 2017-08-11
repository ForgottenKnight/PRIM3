using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SulphurArea : MonoBehaviour {
	public float duration = 0.5f;
	public float damage = 20.0f;

	[Header("Parametros push")]
	public bool push = true;
	public float pushTime = 0.5f;
	public float pushSpeed = 10.0f;

	List<GameObject> alreadyDamaged;

	public LayerMask affectingLayers;

	SulphurController sc;

	// Use this for initialization
	void Start () {
		alreadyDamaged = new List<GameObject> ();
		Invoke ("Die", duration);
		sc = FindObjectOfType<SulphurController> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col) {
		bool lhit = true;
		if (affectingLayers == (affectingLayers | (1 << col.gameObject.layer))) {
			foreach (GameObject enemy in alreadyDamaged) {
				if (enemy == col.gameObject) {
					lhit = false;
				}
			}
			if (lhit) {
				Damageable h = col.transform.GetComponent<Damageable> ();
				if (h) {
					float dmg = h.Damage (damage);
					if (dmg > 0.0f) {
						if (push)
							PushBack (col.gameObject);
						if (sc && col.gameObject.tag != "Player")
							sc.aggro += dmg;
					}
					alreadyDamaged.Add (col.gameObject);
				}
			}
		}
	}

	void Die() {
		Destroy (gameObject);
	}

	void PushBack(GameObject go) {
		IPushable l_Pushable = go.GetComponent<IPushable> ();
		if (l_Pushable != null) {
			l_Pushable.Push (pushSpeed, pushTime, transform.position);
		}
	}
}
