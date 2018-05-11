// A stencil shader that renders a mesh to the color buffer and stencil buffer.
// Use this shader if you want a stencil that is visible and opaque.
// @see: OpaqueMesh.shader
// @see: Stencil.shader

Shader "Sago/Opaque Mesh Stencil" {
	Properties {
		[HideInInspector]_Offset ("Offset", Float) = 0
		_Ref ("Ref", int) = 1
		_WriteMask ("WriteMask", int) = 1
	}
	SubShader {
		Tags { "Queue"="Geometry" }
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
				Ref [_Ref]
				Comp Always
				Pass Replace
				WriteMask [_WriteMask]
			}
		}
	}
}
