// A stencilled shader to render opaque meshes inverse-masked by a stencil shader.
// @see: OpaqueMesh.shader
// @see: Stencil.shader

Shader "Sago/Opaque Mesh Stencilled Inverse" {
	Properties {
		[HideInInspector]_Offset ("Offset", Float) = 0
		_ReadMask ("ReadMask", int) = 1
		_Ref ("Ref", int) = 1
	}
	SubShader {
		Tags { "Queue"="Geometry+1" }
		Pass {
			BindChannels {
				Bind "Color", color
				Bind "Vertex", vertex
			}
			Blend Off
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