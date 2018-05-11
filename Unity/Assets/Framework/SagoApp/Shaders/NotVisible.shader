// A shader to render objects without writing to the color, depth or stencil 
// buffers which allows an invisible quad to trigger the OnBecameVisible and 
// OnBecameInvisible methods in a MonoBehaviour.

Shader "Sago/Not Visible" {
	SubShader {
		Pass {
			Blend Off
			ColorMask 0
			Cull Off
			Lighting Off
			ZTest Always
			ZWrite Off
		}
	} 
}
