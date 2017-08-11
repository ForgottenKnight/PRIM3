using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RescueTrigger : MonoBehaviour {

	public float rescueActivationTime = 4.0f;
	public Slider rescueBar;
	public string currentPlayer;
	public float currentTimer;

	public bool rescuing = false;

	// Use this for initialization
	void Start () {
		currentTimer = rescueActivationTime;
		if (rescueBar) {
			rescueBar.maxValue = rescueActivationTime;
			rescueBar.minValue = 0.0f;
			rescueBar.wholeNumbers = false;
			rescueBar.value = 0.0f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(currentTimer == rescueActivationTime)
		{
			rescueBar.gameObject.SetActive(false);
		}else{
			rescueBar.gameObject.SetActive(true);
			rescueBar.gameObject.transform.LookAt(Camera.main.transform);
		}

		if(rescuing)
		{
			currentTimer -= Time.deltaTime;
		}else{
			currentTimer += Time.deltaTime;
			if(currentTimer > rescueActivationTime)
				currentTimer = rescueActivationTime;
		}

		if(currentTimer <= 0.0f)
		{
			currentTimer = 0.0f;
			Incapacitate ic = transform.parent.gameObject.GetComponent<Incapacitate>();
			if(ic)
				ic.Revive();
			Destroy(gameObject,0.1f);
		}

		if (rescueBar) {
			rescueBar.value = currentTimer;
		}
	}

	public float getRescueAsPercentage() {
		return currentTimer / rescueActivationTime * 100;
	}

	public float getRescueAsUnit() {
		return currentTimer / rescueActivationTime;
	}


	void OnTriggerStay(Collider col) 
	{
		if (col.tag == "Player" && col.name != currentPlayer) {
			Incapacitate I = col.gameObject.GetComponent<Incapacitate>();
			if(I)
			{
				Rescuing r = col.gameObject.GetComponent<Rescuing>();
				if(r)
				{
					bool incap = I.isActionActive();			
					if(!incap)
					{
						if(!r.isActionActive())
							r.SetRescue(true, this);
					}else{
						r.SetRescue(false, null);
					}
				}
			}
		}
	}

	public void SetRescue(bool resc)
	{
		rescuing = resc;
	}

	void OnTriggerExit(Collider col) 
	{
		if (col.tag == "Player") {
			Rescuing r = col.gameObject.GetComponent<Rescuing>();
			if(r)
			{
				r.SetRescue(false, null);
			}
		}
	}
}
