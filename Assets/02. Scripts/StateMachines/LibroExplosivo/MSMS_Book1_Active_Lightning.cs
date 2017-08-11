using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSMS_Book1_Active_Lightning : MonoStateMachineState
{
    [Header("Lightning parameters")]
    public int numberOfLightnings = 3;
    public float lightningScatteringArea = 10f;
    public float maxYDifference = 3.0f;
    public float maxAngle = 90f;
    [Tooltip("Este parámetro controla el tiempo entre que empieza a caer un relámpago y empieza a caer otro.")]
    public float delayBetweenLightnings = 1f;
    public GameObject lightningPrefab;
    public LayerMask ignoreLayers;
    //[Tooltip("Este parámetro controla el tiempo entre que se ve dónde caerá el relámpago y cae.")]
    //public float delayBeforeLightning = 0.5f; // En el prefab!!!

    private Animator m_Anim;
    private TargetSelector m_Target;

    public override void StateUpdate()
    {

    }

    public override void OnEnter()
    {
        m_Anim.SetTrigger("lightning");
        m_Anim.speed = 1.0f;
        DoLightning();
        m_Parent.ChangeToPreviousState();
    }

    public override void OnExit()
    {
    }

    public override void OnStart()
    {
        m_Anim = GetComponentInChildren<Animator>();
        m_Target = GetComponent<TargetSelector>();
    }

    private void DoLightning()
    {
        List<Vector3> l_LightningPoints = new List<Vector3>();
        float l_HalfAngle = maxAngle * 0.5f;
        float beginTime = Time.time;
        float securityTime = 2f;

        while (l_LightningPoints.Count < numberOfLightnings && Time.time < beginTime + securityTime)
        {
            Vector3 l_Pos = m_Target.target.transform.position;
            if (l_LightningPoints.Count > 0) {
                l_Pos.x += lightningScatteringArea * Random.RandomRange(-1f, 1f);
                l_Pos.z += lightningScatteringArea * Random.RandomRange(-1f, 1f);
            }
            Ray ray = new Ray(l_Pos + Vector3.up, -Vector3.up);
            if (Physics.Raycast(ray, maxYDifference, ignoreLayers))
            {
                l_LightningPoints.Add(l_Pos);
            }
            else
            {
                break;
            }
        }
        StartCoroutine(CreateLightnings(l_LightningPoints));
    }

    IEnumerator CreateLightnings(List<Vector3> l_Points)
    {
        for (int i = 0; i < l_Points.Count; ++i)
        {
            Instantiate(lightningPrefab, l_Points[i], Quaternion.identity);
            yield return new WaitForSeconds(delayBetweenLightnings);
        }
        yield return null;
    }
	
}
