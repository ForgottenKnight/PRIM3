using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformAnimatorTrigger : MonoBehaviour {
	private PlatformAnimator m_platformAnimator;
	[HideInInspector]
	public ReestartCheckpoint checkpoint;
	[HideInInspector]
	public bool useAsCheckpoint;
	[HideInInspector]
	public List<GameObject> players = new List<GameObject>();

	// Use this for initialization
	void Start () {
		m_platformAnimator = gameObject.GetComponentInParent<PlatformAnimator> ();
	}
	
	// Update is called once per frame
	void Update () {
		CheckPlayersActive ();
	}

	void OnTriggerEnter (Collider collider) {
		if (!players.Contains (collider.gameObject)) {
			players.Add(collider.gameObject);
			m_platformAnimator.OnPlayerEnter ();
		}
	}

	void OnTriggerExit (Collider collider) {
		players.Remove(collider.gameObject);
		m_platformAnimator.OnPlayerExit ();
	}


	private void CheckPlayersActive () {
		for(int i = players.Count - 1; i > -1; i--){
            if (players[i].gameObject) {
                if (!players[i].gameObject.activeSelf) {
                    players.RemoveAt(i);
                    m_platformAnimator.OnPlayerExit();
                }
            }
		}
	}


}
