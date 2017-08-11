using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaltShieldExplosion : MonoBehaviour {
    Animator anim;
	ParticleSystem destroyParticles;

	void Start () {
        anim = GetComponent<Animator>();
		destroyParticles = GetComponentInChildren<ParticleSystem> ();
	}

    public void DestroyAnimation() {
        anim.SetBool("Destroy", true);
        anim.enabled = true;
		destroyParticles.Play ();
        Destroy(gameObject, 3.0f);
    }
}
