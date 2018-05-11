namespace SagoApp {
	
	using System.Collections;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using UnityEngine;
	using SagoUtils;

	public class App {

		//[JsonProperty("url_scheme")]
		public string UrlScheme;

		//[JsonProperty("installed")]
		public bool Installed;

	}

	/// <summary>
	/// Native bindings for SAInstalledAppsHelper.
	/// </summary>
	public class SAInstalledAppsHelper : MonoBehaviour {


		#region SAInstalledAppsHelper binding

		#if SAGO_IOS && !UNITY_EDITOR

		[DllImport ("__Internal")]
		public static extern string _GetInstalledApps(string appCodeJSON);

		#endif

		#endregion


		#region Public methods

		public static List<App> CheckAppsAvailability(List<App> apps) {
			#if SAGO_IOS && !UNITY_EDITOR
			string appArray = JsonConvert.SerializeObject(apps, JsonConvert.Formatting.None, JsonConvert.Settings.CSharpToJson);

			string result = _GetInstalledApps(appArray);
			List<App> returnedList = JsonConvert.DeserializeObject<List<App>>(result, JsonConvert.Settings.CSharpToJson);

			return returnedList;
			#else
			return apps;
			#endif
		}

		#endregion

	}
	
}