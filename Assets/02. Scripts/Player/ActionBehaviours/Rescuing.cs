using UnityEngine;
using System.Collections;


public class Rescuing : ActionBehaviour {
	//[HideInInspector]
	public RescueTrigger rt;

	bool rescuing;

	
	// Use this for initialization
	void Start () {
		rescuing = false;
		rt = null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	/* Obtiene los estados o valores del Input y los asigna a las diferentes acciones */
	override public void GetInput() {
		
	}
	
	override public void ActionUpdate() {
		if (isActionActive () && rt != null) {			
			rt.SetRescue(true);
		}else{
			if(rt == null)
				rescuing = false;
		}
	}
	
	public void SetRescue(bool resc, RescueTrigger rescTrig) {
		rescuing = resc;
		if(rt != null)		
			rt.SetRescue(resc);
		rt = rescTrig;
		if(rt != null)
			rt.SetRescue(resc);

	}

	override public bool isActionActive() {
		return rescuing;
	}
}
