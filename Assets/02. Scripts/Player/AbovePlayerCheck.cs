using UnityEngine;
using System.Collections;

public class AbovePlayerCheck : MonoBehaviour {
	[HideInInspector]
	public bool abovePlayer;

	// Use this for initialization
	void Start () {
		abovePlayer = false;
	}
	
	// Update is called once per frame
	void Update () {
		/*aboveBehaviour.active = abovePlayer;
		if (abovePlayer == true) {
			Debug.Log ("ABOVE!");
		}
		abovePlayer = false;*/
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player" || other.tag == "Enemy") {
			Jump j = transform.parent.GetComponent<Jump>();
			if (j) {
				if (j.isFalling()) {
					abovePlayer = true;
				}
			} else {
				abovePlayer = true;
			}
		}
	}

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Enemy")
        {
            Jump j = transform.parent.GetComponent<Jump>();
            if (j)
            {
                if (j.isFalling())
                {
                    abovePlayer = true;
                }
            }
            else
            {
                abovePlayer = true;
            }
        }
    }

	void OnTriggerExit(Collider other) {
        if (other.tag == "Player" || other.tag == "Enemy")
        {
			abovePlayer = false;
		}
	}
}
