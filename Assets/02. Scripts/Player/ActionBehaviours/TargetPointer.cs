using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XboxCtrlrInput;

public class TargetPointer : ActionBehaviour {
	[Header("Información sobre el Target")]
	public GameObject target;
    public List<VisitedTarget> targetPool = new List<VisitedTarget>();


	public class VisitedTarget {
		public GameObject target = null;
		public bool visited = false;
	}

	public enum TargetMode {
		Continous,
		OnTriggerClick,
		OnTriggerHold,
		OnAttack
	};

	public enum TargetAlgorism {
		TwoDimensionProximity,
		ThreeDimensionProximity,
		AngleProximity,
		AxisProximity
	};

	public enum TargetSelector {
		IterateTargetList,
		BestTargetFromList
	};

	public enum Pivot {
		Target,
		TargetPointer
	};


	public enum Axis{
		X,
		Y,
		Z
	}
	public enum IndicatorTransform {
		LocalPosition,
		WorldPosition,
		CameraPosition
	}

	[Header("Configuración")]
	public TargetMode targetMode = TargetMode.OnAttack;
	public TargetAlgorism targetAlgorism = TargetAlgorism.ThreeDimensionProximity;
	public TargetSelector targetList = TargetSelector.IterateTargetList;
	public AudioSource audioSource;
	public AudioClip targetSwitch;
	public bool active = false;

	[Header("Avanzados")]
	public float AxisProximityMinAngle = 45;
	public Pivot AxisProximityPivot = Pivot.TargetPointer;

	[Header("Indicador de Target")]
	private PrimeCamera primeCamera;
	public IndicatorTransform indicatorTransform = IndicatorTransform.WorldPosition;
	public GameObject targetLockIndicator;
	[Range(0.0f, 2.0f)]
	public float scale = 1;
	[Range(-50.0f, 50.0f)]
	public float offsetX = 0;
	[Range(-50.0f, 50.0f)]
	public float offsetY = 0;
	[Range(-50.0f, 50.0f)]
	public float offsetZ = 0;
	[Range(0.0f, 360.0f)]
	public float angleX = 0;
	[Range(0.0f, 360.0f)]
	public float angleY = 0;
	[Range(0.0f, 360.0f)]
	public float angleZ = 0;
	
	[Header("Input")]
	private float verticalTargetAmount;
	private float horizontalTargetAmount;
	//private bool lockTargetPressed;
	private bool lockTargetReleased;
	private float lockTargetTriggerLeft;
	private float lockTargetTriggerRight;
	private bool canTargetNext = true;
	private float minAmountToChangeTarget = 0.5f;
	private float maxAmountToAllowChange = 0.1f;
	private bool triggerActive = false;
	private bool alreadyIn = false;

	// Componentes obligatorios
	private GeneralPlayerController gc;
	
	// Use this for initialization
	void Start () {
		TargetController.Instance.addTargetPointer (this.gameObject);
		audioSource = Camera.main.GetComponent<AudioSource> ();
		gc = GetComponent<GeneralPlayerController> ();

		string charName = "TargetLocker";
		switch(gc.character) {
		case 0:
			charName += "Salt";
			break;
		case 1:
			charName += "Sulphur";
			break;
		case 2:
			charName += "Mercury";
			break;
		}
		primeCamera = Camera.main.GetComponent<PrimeCamera> ();
		targetLockIndicator = GameObject.Find (charName);
		targetLockIndicator.transform.GetComponent<MeshRenderer>().enabled = false;

		Health h = GetComponent<Health> ();
		if (h != null) { // Ahora cuando muere el jugador el marcador de objetivo se quita.
			h.RegisterOnDie (unlockTarget);
		}
	}
	
	// Update is called once per frame
	override public void ActionUpdate() {
		manageStatus ();
		if(active) {
			manageTarget ();
			manageLookAtTarget ();
			manageTargetIndicator();
			manageAnimator();
		}
	}

	override public void GetInput() {
		if (gc) 
		{
            if (gc.player == 0 && StaticParemeters.useKeyboard)
            {
                lockTargetReleased = Input.GetButtonUp ("LockTarget0");
                horizontalTargetAmount = Input.GetAxis ("HorizontalTarget0");
                verticalTargetAmount = Input.GetAxis ("VerticalTarget0");
            }
            else
            {
                //lockTargetPressed = Input.GetButtonDown ("LockTarget" + gc.player);
                lockTargetReleased = XCI.GetButtonDown(XboxButton.RightStick, gc.controller);//Input.GetButtonUp ("LockTarget" + gc.player);
                horizontalTargetAmount = XCI.GetAxis(XboxAxis.RightStickX, gc.controller);//Input.GetAxis ("HorizontalTarget" + gc.player);
                verticalTargetAmount = XCI.GetAxis(XboxAxis.RightStickY, gc.controller);//Input.GetAxis ("VerticalTarget" + gc.player);
                //lockTargetTriggerLeft = //Input.GetAxis ("LockTargetTriggerLeft" + gc.player);
                //lockTargetTriggerRight = //Input.GetAxis ("LockTargetTriggerRight" + gc.player);
            }
		}
	}

	private void manageTarget() {
		if ((Mathf.Abs (horizontalTargetAmount) >= minAmountToChangeTarget || Mathf.Abs (verticalTargetAmount) >= minAmountToChangeTarget) && canTargetNext) {
			canTargetNext = false;
			nextTarget (horizontalTargetAmount, verticalTargetAmount);
			
		} else if (Mathf.Abs (horizontalTargetAmount) <= maxAmountToAllowChange && Mathf.Abs (verticalTargetAmount) <= maxAmountToAllowChange) {
			canTargetNext = true;
		}
	}

	private void manageStatus() {
		if (lockTargetReleased) {
			if (active) {
				setActive (false);
				unlockTarget();
			} else {
				setActive (true);
				nextTarget ();
			}
		}
	}

	public void setActive (bool active) {
		this.active = active;
	}

	public void addToVisitedPool(GameObject target) {
		bool exists = false;
		foreach (VisitedTarget vt in targetPool) {
			if (vt.target == target) {
				exists = true;
			}
		}
		if (!exists) {
			VisitedTarget vt = new VisitedTarget ();
			vt.target = target;
			vt.visited = false;
			targetPool.Add (vt);
		} 
	}
	public void removeFromVisitPool(GameObject target) {
		if (target != null) {
			targetPool.RemoveAll (item => item.target == target);
		}
		nextTarget ();
	} 

	public void setTargetVisited(GameObject target, bool visited) {
		foreach (VisitedTarget vt in targetPool) {
			if (vt.target == target) {
				vt.visited = visited;
			}
		}
	}

	public void setAllTargetsVisited(bool visited) {
		foreach (VisitedTarget vt in targetPool) {
			vt.visited = visited;
		}
	}

	public bool isTargetVisited(GameObject target) {
		foreach (VisitedTarget vt in targetPool) {
			if (vt.target == target) {
				if (vt.visited) {
					return true;
				}
			}
		}
		return false;
	}

	public int countTargetPoolElements() {
		return targetPool.Count ();
	}

	public void nextTarget(float horizontal = 0.0f, float vertical = 0.0f) {
		switch (targetAlgorism) {
		case TargetAlgorism.TwoDimensionProximity:
			switch (targetList) {
			case TargetSelector.BestTargetFromList:
				unlockTarget ();
				target = TargetController.Instance.getClosest2DPTarget (this.gameObject);
				lockTarget ();
				break;
			case TargetSelector.IterateTargetList:
				unlockTarget ();
				target = TargetController.Instance.getNext2DPTarget (this.gameObject);
				lockTarget ();
				break;
			}
			break;
		case TargetAlgorism.ThreeDimensionProximity:
			switch (targetList) {
			case TargetSelector.BestTargetFromList:
				unlockTarget ();
				target = TargetController.Instance.getClosest3DPTarget (this.gameObject);
				lockTarget ();
				break;
			case TargetSelector.IterateTargetList:
				unlockTarget ();
				target = TargetController.Instance.getNext3DPTarget (this.gameObject);
				lockTarget ();
				break;
			}
			break;
		case TargetAlgorism.AxisProximity:
			switch (targetList) {
			case TargetSelector.BestTargetFromList:
				unlockTarget ();
				target = TargetController.Instance.getClosestAPTarget (this.gameObject, horizontal, vertical);
				lockTarget ();
				break;
			case TargetSelector.IterateTargetList:
				unlockTarget ();
				target = TargetController.Instance.getNextAPTarget (this.gameObject, horizontal, vertical);
				lockTarget ();
				break;
			}
			break;
		case TargetAlgorism.AngleProximity:
			switch (targetList) {
			case TargetSelector.BestTargetFromList:
				unlockTarget ();
				target = TargetController.Instance.getClosestAngleTarget (this.gameObject);
				lockTarget ();
				break;
			case TargetSelector.IterateTargetList:
				unlockTarget ();
				target = TargetController.Instance.getNextAngleTarget (this.gameObject);
				lockTarget ();
				break;
			}
			break;
		}
	}

	public void unlockTarget() {
		if (targetLockIndicator != null) {
			targetLockIndicator.transform.GetComponent<MeshRenderer>().enabled = false;
			targetLockIndicator.transform.SetParent (TargetController.Instance.transform);
		}
	}
	private void lockTarget() {
		if (target != null) {
		
			targetLockIndicator.transform.SetParent (target.transform);
			targetLockIndicator.transform.position = target.transform.position;

			if(active) {
				targetLockIndicator.transform.GetComponent<MeshRenderer>().enabled = true;
				audioSource.PlayOneShot (targetSwitch);
			} else {
				targetLockIndicator.transform.GetComponent<MeshRenderer>().enabled = false;
			}
		} else {
			setActive(false);
		}
	}

	private void manageLookAtTarget() {
		switch (targetMode) {
		case TargetMode.OnAttack:
			break;
		case TargetMode.OnTriggerClick:
			if((lockTargetTriggerLeft == 1 || lockTargetTriggerRight == 1)) {
				if(!alreadyIn){
					alreadyIn = true;
					if(!triggerActive) {
						triggerActive = true;
					} else {
						triggerActive = false;
					}
				}
			} else {
				alreadyIn = false;
			}
			if(triggerActive) {
				lookAtTarget ();
			}
			break;
		case TargetMode.OnTriggerHold:
			if((lockTargetTriggerLeft == 1 || lockTargetTriggerRight == 1)) {
				if(!triggerActive){
					triggerActive = true;
				}
				lookAtTarget ();
			} else {
				triggerActive = false;
			}
			break;
		case TargetMode.Continous:
			lookAtTarget ();
			break;
		}
	}

	private void lookAtTarget() {
		if (target != null) {
			transform.LookAt (target.transform);
			transform.rotation =  Quaternion.Euler (0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z );
		}
	}

	public void lookAtTargetRequest() {
		if (targetMode == TargetMode.OnAttack && active == true)
			lookAtTarget();
	}

	private void manageTargetIndicator() {
		Target targetScript = target.GetComponent<Target>();
		switch(indicatorTransform) {
		case IndicatorTransform.LocalPosition:
			targetLockIndicator.transform.localScale = new  Vector3 (targetScript.scale * scale, targetScript.scale * scale, targetScript.scale * scale);
			targetLockIndicator.transform.localEulerAngles  = new Vector3(targetScript.angleX + angleX, targetScript.angleY + angleY,targetScript.angleZ + angleZ);
			targetLockIndicator.transform.localPosition = Vector3.zero +  
				Vector3.up * (targetScript.offsetY + offsetY) + 
				Vector3.right * (targetScript.offsetX + offsetX) + 
				Vector3.forward * (targetScript.offsetZ + offsetZ) ;
			break;
		case IndicatorTransform.WorldPosition:
			targetLockIndicator.transform.localScale = new  Vector3 (targetScript.scale * scale, targetScript.scale * scale, targetScript.scale * scale);
			targetLockIndicator.transform.eulerAngles  = new Vector3(targetScript.angleX + angleX, targetScript.angleY + angleY,targetScript.angleZ + angleZ);
			targetLockIndicator.transform.position = target.transform.position +  
				Vector3.up * (targetScript.offsetY + offsetY) + 
					Vector3.right * (targetScript.offsetX + offsetX) + 
					Vector3.forward * (targetScript.offsetZ + offsetZ) ;
			break;
		case IndicatorTransform.CameraPosition:
			targetLockIndicator.transform.localScale = new  Vector3 (targetScript.scale * scale, targetScript.scale * scale, targetScript.scale * scale);
			//targetLockIndicator.transform.eulerAngles  = new Vector3(primeCamera.transform.eulerAngles.x + targetScript.angleX + angleX, primeCamera.transform.eulerAngles.y + targetScript.angleY + angleY, primeCamera.transform.eulerAngles.z + targetScript.angleZ + angleZ);
			targetLockIndicator.transform.LookAt(primeCamera.transform);
			var up = targetLockIndicator.transform.TransformDirection(Vector3.up);
			var right = targetLockIndicator.transform.TransformDirection(Vector3.right);
			var forward = targetLockIndicator.transform.TransformDirection(Vector3.forward);
			targetLockIndicator.transform.position = target.transform.position  +  
				up * (targetScript.offsetY + offsetY) + 
				right *	(targetScript.offsetX + offsetX) + 
				forward *(targetScript.offsetZ + offsetZ);
			break;
		}
	}

	private void manageAnimator() {
		Target targetScript = target.GetComponent<Target>();
		if (targetScript.movementAnimator) {
			float dif = targetScript.movementPositiveX + targetScript.movementNegativeX;
			if (dif > 0) {
				float movement = Mathf.PingPong (Time.time, targetScript.movementPositiveX + targetScript.movementNegativeX) - targetScript.movementNegativeX;
				targetLockIndicator.transform.localPosition += Vector3.right * movement * targetScript.movementSpeed;
			} 
			dif = targetScript.movementPositiveY + targetScript.movementNegativeY;
			if (dif > 0) {
				float movement = Mathf.PingPong (Time.time, targetScript.movementPositiveY + targetScript.movementNegativeY) - targetScript.movementNegativeY;
				targetLockIndicator.transform.localPosition += Vector3.up * movement * targetScript.movementSpeed;
			} 
			dif = targetScript.movementPositiveZ + targetScript.movementNegativeZ;
			if (dif > 0) {
				float movement = Mathf.PingPong (Time.time, targetScript.movementPositiveZ + targetScript.movementNegativeZ) - targetScript.movementNegativeZ;
				targetLockIndicator.transform.localPosition += Vector3.forward * movement * targetScript.movementSpeed;
			}
		}

		if (targetScript.rotationAnimator) {
			targetScript.rotationTimer += Time.deltaTime * targetScript.rotationSpeed;
			if(targetScript.rotationTimer > 1.0f){
				targetScript.rotationTimer = 0.0f;
			}
			float angle = Mathf.Lerp (0.0f, 360.0f, targetScript.rotationTimer);
			switch(targetScript.rotationAxis) {
			case Target.Axis.X:
				//targetLockIndicator.transform.localEulerAngles += new Vector3(angle, 0, 0);
				targetLockIndicator.transform.eulerAngles += new Vector3(angle, 0, 0);
				break;
			case Target.Axis.Y:
				//targetLockIndicator.transform.localEulerAngles += new Vector3(0, angle, 0);
				targetLockIndicator.transform.eulerAngles += new Vector3(0, angle, 0);
				break;
			case Target.Axis.Z:
				//targetLockIndicator.transform.localEulerAngles += new Vector3(0, 0, angle);
				targetLockIndicator.transform.eulerAngles += new Vector3(0, 0, angle);
				break;
			}
		}

		if (targetScript.scaleAnimator) {
			float scale = Mathf.PingPong (Time.time * targetScript.scaleSpeed, targetScript.scaleMax  - targetScript.scaleMin) + targetScript.scaleMin;
			targetLockIndicator.transform.localScale *= scale;

		}

	}

	void OnDestroy() {
		unlockTarget ();
		TargetController.Instance.removeTargetPointer (this.gameObject);
	}
}
