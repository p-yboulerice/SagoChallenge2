// A stencilled shader to render transparent meshes inverse-masked by a stencil shader.
// @see: TransparentMesh.shader
// @see: Stencil.shader

Shader "Sago/Transparent Mesh Stencilled Inverse" {
	Properties {
		[HideInInspector]_Offset ("Offset", Float) = 0
		_ReadMask ("ReadMask", int) = 1
		_Ref ("Ref", int) = 1
	}
	SubShader {
		Tags { "Queue"="Transparent"}
		Pass {
			BindChannels {
				Bind "Color", color
				Bind "Vertex", vertex
			}
			Blend SrcAlpha OneMinusSrcAlpha
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