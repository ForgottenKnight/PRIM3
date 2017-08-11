using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionBehaviour : MonoBehaviour {

	[Header("Configuracion de prioridad")]
	//Sistema de prioridades
	public int priority;
	public List<ActionBehaviour> blockActions;
	
	public int getPriority() {
		return priority;
	}

	public void setPriority(int priority) {
		this.priority = priority;
	}

	public virtual bool isActionActive() {
		return false;
	}

	public virtual void ActionUpdate() {
	}

	public virtual void GetInput() {
	}

	public void blockAction(ActionBehaviour action) {
		blockActions.Add (action);
		GetComponent<PlayerManager> ().syncronize ();
	}

	public void deleteAction(ActionBehaviour action) {
		blockActions.Remove (action);
		GetComponent<PlayerManager> ().syncronize ();
	}

	/*private void syncronize() {
		GetComponent<MovementManager> ().syncronize ();
	}*/
}
