using UnityEngine;
using System.Collections;

public class FalltoVoid : MonoBehaviour {
    public static FalltoVoid instance;
	
	public bool killPlayer = false;
	public bool killEnemy = true;
	public float fallDamage = 10.0f;

    public LayerMask raycastMask;

    public Transform globalReespawn;
	
	// Use this for initialization
	void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RemoveGlobalReespawn()
    {
        globalReespawn = null;
    }

    public void SetGlobalReespawn(Transform aTransform)
    {
        globalReespawn = aTransform;
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
                    h.ConditionalDamage(fallDamage, false, false, false);
                    if (globalReespawn != null)
                    {
                        col.gameObject.transform.position = globalReespawn.position;
                    }
                    else
                    {
                        Vector3 lastPos = col.gameObject.GetComponent<Movement>().lastGroundedPosition;
                        Vector3 up = col.gameObject.transform.up;
                        Ray ray = new Ray(lastPos, -up);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 5f, raycastMask))
                        {
                            RespawnPoint respawn = hit.collider.gameObject.GetComponent<RespawnPoint>();
                            if (respawn)
                            {
                                if (respawn.spawnPoint)
                                {
                                    col.gameObject.transform.position = respawn.spawnPoint.position;
                                }
                                else
                                {
                                    col.gameObject.transform.position = lastPos;
                                }
                            }
                            else
                            {
                                col.gameObject.transform.position = lastPos;
                            }
                        }
                        else
                        {
                            col.gameObject.transform.position = lastPos;
                        }
                    }
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
					h.ConditionalDamage(fallDamage, false, false, false);
					/*Vector3 lastPos = col.gameObject.GetComponent<GolemMovement>().lastGroundedPosition;
					col.gameObject.transform.position = lastPos;*/ // TODO
				}
			}
		}
	}
}
