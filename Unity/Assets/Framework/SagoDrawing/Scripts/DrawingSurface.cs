namespace SagoDrawing {

	using System.Collections;
	using UnityEngine;
		
	/// <summary>
	/// The DrawingSurface class owns the render texture used for drawing.
	/// </summary>
	public class DrawingSurface : MonoBehaviour {
		

		#region Types

		[System.Serializable]
		public enum StartMode {
			InitializeSurfaceAndRenderTexture,
			InitializeSurface,
			None
		}

		#endregion


		#region Fields
		
		[SerializeField]
		private Camera m_Camera;
		
		[SerializeField]
		private Color m_Color;

		[SerializeField]
		protected StartMode m_StartMode;

		[System.NonSerialized]
		private Renderer m_Renderer;
		
		[System.NonSerialized]
		private Transform m_Transform;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// The camera used to calculate the size of the render texture.
		/// </summary>
		public Camera Camera {
			get { return m_Camera; }
			set { m_Camera = value; }
		}
		
		/// <summary>
		/// The initial color of the render texture.
		/// </summary>
		public Color Color {
			get { return m_Color; }
			set { m_Color = value; }
		}
		
		/// <summary>
		/// The renderer that will display the render texture.
		/// </summary>
		public Renderer Renderer {
			get { return m_Renderer = m_Renderer ?? GetComponent<Renderer>(); }
		}
		
		/// <summary>
		/// The render texture.
		/// </summary>
		public RenderTexture RenderTexture {
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets how this should behave at Start()
		/// </summary>
		public StartMode StartupMode {
			get { return m_StartMode; }
			set { m_StartMode = value; }
		}

		/// <summary>
		/// The temporary render texture, used to save and restore the content of the render texture when the app pauses on Android.
		/// </summary>
		private Texture2D TemporaryTexture {
			get;
			set;
		}
		
		/// <summary>
		/// The transform (cached).
		/// </summary>
		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Makes the render texture active, executes the block, then restores the previously active render texture.
		/// </summary>
		public void Invoke(System.Action block) {
			
			Debug.AssertFormat(
				RenderTexture != null, 
				"Cannot invoke block: {0}", 
				"Render texture is null"
			);
			
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = RenderTexture;
			if (block != null) {
				block();
			}
			RenderTexture.active = previous;
		}
		
		/// <summary>
		/// Clears the render texture to the initial color.
		/// </summary>
		public void Clear() {
			if (RenderTexture != null) {
				Invoke(() => {
					GL.Clear(true, true, Color);
				});
			}
		}

		/// <summary>
		/// Saves a Texture2D from the RenderTexture.
		/// </summary>
		/// <returns>The saved Texture2D.</returns>
		public Texture2D SaveToTexture2D() {

			Texture2D texture = null;

			if (RenderTexture != null) {
				Invoke(() => {
					texture = new Texture2D(RenderTexture.width, RenderTexture.height, TextureFormat.RGB24, false);
					texture.ReadPixels(new Rect(0,0,RenderTexture.width,RenderTexture.height), 0, 0);
					texture.Apply();
				});
			}
		
			return texture;

		}
		
		#endregion
		
		
		#region MonoBehaviour Methods
		
		private IEnumerator OnApplicationPause(bool isPaused) {
			#if UNITY_ANDROID
				if (isPaused) {
					SaveToTemporaryTexture();
				} else {
					yield return null;
					RestoreFromTemporaryTexture();
				}
			#endif
			yield break;
		}
		
		private void OnDestroy() {
			DestroyRenderTexture();
		}
		
		private void Reset() {
			m_Color = Color.white;
		}
		
		private void Start() {
			switch (this.StartupMode) {

			case StartMode.InitializeSurfaceAndRenderTexture:
				InitializeSurfaceAndRenderTexture();
				break;
			
			case StartMode.InitializeSurface:
				InitializeSurface();
				break;

			default:
				break;

			}
		}
		
		#endregion
		
		
		#region RenderTexture Methods

		/// <summary>
		/// Performs all initialization.
		/// (previously CreateRenderTexture())
		/// </summary>
		public void InitializeSurfaceAndRenderTexture() {
			if (this.Camera) {
				InitializeSurface();
				CreateRenderTexture();
			}
		}

		/// <summary>
		/// Performs all initialization, other than creating the RenderTexture.
		/// </summary>
		public void InitializeSurface() {
			if (this.Camera) {
				Transform.localScale = (
					(Vector3.right * Camera.aspect * Camera.orthographicSize * 2) + 
					(Vector3.up * Camera.orthographicSize * 2) + 
					(Vector3.forward)
				);
			} else {
				Debug.LogWarning("Cannot initialize DrawingSurface without a camera", this);
			}
		}

		public void CreateRenderTexture() {
			
			Debug.AssertFormat(
				RenderTexture == null, 
				"Could create render texture: {0}", 
				"Render texture is not null"
			);
			
			Debug.AssertFormat(
				Camera != null, 
				"Could create render texture: {0}", 
				"Camera is null"
			);
			
			Debug.AssertFormat(
				Renderer != null,
				"Could create render texture: {0}", 
				"Renderer is null"
			);

			RenderTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
			RenderTexture.antiAliasing = 1;
			RenderTexture.isPowerOfTwo = false;
			RenderTexture.Create();
			
			MaterialPropertyBlock block;
			block = new MaterialPropertyBlock();
			block.SetTexture("_MainTex", RenderTexture);
			Renderer.SetPropertyBlock(block);
			
			Clear();
			
		}
		
		public void DestroyRenderTexture() {
			if (RenderTexture != null) {
				RenderTexture.DiscardContents();
				RenderTexture.Release();
				Destroy(RenderTexture);
				RenderTexture = null;
			}
		}
		
		private void RestoreFromTemporaryTexture() {
			if (TemporaryTexture != null) {
				Graphics.Blit(TemporaryTexture, RenderTexture);
				Destroy(TemporaryTexture);
				TemporaryTexture = null;
			}
		}
		
		private void SaveToTemporaryTexture() {
			
			Debug.AssertFormat(
				RenderTexture != null, 
				"Could not save to temporary texture: {0}", 
				"Render texture is null"
			);
			
			Debug.AssertFormat(
				TemporaryTexture == null, 
				"Could not save to temporary texture: {0}", 
				"Temporary texture is not null"
			);
			
			Invoke(() => {
				TemporaryTexture = new Texture2D(RenderTexture.width, RenderTexture.height, TextureFormat.ARGB32, false);
				TemporaryTexture.ReadPixels(new Rect(0,0,RenderTexture.width,RenderTexture.height), 0, 0);
				TemporaryTexture.Apply();
			});
			
		}

		#endregion
		
		
	}
	
}