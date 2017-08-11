using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeCollider : MonoBehaviour {


    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float pushTime;
    [HideInInspector]
    public float pushSpeed;


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("JumpingPlayer") || collision.gameObject.layer == LayerMask.NameToLayer("SaltShield"))
        {
            Damageable h = collision.gameObject.GetComponent<Damageable>();
			if (h) {
				if (h.Damage(damage) > 0.0f) {
                    PushBack(collision.gameObject);	
				}
			}
		}

	}

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("SaltShield"))
        {
            Damageable h = collision.gameObject.GetComponent<Damageable>();
            if (h)
            {
                if (h.Damage(damage) > 0.0f)
                {
                    PushBack(collision.gameObject);
                }
            }
        }
    }

	void PushBack(GameObject go) {
		IPushable p = go.GetComponent<IPushable> ();
		if (p != null) {
			p.Push(pushSpeed, pushTime, transform.position);
		}
	}
}
