// A shader to render objects using an opaque color.

Shader "Sago/Opaque Color" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		[HideInInspector]_Offset ("Offset", Float) = 0
	}
	SubShader {
		Pass {
			Blend Off
			Color [_Color]
			Cull Off
			Lighting Off
			Offset 0, [_Offset]
		}
	}
}