namespace SagoPhoto.Presets {

	using UnityEngine;

	/// <summary>
	/// Interface required for all PhotoPresets in order for them to be compatible
	/// with the MultiPreset.
	/// </summary>
	public interface IPhotoPreset {
		
		void OnPostRender(RenderTexture renderTexture, Transform camera);

		int PhotoHeight {
			get;
			set;
		}

		int PhotoWidth {
			get;
			set;
		}

	}

}