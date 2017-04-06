//Debug.shader
//Created by Aaron C Gaudette on 02.03.15
//Shader for debug rendering

Shader "Custom/Debug"{
	SubShader{
		Pass{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest Always
			Cull Off
			Fog{Mode Off}
			BindChannels{
				Bind "vertex", vertex
				Bind "color", color
			}
		}
	} 
	FallBack Off
}
