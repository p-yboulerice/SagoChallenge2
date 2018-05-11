namespace SagoEasing {
	
	using UnityEngine;
	
	public class Bounce {
		
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
	
		static float _EaseOut(float t, float b, float c, float d) {
			if ((t /= d) < (1f / 2.75f))
				return c * (7.5625f * t * t) + b;
		
			else if (t < (2f / 2.75f))
				return c * (7.5625f * (t -= (1.5f / 2.75f)) * t + 0.75f) + b;
		
			else if (t < (2.5f / 2.75f))
				return c * (7.5625f * (t -= (2.25f / 2.75f)) * t + 0.9375f) + b;
		
			else
				return c * (7.5625f * (t -= (2.625f / 2.75f)) * t + 0.984375f) + b;
		}
		
		static float _EaseIn(float t, float b, float c, float d) {
			return c - _EaseOut(d - t, 0f, c, d) + b;
		}
		
		static float _EaseInOut(float t, float b, float c, float d) {
			if (t < d/2f)
				return _EaseIn(t * 2f, 0f, c, d) * 0.5f + b;
			else
				return _EaseOut(t * 2f - d, 0f, c, d) * 0.5f + c * 0.5f + b;
		}
		
		
	}
	
}