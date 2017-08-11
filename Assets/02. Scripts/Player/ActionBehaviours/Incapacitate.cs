using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Incapacitate : ActionBehaviour {
	bool incapacitated = false;
	//bool Rotating = false;
	//bool Reviving = false;

    public GameObject slideChecker;
	public Animator AnimControler;
	public PlayerManager PM;

	//public float initAngle = 0.0f;
	//public float finalAngle = -90.0f;

	public GameObject RescArea;
    public float reviveTimer = 2.0f;
	public float rescueLength = 3.0f;
	public float rescueWidth = 3.0f;
	public float rescueHeight = 3.0f;

	public float fallspeed = 1.0f;

	[Range(0,1)]
	public float regainedHealth = 0.25f; //valor entre 0 y 1
	
	public Slider rescueBar;

	private bool moving = true;
	private Vector3 lastPos = default(Vector3);
	private bool charEnabled = true;
	private CharacterController charController;
	// Use this for initialization
	void Awake () {
		rescueBar.gameObject.SetActive(false);
		charController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		if(incapacitated && moving)
		{
            gameObject.layer = LayerMask.NameToLayer("Incapacitate");
            charController.Move(-Vector3.up * 9.81f * Time.deltaTime);
			/*if(lastPos != transform.position)
			{
				PM.movement = Vector3.down * fallspeed * Time.deltaTime;
				lastPos = transform.position;
			}else{
				charController.enabled = false;
				charEnabled = false;
			}*/
		}

		if(!AnimControler.GetBool("Incap") && !AnimControler.GetBool("Reviving") && !charEnabled)
		{
			charEnabled = true;
			//charController.enabled = true;
		}
	
	}

	public void Incap()
	{
		//Rotating = true;
		Health h = transform.GetComponent<Health>();
		if (h)
			h.invincible = true;
		incapacitated = true;
        slideChecker.SetActive(false);
		AnimControler.SetBool("Incap",incapacitated);
		RescueArea();
	}

	public void Revive()
	{
		Health h = transform.GetComponent<Health>();
		if (h && incapacitated)
		{
			h.invincible = false;
            h.FillPercent(regainedHealth * 100f);
			//h.Heal(h.maxHealth * regainedHealth);
		}
		incapacitated = false;
        slideChecker.SetActive(true);
		//Reviving = true;
		AnimControler.SetBool("Incap",incapacitated);
		AnimControler.SetBool("Reviving",true);
		rescueBar.gameObject.SetActive(false);

	}

	private void RescueArea()
	{
		RescArea = GameObject.CreatePrimitive(PrimitiveType.Cube);
		RescArea.transform.localScale = new Vector3(rescueWidth, rescueHeight, rescueLength);			
		RescArea.transform.position = transform.position;
		RescArea.GetComponent<BoxCollider>().isTrigger = true;
		RescueTrigger rt = RescArea.AddComponent<RescueTrigger>();
		rt.rescueBar = rescueBar;
		rt.currentPlayer = gameObject.name;
        rt.rescueActivationTime = reviveTimer;
        RescArea.layer = LayerMask.NameToLayer("Ignore Raycast");
		RescArea.transform.SetParent(transform);
		RescArea.GetComponent<MeshRenderer>().enabled = false;
	}

	void endReviving(){	
		//Reviving = false;
		//charController.enabled = true;
        gameObject.layer = LayerMask.NameToLayer("Player");

    }


	override public bool isActionActive() {
		return (AnimControler.GetBool("Incap") || AnimControler.GetBool("Reviving"));
	}
}
