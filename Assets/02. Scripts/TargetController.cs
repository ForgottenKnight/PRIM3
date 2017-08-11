using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetController : MonoBehaviour {
	private static TargetController instance; // SINGLETON

	public List<GameObject> targets = new List<GameObject>();
	public List<GameObject> targetPointers =  new List<GameObject>();
	public bool active = true;

	public enum Direction {
		right,
		left,
		up,
		down
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
		} else {
			instance = this;
		}
	}

	public static TargetController Instance
	{
		get 
		{
			return instance;
		}
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void syncronizeTargets() {
		foreach (GameObject t in targetPointers) {
			foreach (GameObject tp in targets) {
				t.GetComponent<TargetPointer> ().addToVisitedPool (tp);
			}
		}
	}

	public void addTarget(GameObject target) {
		targets.Add (target);
		syncronizeTargets ();
	}
	public void removeTarget(GameObject target) {
		targets.Remove (target);
		foreach (GameObject t in targetPointers) {
			t.GetComponent<TargetPointer> ().removeFromVisitPool (target);
		}
	}

	public void addTargetPointer(GameObject targetPointer) {
		targetPointers.Add (targetPointer);
		syncronizeTargets ();
	}
	public void removeTargetPointer(GameObject targetPointer) {
		targetPointers.Remove (targetPointer);
	}

	public GameObject getClosest2DPTarget(GameObject targetPointer) {
		if (active && (targetPointer.GetComponent<TargetPointer>().countTargetPoolElements() > 0)) {
			GameObject nextTarget = null;
			float minDistance = 999999;
			foreach (GameObject target in targets) {
				if(isTargetCloseEnough(target, targetPointer)){
					if (target != targetPointer) {
						Vector3 p1 = new Vector3 (target.transform.position.x, 0, target.transform.position.z);
						Vector3 p2 = new Vector3 (targetPointer.transform.position.x, 0, targetPointer.transform.position.z);
						float distance = Vector3.Distance (p1, p2);
						if (distance < minDistance) {
							minDistance = distance;
							nextTarget = target;
						}
					}
				}
			}
			return nextTarget;
		} else {
			return null;
		}
	}


	public GameObject getNext2DPTarget(GameObject targetPointer) {
		if (active && (targetPointer.GetComponent<TargetPointer>().countTargetPoolElements() > 0)) {
			GameObject nextTarget = null;
			float minDistance = 999999;
			foreach (GameObject target in targets) {
				if(isTargetCloseEnough(target, targetPointer)){
					if (target != targetPointer.GetComponent<TargetPointer> ().target  && !targetPointer.GetComponent<TargetPointer> ().isTargetVisited(target)) {
						Vector3 p1 = new Vector3 (target.transform.position.x, 0, target.transform.position.z);
						Vector3 p2 = new Vector3 (targetPointer.transform.position.x, 0, targetPointer.transform.position.z);
						float distance = Vector3.Distance (p1, p2);
						if (distance < minDistance) {
							minDistance = distance;
							nextTarget = target;
						}
					}
				}
			}
			if (nextTarget != null) {
				targetPointer.GetComponent<TargetPointer> ().setTargetVisited (nextTarget, true);
				return nextTarget;
			} else {
				targetPointer.GetComponent<TargetPointer> ().setAllTargetsVisited (false);
				return getNext2DPTarget(targetPointer);
			}
		} else {
			return null;
		}
	}

	public GameObject getClosest3DPTarget(GameObject targetPointer) {
		if (active && (targetPointer.GetComponent<TargetPointer>().countTargetPoolElements() > 0)) {
			GameObject nextTarget = null;
			float minDistance = 999999;
			foreach (GameObject target in targets) {
				if(isTargetCloseEnough(target, targetPointer)){
					if (target != targetPointer) {
						float distance = Vector3.Distance (target.transform.position, targetPointer.transform.position);
						if (distance < minDistance) {
							minDistance = distance;
							nextTarget = target;
						}
					}
				}
			}
			return nextTarget;
		} else {
			return null;
		}
	}

	public GameObject getNext3DPTarget(GameObject targetPointer) {
		if (active && (targetPointer.GetComponent<TargetPointer>().countTargetPoolElements() > 0)) {
			GameObject nextTarget = null;
			float minDistance = 999999;
			foreach (GameObject target in targets) {
				if(isTargetCloseEnough(target, targetPointer)){
					if (target != targetPointer.GetComponent<TargetPointer> ().target  && !targetPointer.GetComponent<TargetPointer> ().isTargetVisited(target)) {
						float distance = Vector3.Distance (target.transform.position, targetPointer.transform.position);
						if (distance < minDistance) {
							minDistance = distance;
							nextTarget = target;
						}
					}
				}
			}
			if (nextTarget != null) {
				targetPointer.GetComponent<TargetPointer> ().setTargetVisited (nextTarget, true);
				return nextTarget;
			} else {
				targetPointer.GetComponent<TargetPointer> ().setAllTargetsVisited (false);
				return getNext3DPTarget(targetPointer);
			}
		} else {
			return null;
		}
	}

	public GameObject getClosestAPTarget(GameObject targetPointer, float horizontal, float vertical) {
		if (active && (targetPointer.GetComponent<TargetPointer> ().countTargetPoolElements () > 0)) {
			GameObject nextTarget = null;
			float minDistance = 999999;
			float viewAngle = targetPointer.GetComponent<TargetPointer>().AxisProximityMinAngle;
			foreach (GameObject target in targets) {
				if(isTargetCloseEnough(target, targetPointer)){
					Vector3 screenPosPointer;
					if (targetPointer.GetComponent<TargetPointer> ().AxisProximityPivot == TargetPointer.Pivot.Target && targetPointer.GetComponent<TargetPointer> ().target != null) {
						screenPosPointer = Camera.main.WorldToScreenPoint (targetPointer.GetComponent<TargetPointer> ().target.transform.position);
						screenPosPointer = new Vector3 (screenPosPointer.x, screenPosPointer.y, 0);
					} else {
						screenPosPointer = Camera.main.WorldToScreenPoint (targetPointer.transform.position);
						screenPosPointer = new Vector3 (screenPosPointer.x, screenPosPointer.y, 0);
					}
					Vector3 screenPosTarget = Camera.main.WorldToScreenPoint (target.transform.position);
					screenPosTarget = new Vector3 (screenPosTarget.x, screenPosTarget.y, 0);
					Vector3 PointerToTarget = screenPosTarget - screenPosPointer;
					PointerToTarget = PointerToTarget.normalized;


					Vector3 direction = new Vector3 (horizontal, vertical*(-1), 0); //Hay que invertir el eje "Y" ya que el mando lo devuelve al revés
					direction = direction.normalized;


					float dot = Vector3.Dot (PointerToTarget, direction);
					float angle = Mathf.Acos (dot) * Mathf.Rad2Deg;

					if (angle < viewAngle) {
						float distance;
						if (targetPointer.GetComponent<TargetPointer> ().AxisProximityPivot == TargetPointer.Pivot.Target && targetPointer.GetComponent<TargetPointer> ().target != null) {
							distance = Vector3.Distance (targetPointer.GetComponent<TargetPointer> ().target.transform.position, target.transform.position);
						} else {
							distance = Vector3.Distance (targetPointer.transform.position, target.transform.position);
						}
						if (distance < minDistance) {
							minDistance = distance;
							nextTarget = target;
						}
					}	
				}
			}
			if (nextTarget != null) {
				return nextTarget;
			} else {
				return getClosest2DPTarget(targetPointer);
			}
		} else {
			return null;
		}
	}

	public GameObject getNextAPTarget(GameObject targetPointer, float horizontal, float vertical) {
		if (active && (targetPointer.GetComponent<TargetPointer> ().countTargetPoolElements () > 0)) {
			GameObject nextTarget = null;
			List<GameObject> posibleTargets = new List<GameObject> ();
			float minDistance = 999999;
			float viewAngle = targetPointer.GetComponent<TargetPointer>().AxisProximityMinAngle;

			foreach (GameObject target in targets) {
				if(isTargetCloseEnough(target, targetPointer)){
					if (target != targetPointer.GetComponent<TargetPointer> ().target /*&& !targetPointer.GetComponent<TargetPointer> ().isTargetVisited (target)*/) {
						Vector3 screenPosPointer;
						if (targetPointer.GetComponent<TargetPointer> ().AxisProximityPivot == TargetPointer.Pivot.Target && targetPointer.GetComponent<TargetPointer> ().target != null) {
							screenPosPointer = Camera.main.WorldToScreenPoint (targetPointer.GetComponent<TargetPointer> ().target.transform.position);
							screenPosPointer = new Vector3 (screenPosPointer.x, screenPosPointer.y, 0);
						} else {
							screenPosPointer = Camera.main.WorldToScreenPoint (targetPointer.transform.position);
							screenPosPointer = new Vector3 (screenPosPointer.x, screenPosPointer.y, 0);
						}
						Vector3 screenPosTarget = Camera.main.WorldToScreenPoint (target.transform.position);
						screenPosTarget = new Vector3 (screenPosTarget.x, screenPosTarget.y, 0);
						Vector3 PointerToTarget = screenPosTarget - screenPosPointer;
						PointerToTarget = PointerToTarget.normalized;


						Vector3 direction = new Vector3 (horizontal, vertical * (-1), 0); //Hay que invertir el eje "Y" ya que el mando lo devuelve al revés
						direction = direction.normalized;


						float dot = Vector3.Dot (PointerToTarget, direction);
						float angle = Mathf.Acos (dot) * Mathf.Rad2Deg;
						if (angle < viewAngle) {
							posibleTargets.Add (target);

							if (!targetPointer.GetComponent<TargetPointer> ().isTargetVisited (target)) {

								float distance = Vector3.Distance (targetPointer.transform.position, target.transform.position);
								if (distance < minDistance) {
									minDistance = distance;
									nextTarget = target;
								}
							}
						}	
					}
				}
			}
			if (nextTarget != null) {
				targetPointer.GetComponent<TargetPointer> ().setTargetVisited (nextTarget, true);
				return nextTarget;
			} else {
				targetPointer.GetComponent<TargetPointer> ().setAllTargetsVisited (false);
				float distance2;
				float minDistance2 = 999999;

				foreach (GameObject pt in posibleTargets) {
					distance2 = Vector3.Distance (targetPointer.transform.position, pt.transform.position);
					if (distance2 < minDistance2) {
						minDistance2 = distance2;
						nextTarget = pt;
					}
				}
				targetPointer.GetComponent<TargetPointer> ().setTargetVisited (nextTarget, true);
				if (nextTarget != null) {
					return nextTarget;
				} else {
					if(targetPointer.GetComponent<TargetPointer> ().target != null){
						return targetPointer.GetComponent<TargetPointer> ().target;
					} else {
						return getClosest2DPTarget(targetPointer);
					}
				}
			}
		} else {
			return null;
		}
	}

	public GameObject getClosestAngleTarget(GameObject targetPointer) {
		if (active && (targetPointer.GetComponent<TargetPointer>().countTargetPoolElements() > 0)) {
			GameObject nextTarget = null;
			float minAngle = 360;
			foreach (GameObject target in targets) {
				if(isTargetCloseEnough(target, targetPointer)){
					Vector3 p1 = target.transform.position;
					p1 = new Vector3 (p1.x, 0, p1.z);
					Vector3 p2 = targetPointer.transform.position;
					p2 = new Vector3 (p2.x, 0, p2.z);
					Vector3 forward = targetPointer.transform.forward;
					forward = new Vector3 (forward.x, 0, forward.z);

					Vector3 targetDir = p1 - p2;
					targetDir = targetDir.normalized;

					forward = forward.normalized;

					float dot = Vector3.Dot (targetDir, forward);
					float angle = Mathf.Acos (dot) * Mathf.Rad2Deg;

					if (angle < minAngle) {
						minAngle = angle;
						nextTarget = target;
					}
				}

			}
			return nextTarget;

		} else {
			return null;
		}
	}


	public GameObject getNextAngleTarget(GameObject targetPointer) {
		if (active && (targetPointer.GetComponent<TargetPointer>().countTargetPoolElements() > 0)) {
			GameObject nextTarget = null;
			float minAngle = 360;
			foreach (GameObject target in targets) {
				if(isTargetCloseEnough(target, targetPointer)){

					if (target != targetPointer.GetComponent<TargetPointer> ().target && !targetPointer.GetComponent<TargetPointer> ().isTargetVisited(target)) {

						Vector3 p1 = target.transform.position;
						p1 = new Vector3 (p1.x, 0, p1.z);
						Vector3 p2 = targetPointer.transform.position;
						p2 = new Vector3 (p2.x, 0, p2.z);
						Vector3 forward = targetPointer.transform.forward;
						forward = new Vector3 (forward.x, 0, forward.z);

						Vector3 targetDir = p1 - p2;
						targetDir = targetDir.normalized;

						forward = forward.normalized;

						float dot = Vector3.Dot (targetDir, forward);
						float angle = Mathf.Acos (dot) * Mathf.Rad2Deg;

						if (angle < minAngle) {
							minAngle = angle;
							nextTarget = target;
						}
					}
				}
			}
			if (nextTarget != null) {
				targetPointer.GetComponent<TargetPointer> ().setTargetVisited (nextTarget, true);
				return nextTarget;
			} else {
				targetPointer.GetComponent<TargetPointer> ().setAllTargetsVisited (false);
				return getNextAngleTarget (targetPointer);
			}
		} else {
			return null;
		}
	}

	private bool isTargetCloseEnough(GameObject target, GameObject targetPointer) {
		float distanceToTarget =  Vector3.Distance(targetPointer.transform.position, target.transform.position);
		return (target.GetComponent<Target> ().maxTargetDistance >= distanceToTarget);
	}





}
