// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

 Shader "Instanced/InstancedSurfaceShader" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _Color2 ("Main Color end", Color) = (0,0,0,1)
	    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	    _ShakeDisplacement ("Displacement", Range (0, 1.0)) = 1.0
	    _ShakeTime ("Shake Time", Range (0, 1.0)) = 1.0
	    _ShakeTimeVariation ("Shake Time Variation", Range(0.0, 1.0)) = 0.0
	    _ShakeWindspeed ("Shake Windspeed", Range (0, 1.0)) = 1.0
	    _ShakeBending ("Shake Bending", Range (0, 1.0)) = 1.0
	    _PlayerMaxDistance("Player distance", float) = 1.0
	    _Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        LOD 200

        CGPROGRAM
        #include "UnityCG.cginc"
        #pragma target 3.0
        #pragma surface surf Standard alphatest:_Cutoff vertex:vert addshadow
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:setup

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _Color2;
		float _ShakeDisplacement;
		float _ShakeTime;
		float _ShakeTimeVariation;
		float _ShakeWindspeed;
		float _ShakeBending;
		half _Glossiness;
		half _Metallic;

		uniform float4 _G_PlayerPosition0 = float4(0,0,0,0);
		uniform float4 _G_PlayerPosition1 = float4(0,0,0,0);
		uniform float4 _G_PlayerPosition2 = float4(0,0,0,0);
		uniform float _PlayerMaxDistance;

		int instanceID;
		float colorT;

        struct Input {
            float2 uv_MainTex;
        };

    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        StructuredBuffer<float4> positionBuffer;
        StructuredBuffer<float> colorBuffer;
    #endif

        void rotate2D(inout float2 v, float r)
        {
            float s, c;
            sincos(r, s, c);
            v = float2(v.x * c - v.y * s, v.x * s + v.y * c);
        }

        void setup()
        {
        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            float4 data = positionBuffer[unity_InstanceID];
            instanceID = unity_InstanceID;
            colorT = colorBuffer[unity_InstanceID];

            unity_ObjectToWorld._11_21_31_41 = float4(data.w, 0, 0, 0);
            unity_ObjectToWorld._12_22_32_42 = float4(0, data.w, 0, 0);
            unity_ObjectToWorld._13_23_33_43 = float4(0, 0, data.w, 0);
            unity_ObjectToWorld._14_24_34_44 = float4(data.xyz, 1);
            unity_WorldToObject = unity_ObjectToWorld;
            unity_WorldToObject._14_24_34 *= -1;
            unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
        #endif
        }

		void FastSinCos (float4 val, out float4 s, out float4 c) {
		    val = val * 6.408849 - 3.1415927;
		    float4 r5 = val * val;
		    float4 r6 = r5 * r5;
		    float4 r7 = r6 * r5;
		    float4 r8 = r6 * r5;
		    float4 r1 = r5 * val;
		    float4 r2 = r1 * r5;
		    float4 r3 = r2 * r5;
		    float4 sin7 = {1, -0.16161616, 0.0083333, -0.00019841} ;
		    float4 cos8  = {-0.5, 0.041666666, -0.0013888889, 0.000024801587} ;
		    s =  val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
		    c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
		}

		float4 RotateAroundYInDegrees (float4 vertex, float degrees)
		{
		 float alpha = degrees * UNITY_PI / 180.0;
		 float sina, cosa;
		 sincos(alpha, sina, cosa);
		 float2x2 m = float2x2(cosa, -sina, sina, cosa);
		 return float4(mul(m, vertex.xz), vertex.yw).xzyw;
		}

		bool PlayerNear(int player) {
		return false;
			float4 playerPos;
			switch(player) {
				case 0:
					playerPos = _G_PlayerPosition0;
					break;
				case 1:
					playerPos = _G_PlayerPosition1;
					break;
				case 2:
					playerPos = _G_PlayerPosition2;
					break;
				default:
					playerPos = float4(0,0,0,0);
					break;
			}
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			if (playerPos.w == 0) {
				return false;
			} else if (distance(positionBuffer[instanceID], playerPos) < _PlayerMaxDistance) {
				return true;
			} else {
				return false;
			}
			#endif
			return false;
		}
	 
		void vert (inout appdata_full v) {

			v.vertex = RotateAroundYInDegrees(v.vertex, instanceID*15.0); // "Random" rotation
			float4 n = float4(v.normal, 1);
			n = RotateAroundYInDegrees(n, instanceID*15.0); // "Random" rotation
			v.normal = n.xyz;
		   
		    float factor = (1 - _ShakeDisplacement -  v.color.r) * 0.5;
		       
		    const float _WindSpeed  = (_ShakeWindspeed  +  v.color.g );    
		    const float _WaveScale = _ShakeDisplacement;
		   
		    const float4 _waveXSize = float4(0.048, 0.06, 0.24, 0.096);
		    const float4 _waveZSize = float4 (0.024, .08, 0.08, 0.2);
		    const float4 waveSpeed = float4 (1.2, 2, 1.6, 4.8);
		 
		    float4 _waveXmove = float4(0.024, 0.04, -0.12, 0.096);
		    float4 _waveZmove = float4 (0.006, .02, -0.02, 0.1);
		   
		    float4 waves;
		    waves = v.vertex.x * _waveXSize;
		    waves += v.vertex.z * _waveZSize;
		 
		    waves += (_Time.x + instanceID%100 * _ShakeTimeVariation) * (1 - _ShakeTime * 2 - v.color.b ) * waveSpeed *_WindSpeed;
		 
		    float4 s, c;
		    waves = frac (waves);
		    FastSinCos (waves, s,c);
		 
		    float waveAmount = v.texcoord.y * (v.color.a + _ShakeBending);
		    s *= waveAmount;
		 
		    s *= normalize (waveSpeed);
		 
		    s = s * s;
		    float fade = dot (s, 1.3);
		    s = s * s;
		    float3 waveMove = float3 (0,0,0);
		    waveMove.x = dot (s, _waveXmove); // Estos valores indican hacia donde se mueve la hierba
		    waveMove.z = dot (s, _waveZmove);


		    if (PlayerNear(0) || PlayerNear(1) || PlayerNear(2)) {
		    	float4 staticWaves;
		    	staticWaves = v.vertex.x * _waveXSize;
			    staticWaves += v.vertex.z * _waveZSize;
			 
			    staticWaves += (1 - _ShakeTime * 2 - v.color.b ) * waveSpeed * _WindSpeed;
			 
			    float4 s, c;
			    staticWaves = frac (staticWaves);
			    FastSinCos (staticWaves, s,c);
			 
			    float staticWaveAmount = v.texcoord.y * (v.color.a + _ShakeBending * 2.0);
			    s *= staticWaveAmount;
			 
			    s *= normalize (waveSpeed);
			 
			    s = s * s;
			    float fade = dot (s, 1.3);
			    s = s * s;
			    float3 staticWaveMove = float3 (0,0,0);
			    staticWaveMove.x = dot (s, _waveXmove) * 5; // Estos valores indican hacia donde se mueve la hierba
			    //staticWaveMove.z = dot (s, _waveZmove) * 5;
		    	v.vertex.xz -= mul ((float3x3)unity_WorldToObject, staticWaveMove).xz;
		    } else {
		    	v.vertex.xz -= mul ((float3x3)unity_WorldToObject, waveMove).xz;
		    }

		   


		}

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * lerp(_Color, _Color2, colorT);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
            o.Alpha = c.a;
       }
        ENDCG
    }
    FallBack "Diffuse"
}