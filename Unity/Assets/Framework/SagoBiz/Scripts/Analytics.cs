namespace SagoBiz {
	
	using SagoPlatform;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Linq;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// AnalyticsMode defines the modes for the Analytics component, and is used
	/// to determine which Mixpanel token is used when tracking events.
	/// </summary>
	public enum AnalyticsMode {
		/// <summary>Events are not tracked.</summary>
		Disabled,
		/// <summary>Events are tracked using the debug token. Events are stored in the dev/testing MixPanel project.</summary>
		Development,
		/// <summary>Events are tracked using the release token. Events are stored in the production MixPanel project.</summary>
		Production
	}
	
	/// <summary>
	/// The Analytics class provides methods for tracking events via a 
	/// third-party analytics service (i.e. Mixpanel). The class is designed so
	/// that it can initialize and send events to the service, but keep the 
	/// public interface generic so that our game code doesn't need to 
	/// know which service we're using or how it works.
	/// </summary>
	public class Analytics : MonoBehaviour {
		
		
		#region Properties

		/// <summary>
		/// Gets and sets the flag indicating whether or not Mixpanel is initialized.
		/// </summary>
		bool MixpanelInitialized {
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this Mixpanel native is initialized.
		/// </summary>
		/// <value><c>true</c> if mixpanel native initialized; otherwise, <c>false</c>.</value>
		bool MixpanelNativeInitialized {
			get; set;
		}

		/// <summary>
		/// Gets the Mixpanel id.
		/// </summary>
		string MixpanelId {
			get {
				if (Controller.Instance && Controller.Instance.Native) {
					return Controller.Instance.Native.VendorId;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the dictionary of Mixpanel properties. Some of the properties, 
		/// like the list of installed apps, may not be available immediately.
		/// </summary>
		Dictionary<string,object> MixpanelProperties {
			get {
				
				Native native;
				native = Controller.Instance.Native;
				
				Service service;
				service = Controller.Instance.Service;
				
				if (mixpanelProps == null) {
					mixpanelProps = new Dictionary<string,object>();
				}
				
				if (!mixpanelProps.ContainsKey("App Name")) {
					ProductInfo productInfo = PlatformUtil.GetSettings<ProductInfo>(PlatformUtil.ActivePlatform);
					if (productInfo) {
						#if SAGO_IOS
							mixpanelProps.Add("App Name", productInfo.AnalyticsName);
						#else
							mixpanelProps.Add("App Name", productInfo.AnalyticsName + PlatformUtil.ActivePlatform.ToString());
						#endif
					} else {
						mixpanelProps.Add("App Name", "Unknown");
					}

					mixpanelProps.Add("App Id", native.BundleId);
					mixpanelProps.Add("Url Scheme", native.UrlScheme);
					mixpanelProps.Add("Internet Connection Type", native.InternetConnectionType);
					mixpanelProps.Add("Language Code", native.Language);
					mixpanelProps.Add("Locale", native.Locale);
					mixpanelProps.Add("Platform", native.Platform.ToString());
					mixpanelProps.Add("Device Country Code", native.Region);
					foreach (KeyValuePair<string,string> feature in native.PlatformFeatures) {
						mixpanelProps.Add(feature.Key, feature.Value);
					}
					mixpanelProps.Add("Screen Density", native.ScreenDensity);
					mixpanelProps.Add("Screen Orientation", native.ScreenOrientation.ToString());
					mixpanelProps.Add("Screen Height px", native.ScreenHeight);
					mixpanelProps.Add("Screen Width px", native.ScreenWidth);
					mixpanelProps.Add("Timezone", native.TimeZone);
					mixpanelProps.Add("Bundle Version", native.BundleVersion);
					mixpanelProps.Add("Bundle Version Code", native.BundleVersionCode);
					mixpanelProps.Add("X-Promo SDK Version", Version.Current);
				}
				
				if (service.ConfigData != null && service.ConfigData.Apps != null) {
					string[] installedApps = native.CheckAppsAvailability(service.ConfigData.Apps).Where(a => a.Installed == true).Select(a => a.Name).ToArray();
					if (!mixpanelProps.ContainsKey("Installed Apps")) {
						mixpanelProps.Add("X-Promo Server Version", service.ConfigData.WebServiceVersion);
						mixpanelProps.Add("Installed Apps", installedApps);
					} else {
						mixpanelProps["X-Promo Server Version"] = service.ConfigData.WebServiceVersion;
						mixpanelProps["Installed Apps"] = installedApps;
					}
				}
				
				return mixpanelProps;
				
			}
		}
		
		/// <summary>
		/// Gets the current Mixpanel token, based on the <see cref="AnalyticsMode" />.
		/// </summary>
		/// <remarks>The mixpanel token must be set before anything else to initialize the native mixpanel instance.</remarks>
		string MixpanelToken {
			get {
				if (this.Options) {
					switch (this.Options.Mode) {
						case AnalyticsMode.Development:
							return MixpanelConstants.TokenDebug;
						case AnalyticsMode.Production:
							return MixpanelConstants.TokenRelease;
					}
				}
				return null;
			}
		}
		
		/// <summary>
		/// Gets the <see cref="Native" /> component.
		/// </summary>
		Native Native {
			get { return Controller.Instance ? Controller.Instance.Native : null; }
		}
		
		/// <summary>
		/// Gets the <see cref="Options" /> component.
		/// </summary>
		AnalyticsOptions Options {
			get { return PlatformUtil.GetSettings<AnalyticsOptions>(); }
		}
		
		/// <summary>
		/// Gets the <see cref="Service" /> component.
		/// </summary>
		Service Service {
			get { return Controller.Instance ? Controller.Instance.Service : null; }
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Sets the mixpanel property for the specified key.
		/// </summary>
		public void SetProperty(string key, object value) {
			this.MixpanelProperties[key] = value;
			this.ConfigureMixpanel();
		}
		
		/// <summary>
		/// Tracks the event.
		/// </summary>
		public void TrackEvent(string eventName) {
			this.TrackEvent(eventName, null);
		}
		
		/// <summary>
		/// Tracks the event with a dictionary of event info.
		/// </summary>
		public void TrackEvent(string eventName, Dictionary<string,object> eventInfo) {
			
			if (this.Options == null || this.Options.Mode == AnalyticsMode.Disabled) {
				return;
			}

			if (!this.MixpanelInitialized) {
				// We do not wait for this coroutine to finish because
				// we want the first event, App Started, to definitely be
				// the first event sent, and be timed correctly.
				// The coroutine will begin the process of collecting 
				// the installed apps (incl. downloading the list of apps
				// to check for), so that info will be available only 
				// on later events.
				this.StartCoroutine(this.ConfigureMixpanelAsync());
				this.MixpanelInitialized = true;
			}
			
			if (this.Native && this.Native.HasMixpanel) {
				this.Native.MixpanelEvent(eventName, eventInfo);
			} else {
				Mixpanel.SendEvent(eventName, eventInfo);
			}
			
		}

		/// <summary>
		/// Start timing an event.
		/// </summary>
		public void TimeEvent(string eventName) {

			if (this.Options == null || this.Options.Mode == SagoBiz.AnalyticsMode.Disabled) {
				return;
			}

			if (!this.MixpanelInitialized) {
				this.StartCoroutine(this.ConfigureMixpanelAsync());
				this.MixpanelInitialized = true;
			}

			if (this.Native && this.Native.HasMixpanel) {
				this.Native.MixpanelTimeEvent(eventName);
			} else {
				Debug.LogWarning("TimeEvent is not supported by the active platform");
			}

		}

		public void SaveEventInfoToNative(string eventName, Dictionary<string, object> eventInfo) {

			if (this.Options == null || this.Options.Mode == SagoBiz.AnalyticsMode.Disabled) {
				return;
			}

			if (!this.MixpanelInitialized) {
				this.StartCoroutine(this.ConfigureMixpanelAsync());
				this.MixpanelInitialized = true;
			}

			if (this.Native && this.Native.HasMixpanel) {
				this.Native.SaveEventInfo(eventName, eventInfo);
			} else {
				Debug.LogWarning("SaveEventInfoToNative is not supported by the active platform");
			}

		}

		public void TrackAdjustEvent(string eventToken) {
			if (this.Native) {
				this.Native.AdjustEvent(eventToken);
			}
		}
		
		#endregion
		
		
		#region Helper Methods
		
		/// <summary>
		/// Configures the current Mixpanel implementation.
		/// </summary>
		void ConfigureMixpanel() {
			if (this.Native && this.Native.HasMixpanel) {
				// NOTE: Setting mixpanel token initializes the native mixpanel instance, therefore it has to be called before any other native mixpanel method.
				if (!MixpanelNativeInitialized) {
					this.Native.MixpanelToken = this.MixpanelToken;
					this.Native.MixpanelId = this.MixpanelId;
					MixpanelNativeInitialized = true;
				}
				this.Native.MixpanelProperties = this.MixpanelProperties;
			} else {
				Mixpanel.Token = this.MixpanelToken;
				Mixpanel.DistinctID = this.MixpanelId;
				Mixpanel.SuperProperties.Clear();
                if (this.Native)
                {
                    // Faking built-in Mixpanel behaviour from native Mixpanel implementations
                    Mixpanel.SuperProperties.Add("$model", this.Native.DeviceModel);
                    Mixpanel.SuperProperties.Add("mp_device_model", this.Native.DeviceModel);
                    Mixpanel.SuperProperties.Add("$os", this.Native.OperatingSystem);
                }
				foreach (KeyValuePair<string,object> kv in this.MixpanelProperties) {
					Mixpanel.SuperProperties.Add(kv.Key, kv.Value);
				}
			}
			this.LogMixpanelProperties();
		}
		
		/// <summary>
		/// Waits for the service to finish loading (so that the list of 
		/// installed apps is available), the calls <see cref="ConfigureMixpanel" />.
		/// </summary>
		IEnumerator ConfigureMixpanelAsync() {
			
			if (this.Service == null) {
				yield break;
			}
			
			if (this.Service.IsLoaded) {
				this.ConfigureMixpanel();
				yield break;
			}
			
			this.ConfigureMixpanel();
			while (!this.Service.IsLoaded) {
				yield return null;
			}
			this.ConfigureMixpanel();
			yield break;
			
		}
		
		void LogMixpanelProperties() {
			if (this.Options != null && this.Options.Mode == AnalyticsMode.Development) {
				string msg = string.Empty;
				foreach (KeyValuePair<string,object> kv in this.MixpanelProperties) {
					msg += string.Format("SAGO: {0} = {1}\n", kv.Key, kv.Value);
				}
				Debug.Log(msg);
			}
		}
		
		#endregion
		
		#region Internal Methods

		protected Dictionary<string,object> mixpanelProps;

		#endregion
	}
	
}