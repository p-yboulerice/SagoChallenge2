// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sago Sago/Core/Solid Color Transparent" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		[HideInInspector]_Offset ("Offset", Float) = 0
	}
	SubShader {
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			Lighting Off
			Offset 0, [_Offset]
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			fixed4 _Color;
			
			struct _input {
				float4 vertex : POSITION;
			};
			
			struct _output {
				float4 position : SV_POSITION;
			};
			
			_output vert (_input i) {
				_output o;
				o.position = UnityObjectToClipPos(i.vertex);
				return o;
			}
			
			half4 frag(_output o) : COLOR {
				return _Color;
			}
			
			ENDCG
		}
	}
}