using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[AddComponentMenu("PRIM3/CharacterScripts/Actions/Player Movement")] 
[RequireComponent(typeof(MovementManager))]
public class Movement : ActionBehaviour {
	// Componentes obligatorios
    [HideInInspector]
	public MovementManager mm;
	private CharacterController cc;
	private GeneralPlayerController gc;

	// Componentes opcionales
	private Jump jump;
	private Attack attack;

	
	[Header("Parametros")]
	public float movementSpeed = 10.0f;
    [HideInInspector]
	public float currentSpeed = 0.0f;
	public float rotationSpeed = 10.0f;
	public float acceleration = 2.0f;
	public float deceleration = 2.0f;
	public bool instantAcceleration = false;
	public bool instantDeceleration = false;
    public float animatorSlowDivider = 2f;
    private float m_animatorSlowSpeed;
    public float lastGroundedPoitionTimer = 1.0f;
	private Vector3 lastMovementDirection = Vector3.zero;

	private PrimeCamera primeCamera;
	Transform cameraTransform;
	Vector3 cameraForward;
	Vector3 cameraRight;


	float verticalAmount = 0.0f;
	float horizontalAmount = 0.0f;

	
	[Header("Restricciones")]

	public bool attackWhileJumping = true;
	public bool rotationWhileJumping = false;
	public float rotationSpeedWhileJumping = 1.0f;

	[Range(0.0f, 1.0f)]
	public float JumpFreedomForward = 1.0f;
	[Range(0.0f, 1.0f)]
	public float JumpFreedomBackward = 1.0f;
	[Range(0.0f, 1.0f)]
	public float JumpFreedomRight = 1.0f;
	[Range(0.0f, 1.0f)]
	public float JumpFreedomLeft = 1.0f;
	
	public bool rotationWhileAttacking = false;
	public float rotationSpeedWhileAttacking = 1.0f;
	[Range(0.0f, 1.0f)]
	public float AttackFreedomForward = 1.0f;
	[Range(0.0f, 1.0f)]
	public float AttackFreedomBackward = 1.0f;
	[Range(0.0f, 1.0f)]
	public float AttackFreedomRight = 1.0f;
	[Range(0.0f, 1.0f)]
	public float AttackFreedomLeft = 1.0f;

    [HideInInspector]
	public bool slowDown = false;
	private float slowDownSpeed = 0;

	[HideInInspector]
	public Vector3 lastGroundedPosition = Vector3.zero;

	[HideInInspector]
	public bool m_AllowMovement = true;

	Animator anim;


	// Use this for initialization
	void Start () {
		mm = GetComponent<MovementManager> ();
		cc = GetComponent<CharacterController> ();
		gc = GetComponent<GeneralPlayerController> ();

		primeCamera = Camera.main.GetComponent<PrimeCamera> ();
		cameraTransform = primeCamera.transform;
		jump = GetComponent<Jump> ();
		attack = GetComponent<Attack> ();
		anim = GetComponentInChildren<Animator> ();
        m_animatorSlowSpeed = anim.speed / animatorSlowDivider;
        lastGroundedPosition = transform.position;

       // StartCoroutine("GroundedPosition");
	}

    void OnEnable()
    {
        lastGroundedPosition = transform.position;
        StartCoroutine("GroundedPosition");
    }

	/* Obtiene los estados o valores del Input y los asigna a las diferentes acciones */
	override public void GetInput() {
        if (gc.player == 0 && StaticParemeters.useKeyboard)
        {
            verticalAmount = Input.GetAxis ("Vertical0");
            horizontalAmount = Input.GetAxis ("Horizontal0");
        }
        else
        {
            verticalAmount = XCI.GetAxis(XboxAxis.LeftStickY, gc.controller);//Input.GetAxis ("Vertical" + gc.player);
            horizontalAmount = XCI.GetAxis(XboxAxis.LeftStickX, gc.controller);//Input.GetAxis ("Horizontal" + gc.player);
        }
	} 

	public void SlowDown(float newSpeed, float cameraSlowTime) {
        if(!slowDown)
        {
            anim.speed = m_animatorSlowSpeed;
        }
		slowDown = true;
		slowDownSpeed = newSpeed;
		CancelInvoke ("FastUp");
		Invoke ("FastUp", cameraSlowTime);
	}

	public void FastUp() {
		slowDown = false;
       anim.speed = m_animatorSlowSpeed * animatorSlowDivider;
	}
	
	// Update is called once per frame
	void Update () {
		//GetInput ();
	}

	override public bool isActionActive() {
		if (currentSpeed > 0) {
			return true;
		} else {
			return false;
		}
	}

	public void ResetSpeed() {
		currentSpeed = 0.0f;
	}

    IEnumerator GroundedPosition()
    {
        while (true)
        {
            Vector3 lastPos = transform.position;
            Vector3 up = transform.up;
            Ray ray = new Ray(lastPos, -up);
            RaycastHit hit;
            LayerMask mask;
            mask = 1 << LayerMask.NameToLayer("Player");
            mask |= (1 << LayerMask.NameToLayer("JumpingPlayer"));
            mask = ~mask;

            if (cc && cc.isGrounded && Physics.Raycast(ray, out hit, 10, mask) )
            {
                if (hit.collider.gameObject.layer != gameObject.layer)
                {
                    lastGroundedPosition = transform.position;
                }
            }
            yield return new WaitForSeconds(lastGroundedPoitionTimer);
        }
    }

	/* Calcula y guarda la direccion y cantidad de movimiento con la cual se movera el personaje en los ejes X y Z */
	override public void ActionUpdate() {

		float movementSum = Mathf.Abs (verticalAmount) + Mathf.Abs (horizontalAmount);
		if (movementSum != 0 && !attack.isChargingAttack() && m_AllowMovement) {
			anim.SetBool("Moving", true);

			//Calculamos la velocidad actual

			if(instantAcceleration){
				currentSpeed = movementSpeed;
			} else {
				float newSpeed = currentSpeed + acceleration * Time.deltaTime;
				if(newSpeed <= movementSpeed) {
					currentSpeed = newSpeed;
				} else {
					currentSpeed = movementSpeed;
				}
			}

			if(slowDown) {
				if(currentSpeed > slowDownSpeed) {
					currentSpeed = slowDownSpeed;
				}
			}


			// Calculamos el forward y el right de la camara
			cameraForward = Vector3.Scale (cameraTransform.forward, new Vector3 (1.0f, 0.0f, 1.0f)).normalized;
			cameraRight = cameraTransform.right;
			// Calculamos el movimiento respecto a los ejes de la camara
			lastMovementDirection = (cameraForward * verticalAmount + cameraRight * horizontalAmount).normalized;
			mm.movement += lastMovementDirection * Time.deltaTime * currentSpeed;

			if (attack) {
				if (attack.isAttacking () && rotationWhileAttacking) {
					RotateCharacter ();
					LimitAttackingMovement ();
				} else if (attack.isAttacking ()) {
					LimitAttackingMovement ();
				}
			} 

			if (jump) {
				if (jump.isJumpingOrFalling () && rotationWhileJumping) {					
					RotateCharacter ();
					LimitJumpingMovement ();
				} else if (jump.isJumpingOrFalling ()) {
					LimitJumpingMovement ();
				}
			}
			if (jump && attack) {
				if (!jump.isJumpingOrFalling () && !attack.isAttacking ())
					RotateCharacter ();
			} else if (jump) {
				if (!jump.isJumpingOrFalling ())
					RotateCharacter ();
			} else if (attack) {
				if (!attack.isAttacking ())
					RotateCharacter ();
			} else {
				RotateCharacter ();
			}
		} else {
			//Desaceleramos al personaje
			if(instantDeceleration){
				currentSpeed = 0;
			} else {
				float newSpeed = currentSpeed - deceleration * Time.deltaTime;
				if(newSpeed >= 0) {
					currentSpeed = newSpeed;
				} else {
					currentSpeed = 0;
					anim.SetBool("Moving", false);
				}

				if(slowDown) {
					if(currentSpeed > slowDownSpeed) {
						currentSpeed = slowDownSpeed;
					}
				}
				mm.movement += lastMovementDirection * Time.deltaTime * currentSpeed;
			}
		}
	}

	/* Rota al personaje en funcion de la direccion a la que se mueve. */
	void RotateCharacter() {
		// Calculamos la rotacion y rotamos
		Vector3 mov = transform.InverseTransformDirection(mm.movement);
		mov = Vector3.ProjectOnPlane(mov, Vector3.up);
		float direction;

		direction = Mathf.Atan2 (mov.x, mov.z) * rotationSpeed;

		if (jump) {
			if (jump.isJumpingOrFalling() && rotationWhileJumping)
			{
				direction = Mathf.Atan2 (mov.x, mov.z) * rotationSpeedWhileJumping;
			} 
		}

		/*if (attack) {
			if (attack.isAttacking () && rotationWhileAttacking) {

				direction = Mathf.Atan2 (mov.x, mov.z) * rotationSpeedWhileAttacking;
			}
		}*/
		/*
		if (jump) {
            if (jump.isJumpingOrFalling() && rotationWhileJumping)
            {
				direction = Mathf.Atan2 (mov.x, mov.z) * rotationSpeedWhileJumping;
			} else {
				direction = Mathf.Atan2 (mov.x, mov.z) * rotationSpeed;
			}
		} else{
			direction = Mathf.Atan2 (mov.x, mov.z) * rotationSpeed;
		}
*/
		transform.Rotate(Vector3.up, direction);
	}

	/* Limita el movimiento del personaje en los 4 ejes (Frontal, Trasero, Derecho e Izquierdo) mientras se encuentra en el aire. 
	 * La cantidad de movimiento de un eje dependera de su libertad de movimiento , la cual comprende valores entre 0 (Eje completamente bloqueado)
	 * y 1 (Maxima libertad). */
	void LimitJumpingMovement() {
		// Convertimos los vectores forward y right del personaje a la misma magnitud que el movimiento
		Vector3 mgnForward = Vector3.ClampMagnitude(transform.forward, mm.movement.magnitude);
		Vector3 mgnRight = Vector3.ClampMagnitude(transform.right, mm.movement.magnitude);

		// Calculamos el Dot Product del movimiento y el forward/right. Con ello sabemos si va hacia adelante/atras o izquierda/derecha.
		// Luego multiplicaremos por dotFront y dotRight respectivamente, porque nos podemos mover adelante + derecha a la vez.
		float dotFront = Vector3.Dot (mm.movement.normalized, transform.forward);
		float dotRight = Vector3.Dot (mm.movement.normalized, transform.right);
		if (dotFront > 0) { // Front
			mm.movement -= mgnForward * (1.0f - JumpFreedomForward) * dotFront;
		} else if (dotFront < 0) { // Backwards
			mm.movement -= mgnForward * (1.0f - JumpFreedomBackward) * dotFront;
		}
		if (dotRight > 0) { // Right
			mm.movement -= mgnRight * (1.0f - JumpFreedomRight) * dotRight;
		} else if (dotRight < 0) { // Left
			mm.movement -= mgnRight * (1.0f - JumpFreedomLeft) * dotRight;
		}
	}

	void LimitAttackingMovement() {
		// Convertimos los vectores forward y right del personaje a la misma magnitud que el movimiento
		Vector3 mgnForward = Vector3.ClampMagnitude(transform.forward, mm.movement.magnitude);
		Vector3 mgnRight = Vector3.ClampMagnitude(transform.right, mm.movement.magnitude);
		
		// Calculamos el Dot Product del movimiento y el forward/right. Con ello sabemos si va hacia adelante/atras o izquierda/derecha.
		// Luego multiplicaremos por dotFront y dotRight respectivamente, porque nos podemos mover adelante + derecha a la vez.
		float dotFront = Vector3.Dot (mm.movement.normalized, transform.forward);
		float dotRight = Vector3.Dot (mm.movement.normalized, transform.right);
		if (dotFront > 0) { // Front
			mm.movement -= mgnForward * (1.0f - AttackFreedomForward) * dotFront;
		} else if (dotFront < 0) { // Backwards
			mm.movement -= mgnForward * (1.0f - AttackFreedomBackward) * dotFront;
		}
		if (dotRight > 0) { // Right
			mm.movement -= mgnRight * (1.0f - AttackFreedomRight) * dotRight;
		} else if (dotRight < 0) { // Left
			mm.movement -= mgnRight * (1.0f - AttackFreedomLeft) * dotRight;
		}
	}








}
