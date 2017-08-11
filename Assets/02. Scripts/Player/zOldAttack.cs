using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GeneralPlayerController))]
[AddComponentMenu("PRIM3/CharacterScripts/Actions/Player Attack")] 
public class zOldAttack : ActionBehaviour {	
	GeneralPlayerController gc;
	TargetPointer tp;

	// Layers a las que afecta
	public LayerMask affectingLayers;

	//Attack parameters
	public float attackLength = 1.0f;
	public float attackWidth = 1.0f;
	public float attackHeight = 1.0f;
	public float attackForce = 10.0f;	
	public float attackSpeed = 1.0f;
	public float attackSpeed2 = 1.0f;
	public float attackSpeed3 = 1.0f;	

	//Combo
	public int comboPhases = 4;
	public int upgradePhase = 25;
	public float damageBonus = 25.0f;
	public float maxComboTimer = 10.0f;
	public float phaseChargeTimer = 2.5f;
	[HideInInspector]
	public float chargeTimer = 0.0f;
	[HideInInspector]
	public int currentCombo = 0;
	[HideInInspector]
	public float comboTimer = 0.0f;
	[HideInInspector]
	public int currentPhase = 0;
	public bool chargeInterruptable = true;


	//Attack Animation
	public Animator AnimControl;
	public bool attacking = false;
	//protected TargetPointer tp;
	protected int maxChainAttack = 3;
	protected int currentChainAttack = 0;
	public float maxChainTime = 5.0f;
	public float timer = 0.0f;

	//protected string AttackChain = "Attack";
	//protected string previousAttackChain = "Attack";	
	//protected string previousAttackChain2 = "Attack3";
	
	protected int AttackChain = 1;
	protected int previousAttackChain = 2;	
	protected int previousAttackChain2 = 3;

	//Input
	protected bool attackPressed = false;
	protected bool attackReleased = false;

	//
	protected int id1;
	protected int id2;
	protected int id3;

	protected bool JumpRest;
	protected Jump jump;

	public float aggro = 0.0f;
	public float aggroReductionPerSecond = 5.0f;

	void ReduceAggro() {
		if (aggro > 0.0f) {
			aggro = Mathf.Clamp(aggro - aggroReductionPerSecond * Time.deltaTime, 0.0f, 1000.0f);
		}
	}


	// Use this for initialization
	void Start () {
		gc = gameObject.GetComponent<GeneralPlayerController>();
		tp = gameObject.GetComponent<TargetPointer> ();
		AnimControl = GetComponent<Animator> ();
		AttackChain = id1 = Animator.StringToHash("Attack");
		previousAttackChain = id2 = Animator.StringToHash("Attack2");
		previousAttackChain2 = id3 = Animator.StringToHash("Attack3");

	}

	override public bool isActionActive() {
		return isAttacking ();
	}

	override public void GetInput() {		
		attackPressed = Input.GetButton ("Attack" + gc.player);
		attackReleased = Input.GetButtonUp ("Attack" + gc.player);
	} 

	// Update is called once per frame
	void Update () {
		//GetInput ();
		//attack ();
	}

	override public void ActionUpdate() {
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
		if(attackPressed && !attackReleased)
		{
			comboTimer = 0.0f;
			chargeTimer+= Time.deltaTime;
			if(chargeTimer > phaseChargeTimer)
			{
				chargeTimer = 0.0f;
				currentCombo = 0;
				++currentPhase;
				if(currentPhase>4)
					currentPhase = 4;
			}
		}else{
			chargeTimer = 0.0f;
		}
		JumpRest = transform.GetComponent<Movement>().attackWhileJumping;
		attack ();
	}

	/* El personaje realiza un ataque cuerpo a cuerpo si el estado de Atacar es verdadero */
	void attack(){	
		jump = gc.GetComponent<Jump>();
		bool jumping = false;
		if(jump)
			jumping = jump.isJumpingOrFalling();
		if (attackReleased && !AnimControl.GetBool(previousAttackChain) && !AnimControl.GetBool(previousAttackChain2) && (!jumping || JumpRest )) {	
			if(timer < maxChainTime)
			{
				++currentChainAttack;
				switch (currentChainAttack)
				{
				case 1:
					AttackChain = id1;
					AnimControl.speed = attackSpeed;
				//	AnimControl.SetFloat ("AttackSpeed", attackSpeed);
					previousAttackChain = id1;
					previousAttackChain2 = id3;
					break;
				case 2:
					AttackChain = id2;
					previousAttackChain = id1;
					previousAttackChain2 = id2;					
					AnimControl.speed = attackSpeed2;
					//AnimControl.SetFloat ("AttackSpeed2", attackSpeed2);
					break;
				case 3:
					AttackChain = id3;
					previousAttackChain = id1;
					previousAttackChain2 = id3;			
					AnimControl.speed = attackSpeed3;
					//AnimControl.SetFloat ("AttackSpeed3", attackSpeed3);
					break;
				default:					
					currentChainAttack = 1;
					AttackChain = id1;
					previousAttackChain = id1;
					previousAttackChain2 = id3;	
					AnimControl.speed = attackSpeed;
					//AnimControl.SetFloat ("AttackSpeed", attackSpeed);
					break;
				}
				if(tp != null) {
					tp.lookAtTargetRequest ();
				}
			}else{
				currentChainAttack = 1;
				AttackChain = id1;
				previousAttackChain = id2;					
				previousAttackChain2 = id3;	
				AnimControl.speed = attackSpeed;
				//AnimControl.SetFloat ("AttackSpeed", attackSpeed);
			}
			timer = 0.0f;
			//	tp.lookAtTarget ();
			AnimControl.SetBool (AttackChain, true);
			attacking = true;
			GameObject AttackRange = GameObject.CreatePrimitive(PrimitiveType.Cube);
			AttackRange.transform.localScale = new Vector3(attackWidth, attackHeight, attackLength);			
			AttackRange.transform.position = transform.position + transform.forward * attackLength/2.0f;
			Vector3 pos = new Vector3(AttackRange.transform.position.x, (transform.position.y - transform.localScale.y) + attackHeight/2.0f, AttackRange.transform.position.z);
			//float lY = transform.position.y;
			//AttackRange.transform.position.y = lY;
			AttackRange.transform.localPosition = pos;
			AttackRange.transform.rotation = transform.rotation;
			AttackRange.GetComponent<BoxCollider>().isTrigger = true;
			AttackTrigger at = AttackRange.AddComponent<AttackTrigger>();
			at.affectingLayers = affectingLayers;
			at.damageDone = attackForce;
			at.AnimControl = AnimControl;			
			at.attackid = AttackChain;
			at.comboDamage = attackForce*((float)currentPhase*damageBonus/100.0f);
			at.attck = this;
			AttackRange.transform.SetParent(transform);
			AttackRange.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	/* El personaje cesa su ataque */
	void endAttackAnim(string attackEnd){		
		AnimControl.SetBool (attackEnd, false);
		AnimControl.speed = 1.0f;
		attacking = false;
	}

	public bool isAttacking()
	{
		return AnimControl.GetBool (AttackChain);
	}

	




}
