using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

//[RequireComponent(typeof(GeneralPlayerController))]
[RequireComponent(typeof(MovementManager))]
public class Roll : ActionBehaviour {
	// Componentes obligatorios
	private GeneralPlayerController gc;
	private MovementManager mm;
	private PrimeCamera primeCamera;
    private Health m_health;

	[Header("Tiempos")]
	public float cooldown = 3.0f;
	public float rollDuration = 1;
	private float timer = 0.0f;
	private float rollTimer = 0.0f;

	[Header("Energia")]
	public float energyCost = 3f;
	protected Energy energy;

	[Header("Velocidad")]
	public SpeedOverTime speedOverTime = SpeedOverTime.Constant;
	public float rollStartSpeed = 20f;
	public float rollEndSpeed = 0.0f;
	public float currentSpeed = 0.0f;

	[Header("Direccion")]
	public Direction direction = Direction.Free;


	[Header("Input")]
	private bool rolling = false;
	private bool collisionRolling = false;
	private bool rollButtonPressed;
	private float horizontalAmount;
	private float verticalAmount;

	[Header("Collision options")]
	public float minCollisionSpeed = 2f;
	private bool collisionActivated = false;

    [Header("Animacion")]
    public Animator AnimatorController;


	public enum SpeedOverTime {
		Decreasing,
		Constant
	};

	public enum Direction {
		Free,
		Forward
	};


	void Start() {
		primeCamera = Camera.main.GetComponent<PrimeCamera> ();
		gc = transform.gameObject.GetComponent<GeneralPlayerController>();
		mm = GetComponent<MovementManager> ();
        m_health = GetComponent<Health>();
		energy = GetComponent<Energy> ();
	}

	void Update () {
	}

    void OnEnable()
    {
        rolling = false;
    }


	override public void GetInput() {
		if (gc) 
		{
            if (gc.player == 0 && StaticParemeters.useKeyboard)
            {
                rollButtonPressed = Input.GetButtonDown ("Roll0");
                horizontalAmount =Input.GetAxis ("Horizontal0");
                verticalAmount =Input.GetAxis ("Vertical0");
            }
            else
            {
                rollButtonPressed = XCI.GetButtonDown(XboxButton.A, gc.controller);//Input.GetButtonDown ("Roll" + gc.player);
                horizontalAmount = XCI.GetAxis(XboxAxis.LeftStickX, gc.controller);//Input.GetAxis ("Horizontal" + gc.player);
                verticalAmount = XCI.GetAxis(XboxAxis.LeftStickY, gc.controller);//Input.GetAxis ("Vertical" + gc.player);
            }
			if (rollButtonPressed) {
				//Debug.Log ("Pressed by: " + gc.player);
			}
		}
	}

	void OnTriggerEnter(Collider other)  {
		if (other.GetComponent<Collider> ().gameObject.layer == LayerMask.NameToLayer ("Rolling")) {
			collisionActivated = true;
		}
	}

	void OnTriggerStay(Collider other)  {
		if (other.GetComponent<Collider> ().gameObject.layer == LayerMask.NameToLayer ("Rolling")) {
			collisionActivated = true;
			gameObject.layer = LayerMask.NameToLayer ("Rolling");
		}
	}


	void OnTriggerExit(Collider other)  {
		//Debug.Log ("OnTriggerExit!");
		if (other.GetComponent<Collider> ().gameObject.layer == LayerMask.NameToLayer ("Rolling")) {
			collisionActivated = false;
			collisionRolling = false;
			gameObject.layer = LayerMask.NameToLayer ("Player");
		}
	}
	

	override public void ActionUpdate() {
		timer += Time.deltaTime;

		if (rolling) {
			gameObject.layer = LayerMask.NameToLayer ("Rolling");
			if (rollTimer >= rollDuration) {
				//Debug.Log ("Normal rolling stopped!");
				rolling = false;
				rollTimer = 0;
				gameObject.layer = LayerMask.NameToLayer ("Player");
                if(m_health)
                {
                    m_health.invincible = false;
                }
                {
                    AnimatorController.SetBool("Dash", false);
                }

			} else {
				roll ();
				rollTimer += Time.deltaTime;
			}
		} else if(timer > (cooldown) && rollButtonPressed) {
			if(energy.ConsumeEnergy (energyCost))
			{
				roll ();
				timer = 0.0f;
				rolling = true;
			}
		} else if (collisionActivated){
			collisionRolling = true;
			collisionRoll ();
		}

	}

	private void roll() {
        if(m_health)
        {
            m_health.invincible = true;
            if(AnimatorController)
            {
                AnimatorController.SetBool("Dash", true);
            }
        }
		switch (speedOverTime) {
		case SpeedOverTime.Decreasing:
			decreasingRoll();
			break;
		case SpeedOverTime.Constant:
			constantRoll();
			break;
		}
	}

	private void constantRoll() {
		currentSpeed = rollStartSpeed;
		if (direction == Direction.Free && (verticalAmount != 0.0f || horizontalAmount != 0.0f)) {
			mm.movement += gameObject.transform.forward * rollStartSpeed * Time.deltaTime;
		} else {
			mm.movement += gameObject.transform.forward * rollStartSpeed * Time.deltaTime;
		}


	}
	private void decreasingRoll() {
		Vector3 dir;
		currentSpeed = Mathf.Lerp (rollStartSpeed, rollEndSpeed,  rollTimer / rollDuration);

		if (direction == Direction.Free && (verticalAmount != 0.0f || horizontalAmount != 0.0f)) {
			Vector3 cameraForward  = Vector3.Scale (primeCamera.transform.forward, new Vector3 (1.0f, 0.0f, 1.0f)).normalized;
			Vector3 cameraRight = primeCamera.transform.right;
			dir = (cameraForward * verticalAmount + cameraRight * horizontalAmount).normalized;
			gameObject.transform.forward = dir;
		} else {
			dir = gameObject.transform.forward ;
		}
		mm.movement += dir * Time.deltaTime * currentSpeed;
	}

	private void collisionRoll() {
		float speed;
		Vector3 dir;

		if (currentSpeed >= minCollisionSpeed) {
			speed = currentSpeed;
		} else {
			speed = minCollisionSpeed;
		}

		if (direction == Direction.Free && (verticalAmount != 0.0f || horizontalAmount != 0.0f)) {
			dir = (new Vector3(verticalAmount, 0.0f, 0.0f) + new Vector3(0.0f, 0.0f, -horizontalAmount)).normalized;
			gameObject.transform.LookAt(dir);
		} else {
			dir = gameObject.transform.forward;
		}
		mm.movement += dir * speed * Time.deltaTime;
	}

	override public bool isActionActive() {
		return rolling || collisionRolling;
	}
}
