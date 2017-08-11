using UnityEngine;
using System.Collections;

public class AnimationEnd : MonoBehaviour {
	Animator anim;

	void Start() {
		anim = GetComponent<Animator> ();
	}

	public void EndAnimation(string name) {
		anim.SetBool (name, false);
	}
}
