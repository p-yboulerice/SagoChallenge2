// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sago Sago/Core/Vertex Color Opaque" {
	Properties {
		[HideInInspector]_Offset ("Offset", Float) = 0
	}
	SubShader {
		Pass {
			Blend One Zero
			Cull Off
			Lighting Off
			Offset 0, [_Offset]
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			struct _input {
				float4 vertex : POSITION;
				float4 color : COLOR;
			};
			
			struct _output {
				float4 position : SV_POSITION;
				fixed4 color : COLOR;
			};
			
			_output vert (_input i) {
				_output o;
				o.position = UnityObjectToClipPos(i.vertex);
				o.color = i.color;
				return o;
			}
			
			half4 frag(_output o) : COLOR {
				return o.color;
			}
			
			ENDCG
		}
	}
}