namespace SagoEasing {
	
	using UnityEngine;
	
	public class Back {
		
		// ================================================================= //
		// Float Methods
		// ================================================================= //
		
		public static float EaseIn(float a, float b, float t, float overshoot) {
			return _EaseIn(t, a, b - a, 1f, overshoot);
		}
		
		public static float EaseOut(float a, float b, float t, float overshoot) {
			return _EaseOut(t, a, b - a, 1f, overshoot);
		}
		
		public static float EaseInOut(float a, float b, float t, float overshoot) {
			return _EaseInOut(t, a, b - a, 1f, overshoot);
		}
		
		
		public static float EaseIn(float a, float b, float t) {
			return EaseIn(a, b, t, 0f);
		}
		
		public static float EaseOut(float a, float b, float t) {
			return EaseOut(a, b, t, 0f);
		}
		
		public static float EaseInOut(float a, float b, float t) {
			return EaseInOut(a, b, t, 0f);
		}
		
		
		// ================================================================= //
		// Vector3 Methods
		// ================================================================= //
		
		public static Vector3 EaseIn(Vector3 a, Vector3 b, float t, float overshoot) {
			return a + (b - a) * EaseIn(0f, 1f, t, overshoot);
		}
		
		public static Vector3 EaseOut(Vector3 a, Vector3 b, float t, float overshoot) {
			return a + (b - a) * EaseOut(0f, 1f, t, overshoot);
		}
		
		public static Vector3 EaseInOut(Vector3 a, Vector3 b, float t, float overshoot) {
			return a + (b - a) * EaseInOut(0f, 1f, t, overshoot);
		}
		
		
		public static Vector3 EaseIn(Vector3 a, Vector3 b, float t) {
			return EaseIn(a, b, t, 0f);
		}
		
		public static Vector3 EaseOut(Vector3 a, Vector3 b, float t) {
			return EaseOut(a, b, t, 0f);
		}
		
		public static Vector3 EaseInOut(Vector3 a, Vector3 b, float t) {
			return EaseInOut(a, b, t, 0f);
		}
		
		
		// ================================================================= //
		// Helper Methods
		// ================================================================= //
		
		static bool _Approximately(float a, float b, float threshold = 0.0001f) {
			return Mathf.Abs(b - a) < threshold;
		}
		
		static float _EaseIn(float t, float b, float c, float d, float s) {
			if (_Approximately(s, 0f))
				s = 1.70158f;
			
			return c * (t /= d) * t * ((s + 1f) * t - s) + b;
		}
		
		static float _EaseOut(float t, float b, float c, float d, float s) {
			if (_Approximately(s, 0f))
				s = 1.70158f;
			
			return c * ((t = t / d - 1f) * t * ((s + 1f) * t + s) + 1f) + b;
		}
		
		static float _EaseInOut(float t, float b, float c, float d, float s) {
			if (_Approximately(s, 0f))
				s = 1.70158f;
			
			if ((t /= d / 2f) < 1f)
				return c / 2f * (t * t * (((s *= (1.525f)) + 1f) * t - s)) + b;
			
			return c / 2f * ((t -= 2f) * t * (((s *= (1.525f)) + 1f) * t + s) + 2f) + b;
		}
		
		
	}
	
}