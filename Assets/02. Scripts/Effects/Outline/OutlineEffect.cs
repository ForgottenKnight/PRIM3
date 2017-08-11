using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineEffect : MonoBehaviour {
	public Camera outlineCamera;
	public Material outlineMaterial;
	// Use this for initialization
	void Start () {
	}
	
	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		RenderTexture rt = new RenderTexture(src.width, src.height, 32, RenderTextureFormat.ARGB32);
		outlineCamera.targetTexture = rt;
		rt.Create();
		outlineCamera.Render();

		outlineMaterial.SetTexture("_SceneTex", src);
		Graphics.Blit(rt, dst, outlineMaterial);

		rt.Release();
	}
}
