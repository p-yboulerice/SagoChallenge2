namespace SagoPlatform {
	
	using System.Collections.Generic;
	using System.Linq;
	
	/// <summary>
	/// Platform defines the platforms supported by Sago Sago.
	/// NOTE: Be very careful about changing the names of these platforms. String versions of these enums are serialized
	/// to strings in a number of situations (Ex: Server calls)
	/// </summary>
	public enum Platform {
		
		/// <summary>
		/// The default platform.
		/// </summary>
		Unknown = 0,
		
		/// <summary>
		/// iOS
		/// </summary>
		iOS = 1,
		
		/// <summary>
		/// Google Play
		/// </summary>
		GooglePlay = 2,
		
		/// <summary>
		/// Kindle
		/// </summary>
		Kindle = 3,
		
		/// <summary>
		/// Kindle Free Time
		/// </summary>
		KindleFreeTime = 4,
		
		/// <summary>
		/// Windows Phone
		/// </summary>
		WindowsPhone = 5,
		
		/// <summary>
		/// Windows Store
		/// </summary>
		WindowsStore = 6,

		/// <summary>
		/// Nabi
		/// </summary>
		Nabi = 7,

		/// <summary>
		/// Amazon Teal
		/// </summary>
		AmazonTeal = 8,

		/// <summary>
		/// Samsung Milk.
		/// </summary>
		SamsungMilk = 9,
		
		/// <summary>
		/// Google Play Free
		/// </summary>
		GooglePlayFree = 10,
		
		/// <summary>
		/// Smart Education
		/// </summary>
		SmartEducation = 11,
		
		/// <summary>
		/// Apple TV
		/// </summary>
		tvOS = 12,

		/// <summary>
		/// Bemobi
		/// </summary>
		Bemobi = 13,

		/// <summary>
		/// Thales
		/// </summary>
		Thales = 14,

		/// <summary>
		/// Panasonic Ex
		/// </summary>
		PanasonicEx = 15
		
	}
	
	/// <summary>
	/// The PlatformExtensions class provides extension methods for the <see cref="Platform" /> enum.
	/// </summary>
	public static class PlatformExtensions {
		private const string SAGO_PREFIX = "sago";
		
		/// <summary>
		/// The dictionary used to map platforms to define symbols.
		/// </summary>
		private static readonly Dictionary<Platform,string> DefineSymbolMap = new Dictionary<Platform,string> {
			{ Platform.Unknown, string.Empty },
			{ Platform.iOS, "SAGO_IOS" },
			{ Platform.GooglePlay, "SAGO_GOOGLE_PLAY" },
			{ Platform.GooglePlayFree, "SAGO_GOOGLE_PLAY_FREE" },
			{ Platform.Kindle, "SAGO_KINDLE" },
			{ Platform.KindleFreeTime, "SAGO_KINDLE_FREE_TIME" },
			{ Platform.WindowsStore, "SAGO_WINDOWS_STORE" },
			{ Platform.Nabi, "SAGO_NABI" },
			{ Platform.AmazonTeal, "SAGO_AMAZON_TEAL" },
			{ Platform.SamsungMilk, "SAGO_SAMSUNG_MILK" },
			{ Platform.SmartEducation, "SAGO_SMART_EDUCATION" },
			{ Platform.tvOS, "SAGO_TVOS" },
			{ Platform.Bemobi, "SAGO_BEMOBI" },
			{ Platform.Thales, "SAGO_THALES" },
			{ Platform.PanasonicEx, "SAGO_PANASONIC_EX" }
		};
		
		/// <summary>
		/// Gets the define symbol for the specified platform (or the default 
		/// define symbol if no mapping exists).
		/// </summary>
		public static string ToDefineSymbol(this Platform platform) {
			return (
				DefineSymbolMap.ContainsKey(platform) ? 
				DefineSymbolMap[platform] : 
				DefineSymbolMap[Platform.Unknown]
			);
		}

		/// <summary>
		/// Converts a <see cref="Platform" /> enum to sago unique string representation of that plaform.
		/// </summary>
		/// <remarks>
		/// This is to avoid situations where plugin libraries or directories have the same name based off 
		/// the exact platform name. In some cases Unity will fail to build because native plugin directories 
		/// and native libraries have the same name in a project
		/// </remarks>
		/// <param name="platform">The platform to convert</param>
		/// <returns>The platform string unique to the Sago project</returns>
		public static string ToSagoUniqueString(this Platform platform) {
			return SAGO_PREFIX + platform.ToString();
		}
	}
	
}