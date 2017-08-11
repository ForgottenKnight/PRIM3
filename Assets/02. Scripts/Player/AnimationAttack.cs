using UnityEngine;
using System.Collections;

public class AnimationAttack : MonoBehaviour {
	Animator anim;
	public Attack attk;
	public Transform enemies;
    public Movement m_Movement;

	public GameObject damageParticles;

	void Start() {
		anim = GetComponent<Animator> ();
		enemies = GameObject.FindGameObjectWithTag ("EnemiesContainer").transform;
	}

	public void CalculateAttack() {
		if (attk.tp != null) {
			attk.tp.lookAtTargetRequest ();
		}
		
		int childCount = enemies.childCount;
		for (int i = 0; i < childCount; ++i) {
			float dmg = staticAttackCheck.checkAttack (enemies.GetChild (i), attk.transform, attk.attackLength, attk.attackAngle, attk.attackHeight, attk.attackForce + attk.chargedDamage, attk.damageBonus, attk.currentPhase);
			if (dmg > 0.0f) {
				++attk.currentCombo;
				attk.comboTimer = 0.0f;
				attk.aggro += dmg;

				if (damageParticles != null) {
					Instantiate (damageParticles, enemies.GetChild (i).position, enemies.GetChild (i).rotation);
				}
			}
		}
		attk.chargedDamage = 0.0f;
	}

    public void SlowDown()
    {
        m_Movement.currentSpeed = 0.0f;
    }


}
