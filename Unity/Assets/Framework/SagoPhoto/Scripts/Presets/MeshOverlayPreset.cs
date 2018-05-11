namespace SagoPhoto.Presets {

	using UnityEngine;

	/// <summary>
	/// Preset used creating photos with a mesh overlayed overtop the view area.
	/// </summary>
	public class MeshOverlayPreset : IPhotoPreset {


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SagoPhoto.MultiPreset"/> class defaulting
		/// to a high resolution (1024 x 1024).
		/// </summary>
		public MeshOverlayPreset() : this(PhotoPresetResolution.High) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SagoPhoto.CameraRollPhotoPreset"/> class using
		/// the default width and height values for the specified resolution.
		/// </summary>
		public MeshOverlayPreset(PhotoPresetResolution resolution) {

			this.PhotoWidth = resolution.GetWidth();
			this.PhotoHeight = resolution.GetHeight();

			this.MeshAlignment = TextAnchor.MiddleCenter;
			this.MeshScale = 1f;
			this.Padding = .05f;

		}

		#endregion


		#region Public Properties

		/// <summary>
		/// Where the logo will be placed on the screen.
		/// </summary>
		public TextAnchor MeshAlignment {
			get;
			set;
		}

		/// <summary>
		/// Scale of the logo relative to the screen.
		/// </summary>
		public float MeshScale {
			get;
			set;
		}

		/// <summary>
		/// Width of the photo.
		/// </summary>
		public int PhotoWidth {
			get {
				return (CropToMesh) ? (int)(Mesh.bounds.size.x * MeshSizeToPhotoRatio * MeshScale) : m_PhotoWidth;
			}
			set {
				m_PhotoWidth = value;
			}
		}

		/// <summary>
		/// Height of the photo.
		/// </summary>
		public int PhotoHeight {
			get {
				return (CropToMesh) ? (int)(Mesh.bounds.size.y * MeshSizeToPhotoRatio * MeshScale) : m_PhotoHeight;
			}
			set {
				m_PhotoHeight = value;
			}
		}

		/// <summary>
		/// Padding of the logo relative to the screen.
		/// </summary>
		public float Padding {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the material use to render the mesh.
		/// </summary>
		public Material Material {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the mesh you want to overlay on the photo.
		/// </summary>
		public Mesh Mesh {
			get;
			set;
		}

		/// <summary>
		/// If set to true, will automatically set the PhotoWidth & PhotoHeight
		/// properties such that they match the size of the mesh.
		/// </summary>
		public bool CropToMesh {
			get;
			set;
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Set as the onPostRender parameter when using the PhotoCamera.TakePhoto method.
		/// </summary>
		public void OnPostRender(RenderTexture renderTexture, Transform camera) {

			if (renderTexture.width != m_PhotoWidth || renderTexture.height != m_PhotoHeight) {
				Debug.LogWarning("Render textures width or height does not match the preset.");
			}

			if (Mesh != null && Material != null) {

				// set the active render texture
				RenderTexture.active = renderTexture;

				// draw the preview
				Graphics.SetRenderTarget(renderTexture);

				GL.PushMatrix();
				Material.SetPass(0);
				GL.LoadPixelMatrix();

				Matrix4x4 matrix = Matrix4x4.TRS(
					                   camera.TransformPoint(GetMeshPosition(this.MeshScale)),
					                   Quaternion.identity,
					                   new Vector3 (this.MeshScale, this.MeshScale, this.MeshScale) * MeshSizeToPhotoRatio
				                   );

				Graphics.DrawMeshNow(Mesh, matrix);

				GL.PopMatrix();

				// release the render texture
				RenderTexture.active = null;

			} else {
				Debug.LogError("Mesh or material hasn't been set.");
			}

		}

		#endregion


		#region Fields

		[System.NonSerialized]
		private int m_PhotoWidth;

		[System.NonSerialized]
		private int m_PhotoHeight;

		#endregion


		#region Private Properties

		private float MeshSizeToPhotoRatio {
			get { return m_PhotoWidth / Mesh.bounds.size.x; }
		}

		#endregion


		#region Privates Methods

		private Vector3 GetMeshPosition(float scale) {

			float meshWidth = Mesh.bounds.size.x * MeshSizeToPhotoRatio * scale;
			float meshHeight = Mesh.bounds.size.y * MeshSizeToPhotoRatio * scale;
			float padding = PhotoWidth * Padding;

			switch (MeshAlignment) {

			case TextAnchor.LowerCenter:
				return new Vector3(PhotoWidth * .5f, meshHeight * .5f + padding, 0);
			case TextAnchor.LowerLeft:
				return new Vector3(meshWidth * .5f + padding, meshHeight * .5f + padding, 0);
			case TextAnchor.LowerRight:
				return new Vector3(PhotoWidth - meshWidth * .5f - padding, meshHeight * .5f + padding, 0);
			case TextAnchor.MiddleCenter:
				return new Vector3 (PhotoWidth * 0.5f, PhotoHeight * 0.5f, 0);
			case TextAnchor.MiddleLeft:
				return new Vector3 (meshWidth * .5f + padding, PhotoHeight * .5f, 0);
			case TextAnchor.MiddleRight:
				return new Vector3 (PhotoWidth - meshWidth * .5f - padding, PhotoHeight * .5f, 0);
			case TextAnchor.UpperCenter:
				return new Vector3 (PhotoWidth * .5f, this.PhotoHeight - meshHeight * .5f - padding, 0);
			case TextAnchor.UpperLeft:
				return new Vector3 (meshWidth * .5f + padding, PhotoHeight - meshHeight * .5f - padding, 0);
			case TextAnchor.UpperRight:
				return new Vector3 (PhotoWidth - meshWidth * .5f - padding, PhotoHeight - meshHeight * .5f - padding, 0);
			default:
				return Vector3.zero;

			}

		}

		#endregion


	}

}