using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReestartCheckpoint : MonoBehaviour {
	public static Vector3 checkpoint = Vector3.zero;
	public static bool haveCheckpoint = false;
    public static string eventName = "";
    // Use this for initialization
    void Start () {
    }

    public void ChangeCheckpoint(Transform aCheckpoint)
    {
        ReestartCheckpoint.checkpoint = aCheckpoint.position;
		haveCheckpoint = true;
        SimpleEvent se = aCheckpoint.GetComponent<SimpleEvent>();
        eventName = "";
        if (se != null)
        {
            eventName = se.eventName;
        }
    }
}