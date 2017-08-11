using UnityEngine;
using System.Collections;

public class Level1Rune : MonoBehaviour {

	public GameObject Rock;

	public float runeActivationTime = 4.0f;
	protected  float currentTimer = 0.0f;
	protected bool activated = false;
	protected Color initColor = new Color(0.03f,0.25f,0.0f);
	protected Color FinalColor = new Color(0.15f,1.0f,0.0f);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider col) 
	{
		if (col.tag == "Player") {
			if (currentTimer >= runeActivationTime) {
				activated = true;			
				Rock.gameObject.GetComponent<RockSpawn> ().activated = true;
			} else {
				currentTimer += Time.deltaTime;
				Color c = Color.Lerp (initColor, FinalColor, currentTimer / runeActivationTime);
				GetComponent<Renderer> ().material.SetColor ("_EmissionColor", c);
			}
		}

		//Vector3 pos = Rock.transform.position;
	}

	void OnTriggerExit(Collider col) 
	{
		if(!activated)
		{
			currentTimer = 0.0f;
			GetComponent<Renderer> ().material.SetColor("_EmissionColor",initColor);
		}
	}



}
