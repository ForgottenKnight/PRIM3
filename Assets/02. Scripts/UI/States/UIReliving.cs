using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIReliving : UIState {
	public Image relivingImage;
	// Use this for initialization
	protected  override void Start () {
	
	}
	
	protected override void UpdateUI() {
		Incapacitate incapacitate = character.GetComponent<Incapacitate> ();
		GameObject RescueArea = incapacitate.RescArea;
		if (RescueArea != null) {
			RescueTrigger rt = RescueArea.GetComponent<RescueTrigger> ();
			if (rt) {
				relivingImage.fillAmount = rt.getRescueAsUnit();
				SetUIActive (true);
			} else {
				SetUIActive (false);
			}
		}
	}
}
