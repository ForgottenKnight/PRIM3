using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltShieldExplosionManager : MonoBehaviour {
    SaltShieldExplosion shieldExplosion;

	void Start () {
        shieldExplosion = GetComponentInChildren<SaltShieldExplosion>();
	}

    void OnDestroy()
    {
        shieldExplosion.transform.SetParent(transform.parent);
        shieldExplosion.DestroyAnimation();
    }
}
