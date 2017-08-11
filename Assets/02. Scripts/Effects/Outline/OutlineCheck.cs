using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineCheck : MonoBehaviour {

    public Transform playerContainer;
    public float raycastTimer = 0.5f;
    public LayerMask mask;

    private Vector3 m_Direction;
    private float m_Distance;

	// Use this for initialization
	void Start () {
        StartCoroutine("RaycastCheck");

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator RaycastCheck()
    {
        while (true)
        {
            for(int i = 0; i< playerContainer.childCount; ++i)
            {
                //Debug.Log("Raycast for child " + i);
                Transform l_player = playerContainer.GetChild(i);
                m_Direction = l_player.position - transform.position;
                m_Distance = Vector3.Distance(transform.position, l_player.position);
                Ray ray = new Ray(transform.position, m_Direction);
                RaycastHit hit;
                PlayerOutlineCheck l_POC = l_player.gameObject.GetComponent<PlayerOutlineCheck>();
                Incapacitate l_Incapacitate = l_player.gameObject.GetComponent<Incapacitate>();
                if(Physics.Raycast(ray, out hit, m_Distance, mask) || l_Incapacitate.isActionActive() == true)
                {
                    //oculto
                    if(l_POC && !l_POC.hidden)
                    {
                        l_POC.hidden = true;
                    }
                }
                else
                {
                    //no oculto
                    if (l_POC && l_POC.hidden)
                    {
                        l_POC.hidden = false;
                    }
                }
				yield return null;
            }
            
       

            

            yield return new WaitForSeconds(raycastTimer);
        }
    }
}
