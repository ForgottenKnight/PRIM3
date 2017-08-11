using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOutlineCheck : MonoBehaviour {

    public GameObject outlinePlayer;
    public float speed = 0.25f;
    [HideInInspector]
    public bool hidden = false;
    [HideInInspector]
    public Material outlineMaterial;
    [HideInInspector]
    public float timer = 0f;

	public Color hiddenColor = Color.black;
	public Color visibleColor = Color.white;

	// Use this for initialization
	void Start () {
        outlineMaterial = outlinePlayer.GetComponent<SkinnedMeshRenderer>().material;
		outlineMaterial.color = hiddenColor;
	}
	
	// Update is called once per frame
	void Update () {
        if(hidden)
        {
            timer += speed * Time.deltaTime;
        }
        else
        {
			timer -= speed * Time.deltaTime;
        }
		timer = Mathf.Clamp (timer, 0f, 1f);
		outlineMaterial.color = Color.Lerp (hiddenColor, visibleColor, timer);
	}

	void OnEnable() {
		timer = 0f;
		if (outlineMaterial != null) {
			outlineMaterial.color = hiddenColor;
		}
		hidden = false;
	}
}
