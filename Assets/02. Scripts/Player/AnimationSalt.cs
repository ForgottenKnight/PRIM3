using UnityEngine;
using System.Collections;

public class AnimationSalt : MonoBehaviour {
	Animator anim;
	public Movement saltMovement;

	private float m_Speed;
	private float m_RotationSpeed;

	void Start() {
		anim = GetComponent<Animator> ();
	}
	
	public void ActivateShield() {
		//Impedir movimiento
		saltMovement.m_AllowMovement = false;
		//m_Speed = saltMovement.movementSpeed;
		//m_RotationSpeed = saltMovement.rotationSpeed;

		//saltMovement.movementSpeed = 0.0f;
		//saltMovement.rotationSpeed = 0.0f;
	}

	public void ShieldActive() {
		//Dar movimiento
		
		saltMovement.m_AllowMovement = true;
	//	saltMovement.movementSpeed = m_Speed;
	//	saltMovement.rotationSpeed = m_RotationSpeed;
	}
}
