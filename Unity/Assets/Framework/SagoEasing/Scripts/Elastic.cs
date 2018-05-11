namespace SagoEasing {
	
	using UnityEngine;
	
	public class Elastic {
		
		// ================================================================= //
		// Float Methods
		// ================================================================= //
		
		public static float EaseIn(float a, float b, float t, float amplitude, float period) {
			return _EaseIn(t, a, b - a, 1f, amplitude, period);
		}
		
		public static float EaseOut(float a, float b, float t, float amplitude, float period) {
			return _EaseOut(t, a, b - a, 1f, amplitude, period);
		}
		
		public static float EaseInOut(float a, float b, float t, float amplitude, float period) {
			return _EaseInOut(t, a, b - a, 1f, amplitude, period);
		}
		
		
		public static float EaseIn(float a, float b, float t) {
			return EaseIn(a, b, t, 0f, 0f);
		}
		
		public static float EaseOut(float a, float b, float t) {
			return EaseOut(a, b, t, 0f, 0f);
		}
		
		public static float EaseInOut(float a, float b, float t) {
			return EaseInOut(a, b, t, 0f, 0f);
		}
		
		
		// ================================================================= //
		// Vector3 Methods
		// ================================================================= //
		
		public static Vector3 EaseIn(Vector3 a, Vector3 b, float t, float amplitude, float period) {
			return a + (b - a) * EaseIn(0f, 1f, t, amplitude, period);
		}
		
		public static Vector3 EaseOut(Vector3 a, Vector3 b, float t, float amplitude, float period) {
			return a + (b - a) * EaseOut(0f, 1f, t, amplitude, period);
		}
		
		public static Vector3 EaseInOut(Vector3 a, Vector3 b, float t, float amplitude, float period) {
			return a + (b - a) * EaseInOut(0f, 1f, t, amplitude, period);
		}
		
		
		public static Vector3 EaseIn(Vector3 a, Vector3 b, float t) {
			return EaseIn(a, b, t, 0f, 0f);
		}
		
		public static Vector3 EaseOut(Vector3 a, Vector3 b, float t) {
			return EaseOut(a, b, t, 0f, 0f);
		}
		
		public static Vector3 EaseInOut(Vector3 a, Vector3 b, float t) {
			return EaseInOut(a, b, t, 0f, 0f);
		}
		
		
		// ================================================================= //
		// Helper Methods
		// ================================================================= //
		
		static bool _Approximately(float a, float b, float threshold = 0.0001f) {
			return Mathf.Abs(b - a) < threshold;
		}
		
		static float _EaseIn(float t, float b, float c, float d, float a, float p) {
			if (_Approximately(t, 0f))
				return b;
			
			if (_Approximately(t /= d, 1f))
				return b + c;
			
			if (_Approximately(p, 0f))
				p = d * 0.3f;
			
			float s;
			if (_Approximately(a, 0f) || a < Mathf.Abs(c))
			{
				a = c;
				s = p / 4f;
			}
			else
			{
				s = p / (2f * Mathf.PI) * Mathf.Asin(c / a);
			}
			
			return -(a * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t * d - s) * (2f * Mathf.PI) / p)) + b;
		}
		
		static float _EaseOut(float t, float b, float c, float d, float a, float p) {
			if (_Approximately(t, 0f))
				return b;
			
			if (_Approximately(t /= d, 1f))
				return b + c;
			
			if (_Approximately(p, 0f))
				p = d * 0.3f;
			
			float s;
			if (_Approximately(a, 0f) || a < Mathf.Abs(c))
			{
				a = c;
				s = p / 4f;
			}
			else
			{
				s = p / (2f * Mathf.PI) * Mathf.Asin(c / a);
			}
			
			return a * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * d - s) * (2f * Mathf.PI) / p) + c + b;
		}
		
		static float _EaseInOut(float t, float b, float c, float d, float a, float p) {
			if (_Approximately(t, 0f))
				return b;
			
			if (_Approximately(t /= d / 2f, 2f))
				return b + c;
			
			if (_Approximately(p, 0f))
				p = d * (0.3f * 1.5f);
			
			float s;
			if (_Approximately(a, 0f) || a < Mathf.Abs(c))
			{
				a = c;
				s = p / 4f;
			}
			else
			{
				s = p / (2f * Mathf.PI) * Mathf.Asin(c / a);
			}
			
			if (t < 1f)
			{
				return -0.5f * (a * Mathf.Pow(2f, 10f * (t -= 1f)) *
					   Mathf.Sin((t * d - s) * (2f * Mathf.PI) /p)) + b;
			}
			
			return a * Mathf.Pow(2f, -10f * (t -= 1f)) *
				   Mathf.Sin((t * d - s) * (2f * Mathf.PI) / p ) * 0.5f + c + b;
		}
		
		
	}
	
}