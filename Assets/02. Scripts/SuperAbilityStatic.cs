using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperAbilityStatic : MonoBehaviour {

    public static Transform markers;


    public GameObject markerStone;
    public GameObject markerWater;//Este marker aun no existe
    public GameObject markerWind;//Este marker aun no existe
    public GameObject markerFire;//Este marker aun no existe

    public static SuperAbilityController m_markerManager;


    [HideInInspector]
    public static bool m_active = false;




    public static void Activate()
    {
        SuperAbilityStatic.m_active = true;
    }

    public static void Deactivate()
    {
        m_markerManager.UseMarkersEnergy();
        SuperAbilityStatic.m_active = false;
    }
}
