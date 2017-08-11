using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;


[AddComponentMenu("PRIM3/CharacterScripts/Actions/Player Jump")] 
[RequireComponent(typeof(MovementManager))]
public class Jump : ActionBehaviour {
	// Componentes obligatorios
	private MovementManager mm;
	private CharacterController cc;
	private GeneralPlayerController gc;
	private Animator m_Animator;


	private bool jumpButtonPressed;
	public float jumpAcceleration = 100.0f;
	public float gravity = 20.0f;
	private float ySpeed = 0.0f;
	public float fallingSpeedThreshold = -1.5f;
	float ySpeedLimitLow = -30.0f;
	float ySpeedLimitHigh = 30.0f;

	public bool abilitiesWhileJumping = false;
	private bool jumping = false;

	private bool falling = false;

	protected Attack at;	
	protected bool JumpRest;

	public GameObject marker;
	
	float fallTime;
	public float timeToFall = 0.08f;
	public float maxTimeToJump = 0.3f;

	[Header("Energia")]
	public float energyCost = 0f;
	protected Energy energy;

	// Use this for initialization
	void Start () {
		mm = GetComponent<MovementManager> ();
		cc = GetComponent<CharacterController> ();
		gc = GetComponent<GeneralPlayerController> ();
		energy = GetComponent<Energy> ();
		FindMarker ();
		fallTime = 0f;
		m_Animator = GetComponentInChildren<Animator> ();
	}

	void FindMarker() {
		marker = GameObject.Find(gameObject.name+ "JumpMarker");
	}

	/* Obtiene los estados o valores del Input y los asigna a las diferentes acciones */
	override public void GetInput() {
        if (gc.player == 0 && StaticParemeters.useKeyboard)
        {
            jumpButtonPressed = Input.GetButtonDown("Jump0");
        }
        else
        {
            jumpButtonPressed = XCI.GetButtonDown(XboxButton.B, gc.controller); //Input.GetButtonDown ("Jump" + gc.player);
        }
	} 
	
	// Update is called once per frame
	void Update () {
		//GetInput ();
		//CalcGravity ();
	}

	override public bool isActionActive() {
		return isJumpingOrFalling ();
	}

	/* Calcula y guarda la direccion y cantidad de movimiento con la cual se movera el personaje sobre el eje Y */
	override public void ActionUpdate() {
		if (jumping || falling) {
			fallTime += Time.deltaTime;
		} else {
			fallTime = 0f;
		}
		JumpRest = transform.GetComponent<Movement>().attackWhileJumping;
		at = gc.GetComponent<Attack>();
		/*if (marker == null) {
			FindMarker ();
		}*/
		if (marker) {
			marker.gameObject.GetComponent<MeshRenderer> ().enabled = false;
		}
		bool attacking = false;
		if(at)
			attacking = at.isAttacking();
		// Aplicamos gravedad
		if (cc.isGrounded) {
			gameObject.layer = LayerMask.NameToLayer("Player");
			ySpeed = 0.0f;
			jumping = false;
			falling = false;
			m_Animator.SetBool("jumping", false);
		} else {
			ySpeed -= gravity * Time.deltaTime;
			ySpeed = Mathf.Clamp (ySpeed, ySpeedLimitLow, ySpeedLimitHigh);
		}
		if (ySpeed < 0.0f && (jumping == true || ySpeed < fallingSpeedThreshold)) {
			gameObject.layer = LayerMask.NameToLayer("Player"); // Cambiar por FallingPlayer
			jumping = false;
			falling = true;
		}
		// Aplicamos salto
		//if (jumpButtonPressed && cc.isGrounded && (!attacking || JumpRest) ) {
		if (jumpButtonPressed && canJump () && (!attacking || JumpRest) ) {
			if(energy.ConsumeEnergy (energyCost))
			{
				gameObject.layer = LayerMask.NameToLayer("JumpingPlayer");
				m_Animator.SetBool("jumping", true);
				m_Animator.SetTrigger("jumptrigger");
				ySpeed = jumpAcceleration;
				jumping = true;	
				if (marker) {
					marker.transform.position = new Vector3(transform.position.x,transform.position.y-1.0f,transform.position.z);
				}
				falling = false;
			}
		}
		if(jumping || falling)
		{
			if (marker) {
				marker.gameObject.GetComponent<MeshRenderer> ().enabled = true;
				marker.GetComponent<JumpMarker>().ActionUpdate();
			}
		//	marker.transform.position = new Vector3(transform.position.x,marker.transform.position.y,transform.position.z);
			//marker.transform.localPosition = new Vector3(marker.transform.localPosition.x,marker.transform.localPosition.y,marker.transform.localPosition.z);

			//marker.Move(new Vector3(transform.position.x,0.0f,transform.position.z));
			/*transform.position=new Vector3(transform.position.x,
			                           transform.position.y,
			                           transform.position.z);*/
		}/*else{
			marker.transform.position = new Vector3(transform.position.x,transform.position.y-1.0f,transform.position.z);
		}*/
		mm.movement.y += ySpeed * Time.deltaTime;
	}

	public bool isJumping() {
		return jumping;
	}

	public bool isFalling() {
		return falling;
	}

	public void ForceFall() {
		fallTime = timeToFall;
	}

	public bool canJump() {
		return !(jumping || fallTime >= maxTimeToJump);
	}

    public bool isJumpingOrFalling()
    {
		return fallTime >= timeToFall;
		//return jumping || falling;
    }
}
