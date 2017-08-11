using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractDepth : MonoBehaviour
{

    #region PrivateMembers
    
    #endregion

    #region PublicMembers
    public Camera depthCamera;
	public Camera colorCamera;
    public Material extractDepthMaterial;
    public Material outlineMaterial;
    [Range(0f, 3f)]
    public float depthLevel = 0.5f;
    #endregion

    void Awake()
    {
		depthCamera.depthTextureMode = DepthTextureMode.Depth;
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        // Camara A pinta todo src ok color depth

        // Camara B pinta player depth y se lo pasa a camara A

        // Camara A pinta todo comprobando el depth del player

        RenderTexture rt = new RenderTexture(src.width, src.height, 32, RenderTextureFormat.Depth);
		depthCamera.targetTexture = rt;
        rt.Create();
		depthCamera.Render();

		RenderTexture rtcolor = new RenderTexture(src.width, src.height, 32, RenderTextureFormat.ARGB32);
		colorCamera.targetTexture = rtcolor;
		rtcolor.Create();
		colorCamera.Render();

        outlineMaterial.SetTexture("_DepthTexture", rt);
		outlineMaterial.SetTexture ("_ColorTexture", rtcolor);
        Graphics.Blit(src, dst, outlineMaterial);

        rt.Release();
    }
}
