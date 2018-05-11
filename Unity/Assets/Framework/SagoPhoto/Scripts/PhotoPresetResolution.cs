namespace SagoPhoto.Presets {

	/// <summary>
	/// Standard resolutions for the PhotoPresets.
	/// HighRes == 1024 * 1024
	/// LowRes == 512 * 512
	/// </summary>
	public enum PhotoPresetResolution {
		High,
		Low
	}

	/// <summary>
	/// Extension to get the standard resolutions for the PhotoPresetResolution enum.
	/// </summary>
	public static class PhotoPresetResolutionExtensions {

		/// <summary>
		/// Gets standard width for the specified resolution.
		/// </summary>
		public static int GetWidth(this PhotoPresetResolution resolution) {
			return resolution == PhotoPresetResolution.High ? 1024 : 512;
		}

		/// <summary>
		/// Gets standard height for the specified resolution.
		/// </summary>
		public static int GetHeight(this PhotoPresetResolution resolution) {
			return resolution == PhotoPresetResolution.High ? 1024 : 512;
		}

	}

}