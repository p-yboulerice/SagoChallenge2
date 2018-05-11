namespace SagoPhoto.Presets {

	using UnityEngine;
	using System.Collections.Generic;

	/// <summary>
	/// Preset used for chaining multiple presets together. PhotoWidth & PhotoHeight of all the
	/// chained presets will be set to the PhotoWidth & PhotoHeight of the MultiPreset, and the
	/// OnPosterRender methods will be executed in the order they're passed in.
	/// </summary>
	public class MultiPreset {


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SagoPhoto.MultiPreset"/> class defaulting
		/// to a high resolution (1024 x 1024).
		/// </summary>
		public MultiPreset() : this(PhotoPresetResolution.High) {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SagoPhoto.CameraRollPhotoPreset"/> class using
		/// the default width and height values for the specified resolution.
		/// </summary>
		public MultiPreset(PhotoPresetResolution resolution) {
			this.PhotoWidth = resolution.GetWidth();
			this.PhotoHeight = resolution.GetHeight();
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// A list of presets that will be executed in
		/// order by the MultiPreset's OnPostRender method.
		/// </summary>
		public List<IPhotoPreset> Presets {
			get {
				m_Presets = m_Presets ?? new List<IPhotoPreset>();
				return m_Presets;
			}
		}

		/// <summary>
		/// Width of the photo. Propagates to all presets in the Presets list.
		/// </summary>
		public int PhotoWidth {
			get;
			set;
		}

		/// <summary>
		/// Height of the photo. Propagates to all presets in the Presets list.
		/// </summary>
		public int PhotoHeight {
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

			foreach (IPhotoPreset preset in Presets) {
				preset.PhotoWidth = this.PhotoWidth;
				preset.PhotoHeight = this.PhotoHeight;
				preset.OnPostRender(renderTexture, camera);
			}

		}

		#endregion


		#region Fields

		[System.NonSerialized]
		private List<IPhotoPreset> m_Presets;

		#endregion


	}

}