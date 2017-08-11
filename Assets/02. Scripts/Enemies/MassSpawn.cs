using UnityEngine;
using System.Collections;

public class MassSpawn : MonoBehaviour {
	[System.Serializable]
	public class EnemySpawn {
		public GameObject prefab;
		public float atTime;
		public Transform targetPosition;
		[HideInInspector]
		public bool spawned = false;
	}

	public Transform spawnPosition;
	public Transform[] possibleTargets;
	public EnemySpawn[] enemies;

	Transform enemyContainer;
	
	float timer;
	float maxTime;
	bool activated;

	// Use this for initialization
	void Start () {
		enemyContainer = GameObject.FindGameObjectWithTag ("EnemiesContainer").transform;
		activated = false;
		timer = 0.0f;
		maxTime = 0.0f;
		for (int i = 0; i < enemies.Length; ++i) {
			maxTime = Mathf.Max (maxTime, enemies[i].atTime);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (activated) {
			timer += Time.deltaTime;

			for (int i = 0; i < enemies.Length; ++i) {
				if (!enemies[i].spawned) {
					if (timer >= enemies[i].atTime) {
						SpawnEnemy(enemies[i]);
					}
				}
			}

			if (timer >= maxTime) {
				Destroy (gameObject);
			}
		}
	}

	public void Activate() {
		activated = true;
	}

	public void SpawnEnemy(EnemySpawn es) {
		GameObject go = (GameObject)Instantiate (es.prefab, es.targetPosition.position, es.targetPosition.rotation);
		go.transform.SetParent (enemyContainer);
		/*NavMeshAgent nma = go.GetComponent<NavMeshAgent> ();
		if (nma && possibleTargets.Length > 0) {
			if (es.targetPosition) {
				nma.SetDestination(es.targetPosition.position);
			} else {
				nma.SetDestination(possibleTargets[Random.Range (0, possibleTargets.Length)].position);
			}
		}*/
		es.spawned = true;
	}
}
