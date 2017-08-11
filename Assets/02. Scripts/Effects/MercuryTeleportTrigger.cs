using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercuryTeleportTrigger : MonoBehaviour {
	private MercuryTeleport mercuryTeleport;

	public void Start() {
		mercuryTeleport = transform.GetComponentInParent<MercuryTeleport> ();
	}

	public void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("SulphurProjectile"))
        {
            ProjectileController l_PC = col.gameObject.GetComponent<ProjectileController>();
            if (l_PC)
            {
                l_PC.enhanced = true;
                l_PC.changeProjectile();
            }
        }
	}


	public void OnTriggerStay(Collider col) {
		if (col.tag == "Enemy") {
			mercuryTeleport.TriggerPulseDamage (col.gameObject);
			mercuryTeleport.TriggerPulseSlow (col.gameObject);
        }
	}
}
