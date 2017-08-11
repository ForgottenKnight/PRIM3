Shader "PRIM3/Overlay" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SpecColor ("Spec Color", Color) = (1,1,1,1)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.7
	}
	SubShader 
	{ 
		Tags { "Queue"="Overlay+1" }
		
		Pass 
		{
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
