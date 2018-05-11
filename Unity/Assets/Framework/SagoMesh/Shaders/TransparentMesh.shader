Shader "SagoMesh/Transparent Mesh" {
	Properties {
		[HideInInspector]_Offset ("Offset", Float) = 0
	}
	SubShader {
		Tags {
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent" 
		}
		Pass {
			BindChannels {
				Bind "Color", color
				Bind "Vertex", vertex
			}
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting Off
			Offset 0, [_Offset]
			ZWrite Off
		}
	}
}
