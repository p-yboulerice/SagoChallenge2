namespace SagoPhoto {
	
	using System.Collections;
	using UnityEngine;
	
	/// <summary>
	/// Used to capture a texture from a camera and apply post-processing effects.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public class PhotoCamera : MonoBehaviour {
		
		
		#region Public Properties
		
		virtual public Camera Camera {
			get {
				m_Camera = m_Camera ?? GetComponent<Camera>();
				return m_Camera;
			}
		}
		
		virtual public Transform Transform {
			get {
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}
		
		#endregion
		
		
		#region Public Methods
		
		/// <summary>
		/// Renders the camera to a texture that will be sent to the provided callback
		/// the next frame.
		/// </summary>
		/// <param name="widthInPixels">Width in pixels.</param>
		/// <param name="heightInPixels">Height in pixels.</param>
		/// <param name="onPostRender">Specified method will be executed during OnPostRender.</param>
		/// <param name="onComplete">Completion callback.</param>
		public void TakePhoto(int widthInPixels, int heightInPixels, System.Action<RenderTexture,Transform> onPostRender, System.Action<Texture2D> onComplete) {
			this.TakePhoto(widthInPixels, heightInPixels, 8, onPostRender, onComplete);
		}
		
		/// <summary>
		/// Renders the camera to a texture that will be sent to the provided callback
		/// the next frame.
		/// </summary>
		/// <param name="sizeInPixels">Size in pixels.</param>
		/// <param name="callback">Callback.</param>
		public void TakePhoto(int sizeInPixels, System.Action<Texture2D> onComplete) {
			TakePhoto(sizeInPixels, sizeInPixels, null, onComplete);
		}

		public void TakePhoto(int widthInPixels, int heightInPixels, int antiAliasing, System.Action<RenderTexture,Transform> onPostRender, System.Action<Texture2D> onComplete) {
			this.OnPostRenderAction = onPostRender;
			this.OnCompleteAction = onComplete;
			this.RenderTarget = GetRenderTexture(widthInPixels, heightInPixels, antiAliasing);

			Render();
		}
		
		#endregion
		
		
		#region MonoBehaviour

		private void Awake() {
			this.Camera.enabled = false;
		}

		virtual protected void OnPostRender() {

			if (OnPostRenderAction != null) {
				OnPostRenderAction(this.RenderTarget, Transform);
			}

			if (this.OnCompleteAction != null) {
				
				int height = this.RenderTarget.height;
				int width = this.RenderTarget.width;

				Texture2D photo = new Texture2D(width, height, TextureFormat.ARGB32, false);
				photo.anisoLevel = 0;
				photo.filterMode = FilterMode.Trilinear;

				RenderTexture aliasedRenderTexture = GetRenderTexture(this.RenderTarget.width, this.RenderTarget.height, 1);
				Graphics.Blit(this.RenderTarget, aliasedRenderTexture);
				RenderTexture.active = aliasedRenderTexture;
				photo.ReadPixels(new Rect(0, 0, width, height), 0, 0);
				photo.Apply();
				RenderTexture.active = null;
				RenderTexture.ReleaseTemporary(aliasedRenderTexture);
				aliasedRenderTexture = null;

				RenderTexture.ReleaseTemporary(this.RenderTarget);
				this.RenderTarget = null;
				
				StartCoroutine(CallbackDelayed(this.OnCompleteAction, photo));

				this.OnCompleteAction = null;
			}
		}
				
		#endregion
		
		
		#region Internal Fields
		
		[System.NonSerialized]
		private Camera m_Camera;
		
		[System.NonSerialized]
		private Transform m_Transform;

		#endregion
		
		
		#region Internal Properties
		
		private System.Action<Texture2D> OnCompleteAction {
			get;
			set;
		}

		private System.Action<RenderTexture,Transform> OnPostRenderAction {
			get;
			set;
		}

		private RenderTexture RenderTarget {
			get;
			set;
		}

		#endregion
		
		
		#region Internal Methods
		
		virtual protected void Render() {

			this.RenderTarget.DiscardContents();
			this.Camera.targetTexture = this.RenderTarget;
			this.Camera.Render();
		}
		
		virtual protected IEnumerator CallbackDelayed(System.Action<Texture2D> callback, Texture2D photo) {
			yield return null;
			callback(photo);
		}
		
		virtual protected RenderTexture GetRenderTexture(int width, int height, int antiAliasing) {
			return RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, antiAliasing);
		}
		
		#endregion
		
		
	}
	
}