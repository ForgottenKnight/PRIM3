using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GeneralController))]
public class MovementManager : MonoBehaviour {
	//[HideInInspector]
	public bool noInput = false;
	//[HideInInspector]
	public bool noUpdate = false;

	protected CharacterController cc;
	[HideInInspector]
	public Vector3 movement;
	
	public List<ActionBehaviour> allBehaviours = new List<ActionBehaviour> ();
	public List<ActionBehaviour> blockedBehaviours = new List<ActionBehaviour>();
	public List<ActionBehaviour> executedBehaviours = new List<ActionBehaviour>();

	// Use this for initialization
	protected virtual void Start () {
		cc = GetComponent<CharacterController> ();
		ActionBehaviour[] mb = GetComponents<ActionBehaviour> ();
		for (int i = 0; i < mb.Length; i++) {
			allBehaviours.Add(mb[i]);
		}
		allBehaviours.Sort((priority1,priority2)=>priority1.getPriority().CompareTo(priority2.getPriority()));
	}

	public void syncronize() {
		//Debug.Log ("INSIDE");
		ActionBehaviour[] mb = GetComponents<ActionBehaviour> ();
		allBehaviours.Clear ();
		for (int i = 0; i < mb.Length; i++) {
			allBehaviours.Add(mb[i]);
		}
		allBehaviours.Sort((priority1,priority2)=>priority1.getPriority().CompareTo(priority2.getPriority()));
	}
	
	// Update is called once per frame
	protected virtual void  Update () {
		executedBehaviours.Clear ();
		blockedBehaviours.Clear ();
		for (int i = 0; i < allBehaviours.Count; i++) { //Recorremos las ActionBehaviours
			if (!blockedBehaviours.Contains(allBehaviours[i])){ //Sino esta bloqueada ya por otra
				if (allBehaviours[i].isActionActive()) { //y la accion esta activa bloquea las de su lista de bloqueos.
					for (int n = 0; n < allBehaviours[i].blockActions.Count; n++) {
						//Debug.Log ("BLOCKED: " + allBehaviours[i].blockActions [n].GetType());
						blockedBehaviours.Add (allBehaviours [i].blockActions [n]);
					}
				}
				executedBehaviours.Add(allBehaviours [i]); //Esta lista se usa para no repetir el proceso anterior en el siguiente for
			} else {
				//Debug.Log ("Bloqueado: " + allBehaviours[i].GetType());
			}
		}

		DoManagement ();
	}

	protected virtual void DoManagement() {
	}
}
