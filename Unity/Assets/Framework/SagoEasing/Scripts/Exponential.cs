namespace SagoEasing {
	
	using UnityEngine;
	
	public class Exponential {
		
		// ================================================================= //
		// Float Methods
		// ================================================================= //
		
		public static float EaseIn(float a, float b, float t) {
			return _EaseIn(t, a, b - a, 1f);
		}
		
		public static float EaseOut(float a, float b, float t) {
			return _EaseOut(t, a, b - a, 1f);
		}
		
		public static float EaseInOut(float a, float b, float t) {
			return _EaseInOut(t, a, b - a, 1f);
		}
		
		
		// ================================================================= //
		// Vector3 Methods
		// ================================================================= //
		
		public static Vector3 EaseIn(Vector3 a, Vector3 b, float t) {
			return a + (b - a) * EaseIn(0f, 1f, t);
		}
		
		public static Vector3 EaseOut(Vector3 a, Vector3 b, float t) {
			return a + (b - a) * EaseOut(0f, 1f, t);
		}
		
		public static Vector3 EaseInOut(Vector3 a, Vector3 b, float t) {
			return a + (b - a) * EaseInOut(0f, 1f, t);
		}
		
		
		// ================================================================= //
		// Helper Methods
		// ================================================================= //
		
		static bool _Approximately(float a, float b, float threshold = 0.0001f) {
			return Mathf.Abs(b - a) < threshold;
		}
		
		static float _EaseIn(float t, float b, float c, float d) {
			return _Approximately(t,0f) ? b : c * Mathf.Pow(2f, 10f * (t / d - 1f)) + b;
		}
		
		static float _EaseOut(float t, float b, float c, float d) {
			return _Approximately(t,d) ? b + c : c * (-Mathf.Pow(2f, -10f * t / d) + 1f) + b;
		}
		
		static float _EaseInOut(float t, float b, float c, float d) {
			if (_Approximately(t,0f))
				return b;
			
			if (_Approximately(t,d))
				return b + c;
			
			if ((t /= d / 2f) < 1f)
				return c / 2f * Mathf.Pow(2f, 10f * (t - 1f)) + b;
			
			return c / 2f * (-Mathf.Pow(2f, -10f * --t) + 2f) + b;
		}
		
		
	}
	
}