using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemStompTrigger : MonoBehaviour {

    public float pushSpeed = 5.0f;
    public float pushTime = 1.0f;
    
    private GolemStomp m_golemStomp;
    
	public void Start() {
		m_golemStomp = transform.GetComponentInParent<GolemStomp> ();
	}


	public void OnTriggerStay(Collider col) {
		if (col.tag == "Player") {
			Pushable l_pushable = col.gameObject.GetComponent<Pushable> ();
			if (!l_pushable.beingPushed) {
				Vector3 l_right = transform.right;
				Vector3 l_toOther = col.transform.position - transform.position;
				Vector3 l_aSource;

				if (Vector3.Dot (l_right, l_toOther) > 0) {
					l_aSource = col.transform.position + transform.right;
				} else {
					l_aSource = col.transform.position - transform.right;
				}

                l_pushable.Push(pushSpeed, pushTime, l_aSource, false);
			}
			m_golemStomp.DoDamage (col.gameObject);
		}
	}

	public void OnCollisionStay(Collision collision) {
		m_golemStomp.DoDamage (collision.gameObject);
	}
}
