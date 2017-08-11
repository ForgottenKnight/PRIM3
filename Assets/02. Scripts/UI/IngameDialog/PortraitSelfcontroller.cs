using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitSelfcontroller : MonoBehaviour {
	public GameObject dialogCanvas;
	public GameObject dialogPanel;

	public Vector3 offset;


	// Use this for initialization
	void Start () {
	}

	public void ShowDialog(float time) {
		StopAllCoroutines ();
		dialogPanel.transform.position = transform.position + offset;
		StartCoroutine (PortraitDialog (time));
	}

	IEnumerator PortraitDialog(float time) {
		yield return new WaitForSeconds (time);
		dialogCanvas.SetActive (false);
		gameObject.SetActive (false);
		yield return null;
	}


}
