using UnityEngine;
using System.Collections;

[AddComponentMenu("PRIM3/CharacterScripts/Actions/Abilities/Sulphur")] 
public class SulphurController : SpecialAbility
{
	//Animatior
	public bool active = false;
	[Header("Propiedades de proyectil")]
    public float forwardOffset = 2.0f;
	public float yOffset = 1.0f;
	public float zOffset = 0.0f;
    public float costeEnergia = 10.0f;
	public float shotDelay = 2.0f;
	public GameObject proyectilPrefab;
	public GameObject particlePrefab;
	[HideInInspector]
	public GameObject proyectil;

	[HideInInspector]
	public float timer = 0.0f;
    override public void ActionUpdate()
    {
		active = isActionActive ();
		ReduceAggro();
        bool jumping = true;
		if (timer < shotDelay) {
			timer += Time.deltaTime;
		} else {
			bool abilitiesWhileJumping = true;
			if (jump) {
				jumping = jump.isJumpingOrFalling ();
				abilitiesWhileJumping = jump.abilitiesWhileJumping;
			}
			if (specialPressed) {

			}
			if (specialReleased && (abilitiesWhileJumping || !jumping)) {
				if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Sulphur Fire") && energy.ConsumeEnergy (costeEnergia)) {
					//if (tp)
					//	tp.lookAtTargetRequest ();
					anim.Play("Sulphur Fire");
					//AnimControl.SetTrigger("Sulphur Ability");
					//proyectil = (GameObject)Instantiate (proyectilPrefab, transform.position + transform.forward * forwardOffset + transform.up * yOffset, transform.rotation);
					//timer = 0.0f;
				}
			}
		}
	}

	public override bool isActionActive ()
	{
		return (proyectil != null);
	}
}
