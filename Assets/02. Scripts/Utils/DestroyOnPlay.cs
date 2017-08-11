using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnPlay : MonoBehaviour {
	void Start () {
		if (Application.isPlaying)
			Destroy (gameObject);
	}
}
