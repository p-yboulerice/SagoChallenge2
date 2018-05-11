// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SagoSceneNavigation/FadeTransition" {
	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader {
		Tags { "Queue" = "Overlay" }
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
			Lighting Off
			ZWrite Off
			ZTest Always
			
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
			
			_output vert(_input i) {
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
