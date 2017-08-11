using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class GolemBurried : MonoBehaviour {

	public float radius = 5f;
	public float velocity = 1f;
	public GameObject golemPrefab;
	public Transform enemies;
	public SharedFloat selectionRange = 25f;
	public SharedFloat selectionRangeStop = 60f;	
	public SharedFloat rockDelay = 10f;
	public SharedBool haveNavAgent = true;

	private bool wakeUp = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!wakeUp)
		{
			CheckDistance();
		}else{
			SpawnGolem();
		}
	}

	private void CheckDistance()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player"); // TODO: Game object que gestione lista de players
		for(int i = 0; i< players.Length; ++i)
		{
			float dist = Vector3.Distance(transform.position, players[i].transform.position);
			if(dist < radius)
				wakeUp = true;
		}
	}

	private void SpawnGolem()
	{
		GameObject golem = Instantiate(golemPrefab, transform.position,Quaternion.identity) as GameObject;
		golem.transform.parent = enemies;
		BehaviorTree BT = golem.GetComponent<BehaviorTree>();
		if(BT)
		{
			BT.SetVariable("targetSelectionRange", selectionRange);
			BT.SetVariable("targetSelectionRangeStop",selectionRangeStop);
			BT.SetVariable("rockDelay",rockDelay);
			BT.SetVariable("haveNavAgent", haveNavAgent);
		}
		Destroy(gameObject);
	}
}
