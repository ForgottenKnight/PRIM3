using UnityEngine;
using System.Collections;

public class zOldFalltoVoid : MonoBehaviour {

	public bool killPlayer = false;
	public bool killEnemy = true;
	public float fallDamage = 10.0f;
	public GameObject SpawnPoint;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col) 
	{
        if (col.gameObject.layer == LayerMask.NameToLayer("Player") || col.gameObject.layer == LayerMask.NameToLayer("JumpingPlayer"))
		{
			Damageable h = col.transform.GetComponent<Damageable>();
			if (h) 
			{
				if(killPlayer)
				{
					h.RemoveHealth();
				}else{
					h.Damage(fallDamage);
					//Vector3 lastPos = col.gameObject.GetComponent<Movement>().lastGroundedPosition;
					col.gameObject.transform.position = SpawnPoint.transform.position;
				}
			}
		}else if(col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			Damageable h = col.transform.GetComponent<Damageable>();
			if (h) 
			{
				if(killEnemy)
				{
					h.RemoveHealth();
				}else{
					h.Damage(fallDamage);
					/*Vector3 lastPos = col.gameObject.GetComponent<GolemMovement>().lastGroundedPosition;
					col.gameObject.transform.position = lastPos;*/ // TODO
				}
			}
		}
	}
}
