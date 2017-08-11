using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAnimationEvents : MonoBehaviour
{

    public MSMS_Book1_Active_Fire stateFire;

	// Use this for initialization
	void Start () {
        stateFire = transform.parent.GetComponent<MSMS_Book1_Active_Fire>();
	}

    public void ShotAttack()
    {
        if(stateFire)
        {
            stateFire.DoAttack();
        }
    }   
}
