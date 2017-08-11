using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackTrigger : MonoBehaviour {


	public float damageDone;
	public float comboDamage;
	public Animator AnimControl;
	public int attackid;
	public zOldAttack attck;

	public LayerMask affectingLayers;

	protected List<GameObject> enemiesHit = new List<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//if(!transform.parent.GetComponent<Attack>().AnimControl.GetBool("Attack"))
		if(!AnimControl.GetBool(attackid))
		{
			Destroy(transform.gameObject);
		}
	}

	
	void OnTriggerEnter(Collider col) 
	{
		//int ly = transform.gameObject.layer;
		bool lhit = true;
		if (affectingLayers == (affectingLayers | (1 << col.gameObject.layer))) {
			foreach (GameObject enemy in enemiesHit) {
				if (enemy == col.transform.gameObject)
					lhit = false;
			}
			//int damage = transform.parent.GetComponent<Attack>().attackForce;
			if(lhit)
			{
				float dmg = 0.0f;
				dmg = col.transform.GetComponent<Health>().Damage(damageDone + comboDamage);
				enemiesHit.Add(col.transform.gameObject);
				if(dmg > 0.0f)
				{
					//TO DO: increase combo
					++attck.currentCombo;
					attck.comboTimer = 0.0f;
					attck.aggro += dmg;
				}
			}
		}
	}
}

