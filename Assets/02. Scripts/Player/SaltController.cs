using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("PRIM3/CharacterScripts/Actions/Abilities/Salt")] 
public class SaltController : SpecialAbility {

	private bool m_Active = false;
	[Header("Propiedades del escudo")]
	public float radius = 5.0f;
	public GameObject prefabEscudo;
	public float movementModifier = 0.3f;
	public float activationDelay = 3.0f;
	private float m_Timer = 0.0f;

	public float energyAtActivation = 10.0f;
	public float energyPerSecond = 5.0f;
	public float energyPerDamageRatio = 1.0f;

	[Header("Propiedades del impulso del escudo")]
	public bool pushOnDeactivate = true;
	public float pushRadius = 20.0f;
	public float pushForce = 5.0f;
	public float pushTime = 1.0f;
	public float pushDamage = 15.0f;

	[Header("Propiedades del pull del escudo")]
	public bool pullWhenActive = true;
	public float pullRadius = 30.0f;
	public float pullForce = 3.0f;
	public float pullTime = 0.2f;
	public float pullMinDistance = 1.0f;

    override public void ActionUpdate() {
		bool l_Jumping = true;
		bool l_AbilitiesWhileJumping = true;
		if (m_Timer < activationDelay) {
			m_Timer += Time.deltaTime;
		}
		if (jump) {
			l_Jumping = jump.isJumpingOrFalling();
			l_AbilitiesWhileJumping = jump.abilitiesWhileJumping;
		}

        if (l_Jumping)
        {
            m_Active = true;
            movementBehaviour.m_AllowMovement = true;
        }

		GameObject l_Shield = GameObject.FindGameObjectWithTag("SaltShield");
		if (specialPressed && !l_Shield && (l_AbilitiesWhileJumping || !l_Jumping) && energy.ConsumeEnergy (energyAtActivation)) {
			if (m_Timer >= activationDelay) {
				anim.Play("Activate Shield");
				l_Shield = (GameObject)Instantiate (prefabEscudo, transform.position, transform.rotation);
				l_Shield.transform.parent = transform;
				l_Shield.transform.position -= Vector3.up;
				l_Shield.transform.localScale = Vector3.one * radius;
				l_Shield.GetComponent<DamageToEnergy> ().source = energy;
				l_Shield.GetComponent<DamageToEnergy> ().energyPerDamageRatio = energyPerDamageRatio;
				m_Active = true;
				if (jump) {
					jump.ForceFall();
				}
			} else {
				energy.RecoverEnergy(energyAtActivation);
			}
		} else
		if ((specialReleased || (l_Jumping && !l_AbilitiesWhileJumping)) && l_Shield) {
			Destroy (l_Shield);
			m_Active = false;
			DoPush ();
		} else
		if (l_Shield) {
			anim.SetBool("ShieldActive",true);
			//mm.movement.y = 0.0f;
			mm.movement *= movementModifier;
			DoPull();
			if (!energy.ConsumeEnergy (energyPerSecond * Time.deltaTime)) {
				Destroy (l_Shield);
				anim.SetBool("ShieldActive",false);
			}
			aggro += abilityAggro * Time.deltaTime;
			m_Timer = 0f;
		} else {
			anim.SetBool("ShieldActive",false);
			ReduceAggro();
		}
	}

	private void DoPush() {
		if (pushOnDeactivate) {
			List<GameObject> l_Targets = CustomTagManager.GetObjectsByTag ("Enemy");
			for (int i = 0; i < l_Targets.Count; ++i) {
				staticAttackCheck.CheckPush (l_Targets [i].transform, transform.position, pushRadius, pushForce, pushTime);
				if (pushDamage > 0f) {
					staticAttackCheck.checkAttack(l_Targets[i].transform, transform, pushRadius, 360.0f, pushRadius, pushDamage);
				}
			}
		}
	}

	private void DoPull() {
		if (pullWhenActive) {
			List<GameObject> l_Targets = CustomTagManager.GetObjectsByTag ("Enemy");
			for (int i = 0; i < l_Targets.Count; ++i) {
				staticAttackCheck.CheckPull (l_Targets [i].transform, transform.position, pullRadius, pullForce, pullTime, pullMinDistance);
			}
		}
	}

    public void DeactivateShield()
    {
        GameObject l_Shield = GameObject.FindGameObjectWithTag("SaltShield");
        Destroy(l_Shield);
        m_Active = false;
    }

	public override bool isActionActive ()
	{
		return m_Active;
	}
}
