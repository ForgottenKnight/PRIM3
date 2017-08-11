using UnityEngine;
using System.Collections.Generic;

public class ProjectileController : MonoBehaviour {
	[Header("Projectile parameters")]
	public float speed;
	public float timeToDestroy;
	public bool damageToPlayers = true;
	public float percentDamageToPlayers = 0.5f;
    public bool enhanced = false;
    public GameObject normalProjectile;
    public GameObject enhancedProjectile;

	//public GameObject area;
	[Header("Area parameters")]
	public GameObject explosionEffect;
	[HideInInspector]
	public GameObject source;
	public float normalRadius = 2.5f;
    public float enhancedRadius = 5f;
	public float normalForce = 15.0f;
    public float enhancedForce = 20.0f;
	[Header("Push parameters")]
	public bool push = true;
	public float pushTime = 0.4f;
	public float pushSpeed = 8f;

	// Use this for initialization
	void Start () {
		Invoke ("Die", timeToDestroy);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += transform.forward * Time.deltaTime * speed;
	}

	void Die() {
		Destroy (gameObject);
	}

    public void changeProjectile()
    {
        normalProjectile.SetActive(false);
        enhancedProjectile.SetActive(true);
    }

	void OnTriggerEnter(Collider coll) {
        if (coll.gameObject.layer != LayerMask.NameToLayer("MercuryTeleportEffect"))
        {
            CancelInvoke();
            List<GameObject> enemies = CustomTagManager.GetObjectsByTag("Enemy");
            for (int i = 0; i < enemies.Count; ++i)
            {
                Transform t = enemies[i].transform;
                if (enhanced)
                {
                    staticAttackCheck.checkAttack(t, transform, enhancedRadius, 360.0f, enhancedRadius, enhancedForce);
                }
                else
                {
                    staticAttackCheck.checkAttack(t, transform, normalRadius, 360.0f, normalRadius, normalForce);
                }
                if (push && enhanced)
                {
                    staticAttackCheck.CheckPush(t, transform.position, enhancedRadius, pushSpeed, pushTime, source.transform.position);
                }
            }
            if (coll.gameObject.layer == LayerMask.NameToLayer("Tree"))
            {
                Damageable l_Damageable = coll.GetComponent<Damageable>();
                if (l_Damageable != null)
                {
                    l_Damageable.Damage(1f);
                }
            }
            if (damageToPlayers == true)
            {
                List<GameObject> players = CustomTagManager.GetObjectsByTag("Player");
                for (int i = 0; i < players.Count; ++i)
                {
                    Transform t = players[i].transform;
                    if (enhanced)
                    {
                        staticAttackCheck.checkAttack(t, transform, enhancedRadius, 360.0f, enhancedRadius, enhancedForce * percentDamageToPlayers);
                    }
                    else
                    {
                        staticAttackCheck.checkAttack(t, transform, normalRadius, 360.0f, normalRadius, normalForce * percentDamageToPlayers);
                    }
                    if (push && enhanced)
                    {
                        staticAttackCheck.CheckPush(t, transform.position, enhancedRadius, pushSpeed, pushTime, source.transform.position);
                    }
                }
                GameObject saltShield = CustomTagManager.GetObjectByTag("SaltShield");
                if (saltShield != null)
                {
                    Transform t = saltShield.transform;
                    if (enhanced)
                    {
                        staticAttackCheck.checkAttack(t, transform, enhancedRadius, 360.0f, enhancedRadius, enhancedForce * percentDamageToPlayers);
                    }
                    else
                    {
                        staticAttackCheck.checkAttack(t, transform, normalRadius, 360.0f, normalRadius, normalForce * percentDamageToPlayers);
                    }
                    if (push && enhanced)
                    {
                        staticAttackCheck.CheckPush(t, transform.position, enhancedRadius, pushSpeed, pushTime, source.transform.position);
                    }
                }
            }
            Instantiate(explosionEffect, transform.position, transform.rotation);
            Die();
        }
	}
}
