using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GolemStomp : MonoBehaviour {
	[Header("Lifetime")]
	public float lifetime = 2.0f;
	public float growtime = 1.0f;
	public float shrinktime = 1.0f;
	[Tooltip("Particles stop at 'lifetime - shrinktime + particlesStop'")]
	public float particlesShrinkDuration = 0.0f;
	private bool particlesPlaying = true;
	public float damageLifetime = 2.0f;
	public float m_currentLifetime = 0.0f;
	private float m_currentDamageLifetime = 0.0f;

	[Header("Damage")]
	public float damage = 0.0f;
	public int ticks = 2;
	private float m_currentTickTimer = 0.0f;
	private bool m_damageFrame = false;

	[Header("Size and form")]
	public int numberOfRocks;
	public float maxRadius;
	public float maxScale;
	public float minScale;
	public float finalHeight;
	public Vector3 initialEffectSize;
	[Range(0.0f, 360.0f)]
	public float rockAngle;
	private float m_initialHeight;

	[Header("Components")]
	public List<GameObject> m_rockPrefabs;
	private BoxCollider m_collider;
	private GameObject m_rocks;
	private ParticleSystem m_rockParticles;
	private NavMeshObstacle m_obstacle;
	private List<GameObject> m_rockList = new List<GameObject>();

	void Start () {
		m_collider = transform.Find ("Collider").GetComponent<BoxCollider> ();
		m_collider.size = initialEffectSize;
		m_rocks = transform.Find ("Rocks").gameObject;
		m_rockParticles =  transform.Find ("RockParticles").GetComponent<ParticleSystem> ();
		m_obstacle = GetComponent<NavMeshObstacle> ();
		m_initialHeight = initialEffectSize.y;

		if (growtime + shrinktime > lifetime) {
			growtime = lifetime * 0.5f;
			shrinktime = growtime;
		}

		CreateRocks ();
	}

	void Update () {
		ManageLifetime ();
		ManageSize ();
	}

	public void DoDamage(GameObject objective) {
		if (m_currentTickTimer <= 0.0f && m_currentDamageLifetime <= damageLifetime) {
			m_damageFrame = true;
            Health h = objective.GetComponent<Health>();

            if (h)
            {
                h.Damage(damage);
            }
		} 
	}

	private void Push() {
		
	}

	private void CreateRocks() {
		for (int i = 0; i < numberOfRocks; i++) {
			int l_rand = Random.Range (0, m_rockPrefabs.Count);
			float l_radius =  Random.Range (0.05f, maxRadius);
			Vector3 l_positionXY = Random.insideUnitCircle * l_radius;
			Vector3 l_positionXZ = new Vector3 (l_positionXY.x, - finalHeight, l_positionXY.y);
			GameObject l_newRock = Instantiate (m_rockPrefabs [l_rand]);
			l_newRock.transform.parent = m_rocks.transform;
			l_newRock.transform.localPosition = l_newRock.transform.InverseTransformPoint(l_positionXZ);
			l_newRock.transform.localEulerAngles = new Vector3 (rockAngle, 0.0f, 0.0f);
			l_newRock.transform.localScale *= Mathf.Lerp (minScale, maxScale, 1 - (l_radius / maxRadius));
			m_rockList.Add (l_newRock);
		}
	}

	private void ManageLifetime() {
		//Update Timers
		m_currentDamageLifetime += Time.deltaTime;
		m_currentLifetime += Time.deltaTime;
		m_currentTickTimer -= Time.deltaTime;

		//Update Damage timers
		if (m_damageFrame) {
			float l_stepDuration = lifetime / ticks;
			m_currentTickTimer = 0.0f + l_stepDuration;
			m_damageFrame = false;
		}

		//Update Trigger if necesary
		if (m_currentLifetime > growtime) {
			m_collider.isTrigger = false;
		}

		//Particle System life
		if (m_currentLifetime > growtime && m_currentLifetime < lifetime - shrinktime) {
			if (particlesPlaying) {
				m_rockParticles.Stop ();
				particlesPlaying = false;
			}
		} else if (m_currentLifetime > lifetime - shrinktime && m_currentLifetime < lifetime) {
			if (!particlesPlaying) {
				m_rockParticles.Play ();
				particlesPlaying = true;
			}
		}

		//Kill object
		if (lifetime < m_currentLifetime) {
			if (particlesPlaying) {
				m_rockParticles.Stop ();
				particlesPlaying = false;
				m_collider.enabled = false;
			}
			if (!m_rockParticles.IsAlive()) {
				Destroy (gameObject);
			}
		}
	}

	private void ManageSize() {
		bool l_changeSize = false;
		float currentStep = 0.0f;

		if (m_currentLifetime < growtime) {
			l_changeSize = true;
			currentStep = m_currentLifetime / growtime;
		} else if (m_currentLifetime > (lifetime - shrinktime) && m_currentLifetime < lifetime) {
			l_changeSize = true;
			currentStep = (lifetime - m_currentLifetime) / shrinktime;
		}

		if (l_changeSize) {
			float l_newHeight = Mathf.Lerp (initialEffectSize.y, finalHeight, currentStep);
			Vector3 l_newSize = new Vector3 (m_collider.size.x, l_newHeight, m_collider.size.z);
			m_collider.size = m_obstacle.size = l_newSize;
			//m_collider.center = m_obstacle.center = Vector3.zero;//new Vector3 (transform.position.x, l_newHeight * 0.5f, transform.position.z);
			m_rocks.transform.localPosition = new Vector3 (0.0f, l_newHeight, 0.0f);
		}
	}
}
