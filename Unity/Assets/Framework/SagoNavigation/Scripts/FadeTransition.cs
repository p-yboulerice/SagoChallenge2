namespace SagoNavigation {
	
	using SagoCore.Resources;
	using System.Collections;
	using UnityEngine;

	public class FadeTransition : SceneTransition {
		
		// 
		// Factory Methods
		// 
		public static FadeTransition Create() {
			
			// guid: SagoNavigation/Resources/SagoNavigation/FadeTransition.prefab
			
			ResourceReference reference;
			reference = ScriptableObject.CreateInstance<ResourceReference>();
			reference.Guid = "e351f7bdd660e4972aca465aecbb0b37";
			
			ResourceReferenceLoaderRequest request;
			request = ResourceReferenceLoader.Load(reference, typeof(GameObject));
			
			GameObject instance;
			instance = Object.Instantiate(request.asset) as GameObject;
			
			return instance ? instance.GetComponent<FadeTransition>() : null;
			
		}
		
		
		// 
		// Inspector Properties
		// 
		public Material Material;
		
		
		//
		// Properties
		//
		protected Camera Camera {
			get {
				m_Camera = m_Camera ? m_Camera : CreateCamera();
				return m_Camera;
			}
		}
		
		protected FadeTransitionVeil Veil {
			get {
				m_Veil = m_Veil ? m_Veil : CreateVeil();
				return m_Veil;
			}
		}
		
		
		//
		// Member Variables
		//
		protected Camera m_Camera;
		protected FadeTransitionVeil m_Veil;
		
		
		//
		// MonoBehaviour
		//
		override protected void Start() {
			base.Start();
			this.Veil.Init();
		}
		
		
		//
		// Public Methods
		//
		override public IEnumerator Run() {
			gameObject.SetActive(true);
			yield return StartCoroutine(this.Veil.Run());
		}
		
		
		//
		// ISceneTransitionObserver
		//
		override public void OnSceneDidTransitionOut(SceneController sceneController, SceneTransition transition) {
			if (this.Direction == SceneTransition.Directions.In) {
				gameObject.SetActive(true);
			}
			if (sceneController) {
				sceneController.Content.SetActive(false);
			}
		}
		
		override public void OnSceneWillTransitionIn(SceneController sceneController, SceneTransition transition) {
			if (this.Direction == SceneTransition.Directions.Out) {
				gameObject.SetActive(false);
			}
			if (sceneController) {
				sceneController.Content.SetActive(true);
			}
		}

		
		//
		// Camera
		//
		protected Camera CreateCamera() {
			
			Transform transform;
			transform = new GameObject("Camera").transform;
			transform.parent = this.Transform;
			
			Camera camera;
			camera = transform.gameObject.AddComponent<Camera>();
			camera.clearFlags = CameraClearFlags.Depth;
			camera.cullingMask = 1 << this.Layer;
			camera.depth = this.CameraDepth;
			camera.farClipPlane = 2;
			camera.nearClipPlane = 1;
			camera.orthographic = true;
			camera.orthographicSize = 1;
			
			return camera;
			
		}
		
		
		//
		// Veil
		//
		protected FadeTransitionVeil CreateVeil() {
			
			Vector3 position;
			position = this.Camera.transform.position + this.Camera.nearClipPlane * Vector3.forward;
			
			Vector3 localScale;
			localScale = Mathf.Max(2, this.Camera.aspect) * this.Camera.orthographicSize * Vector3.one;
			
			Transform transform;
			transform = new GameObject("Fade").transform;
			transform.gameObject.layer = this.Layer;
			transform.parent = this.Transform;
			transform.position = position;
			transform.localScale = localScale;
			
			FadeTransitionVeil veil;
			veil = transform.gameObject.AddComponent<FadeTransitionVeil>();
			veil.Duration = this.Duration;
			veil.Renderer.material = this.Material;
			veil.From = (this.Direction == SceneTransition.Directions.In) ? 1 : 0;
			veil.To = (this.Direction == SceneTransition.Directions.Out) ? 1 : 0;
			
			return veil;
			
		}
		
	}
	
}


namespace SagoNavigation {

	using System.Collections;
	using UnityEngine;
	using UnityEngine.Rendering;
	
	public class FadeTransitionVeil : MonoBehaviour {
		
		
		//
		// Static Mesh
		//
		static protected Mesh Mesh {
			get {
				s_Mesh = s_Mesh ? s_Mesh : CreateMesh();
				return s_Mesh;
			}
		}
		
		static protected Mesh s_Mesh;
		
		static protected Mesh CreateMesh() {
			
			Vector3 p0;
			p0 = new Vector3(-1, -1, 0);
			
			Vector3 p1;
			p1 = p0 + 2 * Vector3.up;
			
			Vector3 p2;
			p2 = p1 + 2 * Vector3.right;
			
			Vector3 p3;
			p3 = p2 + 2 * Vector3.down;
			
			Mesh mesh;
			mesh = new Mesh();
			mesh.Clear();
			mesh.vertices = new Vector3[] { p0, p1, p2, p3 };
			mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			
			return mesh;
			
		}
		
		
		//
		// Properties
		//
		public float Duration {
			get;
			set;
		}
		
		public float From {
			get { return m_From; }
			set {
				m_From = value;
				
				Color color;
				color = Renderer.material.color;
				color.a = value;
				
				Renderer.material.color = color;
			}
		}
		
		public MeshRenderer Renderer {
			get {
				m_Renderer = (m_Renderer != null) ? m_Renderer : AddMeshComponents();
				return m_Renderer;
			}
		}
		
		public float To {
			get;
			set;
		}
		
		
		//
		// Member Variables
		//
		protected float m_From;
		protected MeshRenderer m_Renderer;
		
		
		//
		// Public Methods
		//
		public void Init() {
		}
		
		public IEnumerator Run() {
			
			Material material;
			material = this.Renderer.material;
            
			Color color;
			color = material.color;
			color.a = this.From;
			material.color = color;
			
			float elapsed;
			elapsed = 0;
			
			float t;
			t = 0;
			
			float maxDeltaTime;
			maxDeltaTime = 0.1f;
			
			do {
				elapsed += Mathf.Min(Time.deltaTime, maxDeltaTime);
				t = (this.Duration > 0) ? elapsed / this.Duration : 1;
				color.a = SinEaseInOut(t, this.From, this.To);
				material.color = color;
				yield return null;
            } while (elapsed < this.Duration);
			
		}
		
		
		//
		// MeshRenderer
		//
		protected MeshRenderer AddMeshComponents() {
			
			MeshFilter meshFilter;
			meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = FadeTransitionVeil.Mesh;
			
			MeshRenderer renderer;
			renderer = gameObject.AddComponent<MeshRenderer>();
			renderer.shadowCastingMode = ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			renderer.lightProbeUsage = LightProbeUsage.Off;
			renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			
			return renderer;
			
		}
		
		
		//
		// Easing
		//
		protected float SinEaseInOut(float t, float a, float b) {
			return 0.5f * (1 + (a - b) * Mathf.Cos(Mathf.PI * Mathf.Clamp(t, 0, 1)));
		}
		
		
	}

}


