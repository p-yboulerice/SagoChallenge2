namespace SagoPhoto.Presets {

	using UnityEngine;

	/// <summary>
	/// Preset used for creating the stock version of the photos
	/// saved to the Camera Roll with a Sago logo imbedded.
	/// </summary>
	public class CameraRollPhotoPreset : IPhotoPreset {


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SagoPhoto.MultiPreset"/> class defaulting
		/// to a high resolution (1024 x 1024).
		/// </summary>
		public CameraRollPhotoPreset() : this(PhotoPresetResolution.High) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SagoPhoto.CameraRollPhotoPreset"/> class using
		/// the default width and height values for the specified resolution.
		/// </summary>
		public CameraRollPhotoPreset(PhotoPresetResolution resolution) {

			this.PhotoWidth = resolution.GetWidth();
			this.PhotoHeight = resolution.GetHeight();

			this.LogoAlignment = TextAnchor.LowerRight;
			this.LogoScale = .29f;
			this.LogoPadding = new Vector2(.05f, .05f);

		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Where the logo will be placed on the screen.
		/// </summary>
		public TextAnchor LogoAlignment {
			get;
			set;
		}

		/// <summary>
		/// Scale of the logo relative to the screen.
		/// </summary>
		public float LogoScale {
			get;
			set;
		}

		/// <summary>
		/// Width of the photo.
		/// </summary>
		public int PhotoWidth {
			get;
			set;
		}

		/// <summary>
		/// Height of the photo.
		/// </summary>
		public int PhotoHeight {
			get;
			set;
		}

		/// <summary>
		/// Padding of the logo relative to the screen.
		/// </summary>
		public Vector2 LogoPadding {
			get;
			set;
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Set as the onPostRender parameter when using the PhotoCamera.TakePhoto method.
		/// </summary>
		public void OnPostRender(RenderTexture renderTexture, Transform camera) {

			if (renderTexture.width != PhotoWidth || renderTexture.height != PhotoHeight) {
				Debug.LogWarning("Render textures width or height does not match the preset.");
			}

			if (LogoMesh != null && LogoMaterial != null) {

				// set the active render texture
				RenderTexture.active = renderTexture;

				// draw the preview
				Graphics.SetRenderTarget(renderTexture);

				GL.PushMatrix();
				LogoMaterial.SetPass(0);
				GL.LoadPixelMatrix();

				Matrix4x4 matrix = Matrix4x4.TRS(
					                   camera.TransformPoint(GetLogoPosition(this.LogoScale)),
					                   Quaternion.identity,
					                   new Vector3 (this.LogoScale, this.LogoScale, this.LogoScale) * LogoSizeToPhotoRatio
				                   );

				Graphics.DrawMeshNow(LogoMesh, matrix);

				GL.PopMatrix();

				// release the render texture
				RenderTexture.active = null;

			} else {
				Debug.LogError("Mesh or material couldn't be found.");
			}

		}

		#endregion


		#region Private Properties

		private Material LogoMaterial {
			get { return Resources.Load<Material>("LogoMaterial"); }
		}

		private Mesh LogoMesh {
			get { return Resources.Load<Mesh>("LogoMesh"); }
		}

		private float LogoSizeToPhotoRatio {
			get { return PhotoWidth / LogoMesh.bounds.size.x; }
		}

		#endregion


		#region Privates Methods

		private Vector3 GetLogoPosition(float scale) {

			float meshWidth = LogoMesh.bounds.size.x * LogoSizeToPhotoRatio * scale;
			float meshHeight = LogoMesh.bounds.size.y * LogoSizeToPhotoRatio * scale;
			Vector2 padding = new Vector2(PhotoWidth * LogoPadding.x, PhotoHeight * LogoPadding.y);

			switch (LogoAlignment) {

			case TextAnchor.LowerCenter:
				return new Vector3(PhotoWidth * .5f, meshHeight * .5f + padding.y, 0);
			case TextAnchor.LowerLeft:
				return new Vector3(meshWidth * .5f + padding.x, meshHeight * .5f + padding.y, 0);
			case TextAnchor.LowerRight:
				return new Vector3(PhotoWidth - meshWidth * .5f - padding.x, meshHeight * .5f + padding.y, 0);
			case TextAnchor.MiddleCenter:
				return new Vector3 (PhotoWidth * 0.5f, PhotoHeight * 0.5f, 0);
			case TextAnchor.MiddleLeft:
				return new Vector3 (meshWidth * .5f + padding.x, PhotoHeight * .5f, 0);
			case TextAnchor.MiddleRight:
				return new Vector3 (PhotoWidth - meshWidth * .5f - padding.x, PhotoHeight * .5f, 0);
			case TextAnchor.UpperCenter:
				return new Vector3 (PhotoWidth * .5f, this.PhotoHeight - meshHeight * .5f - padding.y, 0);
			case TextAnchor.UpperLeft:
				return new Vector3 (meshWidth * .5f + padding.x, PhotoHeight - meshHeight * .5f - padding.y, 0);
			case TextAnchor.UpperRight:
				return new Vector3 (PhotoWidth - meshWidth * .5f - padding.x, PhotoHeight - meshHeight * .5f - padding.y, 0);
			default:
				return Vector3.zero;

			}

		}

		#endregion


	}

}