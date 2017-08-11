using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
	public enum Axis {
		X,
		Y,
		Z
	}
	
	[Header("Indicador de Target")]
	public float maxTargetDistance = 20.0f;
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
	[Range(0.0f, 20.0f)]
	public float scale = 1;

	[Header("Animador")]
	public bool movementAnimator = false;
	public float movementNegativeX = 0;
	public float movementPositiveX = 0;
	public float movementNegativeY = 0;
	public float movementPositiveY = 0;
	public float movementNegativeZ = 0;
	public float movementPositiveZ = 0;
	public float movementSpeed = 0;
	public bool rotationAnimator = false;
	public float rotationSpeed = 0;
	public Axis rotationAxis = Axis.Y;
	[HideInInspector]
	public float rotationTimer = 0.0f;
	public bool scaleAnimator = false;
	public float scaleMin = 0;
	public float scaleMax = 0;
	public float scaleSpeed = 0;

	void Awake() {
		
	}
	// Use this for initialization
	void Start () {
        if (TargetController.Instance)
        {
            TargetController.Instance.addTarget(this.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy() {
        if (TargetController.Instance)
        {
            TargetController.Instance.removeTarget(this.gameObject);
        }
	}

	public void DeactivateTarget() {
		TargetController.Instance.removeTarget (this.gameObject);
	}

	public void ActivateTarget() {
		TargetController.Instance.addTarget (this.gameObject);
	}


}
