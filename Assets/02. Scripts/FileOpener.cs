using UnityEngine;
using System.Collections;

public class FileOpener : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OpenFile(string fileName) {
		Application.OpenURL ((Application.dataPath) + "/" + fileName);
	}
}
