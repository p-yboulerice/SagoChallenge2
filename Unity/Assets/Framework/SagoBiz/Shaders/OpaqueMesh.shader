Shader "SagoBiz/Opaque Mesh" {
	Properties {
		[HideInInspector]_Offset ("Offset", Float) = 0
	}
	SubShader {
		Pass {
			BindChannels {
				Bind "Color", color
				Bind "Vertex", vertex
			}
			Blend Off
			Cull Off
			Lighting Off
			Offset 0, [_Offset]
		}
	}
}
