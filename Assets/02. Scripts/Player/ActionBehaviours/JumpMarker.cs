using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MovementManager))]
public class JumpMarker : ActionBehaviour {

	public float speed = 999.99f;
	CharacterController marker;
	// Use this for initialization
	void Start () {	
		marker = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	override public void GetInput() {
		
	}
	
	override public void ActionUpdate() {
		Vector3 obj = transform.parent.transform.position;
		marker.transform.position = transform.parent.transform.position;
		obj = transform.InverseTransformPoint(obj);
		marker.Move (transform.up*(-1000.0f));
//		Debug.Log("Pos: X: " + obj.x + " Y: "+ obj.y + "Z: " + obj.z);

	}

	override public bool isActionActive() {
			return true;
		}

}
