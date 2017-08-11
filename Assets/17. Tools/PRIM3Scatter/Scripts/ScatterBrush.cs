using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScatterBrush : MonoBehaviour {
	static ScatterBrush _instance;
	private Projector m_Projector;

	public static ScatterBrush instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<ScatterBrush> ();
			}
			return _instance;
		}
	}

	// Use this for initialization
	void Start () {
		m_Projector = GetComponent<Projector> ();
		m_Projector.enabled = false;
	}

	public void DrawBrush(Vector3 aPosition, float aSize) {
		m_Projector.enabled = true;
		m_Projector.transform.position = aPosition;
		m_Projector.orthographicSize = aSize;
	}

	public void DisableBrush() {
		m_Projector.enabled = false;
	}
}
