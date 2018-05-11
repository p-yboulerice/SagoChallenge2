namespace SagoTouch {
	
	using UnityEngine;
	
	/// <summary>
	/// A utility class with methods to find the camera and convert 
	/// touch positions from screen space into world space.
	/// </summary>
	public static class CameraUtils {
		
		
		#region Static Methods
		
		/// <summary>
		/// Finds the main camera in the root of the transform or any of its children.
		/// </summary>
		/// <param name="transform">
		/// The child of the transform that contains the camera tagged <c>MainCamera</c>.
		/// </param>
		/// <returns>
		/// The root camera or null if a camera could not be found.
		/// </returns>
		public static Camera FindRootCamera(Transform transform) {
			return CameraUtils.FindRootCamera(transform, "MainCamera");
		}
		
		/// <summary>
		/// Finds the camera with the specified tag in the root of the transform or any of its children.
		/// </summary>
		/// <param name="transform">
		/// TODO:
		/// </param>
		/// <param name="tag">
		/// TODO:
		/// </param>
		/// <returns>
		/// The root camera or null if a camera could not be found.
		/// </returns>
		public static Camera FindRootCamera(Transform transform, string tag) {
			if (transform) {
				foreach (Camera camera in transform.root.GetComponentsInChildren<Camera>()) {
					if (camera.tag == tag) {
						return camera;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Converts the screen point to a camera point, where the z coordinate is the 
		/// distance from the camera to the transform on the z axis.
		/// </summary>
		public static Vector3 ScreenToCameraPoint(Vector2 point, Transform transform, Camera camera) {
			return new Vector3(point.x, point.y, transform.position.z - camera.transform.position.z);
		}
		
		/// <summary>
		/// Converts the screen point to a camera point using the transform's root 
		/// camera, where the z coordinate is the distance from the camera to the 
		/// transform on the z axis.
		/// </summary>
		public static Vector3 ScreenToCameraPoint(Vector2 point, Transform transform) {
			return CameraUtils.ScreenToCameraPoint(point, transform, CameraUtils.FindRootCamera(transform));
		}
		
		/// <summary>
		/// Converts the screen point to a world point. The distance from the camera 
		/// to the world point will be the same as the distance from the camera to 
		/// the transform along the z axis.
		/// </summary>
		public static Vector3 ScreenToWorldPoint(Vector2 point, Transform transform, Camera camera) {
			return camera.ScreenToWorldPoint(CameraUtils.ScreenToCameraPoint(point, transform, camera));
		}
		
		/// <summary>
		/// Converts the screen point to a world point using the transform's root 
		/// camera. The distance from the camera to the world point will be the 
		/// same as the distance from the camera to the transform along the z axis.
		/// </summary>
		public static Vector3 ScreenToWorldPoint(Vector2 point, Transform transform) {
			return CameraUtils.ScreenToWorldPoint(point, transform, CameraUtils.FindRootCamera(transform));
		}
		
		/// <summary>
		/// Converts the touch to a camera point, where the z coordinate is the 
		/// distance from the camera to the transform on the z axis.
		/// </summary>
		public static Vector3 TouchToCameraPoint(Touch touch, Transform transform, Camera camera) {
			return CameraUtils.ScreenToCameraPoint(touch.Position, transform, camera);
		}
		
		/// <summary>
		/// Converts the touch to a camera point using the transform's root camera, 
		/// where the z coordinate is the distance from the camera to the 
		/// transform on the z axis.
		/// </summary>
		public static Vector3 TouchToCameraPoint(Touch touch, Transform transform) {
			return CameraUtils.TouchToCameraPoint(touch, transform, CameraUtils.FindRootCamera(transform));
		}
		
		/// <summary>
		/// Converts the touch to a world point. The distance from the camera 
		/// to the world point will be the same as the distance from the camera to 
		/// the transform along the z axis.
		/// </summary>
		public static Vector3 TouchToWorldPoint(Touch touch, Transform transform, Camera camera) {
			return CameraUtils.ScreenToWorldPoint(touch.Position, transform, camera);
		}
		
		/// <summary>
		/// Converts the touch to a world point using the transform's root camera. 
		/// The distance from the camera to the world point will be the same as the 
		/// distance from the camera to the transform along the z axis.
		/// </summary>
		public static Vector3 TouchToWorldPoint(Touch touch, Transform transform) {
			return CameraUtils.TouchToWorldPoint(touch, transform, CameraUtils.FindRootCamera(transform));
		}
		
		#endregion
		
		
	}
	
}