// A stencil shader that renders objects to a single bit in the stencil buffer 
// and works with a stencilled shader (another shader that reads from the 
// stencil buffer) to create a masking effect. The stencil shader must render 
// before the stencilled shader. The stencil and stencilled shaders must use 
// the same value for ReadMask and WriteMask properties.

Shader "Sago/Stencil" {
	Properties {
		[HideInInspector]_Offset ("Offset", Float) = 0
		_Ref ("Ref", int) = 1
		_WriteMask ("WriteMask", int) = 1
	}
	SubShader {
		Tags { "Queue"="Geometry" }
		Pass {
			Blend Off
			ColorMask 0
			Cull Off
			Lighting Off
			Offset 0, [_Offset]
			Stencil {
				Comp Always
				Pass Replace
				Ref [_Ref]
				WriteMask [_WriteMask]
			}
			ZWrite Off
		}
	}
}
