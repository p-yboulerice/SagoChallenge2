namespace SagoBiz {
	
	using UnityEngine;
	using System.Collections;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// Service debug.
	/// </summary>
	public static class ServiceDebug {

		private static string playerPrefsConfigUrl = "SagoBizConfigUrl";
		private static string playerPrefsPromoUrl = "SagoBizPromoUrl";
		
		// public static string ConfigUrlDefault = "http://crosspromo-api-dev.sagosago.com/api/appList";
		// public static string ConfigUrlDefault = "http://192.168.1.106:1337/applist";
		// public static string ConfigUrlDefault = "http://45.55.145.14:1337/applist";

		// Using loopback IP address as a default.
		public static string ConfigUrlDefault = "https://127.0.0.1:8000/appList";
		
		public static string ConfigUrl {
			get {
				if (PlayerPrefs.HasKey(playerPrefsConfigUrl)) {
					return PlayerPrefs.GetString(playerPrefsConfigUrl);
				} else {
					return ConfigUrlDefault;
				}
			}
			set {
				PlayerPrefs.SetString(playerPrefsConfigUrl, value);
			}
		}
		

		// public static string PromoUrlDefault = "http://crosspromo-api-dev.sagosago.com/api/titleTile";
		// public static string PromoUrlDefault = "http://192.168.1.106:1337/titleitem";
		// public static string PromoUrlDefault = "http://45.55.145.14:1337/titleitem";

		// Using loopback IP adress as a default.
		public static string PromoUrlDefault = "https://127.0.0.1:8000/api/titleTile";

		/// <summary>
		/// The promo URL.
		/// </summary>
		public static string PromoUrl {
			get {
				if (PlayerPrefs.HasKey(playerPrefsPromoUrl)) {
					return PlayerPrefs.GetString(playerPrefsPromoUrl);
				} else {
					return PromoUrlDefault;
				}
			}
			set {
				PlayerPrefs.SetString(playerPrefsPromoUrl, value);
			}
		}

	}
	
}