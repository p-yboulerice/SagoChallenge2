namespace SagoLayout {

	using UnityEngine;

	public class Artboard : LayoutComponent {
		

		//
		// Inspector Properties
		//
		public Color32 BackgroundColor;
		public bool BackgroundOpaque;

		[Range(0, 100)]
		public int CameraDepth;

		public int Layer;
		public bool LockToLayer;
		public Vector2 Size;
		
		
		//
		// Properties
		//
		public float Aspect {
			get { return this.Size.x / this.Size.y; }
		}
		
		public Bounds Bounds {
			get { return CalculateBounds(true); }
		}
		
		public Camera Camera {
			get {
				m_Camera = m_Camera ? m_Camera : GetCamera();
				return m_Camera;
			}
		}

		
		//
		// Member Variables
		//
		protected Camera m_Camera;

		
		//
		// MonoBehaviour
		//
		override protected void OnDrawGizmos() {
			base.OnDrawGizmos();
			
			Bounds bounds;
			bounds = CalculateBounds(false);
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(bounds.center, bounds.size);
			
			bounds = this.Bounds;
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}
		
		override protected void Reset() {
			base.Reset();
			this.BackgroundColor = Color.white;
			this.BackgroundOpaque = true;
			this.CameraDepth = -1;
			this.Layer = 0;
			this.LockToLayer = true;
			this.Size = new Vector2(1136, 640);
		}
		
		
		//
		// Public Methods
		//
		override public void Apply() {

			if (!this.IsPrefabAsset) {

				Camera camera;
				camera = this.Camera;
				camera.backgroundColor = this.BackgroundColor;
				camera.clearFlags = GetCameraClearFlags();
				camera.depth = this.CameraDepth;
				camera.orthographic = true;
				camera.orthographicSize = GetCameraSize();
				camera.transform.position = GetCameraPosition();

				if (this.LockToLayer) {
					camera.cullingMask = 1 << this.Layer;
					foreach (Transform child in GetComponentsInChildren<Transform>()) {
						child.gameObject.layer = this.Layer;
					}
				}

			}
			
		}
		
		
		//
		// Bounds
		//
		protected Bounds CalculateBounds(bool useCameraAspect) {
			
			Vector3 center;
			center = this.Transform.position + this.Camera.orthographicSize * (this.Aspect * Vector3.right + Vector3.down);

			// On Android platforms the phones that are meant for portrait orientation by default may not 
			// have their screen information validated when used immediately in Unity's MonoBehaviour.OnApplicationPause(). 
			// This means that Screen.width, Screen.height, Screen.orientation and Camera.aspect may be incorrect.
			// As a result we're overriding camera aspect based on auto rotation settings.
			float cameraAspect;
			#if !UNITY_EDITOR && UNITY_ANDROID
				// TODO: What do we do if an app needs to support both landscape and portrait variations?

				// Is variation of landscape orientation
				if (Screen.autorotateToLandscapeLeft || Screen.autorotateToLandscapeRight) {
					cameraAspect = Mathf.Max(this.Camera.aspect, 1 / this.Camera.aspect);
				// Is variation of portrait orientation
				} else if (Screen.autorotateToPortrait || Screen.autorotateToPortraitUpsideDown) {
					cameraAspect = Mathf.Min(this.Camera.aspect, 1 / this.Camera.aspect);
				// Screen.orientation == ScreenOrientation.Unknown
				} else {
					cameraAspect = this.Camera.aspect;
					Debug.LogWarning("Invalid auto rotation setting, camera aspect may not be correct.", this);
				}
			#else
				cameraAspect = this.Camera.aspect;
			#endif

			Vector3 extents;
			extents = this.Camera.orthographicSize * ((useCameraAspect ? cameraAspect : this.Aspect) * Vector3.right + Vector3.up);
			return new Bounds(center, 2 * extents);
			
		}
		
		
		//
		// Camera
		//
		protected Camera GetCamera() {
			
			Camera camera;
			camera = GetComponentInChildren<Camera>();
			
			if (camera == null) {
				
				Transform transform;
				transform = new GameObject("Camera").transform;
				transform.localPosition = 10 * Vector3.back;
				transform.parent = this.Transform;
				
				camera = transform.gameObject.AddComponent<Camera>();
				camera.nearClipPlane = 1;
				
			}
			
			return camera;
			
		}
		
		protected CameraClearFlags GetCameraClearFlags() {
			return this.BackgroundOpaque ? CameraClearFlags.SolidColor : CameraClearFlags.Depth;
		}
		
		protected float GetCameraSize() {
			return 0.5f * this.Size.y;
		}
		
		protected Vector3 GetCameraPosition() {
			Vector3 position;
			position = this.Bounds.center;
			position.z = this.Camera.transform.position.z;
			return position;
		}


		//
		// Editor Helper
		//
		protected bool IsPrefabAsset {
			get {

				bool result;
				result = false;

				#if UNITY_EDITOR

				result = UnityEditor.PrefabUtility.GetPrefabType(this.gameObject) == UnityEditor.PrefabType.Prefab;

				#endif

				return result;

			}
		}

		
	}
	
}
