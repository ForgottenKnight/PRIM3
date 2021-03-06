﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIState : MonoBehaviour {
	protected GameObject character;

	public bool active;
	protected GameObject render;


	public void SetCharacter(GameObject character) {
		this.character = character;
	}

	// Use this for initialization
	protected  virtual void Awake () {
		render = transform.Find ("Render").gameObject;
		SetUIActive (false);
	}

	// Use this for initialization
	protected  virtual void Start () {
	
	}
	
	// Update is called once per frame
	private void Update () {
		if (character) {
			UpdateUI ();
		}
	}

	protected  virtual void UpdateUI () {
	}



	protected void SetUIActive(bool active) {
		this.active = active;
		render.SetActive (active);  
	}

	protected bool IsUIActive() {
		return active;
	}
}
