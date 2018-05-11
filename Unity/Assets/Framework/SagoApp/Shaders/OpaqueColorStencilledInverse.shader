// A stencilled shader to render objects using an opaque color inverse-masked by a stencil shader.
// @see: OpaqueColor.shader
// @see: Stencil.shader

Shader "Sago/Opaque Color Stencilled Inverse" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		[HideInInspector]_Offset ("Offset", Float) = 0
		_ReadMask ("ReadMask", int) = 1
		_Ref ("Ref", int) = 1
	}
	SubShader {
		Tags { "Queue"="Geometry+1" }
		Pass {
			Blend Off
			Color [_Color]
			Cull Off
			Lighting Off
			Offset 0, [_Offset]
			Stencil {
				ReadMask [_ReadMask]
				Ref [_Ref]
				Comp NotEqual
			}
		}
	}
}
