using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pause : MonoBehaviour {

	private Text pauseText;
	public KeyCode pauseButton;

	// Use this for initialization
	public void Start () {
		pauseText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	public void Update () {
		ManagePauseControls ();
	}

	private void ManagePauseControls() {
		if (Input.GetButtonDown("Pause") && !PauseGameController.Paused) {
			pauseText.enabled = true;
			PauseGameController.Pause();
		} else if (Input.GetButtonDown("Pause") && PauseGameController.Paused) {
			pauseText.enabled = false;
			PauseGameController.Resume();
		}
	}
}
