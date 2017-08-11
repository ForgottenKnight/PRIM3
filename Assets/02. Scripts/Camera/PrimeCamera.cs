using UnityEngine;
using System.Collections;

[AddComponentMenu("PRIM3/Camera/PRIM3 Camera")] 
public class PrimeCamera : MonoBehaviour {
	public enum PlayerCheckTypes {Distance, ScreenBorders, SlowDownBorders, SlowDownDistance, NoCheck};
	public enum LerpType {Linear, Sin, Curve};
	public bool followStoppedPlayers = false;
	[Header("Focus")]
	public Transform customFocusPoint;
	[Header("Player Check")]
	public PlayerCheckTypes playerCheck = PlayerCheckTypes.Distance;
	[Header("Slow player parameters")]
	public float slowSpeed = 1.0f;
	public float slowTime = 1.0f;
	[Header("Parametros")]
	public float zoom = 10.0f;
	public float pitch = 45.0f;
	public float yaw = 0.0f;
	public float objectiveYaw;
	public float roll = 0.0f;
    [Header("Offset")]
    public Vector3 focusOffset = Vector3.zero;
	[Header("Velocidades")]
	public bool smoothing = true;
	public float speed = 10.0f;
	public float zoomSpeed = 10.0f;
	public float pitchSpeed = 10.0f;
	public float yawSpeed = 10.0f;
	public float rollSpeed = 10.0f;
	[Header("Aceleraciones")]
	public bool accelerated = true;
	public float timeToDeccel = 0.08f;
	float timePassed = 0f;
	public float acceleration = 20.0f;
	public float minSpeed = 1.0f;
	public float distanceToDeccel = 1.0f;
	public float zoomAcceleration = 20.0f;
	public float minZoomSpeed = 1.0f;
	public float pitchAcceleration = 20.0f;
	public float minPitchSpeed = 1.0f;
	public float yawAcceleration = 20.0f;
	public float minYawSpeed = 1.0f;
	[Header("Velocidades actuales (para debug)")]
	public float currentSpeed = 0.0f;
	public float currentZoomSpeed = 0.0f;
	public float currentPitchSpeed = 0.0f;
	public float currentYawSpeed = 0.0f;
	[Header("Distancia entre jugadores a tener en cuenta")]
	public float minDistance = 0.0f;
	public float maxDistance = 40.0f;
	public LerpType lerpType;
	public AnimationCurve lerpCurve;
	[Header("Parametros del zoom")]
	public float minZoom = 7.0f;
	public float maxZoom = 40.0f;
	[Header("Parametros del pitch")]
	public float minPitch = 10.0f;
	public float maxPitch = 50.0f;
	[Header("Parametros del yaw de rotacion automatica")]
	public bool rotationYaw = false;
	public float beginYaw = 0.0f;
	public float endYaw = 180.0f;
	public Transform beginYawPos;
	public Transform endYawPos;
	public LerpType yawLerpType;
	public AnimationCurve yawLerpCurve;
	[Header("Parametros del pitch de rotacion automatica")]
	public bool rotationPitch = false;
	public float beginMinPitch = 0.0f;
	public float endMinPitch = 180.0f;
	public float beginMaxPitch = 0.0f;
	public float endMaxPitch = 180.0f;
	public Transform beginPitchPos;
	public Transform endPitchPos;
	public LerpType pitchLerpType;
	[Header("Constraints de movimiento")]
	public bool movementConstraint = false;
    public bool rotateInstead = false;
	public bool zoomConstraint = false;
	[Header("Constraint de rotación")]
	public bool yawConstraint = false;
	public bool pitchConstraint = false;
	public bool rollConstraint = false;
	[Header("Parametros para un solo jugador")]
	[Range(0.0f, 1.0f)]
	public float onePlayerLerp = 0.5f;

	Transform players;


	Vector3 follow;
	Vector3 lookAtPosition;
	Vector3 lastPosition;
	float lastObjectiveYaw;

	[Header("Parametros de colision de camara")]
	public float minCollisionDistance = 0.0f;
	private bool cameraCollision = false;
	private Vector3 collisionPoint;
	private Vector3 finalPosition;




	// Use this for initialization
	void Start () {
		objectiveYaw = yaw;
		lastObjectiveYaw = yaw;
		lastPosition = transform.position;
		lookAtPosition = Vector3.zero;
		currentSpeed = speed;
		currentZoomSpeed = zoomSpeed;
		currentPitchSpeed = pitchSpeed;
		currentYawSpeed = yawSpeed;
		players = GameObject.FindGameObjectWithTag ("PlayerContainer").transform;

	}
	
	// Update is called once per frame
	void Update () {
		if (customFocusPoint != null) {
			follow = customFocusPoint.position + focusOffset;
		} else {
			if (!movementConstraint) {
				CalculateFollow ();
                follow += focusOffset;
            }
            else if (rotateInstead)
            {
                CalculateLookat();
            }
		}


		CalculateCameraCollision ();
		CalculateZoom ();


		if (!yawConstraint) {
			CalculateYaw ();
		}
		if (!pitchConstraint)
			CalculateBeginEndPitch ();
//		Vector3 lastPos = transform.position;
//		Quaternion lastRot = transform.rotation;

		UpdateTransform ();
		UpdateSpeed ();
	
		/*if (!CheckPlayersOk ()) {
			transform.rotation = lastRot;
			transform.position = lastPos;
		}*/
		//Debug.Log (GetComponent<Camera> ().WorldToViewportPoint (test.position));

	}

	void CalculateCameraCollision() {
		RaycastHit[] l_hits;
		float l_distance = Vector3.Distance (follow, finalPosition);
		Vector3 l_direction = finalPosition - follow;
		//Debug.Log ("follow: " + follow);
		//Debug.Log ("player: " + players.GetChild (0).transform.position);

		l_hits = Physics.RaycastAll (follow, l_direction, l_distance);

		RaycastHit l_closestHit;
		bool l_hit = false;
		float l_closestHitDistance = 99999;

		for (int i = 0; i < l_hits.Length; i++) {
			RaycastHit hit = l_hits [i];

			if (hit.transform.root.gameObject.tag != "PlayerContainer") {
				float l_hitDistance = Vector3.Distance (follow, hit.point);

				if (l_hitDistance < l_closestHitDistance) {
					l_closestHitDistance = l_hitDistance;
					l_closestHit = hit;
					l_hit = true;

				}
			}
		}

		float l_collisionDistance = Vector3.Distance (follow, l_closestHit.point);
		if (l_hit && (l_collisionDistance >= minCollisionDistance)) {
			collisionPoint = l_closestHit.point;
			cameraCollision = true;
		} else {
			cameraCollision = false;
		}
	}

	void UpdateSpeed() {
		if (accelerated) {
			float distance = Vector3.Distance (follow, lookAtPosition);
			if (distance >= distanceToDeccel) { // Acceleration
				currentSpeed += Time.deltaTime * acceleration;
				timePassed = 0f;
			} else { // Deceleration
				timePassed += Time.deltaTime;
				if (timePassed >= timeToDeccel) {
					currentSpeed -= Time.deltaTime * acceleration;
				} else {
					currentSpeed += Time.deltaTime * acceleration;
				}
			}
			currentSpeed = Mathf.Clamp (currentSpeed, minSpeed, speed);
		}
	}

	bool CheckPlayersOk() {
		//GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		bool playersOk = true;
		switch (playerCheck) {
		case PlayerCheckTypes.Distance:
		case PlayerCheckTypes.SlowDownDistance:
			playersOk = CheckPlayersByDistance (players);
			break;
		case PlayerCheckTypes.ScreenBorders:
		case PlayerCheckTypes.SlowDownBorders:
			playersOk = CheckPlayersByScreenBorders (players);
			break;
		case PlayerCheckTypes.NoCheck:
		default:
			break;
		}

		return playersOk;
	}

	public bool CheckPlayerOk(Transform player) {
		bool playerOk = true;
		switch (playerCheck) {
		case PlayerCheckTypes.Distance:
		case PlayerCheckTypes.SlowDownDistance:
			playerOk = CheckPlayerOkByDistance(player);
			break;
		case PlayerCheckTypes.ScreenBorders:
		case PlayerCheckTypes.SlowDownBorders:
			playerOk = CheckPlayerOkByScreenBorders (player);
			break;
		case PlayerCheckTypes.NoCheck:
		default:
			break;
		}
		return playerOk;
	}

	public bool CheckPlayerOkByScreenBorders(Transform player) {
		Vector3 viewportPos = GetComponent<Camera> ().WorldToViewportPoint (player.position);
		if (viewportPos.x <= 0.05f || viewportPos.y <= 0.05f || viewportPos.x >= 0.95f || viewportPos.y >= 0.95f) {
			if (playerCheck == PlayerCheckTypes.SlowDownBorders) {
				SlowPlayer(player);
			}
			return false;
		}
		else return true;
	}

	/*public bool CheckPlayerOkByDistance(Transform player) {
		int i = 0;
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		bool playerOk = true;
		while (i < players.Length && playerOk == true) {
			if (player != players[i].transform) {
				Vector2 v1 = new Vector2 (player.position.x, player.position.z);
				Vector2 v2 = new Vector2 (players [i].transform.position.x, players [i].transform.position.z);
				playerOk = Vector2.Distance (v1, v2) <= maxDistance;
			}
			i++;
		}
		if (!playerOk && playerCheck == PlayerCheckTypes.SlowDownDistance) {
			SlowPlayer(player);
		}
		return playerOk;
	}*/ // Ahora usamos la otra version

	public bool CheckPlayerOkByDistance(Transform player) {
		int i = 0;
		bool playerOk = false;
		//GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		if (players.childCount == 1)
			return true;
		while (i < players.childCount) {
			if (player != players.GetChild(i)){
				bool auxPlayerOk;
				Vector2 v1 = new Vector2 (player.position.x, player.position.z);
				Vector2 v2 = new Vector2 (players.GetChild(i).position.x, players .GetChild(i).position.z);
				auxPlayerOk = Vector2.Distance (v1, v2) <= maxDistance;
				if (auxPlayerOk == true) playerOk = true;
			}
			i++;
		}
		if (!playerOk && playerCheck == PlayerCheckTypes.SlowDownDistance) {
			SlowPlayer(player);
		}
		return playerOk;
	}

	public void SlowPlayer(Transform player) {
		player.gameObject.GetComponent<Movement> ().SlowDown (slowSpeed, slowTime);
	}

	public bool CheckPlayersByScreenBorders(Transform players) {
		int i = 0;
		bool playersOk = true;
		while (playersOk && i < players.childCount) {
			playersOk = CheckPlayerOk (players.GetChild(i));
			i++;
		}
		return playersOk;
	}

	public bool CheckPlayersByDistance(Transform players) {
		int i = 0, j = 0;
		bool playersOk = true;
		while (i < players.childCount && playersOk == true) {
			while (j < players.childCount && playersOk == true) {
				if (i != j) {
					Transform t1 = players.GetChild(i);
					Transform t2 = players .GetChild(j);
					Vector2 v1 = new Vector2 (t1.position.x, t1.position.z);
					Vector2 v2 = new Vector2 (t2.position.x, t2.position.z);
					playersOk = Vector2.Distance (v1, v2) <= maxDistance;
				}
				j++;
			}
			i++;
		}
		return playersOk;
	}

	void UpdateTransform() {

		if (smoothing) {
			Vector3 lastOkPosition = lastPosition;
			lastPosition = Vector3.MoveTowards (lastPosition, follow, Time.deltaTime * currentSpeed);
			if (!float.IsNaN (lastPosition.x)) {
				//transform.position = lastPosition;
				finalPosition = lastPosition;
			} else {
				lastPosition = lastOkPosition;
				return;
			}
		} else {
			if (!float.IsNaN (follow.x)) {
				//transform.position = follow;
				finalPosition = follow;
			} else {
				return;
			}
		}
		//lookAtPosition = transform.position;
		lookAtPosition = finalPosition;

		/*if (yawConstraint)
		yaw = transform.eulerAngles.y;
	if (rollConstraint)
		roll = transform.eulerAngles.z;
	if (pitchConstraint)
		pitch = transform.eulerAngles.x;*/
		if (!float.IsNaN (pitch) && !float.IsNaN (yaw) && !float.IsNaN (roll) && !(movementConstraint && rotateInstead)) {
			transform.eulerAngles = new Vector3 (pitch, yaw, roll);
		}
		if (!float.IsNaN (zoom)) {
			//transform.position = transform.position - transform.forward * zoom;
			finalPosition = finalPosition  - transform.forward * zoom;
		}

		Debug.DrawLine (follow, finalPosition, Color.red);
		Debug.DrawLine (follow, collisionPoint, Color.blue);

		if (cameraCollision) {
			Debug.Log ("Collision");
			transform.position = collisionPoint;
		} else {
			Debug.Log ("No Collision");

			transform.position = finalPosition;
		}

	}

	void CalculateFollow() {
		//GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		follow = Vector3.zero;
		int playersFollowed = 0;
		for (int i = 0; i < players.childCount; i++) {
			if (followStoppedPlayers || !players.GetChild(i).gameObject.GetComponent<PlayerManager>().insideBoundingBox) {
				follow += players.GetChild(i).position;
				playersFollowed++;
			}
		}
		follow = follow / playersFollowed;
	}

    void CalculateLookat()
    {
        //GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
        Vector3 l_LookatFollow = Vector3.zero;
        int playersFollowed = 0;
        for (int i = 0; i < players.childCount; i++)
        {
            if (followStoppedPlayers || !players.GetChild(i).gameObject.GetComponent<PlayerManager>().noInput)
            {
                l_LookatFollow += players.GetChild(i).position;
                playersFollowed++;
            }
        }
        l_LookatFollow = l_LookatFollow / playersFollowed;
        //objectiveYaw = Vector3.Angle(transform.forward, (l_LookatFollow - transform.position));
        transform.LookAt(l_LookatFollow);
    }

	void CalculateYaw() {
		lastObjectiveYaw = objectiveYaw;
		if (rotationYaw) {
			float beginEndDistance = Vector3.Distance(beginYawPos.position, endYawPos.position);
			float followEndDistance = Vector3.Distance (follow, endYawPos.position);
			float lerp = Mathf.InverseLerp(0, beginEndDistance, followEndDistance);
			switch (yawLerpType) {
				case LerpType.Linear:
					break;
				case LerpType.Sin:
					lerp /= 2;
					lerp = Mathf.Sin (Mathf.PI * lerp);
					break;
				case LerpType.Curve:
					lerp = yawLerpCurve.Evaluate(lerp);
					break;
			}
			objectiveYaw = Mathf.LerpAngle(endYaw, beginYaw, lerp);
		}
		if (smoothing) {
			if (yaw != objectiveYaw) currentYawSpeed += yawAcceleration * Time.deltaTime;
			else currentYawSpeed -= yawAcceleration * Time.deltaTime;
			if (!float.IsNaN (objectiveYaw)) {
				currentYawSpeed = Mathf.Clamp (currentYawSpeed, minYawSpeed, yawSpeed);
				yaw = Mathf.MoveTowards (yaw, objectiveYaw, currentYawSpeed * Time.deltaTime);
			}
		} else {
			yaw = objectiveYaw;
		}
		if (float.IsNaN (objectiveYaw)) {
			objectiveYaw = lastObjectiveYaw;
		}
	}

	void CalculateBeginEndPitch() {
		if (rotationPitch) {
			float beginEndDistance = Vector3.Distance(beginPitchPos.position, endPitchPos.position);
			float followEndDistance = Vector3.Distance (follow, endPitchPos.position);
			float lerp = Mathf.InverseLerp(0, beginEndDistance, followEndDistance);
			switch (pitchLerpType) {
			case LerpType.Linear:
				break;
			case LerpType.Sin:
				lerp /= 2;
				lerp = Mathf.Sin (Mathf.PI * lerp);
				break;
			case LerpType.Curve:
				lerp = lerpCurve.Evaluate(lerp);
				break;
			}
			minPitch = Mathf.LerpAngle(beginMinPitch, endMinPitch, lerp);
			maxPitch = Mathf.LerpAngle(beginMaxPitch, endMaxPitch, lerp);
		}
	}

	void CalculateZoom() {
		//GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		float distance = 0;
		for (int i = 0; i < players.childCount; i++) {
			distance += Vector3.Distance(follow, players.GetChild(i).position);
		}
		float lerp = Mathf.InverseLerp (minDistance, maxDistance, distance);
		if (players.childCount == 1) {
			lerp = onePlayerLerp;
		}
		switch (lerpType) {
			case LerpType.Linear:
				break;
			case LerpType.Sin:
				lerp /= 2;
				lerp = Mathf.Sin (Mathf.PI * lerp);
				break;
			case LerpType.Curve:
				lerp = lerpCurve.Evaluate(lerp);
				break;
		}
		if (smoothing) {
			if (!zoomConstraint) {
				float newzoom = Mathf.Lerp (minZoom, maxZoom, lerp);
				if (zoom != newzoom) currentZoomSpeed += zoomAcceleration * Time.deltaTime;
				else currentZoomSpeed -= zoomAcceleration * Time.deltaTime;
				currentZoomSpeed = Mathf.Clamp (currentZoomSpeed, minZoomSpeed, zoomSpeed);
				zoom = Mathf.MoveTowards (zoom, newzoom, currentZoomSpeed * Time.deltaTime);
			}
			if (!pitchConstraint) {
				float newpitch = Mathf.Lerp (minPitch, maxPitch, lerp);
				if (pitch != newpitch) currentPitchSpeed += pitchAcceleration * Time.deltaTime;
				else currentPitchSpeed -= pitchAcceleration * Time.deltaTime;
				currentPitchSpeed = Mathf.Clamp (currentPitchSpeed, minPitchSpeed, pitchSpeed);
				pitch = Mathf.MoveTowards (pitch, newpitch, currentPitchSpeed * Time.deltaTime);
			}
		} else {
			if (!zoomConstraint) {
				zoom = Mathf.Lerp (minZoom, maxZoom, lerp);
			}
			if (!pitchConstraint) {
				pitch = Mathf.Lerp (minPitch, maxPitch, lerp);
			}
		}
	}
}
