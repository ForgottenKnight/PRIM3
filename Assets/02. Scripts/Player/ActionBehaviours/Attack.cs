using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;


[RequireComponent(typeof(GeneralPlayerController))]
[AddComponentMenu("PRIM3/CharacterScripts/Actions/Player Attack")] 
public class Attack : ActionBehaviour {	
	GeneralPlayerController gc;
	[HideInInspector]
	public TargetPointer tp;


    [Header("Transform de Enemigos")]
	public Transform enemies;

     [Header("Parametros de ataque")]
	public float attackLength = 1.0f;
	public float attackAngle = 180.0f;
	public float attackHeight = 1.0f;

	public float attackForce = 10.0f;

	//public float attackSpeed = 1.0f;
	//public float attackSpeed2 = 1.0f;
	//public float attackSpeed3 = 1.0f;	

     [Header("Parametros de Combo")]
	public int comboPhases = 4;
	public int upgradePhase = 25;
	public float damageBonus = 25.0f;
	public float maxComboTimer = 10.0f;

    [Header("Carga de ataque")]
	public float damageChargeTimer = 2.5f;
    public float maxBonusDamage = 50.0f;
    public float stepDamageIncrease = 5f;

	[HideInInspector]
	public float chargeTimer = 0.0f;
	[HideInInspector]
	public int currentCombo = 0;
	[HideInInspector]
	public float comboTimer = 0.0f;
	[HideInInspector]
	public int currentPhase = 0;
	public bool chargeInterruptable = true;

	[HideInInspector]
	public float chargedDamage = 0.0f;

	//Attack Animation
	public Animator AnimControl;
    private SM_Attack1 m_SMAttack1;
    private SM_Attack2 m_SMAttack2;
    
	//protected TargetPointer tp;
	protected int maxChainAttack = 3;
	protected int currentChainAttack = 0;
	public float maxChainTime = 5.0f;
	public float timer = 0.0f;

	//Input
	protected bool attackPressed = false;
	protected bool attackReleased = false;

	protected bool JumpRest;
	protected Jump jump;

	protected bool m_FirstCharge = true;

	public float aggro = 0.0f;
	public float aggroReductionPerSecond = 5.0f;

	[Header("Energia")]
	public float energyCost = 0f; 
	protected Energy energy;

/*	[Header("Sounds")]
	public AudioClip swingAttack;
	AudioSource camAudioSource;*/

	void ReduceAggro() {
		if (aggro > 0.0f) {
			aggro = Mathf.Clamp(aggro - aggroReductionPerSecond * Time.deltaTime, 0.0f, 1000.0f);
		}
	}

	// Use this for initialization
	void Start () {
		gc = gameObject.GetComponent<GeneralPlayerController>();
		tp = gameObject.GetComponent<TargetPointer> ();
		//AnimControl = GetComponent<Animator> ();

		energy = GetComponent<Energy> ();
		enemies = GameObject.FindGameObjectWithTag ("EnemiesContainer").transform;
		chargedDamage = 0.0f;
        m_SMAttack1 = AnimControl.GetBehaviour<SM_Attack1>();
        m_SMAttack2 = AnimControl.GetBehaviour<SM_Attack2>();
        Movement l_Movement = gameObject.GetComponent<Movement>();
        m_SMAttack1.m_gc = gc;
        m_SMAttack2.m_gc = gc;
        m_SMAttack1.m_Movement = l_Movement;
        m_SMAttack2.m_Movement = l_Movement;

		//camAudioSource = Camera.main.GetComponent<AudioSource> ();

		//Debug
		DebugDrawer.AddAngle (0.0f, 0.0f, attackAngle, attackLength, Color.red, gameObject,transform.rotation);

		//		DebugDrawer.AddLine(0.0f,0.0f,attackLength,0.0f,Color.red,gameObject,new Vector3(0.0f,340.0f,0.0f));		
//		DebugDrawer.AddLine(0.0f,0.0f,0.0f,attackLength,Color.red,gameObject,new Vector3(0.0f,340.0f,0.0f));		
//		DebugDrawer.AddLine(attackLength,0.0f,0.0f,attackLength,Color.red,gameObject,new Vector3(0.0f,340.0f,0.0f));
	}

    void OnEnable()
    {
        gc = gameObject.GetComponent<GeneralPlayerController>();
        m_SMAttack1 = AnimControl.GetBehaviour<SM_Attack1>();
        m_SMAttack2 = AnimControl.GetBehaviour<SM_Attack2>();
        Movement l_Movement = gameObject.GetComponent<Movement>();
        m_SMAttack1.m_gc = gc;
        m_SMAttack2.m_gc = gc;
        m_SMAttack1.m_Movement = l_Movement;
        m_SMAttack2.m_Movement = l_Movement;
    }

	override public bool isActionActive() {
		return isAttacking () ||isChargingAttack();
	}

	override public void GetInput() {
        if (gc.player == 0 && StaticParemeters.useKeyboard)
        {
            attackPressed = Input.GetButton("Attack0");
            attackReleased = Input.GetButtonUp ("Attack0");
        }
        else
        {
            attackPressed = XCI.GetButton(XboxButton.X, gc.controller);//Input.GetButton ("Attack" + gc.player);
            attackReleased = XCI.GetButtonUp(XboxButton.X, gc.controller);//Input.GetButtonUp ("Attack" + gc.player);
        }
	} 

	// Update is called once per frame
	void Update () {
		//GetInput ();
		//attack ();
	}

	override public void ActionUpdate() {
        jump = gc.GetComponent<Jump>();
        bool jumping = false;
        JumpRest = transform.GetComponent<Movement>().attackWhileJumping;
        if (jump)
        {
            jumping = jump.isJumpingOrFalling();
        }

		timer += Time.deltaTime;
		comboTimer+= Time.deltaTime;

		ReduceAggro ();
		if(comboTimer > maxComboTimer)
		{
			currentCombo = 0;
			currentPhase = 0;
		}
		if(currentCombo>=25)
		{
			currentCombo = 0;
			++currentPhase;
			if(currentPhase>4)
				currentPhase = 4;
		}

        if (attackPressed && !attackReleased && (!jumping || JumpRest))
		{
			comboTimer = 0.0f;
            if(jumping && JumpRest)
            {
                m_FirstCharge = true;
            }

            if (m_FirstCharge)
            {
                if (energy.ConsumeEnergy(energyCost))
                {
                    AnimControl.SetTrigger("AttackTrigger1");
                    m_FirstCharge = false;
                }
            }

            if (chargeTimer > 0.1 && !jumping)
			{				
				AnimControl.SetBool ("ChargingAttack1", true);
			}

			chargeTimer+= Time.deltaTime;

            if (chargeTimer > damageChargeTimer && !jumping)
			{
				chargeTimer = 0.0f;
				AnimControl.SetBool ("ChargingAttack2", true);
				chargedDamage += stepDamageIncrease;
                if(chargedDamage > maxBonusDamage)
                {
                    chargedDamage = maxBonusDamage;
                }
			}
		}else{
		//	Debug.Log("Attack Pressed: " + attackPressed);
		//	Debug.Log("Attack Released: " + attackReleased);
			if(attackReleased)
			{
				AnimControl.SetBool ("ChargingAttack1", false);
				AnimControl.SetBool ("ChargingAttack2", false);
                m_FirstCharge = true;
				chargeTimer = 0.0f;
			}
		}
	}

	public bool isAttacking()
    {
        bool l_Active = AnimControl.GetCurrentAnimatorStateInfo(0).IsName("PreAttack1");
        l_Active = l_Active || AnimControl.GetCurrentAnimatorStateInfo(0).IsName("Attack1");
		l_Active = l_Active || AnimControl.GetCurrentAnimatorStateInfo (0).IsName ("Attack2");
        l_Active = l_Active || AnimControl.GetCurrentAnimatorStateInfo(0).IsName("End Attack2");
        l_Active = l_Active || AnimControl.GetCurrentAnimatorStateInfo(0).IsName("End Attack1");
		//Debug.Log("IsAttacking: " + l_Active);
		return l_Active;

		//return AnimControl.GetCurrentAnimatorStateInfo (0).IsName ("Attack");
	}

	public bool isChargingAttack()
	{
        bool l_Active = AnimControl.GetCurrentAnimatorStateInfo(0).IsName("PreAttack1");
        l_Active = l_Active || AnimControl.GetCurrentAnimatorStateInfo(0).IsName("Trans Charging Attack1");
        l_Active = l_Active || AnimControl.GetCurrentAnimatorStateInfo(0).IsName("Charging Attack1");
		l_Active = l_Active || AnimControl.GetCurrentAnimatorStateInfo (0).IsName ("Trans Charging Attack2");
		l_Active = l_Active || AnimControl.GetCurrentAnimatorStateInfo (0).IsName ("Charged Attack1");

		return l_Active;
	}
}
