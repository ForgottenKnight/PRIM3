using UnityEngine;
using System.Collections;

public class RockSpawnMultiple : MonoBehaviour {
	public Transform[] targetPosition;
	public bool activated = false;
	public float movSpeed = 1.0f;
	protected bool finishSpawn = false;
	protected int phase;

	// Use this for initialization
	void Start () {
		phase = -1;
	}
	
	// Update is called once per frame
	void Update () {
		if (activated && !finishSpawn) {
			Vector3 pos = Vector3.MoveTowards(transform.position, targetPosition[phase].position, movSpeed * Time.deltaTime);
			transform.position = pos;
			if (pos == targetPosition[phase].position) {
				activated = false;
			}
			if (phase >= targetPosition.Length) {
				finishSpawn = true;
			}
		}
	}

	public void Activate() {
		activated = true;
		phase++;
	}
}
