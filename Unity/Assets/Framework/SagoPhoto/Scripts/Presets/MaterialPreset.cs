namespace SagoPhoto.Presets {

	using UnityEngine;

	/// <summary>
	/// Processes the view area using the material set by the user.
	/// </summary>
	public class MaterialPreset : IPhotoPreset {


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SagoPhoto.MultiPreset"/> class defaulting
		/// to a high resolution (1024 x 1024).
		/// </summary>
		public MaterialPreset() : this(PhotoPresetResolution.High) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SagoPhoto.CameraRollPhotoPreset"/> class using
		/// the default width and height values for the specified resolution.
		/// </summary>
		public MaterialPreset(PhotoPresetResolution resolution) {
			this.PhotoWidth = resolution.GetWidth();
			this.PhotoHeight = resolution.GetHeight();
		}

		#endregion


		#region Public Properties

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
		/// Gets or sets the shader used to postprocess the image.
		/// </summary>
		public Material Material {
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

			if (Material) {

				RenderTexture temp = RenderTexture.GetTemporary(renderTexture.width, renderTexture.height);
				Graphics.Blit(renderTexture, temp, Material);
				Graphics.Blit(temp, renderTexture);
				RenderTexture.ReleaseTemporary(temp);
				RenderTexture.active = null;

			} else {

				Debug.LogError("Material hasn't been set on the MaterialPreset!");

			}

		}

		#endregion


	}

}