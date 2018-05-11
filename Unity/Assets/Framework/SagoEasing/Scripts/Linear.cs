namespace SagoEasing {
	
	using UnityEngine;
	
	public class Linear {
		
		// ================================================================= //
		// Float Methods
		// ================================================================= //
		
		public static float EaseIn(float a, float b, float t) {
			return Mathf.Lerp(a, b, t);
		}
		
		public static float EaseOut(float a, float b, float t) {
			return Mathf.Lerp(a, b, t);
		}
		
		public static float EaseInOut(float a, float b, float t) {
			return Mathf.Lerp(a, b, t);
		}
		
		
		// ================================================================= //
		// Vector3 Methods
		// ================================================================= //
		
		public static Vector3 EaseIn(Vector3 a, Vector3 b, float t) {
			return Vector3.Lerp(a, b, t);
		}
		
		public static Vector3 EaseOut(Vector3 a, Vector3 b, float t) {
			return Vector3.Lerp(a, b, t);
		}
		
		public static Vector3 EaseInOut(Vector3 a, Vector3 b, float t) {
			return Vector3.Lerp(a, b, t);
		}
		
		
		// ================================================================= //
		// Helper Methods
		// ================================================================= //
		
		static float _EaseNone(float t, float b, float c, float d) {
			return c * t / d + b;
		}
		
		static float _EaseIn(float t, float b, float c, float d) {
			return c * t / d + b;
		}
		
		static float _EaseOut(float t, float b, float c, float d) {
			return c * t / d + b;
		}
		
		static float _EaseInOut(float t, float b, float c, float d) {
			return c * t / d + b;
		}
		
		
	}
	
}