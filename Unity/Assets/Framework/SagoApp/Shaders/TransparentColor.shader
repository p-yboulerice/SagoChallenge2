// A shader to render objects using a transparent color. The transparent color 
// shader is much less efficient than the opaque color shader because blending 
// is turned on and it has to run in the transparent queue. Only use this 
// shader if the color is transparent.

Shader "Sago/Transparent Color" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		[HideInInspector]_Offset ("Offset", Float) = 0
	}
	SubShader {
		Tags {
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
		}
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			Color [_Color]
			Cull Off
			Lighting Off
			Offset 0, [_Offset]
			ZWrite Off
		}
	}
}