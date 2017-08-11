Shader "PRIM3/Overlay" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ColorInvisible("OverlayColor", Color) = (1,1,1,1)
		_SpecColor ("Spec Color", Color) = (1,1,1,1)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.7
        _Outline ("Outline", float) = 1.0
	}
	SubShader 
	{ 
		Tags { "Queue"="Overlay+1" }
  
		Pass
		{
			//Blend dstcolor one
			ZTest greater
			ZWrite Off
			Lighting Off
			//Cull off
			Offset -10,0
			Color [_ColorInvisible]
		}
		
		Pass 
		{
			//Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
			//Blend One OneMinusSrcAlpha // Premultiplied transparency
			//Blend One One // Additive
			//Blend OneMinusDstColor One // Soft Additive
			//Blend DstColor Zero // Multiplicative
			//Blend Zero One
			//Blend DstColor SrcColor // 2x Multiplicative
			ZTest lequal
			//ZWrite On
		 	Material {
				Diffuse [_Color]
				Ambient (0,0,0,0)
				Shininess [_Shininess]
				Specular [_SpecColor]
				Emission [_Emission]
			}
			Lighting On
			SeparateSpecular On
			SetTexture [_MainTex] {combine texture * primary DOUBLE, texture * primary}
		}
	}
	FallBack "Diffuse"
}
