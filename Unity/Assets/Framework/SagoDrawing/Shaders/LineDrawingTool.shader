Shader "BugBuilder/Drawing/LineDrawingTool" {
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
			Blend SrcAlpha OneMinusSrcAlpha, One One
			Cull Off
			Lighting Off
			ZWrite Off
		}
	}
}
