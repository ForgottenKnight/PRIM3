using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class MercuryTeleport : MonoBehaviour { //TODO: Comprobar que el enemigo sigue vivo
	[Header("Lifetime")]
	public float m_dotDuration = 0.0f;
	public float m_dotDissapearTime = 0.0f;

	[Header("Damage")]
	public float m_damage = 0.0f;
	public float m_damageInterval = 0.0f;

	[Header("Slow")]
	[Range(0, 1.0f)]
	public float m_slow = 0.0f;
	public float m_slowInterval = 0.0f;
	public float m_slowDuration = 0.0f;

	[Header("Size")]
	public Vector3 effectSize;

	private float m_lifetime = 0.0f;
	public bool m_disappearing = false;

	private float m_pulseDamageInterval = 0.0f;
	private float m_pulseSlowInterval = 0.0f;
	private float m_pulseDamageTimer = 0.0f;
	private float m_pulseSlowTimer = 0.0f;

	private BoxCollider m_collider;
	private ParticleSystem m_particles;

	// Use this for initialization
	void Start () {
		m_collider = transform.Find ("Collider").GetComponent<BoxCollider> ();
		m_particles = transform.Find ("Particles").GetComponent<ParticleSystem> ();
		m_pulseDamageTimer = m_pulseDamageInterval;
		m_pulseSlowTimer = m_pulseSlowInterval;
		m_collider.size = effectSize;

		m_pulseDamageInterval = m_damageInterval;
		m_pulseSlowInterval = m_slowInterval;
	}
	
	// Update is called once per frame
	void Update () {
		ManagePulse ();
		ManageLifetime ();
	}

	public void TriggerPulseDamage(GameObject enemy) {
		if (m_pulseDamageTimer <= 0) {
			Damageable l_damageable = enemy.GetComponent<Damageable> ();
			if (l_damageable != null) {
				l_damageable.ConditionalDamage (m_damage, false, true, true);
			}
			m_pulseDamageTimer = m_pulseDamageInterval;
		}
	}

	public void TriggerPulseSlow(GameObject enemy) {
		if (m_pulseSlowTimer <= 0) {
			DebuffController l_debuffController = enemy.GetComponent<DebuffController> ();
			if (l_debuffController != null) {
				l_debuffController.Slow (m_slow, m_slowDuration);
			}
			m_pulseSlowTimer = m_pulseSlowInterval;
		}
	}

	private void ManagePulse() {
		if (m_pulseDamageTimer > 0) {
			m_pulseDamageTimer -= Time.deltaTime;
		}
		if (m_pulseSlowTimer > 0) {
			m_pulseSlowTimer -= Time.deltaTime;
		}
	}

	private void ManageLifetime() {
		m_lifetime += Time.deltaTime;

		if (m_lifetime >= m_dotDissapearTime && !m_disappearing) {
			ParticleSystem l_ps = transform.Find ("Particles").GetComponent<ParticleSystem> ();
			l_ps.Stop ();
			m_disappearing = true;
		}

		if (m_lifetime >= m_dotDuration) {
			Destroy (gameObject);
		}
	}


}
