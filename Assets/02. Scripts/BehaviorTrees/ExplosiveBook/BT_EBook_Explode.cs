using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskDescription("Explota el libro.")]
[TaskCategory("Enemies/ExplosiveBook")]
public class BT_EBook_Explode : Action {
	public float explosionRadius;
	public float explosionForce;
	public GameObject explosionPrefab;

	public bool damageByDistance = false;
	public bool pushByDistance = false;
	public bool damageByHealth = false;
	
	public bool push = true;
	public float pushSpeed = 20.0f;
	public float pushTime = 1.0f;

	Transform explosionPosition;

	public override void OnAwake ()
	{
		explosionPosition = gameObject.GetComponentInChildren<Renderer> ().transform;
	}

	public override TaskStatus OnUpdate ()
	{
		Explode ();
		GameObject.Destroy (gameObject);
		return TaskStatus.Success;
	}

	void Explode() {
		if (explosionPrefab) {
			GameObject.Instantiate (explosionPrefab, explosionPosition.position, Quaternion.identity);
		}
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player"); // TODO: Game object que gestione lista de players
		for (int i = 0; i < players.Length; ++i) {
			float finalPushSpeed = pushSpeed;
			float finalPushTime = pushTime;
			float finalForce = explosionForce;
			float distance = 0.0f;
			if (damageByDistance || pushByDistance) {
				distance = Vector3.Distance(transform.position, players[i].transform.position);
			}
			if (damageByDistance) {
				finalForce = (explosionRadius-distance)/explosionRadius * finalForce;
			}
			if (pushByDistance) {
				finalPushSpeed = (explosionRadius-distance)/explosionRadius * finalPushSpeed;
				finalPushTime = (explosionRadius-distance)/explosionRadius * finalPushTime;
			}
			if (damageByHealth) {
				Health h = GetComponent<Health>();
				if (h) {
					finalForce *= h.GetHealthAsUnit();
				}
			}
			if (staticAttackCheck.checkAttack (players[i].transform, transform, explosionRadius, 360.0f, explosionRadius, finalForce) > 0.0f) {
				if (push) {
					IPushable p = players[i].GetComponent<IPushable>();
					if (p != null) {
						p.Push(finalPushSpeed, finalPushTime, transform.position);
					}
				}
			}
		}
		GameObject saltShield = GameObject.FindGameObjectWithTag ("SaltShield");
		if (saltShield) {
			staticAttackCheck.checkAttack (saltShield.transform, transform, explosionRadius, 360.0f, explosionRadius, explosionForce);
		}
	}

}
