Shader "PRIM3/Overlay" {
	Properties {
		_ColorInvisible("OverlayColor", Color) = (1,1,1,1)
	}
	SubShader 
	{ 
		Tags { "Queue"="Overlay+1" }
  
		Pass
		{
			ZTest greater
			ZWrite Off
			Lighting Off
			//Cull off
			//Offset 15,50
			Color [_ColorInvisible]
		}
		
	}
	FallBack "Diffuse"
}
