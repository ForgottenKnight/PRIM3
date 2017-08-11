using UnityEngine;
using System.Collections.Generic;

public class QualityManager : MonoBehaviour {
	public static QualityManager instance = null;

	public UnityStandardAssets.ImageEffects.Antialiasing antiAliasingMain;
	public UnityStandardAssets.ImageEffects.Antialiasing antiAliasingCinematic;
	public UnityStandardAssets.ImageEffects.BloomOptimized bloomMain;
	public UnityStandardAssets.ImageEffects.BloomOptimized bloomCinematic;
	public UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration vignettingMain;
	public UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration vignettingCinematic;
	public UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion occlusionMain;
	public UnityStandardAssets.ImageEffects.ScreenSpaceAmbientOcclusion occlusionCinematic;

    public int maxQualityLevel = 5;
    private int m_CurrentQualityLevel = 0;

	private Dictionary<string, GameObject> deactivableObjectsMap;
	[System.Serializable]
	public struct DeactivableObject {
		public string name;
		public GameObject deactivable;
	}
	public DeactivableObject[] deactivableObjects;
	// Use this for initialization
	void Start () {
		instance = this;
		deactivableObjectsMap = new Dictionary<string, GameObject> ();
		for (int i = 0; i < deactivableObjects.Length; ++i) {
			deactivableObjectsMap.Add(deactivableObjects[i].name, deactivableObjects[i].deactivable);
		}

        m_CurrentQualityLevel = Mathf.Clamp(QualitySettings.GetQualityLevel(), 0, maxQualityLevel);
        switch(m_CurrentQualityLevel)
        {
            case 0:
                ActivateEffects(false);
                ActivateObject("waterfall1", false);
                ActivateObject("waterfall2", false);
                ActivateObject("particles", false);
                break;
            case 1:
                ActivateEffects(false);
                ActivateObject("particles", false);
                break;
            case 2:
                ActivateEffects(false);
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            default:
                break;
        }
	}

	public void ActivateEffects(bool activation) {
		antiAliasingMain.enabled = activation;
		antiAliasingCinematic.enabled = activation;
		bloomMain.enabled = activation;
		bloomCinematic.enabled = activation;
		vignettingMain.enabled = activation;
		vignettingCinematic.enabled = activation;
		occlusionMain.enabled = activation;
		occlusionCinematic.enabled = activation;
	}

	public void ActivateObject(string name, bool activation) {
        if (deactivableObjectsMap.ContainsKey(name))
        {
            deactivableObjectsMap[name].SetActive(activation);
        }
	}
}

