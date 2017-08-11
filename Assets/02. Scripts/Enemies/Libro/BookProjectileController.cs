using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookProjectileController : MonoBehaviour {

    [Header("Projectile parameters")]
    public float speed = 1.0f;
    public float damage = 10.0f;
    public float slow = 2f;
    public float slowTimer = 1.5f;
    public float timeToDestroy = 10f;

	public GameObject explosionPrefab;

    // Use this for initialization
    void Start()
    {
        Invoke("Die", timeToDestroy);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    void Die()
    {
		Instantiate (explosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "SaltShield")
        {        
            Damageable h = col.gameObject.GetComponent<Damageable>();
			if (h) {
                if (h.Damage(damage) > 0.0f)
                {									
					Attack l_Attack = col.transform.gameObject.GetComponent<Attack>();
                    if (l_Attack)
					{
                        l_Attack.currentCombo = 0;
                        l_Attack.currentPhase = 0;
					}
				}
			}
            Movement l_Mov = col.gameObject.GetComponent<Movement>();
            if(l_Mov)
            {
                l_Mov.SlowDown(l_Mov.movementSpeed / slow, slowTimer);
            }
		}
        
        Die();
    }
}
