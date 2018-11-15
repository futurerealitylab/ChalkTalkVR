Shader "Unlit/NewUnlitShader"
{
	Properties {
		[PerRendererData]_Color("Color", Color) = (1,1,1,1)
	}

	SubShader {
		LOD 100
		Tags {"RenderType" = "Opaque"}

		ZWrite Off
		Lighting Off
		Fog { Mode Off }

		Pass {
			Color[_Color]
			SetTexture[_MainTex] { combine texture * primary }
		}
	}
}