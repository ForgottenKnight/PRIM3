using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimumQuality : MonoBehaviour {
    [Range(0,5)]
    public int minimumQualityNeeded = 1;

    public bool destroyEntireGameObject = true;
    public MonoBehaviour[] destroyList;

	void Start () {
		if (QualitySettings.GetQualityLevel() < minimumQualityNeeded) {
            if (destroyEntireGameObject == true)
            {
                Destroy(gameObject);
            }
            else
            {
                for (int i = 0; i < destroyList.Length; ++i)
                {
                    Destroy(destroyList[i]);
                }
            }
        }
        Destroy(this);
	}
}
