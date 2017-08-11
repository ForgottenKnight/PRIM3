using UnityEngine;
using System.Collections;

public class TargetSelector : MonoBehaviour {
	public GameObject target;
	[Header("Parametros de seleccion")]
	public float range = 50.0f;
	public float rangeToStop = 60.0f;
	public float timeToChange = 1.0f;
	private float timer = 0.0f;
	public float maxYDistance = 2.0f;
	[Range(1.0f, 3.0f)]
	public float threshold = 1.3f;
	public float maxAggro = 50.0f;
	public float maxAbility = 50.0f;
	public bool directionBetween0And1 = false;
	[Header("Ponderacion para seleccion de objetivo")]
	public float pDistance2DToTarget = 1.0f;
	public float pYDistance = 0.0f;
	public float pTargetHealth = 0.0f;
	public float pTargetAbility = 0.0f;
	public float pTargetAggro = 0.0f;
	public float pTargetEnergy = 0.0f;
	public float pTargetState = 0.0f;
	public float pTargetDirection = 0.0f;
	public Transform players;
	
	
	[HideInInspector]
	public TargetPoints currentTarget;
	
	public class TargetPoints {
		public GameObject target;
		public int player;
		public float points;
		public Incapacitate inc;
		
		public TargetPoints() {
			target = null;
			player = -1;
			points = -1.0f;
			inc = null;
		}
	}

	void Start() {
		currentTarget = new TargetPoints ();
		target = null;
		players = GameObject.FindGameObjectWithTag ("PlayerContainer").transform;
		DebugDrawer.AddOval (0f, 0f, range, range, Color.red, gameObject);
		DebugDrawer.AddOval (0f, 0f, rangeToStop, rangeToStop, Color.magenta, gameObject);
	}

	void Update ()
	{
		timer += Time.deltaTime;
		if (!currentTarget.target && currentTarget.player != -1) {
			currentTarget.target = SearchPlayer (currentTarget.player);
			target = currentTarget.target;
		} else if (!currentTarget.target && currentTarget.player == -1) {
			currentTarget.points = -1f;
		}
		if (timer >= timeToChange) {
			ChangeTarget();
			timer = 0.0f;
		}

        if(currentTarget.target && !currentTarget.target.activeSelf)
        {
            currentTarget.target = null;
            currentTarget.player = -1;
            currentTarget.inc = null;
            ChangeTarget();
        }
		
		if (currentTarget.inc != null) {
			if (currentTarget.inc.isActionActive()) {
				currentTarget = new TargetPoints();
				SelectTarget ();
			}
		}
	}

	void ChangeTarget() {
		currentTarget = SelectTarget ();
		target = currentTarget.target;
	}
	
	TargetPoints SelectTarget() {
		TargetPoints maxTarget = new TargetPoints ();
		for (int i = 0; i < players.childCount; ++i) {
			GameObject player = players.GetChild(i).gameObject;
			Incapacitate inc = player.GetComponent<Incapacitate>();
			bool validTarget = true;
			if (inc) {
				validTarget = !inc.isActionActive();
			}
			if (validTarget) {
				int playerNum = player.GetComponent<GeneralPlayerController>().player;
				float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
				if (distanceFromPlayer < range) {
					TargetPoints targetPoints = new TargetPoints();
					targetPoints.points = 0f;
					targetPoints.target = player;
					targetPoints.player = playerNum;
					targetPoints.inc = inc;
					targetPoints.points += ComputeDistance2D(distanceFromPlayer);
					targetPoints.points += ComputeYDistance(Mathf.Abs (player.transform.position.y - transform.position.y));
					targetPoints.points += ComputeTargetHealth(player);
					targetPoints.points += ComputeTargetAbility(player);
					targetPoints.points += ComputeAggro(player);
					targetPoints.points += ComputeEnergy(player);
					targetPoints.points += ComputeDirection(player);
					targetPoints.points += ComputeTerrain(player);
					
					if (targetPoints.points > maxTarget.points) {
						maxTarget = targetPoints;
					}
					if (currentTarget.player == playerNum) {
						currentTarget.points = targetPoints.points;
					}
				} else if (distanceFromPlayer > rangeToStop && playerNum == currentTarget.player) {
					currentTarget.target = null;
					currentTarget.player = -1;
					currentTarget.inc = null;
				}
			}
		}
		
		TargetPoints result = currentTarget;
		if (maxTarget.player != currentTarget.player) {
			if (maxTarget.points > result.points * threshold) {
				result = maxTarget;
			}
		}
		
		return result;
	}
	
	GameObject SearchPlayer(int player) {
		GameObject playerGo = null;
		//GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.childCount; ++i) {
			GameObject aux = players.GetChild(i).gameObject;
			if (aux.GetComponent<GeneralPlayerController>().player == player) {
				playerGo = aux;
			}
		}
		return playerGo;
	}
	
	float ComputeDistance2D(float distance) {
		return pDistance2DToTarget - ((distance / range) * pDistance2DToTarget);
	}
	
	float ComputeYDistance(float distance) {
		//return pYDistance - ((distance / maxYDistance) * pYDistance);
		return -((distance / maxYDistance) * pYDistance);
	}
	
	float ComputeTargetHealth(GameObject go) {
		Health h = go.GetComponent<Health> ();
		float points = 0.0f;
		if (h) {
			points = h.GetHealthAsUnit() * pTargetHealth;
		}
		return points;
	}
	
	float ComputeTargetAbility(GameObject go) {
		SpecialAbility sa = go.GetComponent<SpecialAbility> ();
		float points = 0.0f;
		if (sa) {
			float aggro = Mathf.Clamp(sa.aggro, 0.0f, maxAbility);
			float multiplier = sa.negativeAggro ? -1.0f : 1.0f;
			points = multiplier * ((aggro / maxAbility) * pTargetAbility);
		}
		return points;
	}
	
	float ComputeAggro(GameObject go) {
		Attack a = go.GetComponent<Attack> ();
		float points = 0.0f;
		if (a) {
			float aggro = Mathf.Clamp(a.aggro, 0.0f, maxAggro);
			points = (aggro / maxAggro) * pTargetAggro;
		}
		return points;
	}
	
	float ComputeEnergy(GameObject go) {
		Energy e = go.GetComponent<Energy> ();
		float points = 0.0f;
		if (e) {
			points = e.GetEnergyAsUnit() * pTargetEnergy;
		}
		return points;
	}
	
	float ComputeTerrain(GameObject go) {
		return 0.0f;
	}
	
	float ComputeDirection(GameObject go) {
		float dot = Vector3.Dot (transform.forward.normalized, (go.transform.position - transform.position).normalized);
		if (directionBetween0And1) {
			dot = (dot + 1.0f) / 2.0f;
		}
		return dot * pTargetDirection;
	}
}
