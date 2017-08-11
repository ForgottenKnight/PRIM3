using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSMS_Book1_Active_Explode : MonoStateMachineState {
	private Animator l_Anim;
	private TargetSelector m_Target;
	[Header("Explosion parameters")]
	public float timeToExplode = 3.0f;
	private float timer;
	public GameObject explosionPrefab;
    public GameObject explosionPrefab2;

	public Vector3 explosionOffset;
	private Transform explosionPosition;
	public float explosionRadius = 10.0f;
	public float explosionInternalRadius = 3.0f;
	public float explosionForce = 50.0f;
	public bool damageByDistance = false;
	public bool pushByDistance = false;
	public bool damageByHealth = false;

	[Header("Push parameters")]
	public bool push = true;
	public float pushSpeed = 20.0f;
	public float pushTime = 1.0f;

	private Material mat;
	private Color originalColor;

	private float lastTime;
    private Health m_Health;


	public override void StateUpdate() {
		timer += Time.deltaTime;
		if (m_Target.target == null) {
			m_Parent.ChangeState ("Active_Chase");
		} else if (timer >= timeToExplode) { // Explota
			Explode ();
		} else if (Vector3.Distance(m_Target.target.transform.position, transform.position) > explosionRadius) { // Deja de intentar explotar
			m_Parent.ChangeState("Active_Chase");
		} else { // Tinte rojo
			mat.color = Color.Lerp(originalColor, Color.red, timer/timeToExplode);
		}
	}

	public override void OnEnter() {
		l_Anim.SetTrigger ("explode");
		timer = Mathf.Clamp (timer - (Time.time - lastTime), 0f, timeToExplode);
	}

	public override void OnExit() {
		l_Anim.Play ("Idle");
		lastTime = Time.time;
		mat.color = originalColor;
	}

	public override void OnStart() {
		l_Anim = GetComponentInChildren<Animator> ();
		m_Target = GetComponent<TargetSelector> ();
        m_Health = GetComponent<Health>();
		explosionPosition = GetComponentInChildren<Renderer> ().transform;
		mat = GetComponentInChildren<Renderer> ().material;
		originalColor = mat.color;
	}

	public override void OnFirstEnter() {
	}

	private void Explode() {
		if (explosionPrefab) {
			GameObject.Instantiate (explosionPrefab, explosionPosition.position + explosionOffset, Quaternion.identity);
		}
        if (explosionPrefab2)
        {
            GameObject.Instantiate(explosionPrefab2, explosionPosition.position + explosionOffset, Quaternion.identity);
        }
		List<GameObject> players = CustomTagManager.GetObjectsByTag ("Player");
		for (int i = 0; i < players.Count; ++i) {
			float distance = Vector3.Distance (transform.position, players [i].transform.position);
			if (distance <= explosionRadius) {
				float finalForce = explosionForce;
				float finalPushSpeed = pushSpeed;
				float finalPushTime = pushTime;
				float proportion = Mathf.InverseLerp (explosionInternalRadius, explosionRadius, distance);
				if (damageByDistance) {
					finalForce = Mathf.Lerp (0, explosionForce, proportion);
				}
				if (pushByDistance) {
					finalPushSpeed = Mathf.Lerp (0, pushSpeed, proportion);
					finalPushTime = Mathf.Lerp (0, pushTime, proportion);
				}
				if (damageByHealth) {
					Health h = GetComponent<Health> ();
					if (h) {
						finalForce *= h.GetHealthAsUnit ();
					}
				}

				if (staticAttackCheck.checkAttack (players [i].transform, transform, explosionRadius, 360.0f, explosionRadius, finalForce) > 0.0f) {
					if (push) {
						IPushable p = players [i].GetComponent<IPushable> ();
						if (p != null) {
							p.Push (finalPushSpeed, finalPushTime, transform.position);
						}
					}
				}
			}
		}
		GameObject saltShield = CustomTagManager.GetObjectByTag ("SaltShield");
		if (saltShield) {
			staticAttackCheck.checkAttack (saltShield.transform, transform, explosionRadius, 360.0f, explosionRadius, explosionForce);
		}
        m_Health.OnDie();
        Destroy (gameObject);
	}
}
