using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointHandler : MonoBehaviour {
    public bool useCheckpoint = true;

	// Use this for initialization
	void Start () {
		if (ReestartCheckpoint.haveCheckpoint && useCheckpoint)
        {
			transform.position = ReestartCheckpoint.checkpoint;
            if (ReestartCheckpoint.eventName != "")
            {
                SimpleEvent.CallEvent(ReestartCheckpoint.eventName);
            }
        }
        else
        {
            useCheckpoint = true;
        }
	}
}
