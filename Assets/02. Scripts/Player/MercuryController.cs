using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("PRIM3/CharacterScripts/Actions/Abilities/Mercury")] 
public class MercuryController : SpecialAbility {
	public CharacterController marker;
	[Header("Propiedades comunes del teleport")]
	public float minDistancia = 3.0f;
	public float maxDistancia = 10.0f;
	public float minTiempo = 0.0f;
	public float maxTiempo = 3.0f;
	public float costeEnergia = 20.0f;
    public float energyPerSecond = 5.0f;
	public bool teleportIfEnergyWasted = false;

	public float delay = 2.0f;
	private float m_DelayTimer = 0.0f;
	public bool lookAtTarget = true;
	public TeleportMode teleportMode = TeleportMode.combat;
	public MarkerMode markerMode = MarkerMode.forward;
	public bool particles = false;

	[Header("Propiedades Teleport precisión")]
	public float speed = 15.0f;
	private Vector3 originalPosition = Vector3.zero;
	private bool m_firstTeleportFrame = true;

	[Header("Propiedades DoT")]
	public float distanceBetweenDots = 3.0f;
	public GameObject mercuryTeleport; 


	private float timer = 0.0f;
	private bool timerActive = false;
	public bool active = false;

	public enum TeleportMode {
		combat,
		precision
	}

	public enum MarkerMode {
		forward,
		grounded
	}
	

	public override bool isActionActive ()
	{
		return active;
	}

	override public void ActionUpdate() {
		bool jumping = true;
		bool abilitiesWhileJumping = true;
		ReduceAggro();

		if (jump) {
            jumping = jump.isJumpingOrFalling();
			abilitiesWhileJumping = jump.abilitiesWhileJumping;
		}

		if (!abilitiesWhileJumping && jumping) {
			if (timerActive) {
				timerActive = false;
                active = false;
				energy.ReleaseReservedEnergy(costeEnergia);
			}
		}
		m_DelayTimer = Mathf.Clamp(m_DelayTimer + Time.deltaTime, 0.0f, delay);
		if (timerActive) {
			anim.SetBool ("Moving", false);
			timer += Time.deltaTime;
			if (lookAtTarget) {
				if (tp)
					tp.lookAtTargetRequest ();
			}
			marker.gameObject.GetComponent<MeshRenderer> ().gameObject.SetActive (true);

			Vector3 movement = Vector3.zero;
            if (!energy.ConsumeEnergy(energyPerSecond * Time.deltaTime))
            {
				if (active)
                {
                    gameObject.transform.Find("MarkerContainer").gameObject.SetActive(false);
                    switch (teleportMode)
                    {
                        case TeleportMode.combat:
                            break;
                        case TeleportMode.precision:
                            gameObject.GetComponent<MercuryController>().deleteAction(gameObject.GetComponent<Movement>());
                            break;
                    }

                    if (energy.ConsumeReservedEnergy(costeEnergia))
                    {
						if (timerActive)
                        {
                            timerActive = false;
							if ((abilitiesWhileJumping || !jumping) && teleportIfEnergyWasted) {
								gameObject.layer = 9;
								createFieldEffect ();
								cc.Move (marker.transform.position - transform.position);
								marker.transform.localPosition = Vector3.zero;
								gameObject.layer = 8;
								aggro += abilityAggro;
								m_DelayTimer = 0.0f;
							} else {
								energy.RecoverEnergy (costeEnergia);
							}
                        }
                        active = false;
                    }
                }
            }
			switch (teleportMode) {
			case TeleportMode.combat:
				marker.transform.localPosition = Vector3.zero;
				float lerp = Mathf.Clamp (Mathf.InverseLerp (minTiempo, maxTiempo, timer), 0.0f, 1.0f);
				float distancia = Mathf.Lerp (minDistancia, maxDistancia, lerp);
				movement = (transform.forward * distancia);
				break;
			case TeleportMode.precision:
				if (m_firstTeleportFrame) {
					marker.transform.localPosition = new Vector3 (marker.transform.localPosition.x, 0.0f, marker.transform.localPosition.z  + minDistancia);
					m_firstTeleportFrame = false;
				} else {
					marker.transform.localPosition = new Vector3 (marker.transform.localPosition.x, 0.0f, marker.transform.localPosition.z);
				}
				Vector3 direction = new Vector3 (horizontalAmount, 0, verticalAmount); 
				direction = Camera.main.transform.TransformDirection (direction);
				movement = (direction * speed * Time.deltaTime);
				float distance  = Vector3.Distance(marker.transform.position + movement, originalPosition);

				if(distance < minDistancia) {
					//marker.transform.position = new Vector3(transform.position.x, 0, transform.position.z) + transform.forward * minDistancia;
				} else if (distance > maxDistancia){
					movement = Vector3.zero;
				}

				break;
			}

			switch(markerMode) {
				case MarkerMode.forward:
					marker.transform.position = marker.transform.position + transform.up;
					marker.Move (movement);
				break;
				case MarkerMode.grounded:
					marker.Move (movement - transform.up * 100.0f);
				break;
			}


		} else {
			marker.gameObject.GetComponent<MeshRenderer> ().gameObject.SetActive (false);
		}

		if (specialPressed && m_DelayTimer >= delay) {
			if ( energy.GetReservedEnergy() >= costeEnergia || energy.ReserveEnergy (costeEnergia)) {
				timerActive = true;
				timer = 0.0f;
		
				GameObject markerContainer = gameObject.transform.Find ("MarkerContainer").gameObject;
				markerContainer.SetActive (true);
				switch (teleportMode) {
				case TeleportMode.combat:
					break;
				case TeleportMode.precision:
					if (!active) {
						m_firstTeleportFrame = true;
						originalPosition = gameObject.transform.position;
					}
					gameObject.GetComponent<MercuryController> ().blockAction (gameObject.GetComponent<Movement> ());
						//markerContainer.transform.position = new Vector3(marker.transform.position.x, marker.transform.position.y, marker.transform.position.z);
					break;
				}
				active = true;
				anim.SetBool ("Moving", false);
				anim.Play ("Idle");
			}
		} 

		if (specialReleased) {
			if(active) {
				gameObject.transform.Find("MarkerContainer").gameObject.SetActive(false);
				switch(teleportMode) {
					case TeleportMode.combat:
						break;
					case TeleportMode.precision:
						gameObject.GetComponent<MercuryController>().deleteAction(gameObject.GetComponent<Movement>());
						break;
				}

				if(energy.ConsumeReservedEnergy(costeEnergia)){
					if (timerActive) {
						timerActive = false;
						if ((abilitiesWhileJumping || !jumping)) {
							gameObject.layer = 9;
							createFieldEffect ();
							cc.Move (marker.transform.position - transform.position);
							marker.transform.localPosition = Vector3.zero;
							gameObject.layer = 8;
							aggro += abilityAggro;
							m_DelayTimer = 0.0f;
						}
					}
					active = false;
				}
			}
		}
	}
		
	public void  createFieldEffect() {

		Vector3 l_direction = (marker.transform.position - transform.position);

		float l_distance = l_direction.magnitude;
		int l_steps = (int)(l_distance / distanceBetweenDots);

		if (l_distance % (distanceBetweenDots) != 0) {
			l_steps++;
		}
	
		float l_distanceBetweenColliders = l_distance / l_steps;
		l_direction = l_direction.normalized;

		for (int i = 0; i < l_steps; i++) {

			//Customizando parametros del prefab
			GameObject l_mercuryEffect = Instantiate (mercuryTeleport);
			l_mercuryEffect.transform.forward = transform.forward;
			GameObject l_mercuryEffectParticles = l_mercuryEffect.transform.Find ("Particles").gameObject;

			if (i == 0 || i == (l_steps - 1)) {
				l_mercuryEffectParticles.transform.position -= new Vector3 (0, 1, 0);
			}

			RaycastHit l_hit;
			int l_layerMask = 1;

			if(Physics.Raycast ((transform.position + l_direction * i * l_distanceBetweenColliders), -Vector3.up, out l_hit, 100.0f, l_layerMask)) {
				l_mercuryEffect.transform.eulerAngles = new Vector3(Vector3.Angle(l_hit.normal, l_mercuryEffect.transform.up), l_mercuryEffect.transform.eulerAngles.y,  l_mercuryEffect.transform.eulerAngles.z);
				l_mercuryEffect.transform.position = new Vector3 (l_hit.point.x, l_hit.point.y + l_mercuryEffect.GetComponent<MercuryTeleport>().effectSize.y * 0.5f, l_hit.point.z);
			}
		}
	}
}


