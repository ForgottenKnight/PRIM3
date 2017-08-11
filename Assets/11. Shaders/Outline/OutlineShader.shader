Shader "PRIM3/Outline/OutlineEffect"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "black"{}
		_DepthTexture("Depth Texture", 2D) = "black"{}
		_OutlineColor("OutlineColor", Color) = (1, 1, 1, 1)
		_ColorTexture("Color Texture", 2D) = "black"{}
	}
	SubShader
		{
			Pass
				{
					CGPROGRAM

					sampler2D _MainTex;
					sampler2D _DepthTexture;
					uniform sampler2D _CameraDepthTexture;
					uniform sampler2D _ColorTexture;
					half4 _OutlineColor;

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

					struct v2f
					{
						float4 pos : SV_POSITION;
						float2 uvs : TEXCOORD0;
					};

					v2f vert(appdata_base v)
					{
						v2f o;

						//Despite the fact that we are only drawing a quad to the screen, Unity requires us to multiply vertices by our MVP matrix, presumably to keep things working when inexperienced people try copying code from other shaders.
						o.pos = UnityObjectToClipPos(v.vertex);

						//Also, we need to fix the UVs to match our screen space coordinates. There is a Unity define for this that should normally be used.
						o.uvs = o.pos.xy / 2 + 0.5;

						return o;
					}

					bool compare_colors(float4 c1, float4 c2) {
						if (c1.r != c2.r) {
							return false;
						}
						if (c1.g != c2.g) {
							return false;
						}
						if (c1.b != c2.b) {
							return false;
						}
						return true;
					}


					half4 frag(v2f i) : COLOR
					{
						float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uvs));
						float playerDepth = tex2D(_DepthTexture, i.uvs).r;
						float4 col = tex2D(_ColorTexture, i.uvs);
						float4 black = float4(0,0,0,1);

						if (playerDepth < depth && depth > 0.0 && playerDepth != 0.0) {
							if (compare_colors(col, black)) {
								return tex2D(_MainTex, i.uvs);
							} else {
								return col;
							}
						}
						return tex2D(_MainTex, float2(i.uvs.x, i.uvs.y));
					}

						ENDCG

				}
				//end pass    
		}
		//end subshader
}
//end shader