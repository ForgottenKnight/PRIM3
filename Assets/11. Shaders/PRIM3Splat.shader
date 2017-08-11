Shader "PRIM3/PRIM3Splat" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Power("Power", float) = 1.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_SplatMap("Splat", 2D) = "white" {}
		_SplatR("R Texture", 2D) = "white" {}
		_SplatG("G Texture", 2D) = "white" {}
		_SplatB("B Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _SplatMap;
		sampler2D _SplatR;
		sampler2D _SplatG;
		sampler2D _SplatB;

		struct Input {
			float2 uv_SplatMap;
			float2 uv_SplatR;
			float2 uv_SplatG;
			float2 uv_SplatB;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		half _Power;

		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 splat = tex2D(_SplatMap, IN.uv_SplatMap);
			fixed4 c =  _Color;
			c *= (tex2D(_SplatR, IN.uv_SplatR) * splat.r + tex2D(_SplatG, IN.uv_SplatG) * splat.g + tex2D(_SplatB, IN.uv_SplatB) * splat.b + tex2D(_SplatG, IN.uv_SplatG) * splat.a) * _Power;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
