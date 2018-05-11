namespace SagoBiz {
	
	using SagoUtils;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.ComponentModel;
	using UnityEngine;
	using Debug = SagoBiz.DebugUtil;
	
	/// <summary>
	/// ServiceMode defines the possible modes for the <see cref="Service" /> component.
	/// </summary>
	public enum ServiceMode {
		/// <summary>The service will not load any data.</summary>
		Disabled,
		/// <summary>
		/// The service will load data from local files in the streaming assets folder.
		/// Service will load data from a local/development server that you setup on your local machine.
		/// </summary>
		Development,
		/// <summary>The service will load data from the development server.</summary>
		Staging,
		/// <summary>The service will load data from the production server.</summary>
		Production
	}
	
	/// <summary>
	/// ServiceState defines the possible states of the <see cref="Service" /> component.
	/// </summary>
	public enum ServiceState {
		/// <summary>The service has not started loading.</summary>
		Unknown,
		/// <summary>The service is loading.</summary>
		Loading,
		/// <summary>The service has finished loading successfully.</summary>
		Complete,
		/// <summary>The service had an error while loading.</summary>
		Error
	}
	
	/// <summary>
	/// The ServiceConfigData class provides a data structure for storing config 
	/// data downloaded from the server and deserialized from json.
	/// </summary>
	public class ServiceConfigData {
		
		
		#region Properties
		
		/// <summary>
		/// Gets and sets the list of url schemes to look for when checking 
		/// which apps are installed on the device.
		/// </summary>
		public List<App> Apps {
			get; set;
		}

		/// <summary>
		/// Gets or sets the cross promo server version.
		/// </summary>
		/// <value>The server version.</value>
		public int WebServiceVersion {
			get; set;
		}
		#endregion
		
		
	}
	
	/// <summary>
	/// The ServicePromoData class provides a data structure for storing promo
	/// data downloaded from the server and deserialized from json.
	/// </summary>
	public class ServicePromoData {
		
		
		#region Properties

		//[JsonProperty("image_url")]
		/// <summary>
		/// The url of the promo image.
		/// </summary>
		public string ImageUrl {
			get; set;
		}

		//[JsonProperty("page_url")]
		/// <summary>
		/// The url of the promo page.
		/// </summary>
		public string PageUrl {
			get; set;
		}

		public string CampaignId {
			get;
			set;
		}

		public string CampaignName {
			get;
			set;
		}

		public string PromoId {
			get;
			set;
		}
		#endregion
		
		
	}

	public class App
	{

		#region Properties

		//[JsonProperty("bundle_id")]
		public string BundleId;

		//[JsonProperty("name")]
		public string Name;

		//[JsonProperty("installed")]
		public bool Installed;

		//[JsonProperty("url_scheme")]
		public string UrlScheme;

		//[JsonProperty("company_name")]
		public string CompanyName;

		#endregion

	}
	
	
	/// <summary>
	/// The Service class provides functionality for loading config and promo data from the server.
	/// </summary>
	public class Service : MonoBehaviour {
		
		
		#region Types
		
		/// <summary>
		/// The method signature for event handlers for the <see cref="Service" /> component.
		/// </summary>
		public delegate void ServiceCallback(Service service);
		
		#endregion
		
		
		#region Events
		
		/// <summary>
		/// Adds and removes event handlers called when the service finishes loading successfully.
		/// </summary>
		public event ServiceCallback OnComplete;
		
		/// <summary>
		/// Adds and removes event handlers called when the service finishes loading with errors.
		/// </summary>
		public event ServiceCallback OnError;
		
		#endregion
		
		
		#region Properties

		public static readonly string CompanyNameSagoSago = "Sago Sago";

		public static readonly string CompanyNameTocaBoca = "Toca Boca";

		public static string CompanyName;

		public static System.Action<System.Action<object>> FetchAppDataDelegate {
			get {
				return _FetchAppDataDelegate;
			}

			set {
				if (value != null) {
					if (_FetchAppDataDelegate != null) {
						UnityEngine.Debug.LogError("[Facade.FetchAppDataDelegate] FetchAppDataDelegate has already been assigned with a delegate and you are trying to assign another one!");
						return;
					}

					System.Delegate[] invocationList = value.GetInvocationList();
					if (invocationList.Length > 1) {
						UnityEngine.Debug.LogError("[Facade.FetchAppDataDelegate] Assigning more than one delegate to FetchAppDataDelagate is not allowed!");
						return;
					}
				}
				_FetchAppDataDelegate = value;
			}
		}

		/// <summary>
		/// Gets generic AppData as object for the current service mode.
		/// </summary>
		public object AppData {
			get; protected set;
		}

		/// <summary>
		/// Gets and sets the data loaded from the <see cref="ConfigUrl" />.
		/// </summary>
		public ServiceConfigData ConfigData {
			get; protected set;
		}
		
		/// <summary>
		/// Gets the url used to load the <see cref="ConfigData" /> for the current service mode.
		/// </summary>
		public string ConfigUrl {
			get {
				switch (this.Mode) {
					case ServiceMode.Development:
						return ServiceDebug.ConfigUrl;
						// return Path.Combine(StreamingAssetsPath, "config.json");
					case ServiceMode.Staging:
						return "https://crosspromo-api-staging.sagosago.com/api/appList";
					case ServiceMode.Production:
						return "https://crosspromo-api.sagosago.com/api/appList";
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the flag that indicates whether the service has finished loading.
		/// </summary>
		public bool IsLoaded {
			get { return this.State == ServiceState.Complete || this.State == ServiceState.Error; }
		}
		
		/// <summary>
		/// Gets and sets the data loaded from the <see cref="PromoUrl" />.
		/// </summary>
		public ServicePromoData PromoData {
			get; protected set;
		}
		
		/// <summary>
		/// Gets the url used to load the <see cref="PromoData" /> for the current service mode.
		/// </summary>
		public string PromoUrl {
			get {
				switch (this.Mode) {
					case ServiceMode.Development:
						return ServiceDebug.PromoUrl;
						// return Path.Combine(StreamingAssetsPath, "promo.json");
					case ServiceMode.Staging:
						return "https://crosspromo-api-staging.sagosago.com/api/titleTile";
						
					case ServiceMode.Production:
						return "https://crosspromo-api.sagosago.com/api/titleTile";
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets and sets the error that occurred while loading.
		/// </summary>
		public string Error {
			get; protected set;
		}
		
		/// <summary>
		/// Gets the current service mode.
		/// </summary>
		public ServiceMode Mode {
			get {
				if (Controller.Instance && Controller.Instance.ServiceOptions) {
					return Controller.Instance.ServiceOptions.Mode;
				}
				return ServiceMode.Disabled;
			}
		}
		
		/// <summary>
		/// Gets and sets the current service state.
		/// </summary>
		public ServiceState State {
			get; protected set;
		}
		
		/// <summary>
		/// Gets the timestamp when the service finished loading.
		/// </summary>
		public float Timestamp {
			get; protected set;
		}
		
		#endregion


		#region Fields

		private static System.Action<System.Action<object>> _FetchAppDataDelegate;

		#endregion
		
		
		#region Public Methods
		
		/// <summary>
		/// Clears all data and resets the service to the default state.
		/// </summary>
		public void Clear() {
			
			// stop
			this.StopAllCoroutines();
			
			// reset
			this.AppData = null;
			this.ConfigData = null;
			this.PromoData = null;
			this.Error = null;
			this.State = ServiceState.Unknown;
			this.Timestamp = 0;
			
		}
		
		/// <summary>
		/// Loads the service.
		/// </summary>
		public void Load() {
			if (this.State == ServiceState.Unknown) {
				this.StartCoroutine(this.LoadAsync());
			}
		}
		
		#endregion
		
		
		#region Coroutine Methods
		
		/// <summary>
		/// Loads the service asynchronously.
		/// </summary>
		protected IEnumerator LoadAsync() {
			
			this.State = ServiceState.Loading;
			this.Timestamp = 0;

			if (FetchAppDataDelegate != null) {
				bool fetchComplete = false;

				FetchAppDataDelegate((appData) => {
					this.AppData = appData;
					fetchComplete = true;
				});

				while (!fetchComplete) {
					yield return null;
				}
			}
			
			if (this.State != ServiceState.Error) {
				yield return this.StartCoroutine(this.LoadConfigDataAsync());
			}
			
			if (this.State != ServiceState.Error) {
				yield return this.StartCoroutine(this.LoadPromoDataAsync());
			}
			
			if (this.State != ServiceState.Error) {
				this.State = ServiceState.Complete;
				this.Timestamp = Time.realtimeSinceStartup;
				if (this.OnComplete != null) {
					this.OnComplete(this);
				}
				yield break;
			}
			
			this.Timestamp = 0;
			if (this.OnError != null) {
				this.OnError(this);
			}
			yield break;
			
		}
		
		/// <summary>
		/// Loads the <see cref="ConfigData" /> asynchronously.
		/// </summary>
		protected IEnumerator LoadConfigDataAsync() {
			
			string url;
			url = this.ConfigUrl;
			if (string.IsNullOrEmpty(url)) {
				this.Error = string.Format("Url is null -> Disabling the service.");
				this.State = ServiceState.Error;
				yield break;
			}

			Native native;
			native = Controller.Instance.Native;

			Dictionary<string,object> data;
			data = new Dictionary<string,object>();
			data.Add("Platform".PascalToUnderscoreCase(), native.Platform.ToString());
			data.Add("SagobizSdkVersion".PascalToUnderscoreCase(), Version.Current);

			Dictionary<string,string> headers;
			headers = new Dictionary<string, string>();
			headers.Add("content-type","application/json");

			string jsonData = JsonConvert.SerializeObject(data, JsonConvert.Formatting.None, JsonConvert.Settings.CSharpToJson);

			byte[] binaryData = System.Text.Encoding.UTF8.GetBytes(jsonData);
	
			Debug.Log ("Config url: " + url);
			WWW www;
			www = new WWW(url, binaryData, headers);
			
			yield return www;
			
			if (!string.IsNullOrEmpty(www.error)) {
				string responseHeader = string.Empty;
				foreach(var key in www.responseHeaders.Keys)
					responseHeader += string.Format("\t{0} = {1}\n", key, www.responseHeaders[key]);
				responseHeader = "{\n" + responseHeader + "}\n";
				this.Error = string.Format("Could not download config data.\n-> Response Header: {0}\n->Actual Error: {1}\nResponse Text: {2}", responseHeader, www.error, string.IsNullOrEmpty(www.text) ? "no repsponse text" : www.text);
				this.State = ServiceState.Error;
				yield break;
			}
			
			try {
				Debug.Log(www.text);
				this.ConfigData = JsonConvert.DeserializeObject<ServiceConfigData>(www.text, JsonConvert.Settings.JsonToCSharp);
			} catch (System.Exception e) {
				this.ConfigData = null;
				this.Error = string.Format("Could not parse config data: {0}", e);
				this.State = ServiceState.Error;
				yield break;
			}
			
		}
		
		/// <summary>
		/// Loads the <see cref="PromoData" /> asynchronously.
		/// </summary>
		/// <remarks>TODO: Remove 'Preferences' key after server side integrates direct access to web preferences through native code.</remarks>
		protected IEnumerator LoadPromoDataAsync() {
			string url;
			url = this.PromoUrl;
			
			Native native;
			native = Controller.Instance.Native;
			
			Dictionary<string,object> data;
			data = new Dictionary<string,object>();

			data.Add("TotalMemory".PascalToUnderscoreCase(), native.TotalMemory);
			data.Add("BundleId".PascalToUnderscoreCase(), native.BundleId);
			data.Add("BundleVersion".PascalToUnderscoreCase(), native.BundleVersion);
			data.Add("BundleVersionCode".PascalToUnderscoreCase(), native.BundleVersionCode);

			ConfigData.Apps = native.CheckAppsAvailability(this.ConfigData.Apps);


			data.Add("apps", JsonConvert.SerializeObject(ConfigData.Apps, JsonConvert.Formatting.None, JsonConvert.Settings.CSharpToJson));
			data.Add("Preferences".PascalToUnderscoreCase(), BizUtil.WebClientPreferences);
			data.Add("InternetConnectionType".PascalToUnderscoreCase(), native.InternetConnectionType);
			data.Add("Language".PascalToUnderscoreCase(), native.Language);
			data.Add("Locale".PascalToUnderscoreCase(), native.Locale);
			data.Add("OperatingSystem".PascalToUnderscoreCase(), native.OperatingSystem);
			data.Add("OperatingSystemVersion".PascalToUnderscoreCase(), native.OperatingSystemVersion);
			data.Add("Platform".PascalToUnderscoreCase(),native.Platform.ToString());
			data.Add("PlatformFeatures".PascalToUnderscoreCase(), native.PlatformFeatures);
			data.Add("Region".PascalToUnderscoreCase(), native.Region);
			data.Add("ScreenDensity".PascalToUnderscoreCase(), native.ScreenDensity);
			data.Add("ScreenOrientation".PascalToUnderscoreCase(), native.ScreenOrientation.ToString());
			data.Add("ScreenHeight".PascalToUnderscoreCase(), native.ScreenHeight);
			data.Add("ScreenWidth".PascalToUnderscoreCase(), native.ScreenWidth);
			data.Add("ScreenAdjustedHeight".PascalToUnderscoreCase(), native.ScreenAdjustedHeight);
			data.Add("ScreenAdjustedWidth".PascalToUnderscoreCase(), native.ScreenAdjustedWidth);
			data.Add("TimeZone".PascalToUnderscoreCase(), native.TimeZone);
			data.Add("UrlScheme".PascalToUnderscoreCase(), native.UrlScheme);
			data.Add("VendorId".PascalToUnderscoreCase(), native.VendorId);
			data.Add("CompanyName".PascalToUnderscoreCase(), CompanyName != "" ? CompanyName : CompanyNameSagoSago);

			data.Add("AppData".PascalToUnderscoreCase(), this.AppData != null ? this.AppData : "");

			#if UNITY_ANDROID
				data.Add("AndroidDeviceArchitecture".PascalToUnderscoreCase(), native.AndroidDeviceArchitecture);
			#endif

			Dictionary<string,string> headers;
			headers = new Dictionary<string, string>();
			headers.Add("content-type","application/json");

			string jsonData = JsonConvert.SerializeObject(data, JsonConvert.Formatting.None, JsonConvert.Settings.CSharpToJson);
			Debug.Log("Sending this to the server: " + jsonData);
			byte[] binaryData = System.Text.Encoding.UTF8.GetBytes(jsonData);
	
			Debug.Log ("Promo url: " + url);
			WWW www;
			www = new WWW(url, binaryData, headers);

			yield return www;

			if (!string.IsNullOrEmpty(www.error)) {
				string responseHeader = "";
				foreach(var key in www.responseHeaders.Keys)
					responseHeader += string.Format("\t{0} = {1}\n", key, www.responseHeaders[key]);
				responseHeader = "{\n" + responseHeader + "}\n";
				this.Error = string.Format("Could not download promo data.\n-> Response Header: {0}\n-> Actual Error: {1}\nResponse Text: {2}", responseHeader, www.error, string.IsNullOrEmpty(www.text) ? "no repsponse text" : www.text);
				this.State = ServiceState.Error;
				yield break;
			}

			try {

				Debug.Log("We got this back from the server:\n" + www.text);
				this.PromoData = JsonConvert.DeserializeObject<ServicePromoData>(www.text, JsonConvert.Settings.JsonToCSharp);
				this.PromoData.ImageUrl = this.PromoData.ImageUrl.Replace(
					"%streamingAssetsPath%", 
					StreamingAssetsPath
				);
				this.PromoData.PageUrl = this.PromoData.PageUrl.Replace(
					"%streamingAssetsPath%", 
					StreamingAssetsPath
				);
			} catch (System.Exception e) {
				this.PromoData = null;
				this.Error = string.Format("Could not parse promo data: {0}", e);
				this.State = ServiceState.Error;
				yield break;
			}
			
		}
		
		#endregion
		
		
		#region Helper Methods



		static string StreamingAssetsPath {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return Application.streamingAssetsPath;
				#else
					return "file://" + Application.streamingAssetsPath;
				#endif
			}
		}
		
		/// <summary>
		/// Creates a WWW object with or without POST data, depending on the url scheme.
		/// </summary>
		WWW CreateWWW(string url, WWWForm form) {
			if (url.Contains("file:")) {
				return new WWW(url);
			}
			return new WWW(url, form);
		}
		
		#endregion
		
		
	}
	
}