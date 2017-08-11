using UnityEngine;
using System.Collections;
using XboxCtrlrInput;


[RequireComponent(typeof(Energy))]
public class SpecialAbility : ActionBehaviour {
	// Componentes obligatorios
	protected MovementManager mm;
	protected CharacterController cc;
	protected GeneralPlayerController gc;
	protected Animator anim;
    protected Movement movementBehaviour;
	
	protected bool specialPressed;
	protected bool specialReleased;
	protected float horizontalAmount;
	protected float verticalAmount;

	protected Jump jump;
	protected Energy energy;
	[HideInInspector]
	public TargetPointer tp;

	public float aggro = 0.0f; // Por uso / por tiempo, dependiendo si es salt, sulphur o mercury
	public float aggroReductionPerSecond = 5.0f;
	public float abilityAggro = 5.0f; // Para sulphur no afecta
	public bool negativeAggro = false; // Caso de mercury seria positivo, ya que reduce aggro al teleportarse
	
	void Start() {
		mm = GetComponent<MovementManager> ();
		cc = GetComponent<CharacterController> ();
		gc = GetComponent<GeneralPlayerController> ();
		jump = GetComponent<Jump> ();
		energy = GetComponent<Energy> ();
		tp = GetComponent<TargetPointer> ();
		anim = GetComponentInChildren<Animator> ();
        movementBehaviour = GetComponent<Movement>();
	}
	
	override public void GetInput() {
        if (gc.player == 0 && StaticParemeters.useKeyboard)
        {
            specialPressed = Input.GetButtonDown ("Special0");
            specialReleased = Input.GetButtonUp ("Special0");

            //TEST
            horizontalAmount = Input.GetAxis ("Horizontal0");
            verticalAmount = Input.GetAxis ("Vertical0");
        }
        else
        {
            specialPressed = XCI.GetButtonDown(XboxButton.Y, gc.controller);//Input.GetButtonDown ("Special" + gc.player);
            specialReleased = XCI.GetButtonUp(XboxButton.Y, gc.controller);//Input.GetButtonUp ("Special" + gc.player);

            //TEST
            horizontalAmount = XCI.GetAxis(XboxAxis.LeftStickX, gc.controller);//Input.GetAxis ("Horizontal" + gc.player);
            verticalAmount = XCI.GetAxis(XboxAxis.LeftStickY, gc.controller);//Input.GetAxis ("Vertical" + gc.player);
        }
	}

	protected void ReduceAggro() {
		if (aggro > 0.0f) {
			aggro = Mathf.Clamp(aggro - aggroReductionPerSecond * Time.deltaTime, 0.0f, 1000.0f);
		}
	}
}
