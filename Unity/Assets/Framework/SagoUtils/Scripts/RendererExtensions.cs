namespace SagoUtils {
	
	using UnityEngine;
	
	public static class RendererExtensions {
		
		/// <summary>
		/// Returns the Renderer.bounds, but accounts for the Unity 5
		/// interpretation that fails to center empty bounds on the object.
		/// </summary>
		/// <param name="renderer">Renderer.</param>
		public static Bounds GetBounds(this Renderer renderer) {
			Bounds bounds = renderer.bounds;
			
			if (bounds.size == Vector3.zero) {
				bounds.center = renderer.GetComponent<Transform>().position;
			}
			
			return bounds;
		}
		
	}
}