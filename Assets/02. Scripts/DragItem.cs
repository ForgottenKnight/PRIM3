using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragItem : MonoBehaviour {

    public float InitialMovementSpeed = 0.1f;
    public float FinalMovementSpeed = 1f;

    private float currentMovementSpeed;

    private bool m_finalSpeed = false;
    private bool m_moving = false;
    private GameObject m_target;
    private DroppedItem fuegoFauto;

    void Start()
    {
        fuegoFauto = gameObject.transform.GetChild(0).GetComponent<DroppedItem>();
        currentMovementSpeed = InitialMovementSpeed;
    }

    void Update()
    {
        if (fuegoFauto.m_timer >= 1.0f && !m_finalSpeed)
        {
            currentMovementSpeed = FinalMovementSpeed;
            m_finalSpeed = true;
        }
        else
        {
            currentMovementSpeed = Mathf.Lerp(InitialMovementSpeed, FinalMovementSpeed, fuegoFauto.m_timer);
        }
    }
    
    void OnTriggerEnter(Collider col)
    {
        m_moving = true;
        if (!m_target)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                m_target = col.gameObject;
            }
        }
    }

    void OnTriggerStay(Collider col)
    {      
        if(m_target)
        {
            float step = currentMovementSpeed * Time.deltaTime;

            Vector3 l_TarPos = col.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, l_TarPos, step);
        }else
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                m_target = col.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        m_moving = false;
        m_target = null;
    }

}
