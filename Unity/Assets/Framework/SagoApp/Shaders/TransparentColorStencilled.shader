// A stencilled shader to render objects using a transparent color masked by a stencil shader.
// @see: TransparentColor.shader
// @see: Stencil.shader

Shader "Sago/Transparent Color Stencilled" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		[HideInInspector]_Offset ("Offset", Float) = 0
		_ReadMask ("ReadMask", int) = 1
		_Ref ("Ref", int) = 1
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
			Stencil {
				ReadMask [_ReadMask]
				Ref [_Ref]
				Comp Equal
			}
			ZWrite Off
		}
	}
}
