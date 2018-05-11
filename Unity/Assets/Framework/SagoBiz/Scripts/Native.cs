namespace SagoBiz {

	using SagoPlatform;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.InteropServices;
	using UnityEngine;
	using Debug = SagoBiz.DebugUtil;
	using SagoUtils;

	// Talk with Luke:
	// - InstalledUrlSchemes logic for both iOS and Android.
	// -- Do we want to call it URL Schemes given that is an iOS only term?
	//

	/// <summary>
	/// The Native class provides functionality for bridging between C# code 
	/// and native plugins. IMPORTANT: Each platform must implement the same 
	/// public interface. The default implementation provides default values and
	/// empty methods so that everything works in the editor, or when a native 
	/// plugin is not available.
	/// </summary>
	public class Native : MonoBehaviour {

		#region iOS Native Binder

		#if !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR

			[DllImport ("__Internal")]
			public static extern void _EnableDebugging();

			[DllImport ("__Internal")]
			public static extern void _DisableDebugging();

			[DllImport ("__Internal")]
			public static extern bool _IsDebuggingEnabled();

			[DllImport ("__Internal")]
			public static extern void _OpenExternalWeb(string url);
		
			[DllImport ("__Internal")]
			public static extern void _OpenWeb(string url, bool isModal);

			[DllImport ("__Internal")]
			private static extern string _BundleId();
			
			[DllImport ("__Internal")]
			private static extern string _BundleVersion();

			[DllImport ("__Internal")]
			private static extern string _BundleVersionCode();

			[DllImport ("__Internal")]
			private static extern string _AppName();

			[DllImport ("__Internal")]
			private static extern string _Language();
			
			[DllImport ("__Internal")]
			private static extern string _LanguageCode();
			
			[DllImport ("__Internal")]
			private static extern string _CountryCode();
			
			[DllImport ("__Internal")]
			private static extern string _OperatingSystem();
			
			[DllImport ("__Internal")]
			private static extern string _OperatingSystemVersion();
			
			[DllImport ("__Internal")]
			private static extern bool _HasCamera();
			
			[DllImport ("__Internal")]
			private static extern string _TimeZone();
			
			[DllImport ("__Internal")]
			private static extern string _UrlScheme();
			
			[DllImport ("__Internal")]
			private static extern string _VendorId();

			[DllImport ("__Internal")]
			private static extern string _GetInstalledAppsWithUrlSchemes(string urlSchemesJson);

			[DllImport ("__Internal")]
			private static extern void _InitializeAppleSearchAd();

			[DllImport ("__Internal")]
			private static extern void _InitializeMixpanelWithToken(string token);

			[DllImport ("__Internal")]
			private static extern void _TrackAdjustEvent(string token);
			
			[DllImport ("__Internal")]
			private static extern void _TrackMixpanelEvent(string eventName);
			
			[DllImport ("__Internal")]
			private static extern void _TrackMixpanelEventWithProperties(string eventName, string properties);

			[DllImport ("__Internal")]
			private static extern void _TimeMixpanelEvent(string eventName);

			[DllImport ("__Internal")]
			private static extern string _GetMixpanelDistinctID();
			
			[DllImport ("__Internal")]
			private static extern void _SetMixpanelDistinctID(string distinctID);
			
			[DllImport ("__Internal")]
			private static extern void _RegisterMixpanelSuperProperties(string properties);

			[DllImport ("__Internal")]
			private static extern void _SaveEventInfo(string eventName, string eventInfo);

		#endif

		#endregion

		#region App Info

		public enum ConnectivityType { WIFI, DATA, NONE };

		/// <summary>
		/// The app's bundle identifier (i.e. com.sagosago.RoadTrip)
		/// </summary>
		public string BundleId {
			get {
				#if UNITY_EDITOR
					ProductInfo info = PlatformUtil.GetSettings<ProductInfo>(); 
					return info ? info.Identifier : null;
				#elif UNITY_ANDROID
					return this.AndroidDeviceInfo.CallStatic<string>("getAppBundleId", new object[]{ 
						this.AndroidActivity
					});
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _BundleId();
				#else
					return null;
				#endif
			}
		}

		/// <summary>
		/// Gets the name of the app store.
		/// </summary>
		/// <example>
		/// com.amazon.venezia, com.android.vending
		/// </example>
		/// <value>The name of the app store.</value>
		public string AppStoreName {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					string appStorePackageName =  this.AndroidDeviceInfo.CallStatic<string>("getAppStoreName", new object[]{ 
						this.AndroidActivity
					});
					switch(appStorePackageName) {
						case "com.amazon.venezia": {
							return "Amazon";
						}
						case "com.android.vending": {
							return "GooglePlay";
							}
						default:{
							return appStorePackageName;
						}
					}
				#elif SAGO_IOS && !UNITY_EDITOR
					return "iTunes";
				#else
					return null;
				#endif
			}
		}

		/// <summary>
		/// The app's bundle version.
		/// Android: 1.1 
		/// iOS: 1.1
		/// </summary>
		public string BundleVersion {
			get { 
				#if !SAGO_DISABLE_SAGOBIZ && UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo
						.CallStatic<string>("getAppVersionName", new object[]{
							this.AndroidActivity
						})
						.ToString();
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _BundleVersion();
				#else
					return null;
				#endif
			}
		}

		/// <summary>
		/// Gets the bundle version code.
		/// Android: 15
		/// iOS: 1.1.20
		/// </summary>
		/// <value>The bundle version code.</value>
		public string BundleVersionCode {
			get { 
				#if !SAGO_DISABLE_SAGOBIZ && UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo
						.CallStatic<int>("getAppVersionCode", new object[]{
							this.AndroidActivity
						})
						.ToString();
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _BundleVersionCode();
				#else
					return null;
				#endif
			}
		}

		public string AndroidDeviceArchitecture {
			get { 
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo.CallStatic<string>("getArchitecture");
				#else
					return "";
				#endif
			}
		}

		/// <summary>
		/// The display name of the app (i.e. Road Trip)
		/// </summary>
		public string DisplayName {
			get {
                #if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo
						.CallStatic<string>("getAppDisplayName", new object[]{ 
							this.AndroidActivity
						});
                #elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _AppName();
                #elif UNITY_WSA
                    return Application.productName;
                #else
                return null;
				#endif
			}
		}
		
		/// <summary>
		/// The app's url scheme (i.e. com.sagosago.RoadTrip or sagoroadtrip). The 
		/// url scheme must be unique, or the device won't know which app to open.
		/// Note that on Android platforms, url scheme can arbitrarily defined in the manifest.
		/// The default however is the bundle id.
		/// </summary>
		public string UrlScheme {
			get { 
				#if UNITY_ANDROID && !UNITY_EDITOR
					return BundleId;
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _UrlScheme();
				#else
					ProductInfo productInfo = PlatformUtil.GetSettings<ProductInfo>(PlatformUtil.ActivePlatform);
					return productInfo ? productInfo.Identifier : null;
				#endif 
			}
		}
		
		#endregion
		
		
		#region Device Info

		/// <summary>
		/// Enables the sago biz debug mode.
		/// </summary>
		public void EnableDebugMode() {

			#if UNITY_ANDROID && !UNITY_EDITOR
				this.AndroidDebug.CallStatic("enableDebugging");
			#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
				_EnableDebugging();
			#endif
		}

		/// <summary>
		/// Disables the sago biz debug mode.
		/// </summary>
		public void DisableDebugMode() {

			#if UNITY_ANDROID && !UNITY_EDITOR
				this.AndroidDebug.CallStatic("disableDebugging");
			#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
				_DisableDebugging();
			#endif
		}

		/// <summary>
		/// Gets a value indicating whether native debugging is enabled.
		/// </summary>
		/// <value><c>true</c> if this instance is native debugging enabled; otherwise, <c>false</c>.</value>
		public bool IsDebuggingEnabled {
			get{
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDebug.CallStatic<bool>("isDebuggingEnabled");
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _IsDebuggingEnabled();
				#else
					return false;
				#endif
			}
		}

		/// <summary>
		/// Gets the manufacturer.
		/// </summary>
		/// <value>The manufacturer.</value>
		public string Manufacturer {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo.CallStatic<string>("getManufacturer");
				#elif SAGO_IOS && !UNITY_EDITOR
					return "Apple";
				#else
					return "Unknown";
				#endif
			}
		}

		/// <summary>
		/// Gets the chipset board.
		/// </summary>
		/// <value>The chipset board.</value>
		public string Board {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo.CallStatic<string>("getBoard");
				#elif SAGO_IOS && !UNITY_EDITOR
					return "Apple";
				#else
					return "Unknown";
				#endif
			}
		}

		/// <summary>
		/// Gets the hardware platform.
		/// </summary>
		/// <value>The hardware platform.</value>
		public string Hardware {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo.CallStatic<string>("getHardware");
				#elif SAGO_IOS && !UNITY_EDITOR
					return "Apple";
				#else
					return "Unknown";
				#endif
			}
		}

		/// <summary>
		/// Gets the model of the device.
		/// Possible examples: iPhone3,1 or iPod4,1.
		/// </summary>
		/// <value>The model.</value>
		public string DeviceModel {
			get { return SystemInfo.deviceModel; }
		}

		/// <summary>
		/// Gets the name of the device.
		/// </summary>
		/// <value>The name of the device.</value>
		public string DeviceName {
			get { return SystemInfo.deviceName; }
		}

		/// <summary>
		/// Gets the brand.
		/// </summary>
		/// <value>The brand.</value>
		public string Brand {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo.CallStatic<string>("getBrand");
				#elif SAGO_IOS && !UNITY_EDITOR
					return "Apple";
				#else
					return "Unknown";
				#endif
			}
		}

		/// <summary>
		/// The device's available memory, in bytes. The server needs to know about
		/// available memory to determine whether the device can handle videos or
		/// large images on certain platforms.
		/// </summary>
		public long TotalMemory {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo.CallStatic<long>("totalMemory", new object[]{ 
						this.AndroidActivity, 
					});
				#elif SAGO_IOS && !UNITY_EDITOR
					return -1; //return _AvailableMemory();
				#else
					return -1;
				#endif
			}
		}

		/// <summary>
		/// The device's available memory, in bytes. The server needs to know about
		/// available memory to determine whether the device can handle videos or
		/// large images on certain platforms.
		/// </summary>
		public long AvailableMemory {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo.CallStatic<long>("availableMemory", new object[]{ 
						this.AndroidActivity, 
					});
				#elif SAGO_IOS && !UNITY_EDITOR
					return -1; //return _AvailableMemory();
				#else
					return -1;
				#endif
			}
		}

		/// <summary>
		/// The device's available memory, in bytes. The server needs to know about
		/// available memory to determine whether the device can handle videos or
		/// large images on certain platforms.
		/// </summary>
		public string LowMemory {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo.CallStatic<bool>("isLowMemory", new object[]{ 
						this.AndroidActivity, 
					})
					.ToString().ToLower();
				#else
					return "false";
				#endif
			}
		}

		/// <summary>
		/// Gets a list of installed apps that start with a given pattern.
		/// </summary>
		/// <param name="prefix">The prefix that all returned package names must have.</param>
		/// <returns>The subset of apps installed with the provided prefix.</returns>
		public string[] GetInstalledAppsWithPackageNamePrefix(string prefix) {
			#if UNITY_ANDROID && !UNITY_EDITOR
				string packages = this.AndroidDeviceInfo.CallStatic<string>("getInstalledAppsWithPackageNamePrefix", this.AndroidActivity, prefix);
				return string.IsNullOrEmpty(packages) ? new string[0] : packages.Split(',');
			#else
				return new string[0];
			#endif
		}
		
		/// <summary>
		/// Gets a list of installed url schemes. 
		/// </summary>
		/// <param name="urlSchemes">The list of url schemes to check.</param>
		/// <returns>The subset of the list of url schemes which are installed on the device.</returns>
		public List<App> CheckAppsAvailability(List<App> apps) { 

			#if UNITY_ANDROID && !UNITY_EDITOR
				string appArray = JsonConvert.SerializeObject(apps, JsonConvert.Formatting.None, JsonConvert.Settings.CSharpToJson);
				Debug.Log ("Sending serialized app list to native:\n" + appArray);
				var result =this.AndroidDeviceInfo
					.CallStatic<string>("getInstalledAppsWithPackageNames", new object[]{ 
						this.AndroidActivity, 
						appArray
					});
				List<App> returnedList = JsonConvert.DeserializeObject<List<App>>(result, JsonConvert.Settings.CSharpToJson);
				
				return returnedList;
			#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
				string appArray = JsonConvert.SerializeObject(apps, JsonConvert.Formatting.None, JsonConvert.Settings.CSharpToJson);

				string result = _GetInstalledAppsWithUrlSchemes(appArray);
				List<App> returnedList = JsonConvert.DeserializeObject<List<App>>(result, JsonConvert.Settings.CSharpToJson);
				
				return returnedList;
			#else
				return apps;
			#endif
		}
		
		/// <summary>
		/// The device's internet connection type (i.e. wifi, 3g, lte, etc.)
		/// </summary>
		public string InternetConnectionType {
			get { 
				switch (Application.internetReachability) {
					case NetworkReachability.ReachableViaLocalAreaNetwork:
						return ConnectivityType.WIFI.ToString();
					case NetworkReachability.ReachableViaCarrierDataNetwork:
						return ConnectivityType.DATA.ToString();
					default:
						return ConnectivityType.NONE.ToString();
				}
			}
		}

		/// <summary>
		/// The device's current locale.
		/// </summary>
		/// <see href="http://en.wikipedia.org/wiki/List_of_ISO_639-1_codes" />
		public string Locale {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return Language + "-" + Region;
				#elif SAGO_IOS && !UNITY_EDITOR
					return Language + "-" + Region;
				#else
					return "en-CA";
				#endif
			}
		}

		/// <summary>
		/// The device's current language.
		/// </summary>
		public string Language {
			get {
				#if UNITY_EDITOR
					return "en";
				#elif UNITY_ANDROID
					return this.AndroidDeviceInfo
						.CallStatic<string>("getLanguage");
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _LanguageCode();
				#else
					return null;
				#endif
			}
		}
		
		/// <summary>
		/// The device's platform (i.e. iOS, Amazon, GooglePlay, Windows, etc.) Note, 
		/// platform is different from operating system. Multiple platforms may use
		/// the same operating system (i.e. Amazon and GooglePlay platforms both use
		/// the Android operating system).
		/// </summary>
		public Platform Platform {
			get {
				#if SAGO_IOS
					return Platform.iOS;
				#elif SAGO_GOOGLE_PLAY
					return Platform.GooglePlay;
				#elif SAGO_GOOGLE_PLAY_FREE
					return Platform.GooglePlayFree;
				#elif SAGO_KINDLE
					return Platform.Kindle;
				#elif SAGO_KINDLE_FREE_TIME
					return Platform.KindleFreeTime;
				#elif SAGO_SMART_EDUCATION
					return Platform.SmartEducation;
				#elif SAGO_WINDOWS_STORE
				    return Platform.WindowsStore;
				#elif SAGO_WINDOWS_PHONE
				    return Platform.WindowsPhone;
				#elif SAGO_BEMOBI
					return Platform.Bemobi;
				#elif SAGO_THALES
					return Platform.Thales;
				#else
				    return Platform.Unknown;
				#endif
            }
		}
		
		/// <summary>
		/// The device's current region. The value is a setting chosen by the
		/// user, but may not reflect the device's actual location (i.e. the 
		/// user has set the device's region to Canada, but is travelling in
		/// the US. The server can use geoip to determine the device's actual 
		/// location.
		/// </summary>
		/// <see href="http://en.wikipedia.org/wiki/ISO_3166-1"/>
		public string Region {
			get {
				#if UNITY_EDITOR
					return "CA";
				#elif UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo
						.CallStatic<string>("getCountry");
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _CountryCode();
				#else
					return null;
				#endif
			}
		}
		
		/// <summary>
		/// The device's operating system (i.e. iOS, Android, Windows)
		/// </summary>
		public string OperatingSystem {
			get {
                #if UNITY_ANDROID && !UNITY_EDITOR
	                return "Android";
                #elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
	                return _OperatingSystem();
                #elif UNITY_WSA
                    return SystemInfo.operatingSystem;
                #else
                    return "UnityOS";
                #endif
			}
		}
		
		/// <summary>
		/// The device's operating system version.
		/// Android: 21
		/// iOS: 8.4
		/// </summary>
		public string OperatingSystemVersion {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo
						.CallStatic<int>("getOSVersion").ToString();
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _OperatingSystemVersion();
				#else
					return Application.unityVersion;
				#endif
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has camera.
		/// </summary>
		/// <value><c>true</c> if this instance has camera; otherwise, <c>false</c>.</value>
		public bool HasCamera {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo
						.CallStatic<bool>("hasCamera", new object[]{ 
							this.AndroidActivity
					});
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _HasCamera();
				#else
					return false;
				#endif
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has front camera.
		/// </summary>
		/// <value><c>true</c> if this instance has front camera; otherwise, <c>false</c>.</value>
		public bool HasFrontCamera {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo
						.CallStatic<bool>("hasFrontCamera", new object[]{ 
							this.AndroidActivity
					});
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _HasCamera();
				#else
					return false;
				#endif
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance has rear camera.
		/// </summary>
		/// <value><c>true</c> if this instance has rear camera; otherwise, <c>false</c>.</value>
		public bool HasRearCamera {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo
						.CallStatic<bool>("hasRearCamera", new object[]{ 
							this.AndroidActivity
					});
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _HasCamera();
				#else
					return false;
				#endif
			}
		}
		
		/// <summary>
		/// The list of platform-specific features supported by the device.
		/// </summary>
		public Dictionary<string, string> PlatformFeatures {
			get { 
				string accelerometer = "Accelerometer";
				string gyroscope = "Gyroscope";
				string locationService = "Location Service";
				string vibration = "Vibration";
				// string frontCamera = "FrontCamera";
				// string rearCamera = "RearCamera";
				string hasCamera = "Has Camera";
				string True = "true";
				string False = "false";

				Dictionary<string, string> features = new Dictionary<string, string>();

				if (SystemInfo.supportsAccelerometer) {
					features.Add(accelerometer, True);
				} else {
					features.Add(accelerometer, False);
				}

				if (SystemInfo.supportsGyroscope) { 
					features.Add(gyroscope, True);
				} else {
					features.Add(gyroscope, False);
				}
				
				if (SystemInfo.supportsLocationService) {
					features.Add(locationService, True);
				} else {
					features.Add(locationService, False);
				}

				if (SystemInfo.supportsVibration) {
					features.Add(vibration, True);
				} else {
					features.Add(vibration, False);
				}
				
				#if UNITY_ANDROID && !UNITY_EDITOR
					features.Add(hasCamera, HasCamera ? True : False);
				#elif SAGO_IOS && !UNITY_EDITOR
					features.Add(hasCamera, HasCamera ? True : False);
				#else
					features.Add(hasCamera, False);
				#endif

				return features;

			}
		}
		
		/// <summary>
		/// The density of the device's screen, in pixels per inch.
		/// </summary>
		public float ScreenDensity {
			get { return Screen.dpi; }
		}
		
		/// <summary>
		/// The orientation of the device's screen.
		/// </summary>
		public ScreenOrientation ScreenOrientation {
			get { return Screen.orientation; }
		}

		/// <summary>
		/// The absolute height of the device's screen, in pixels.
		///  Note: Refer to Android getRealMetrics() docs.
		/// </summary>
		public int ScreenHeight {
			get { return Screen.height; }
			// get {
			// 	#if UNITY_ANDROID && !UNITY_EDITOR
			// 	return this.AndroidDeviceInfo
			// 		.CallStatic<int>("screenRealPhysicalHeight", new object[]{
			// 			this.AndroidActivity
			// 		});
			// 	#elif SAGO_IOS && !UNITY_EDITOR
			// 		return Screen.height;
			// 	#else
			// 		return -1;
			// 	#endif
			// }
		}
		
		/// <summary>
		/// The absolute width of the device's screen, in pixels.
		///  Note: Refer to Android getRealMetrics() docs.
		/// </summary>
		public int ScreenWidth {
			get { return Screen.width; }
			// get {
			// 	#if UNITY_ANDROID && !UNITY_EDITOR
			// 	return this.AndroidDeviceInfo
			// 		.CallStatic<int>("screenRealPhysicalWidth", new object[]{
			// 			this.AndroidActivity
			// 		});
			// 	#elif SAGO_IOS && !UNITY_EDITOR
			// 		return Screen.width;
			// 	#else
			// 		return -1;
			// 	#endif
			// }
		}

		/// <summary>
		/// The adjusted height of the device's screen, in pixels.
		/// Note: On Android devices the size is adjusted to exclude system decors. Refer to Android getMetrics() docs.
		/// </summary>
		public int ScreenAdjustedHeight {
			get { return Screen.height; }
			// get {
			// 	#if UNITY_ANDROID && !UNITY_EDITOR
			// 		return this.AndroidDeviceInfo
			// 			.CallStatic<int>("screenPhysicalHeight", new object[]{
			// 				this.AndroidActivity
			// 		});
			// 	#else
			// 		return -1;
			// 	#endif
			// }
		}
		
		/// <summary>
		/// The adjusted width of the device's screen, in pixels.
		/// Note: On Android devices the size is adjusted to exclude system decors. Refer to Android getMetrics() docs.
		/// </summary>
		public int ScreenAdjustedWidth {
			get { return Screen.width; }
			// get {
			// 	#if UNITY_ANDROID && !UNITY_EDITOR
			// 		return this.AndroidDeviceInfo
			// 			.CallStatic<int>("screenPhysicalWidth", new object[]{
			// 				this.AndroidActivity
			// 		});
			// 	#else
			// 		return -1;
			// 	#endif
			// }
		}
		
		/// <summary>
		/// The device's current local time zone.
		/// TODO: Need to determine what the format/values will be.
		/// </summary>
		public string TimeZone {
			get {
					#if UNITY_ANDROID && !UNITY_EDITOR
						return this.AndroidDeviceInfo.CallStatic<string>("getTimeZone");
					#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
						return _TimeZone();
					#elif UNITY_WSA 
		                // doesn't support System.TimeZone
		                return null;
					#else
		                return System.TimeZone.CurrentTimeZone.StandardName;
					#endif
            }
        }
		
		/// <summary>
		/// A string that uniquely identifies the device for analytics purposes. 
		/// The value must be persistent. The value must be the same for all sago 
		/// apps installed on the same device. The value must be backwards compatible 
		/// with the grow tool, so we don't lose continuity when we switch from the
		/// grow tool to sago biz.
		/// </summary>
		public string VendorId {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidDeviceInfo.CallStatic<string>("getAndroidID", this.AndroidActivity);
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return _VendorId();
				#else
					return SystemInfo.deviceUniqueIdentifier;
				#endif
			}
		}
		
		#endregion

		
		#region Adjust

		public void AdjustEvent(string eventToken) {
			#if !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
				Native._TrackAdjustEvent(eventToken);
			#endif
		}

		#endregion
		
		
		#region Mixpanel
		
		/// <summary>
		/// Gets a flag that indicates whether the current platform has a 
		/// native Mixpanel implementation.
		/// </summary>
		public bool HasMixpanel {
			get { 
				#if UNITY_ANDROID && !UNITY_EDITOR
					return true;
				#elif SAGO_IOS && !UNITY_EDITOR
					return true;
				#else
					return false;
				#endif
			}
		}
			
		/// <summary>
		/// Sends the event name and properties to the native Mixpanel implementation.
		/// </summary>
		public void MixpanelEvent(string name, Dictionary<string, object> properties) {
			#if UNITY_ANDROID && !UNITY_EDITOR
				if (properties == null || properties.Count == 0) {
					this.AndroidMixpanel.CallStatic ("trackEvent", name);
				} else {
					this.AndroidMixpanel.CallStatic ("trackEventWithProperties", name, JsonConvert.SerializeObject (properties, JsonConvert.Formatting.None));
				}
			#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
				if (properties == null || properties.Count == 0) {
					Native._TrackMixpanelEvent(name);
				} else {
					Native._TrackMixpanelEventWithProperties(name, JsonConvert.SerializeObject(properties, JsonConvert.Formatting.None));
				}
			#endif
		}

		public void MixpanelTimeEvent(string name) {
			#if UNITY_ANDROID && !UNITY_EDITOR
				this.AndroidMixpanel.CallStatic("timeEvent", name);
			#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
				Native._TimeMixpanelEvent(name);
			#endif
		}

		/// <summary>
		/// Gets and sets the id for the native Mixpanel implementation.
		/// </summary>
		public string MixpanelId {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return this.AndroidMixpanel.CallStatic<string>("getDistinctID");
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					return Native._GetMixpanelDistinctID();
				#else
					return string.Empty;
				#endif
			}

			set {
				#if UNITY_ANDROID && !UNITY_EDITOR
					this.AndroidMixpanel.CallStatic("setDistinctID", value);
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					Native._SetMixpanelDistinctID(value);
				#endif
			}
		}
		
		/// <summary>
		/// Sets the properties for the native Mixpanel implementation.
		/// </summary>
		public Dictionary<string,object> MixpanelProperties {
			set {
				#if UNITY_ANDROID && !UNITY_EDITOR
					this.AndroidMixpanel.CallStatic("registerSuperProperties", JsonConvert.SerializeObject(value, JsonConvert.Formatting.None));
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					Native._RegisterMixpanelSuperProperties(JsonConvert.SerializeObject(value, JsonConvert.Formatting.None));
				#endif
			}
		}
		
		/// <summary>
		/// Sets the token for the native Mixpanel implementation.
		/// </summary>
		public string MixpanelToken {
			set {
				#if UNITY_ANDROID && !UNITY_EDITOR
					this.AndroidMixpanel.CallStatic("initializeMixPanelWithToken", AndroidActivity, value);
				#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
					Native._InitializeMixpanelWithToken(value);
				#endif
			}
		}
		
		public void SaveEventInfo(string eventName, Dictionary<string, object> eventInfo) {
			#if UNITY_ANDROID && !UNITY_EDITOR
				// TODO
			#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
				Native._SaveEventInfo(eventName, JsonConvert.SerializeObject(eventInfo, JsonConvert.Formatting.None));
			#endif
		}

		#endregion
		
		
		#region Webview

		public bool isWebViewVisible { get; set; }

		public string PromoWebViewError {
			get; protected set;
		}
		
		public bool PromoWebViewReady {
			get; protected set;
		}
		
		public bool PromoWebViewSupported {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					return true;
				#else
					return false;
				#endif
			}
		}


		/// <summary>
		/// Pre load the promo web view.
		/// </summary>
		public void PreLoadPromoWebView() {
			// TODO: Pre-loading is currently not supported. It will be implemeted in the future if required.
			this.OnPromoWebViewReadyToShow();
		}

		public void InitializeWebView() {
			#if UNITY_ANDROID && !UNITY_EDITOR
			using (AndroidJavaClass promoWebView = new AndroidJavaClass("com.sagosago.sagobiz.PromoWebView")) {
				promoWebView.CallStatic("initializeWebView", new object[]{ this.AndroidActivity });
			}
			#endif
		}

		/// <summary>
		/// Opens a web view with the specified url.
		/// </summary>
		public void OpenWebView(string url) {

			#if UNITY_ANDROID && !UNITY_EDITOR
				this.AndroidPromoWebView.Call("display", new object[]{ url });
			#elif !SAGO_DISABLE_SAGOBIZ && SAGO_IOS && !UNITY_EDITOR
				_OpenWeb(url, false);
			#else
				Application.OpenURL(url);
			#endif
		}


		public void OnPromoWebViewReadyToShow() {
			Debug.Log("SAGO: OnPromoWebViewReadyToShow");
			this.PromoWebViewReady = true;
		}
		
		#endregion
		
		
		#region Android Helper Fields
		
		#if UNITY_ANDROID
			[System.NonSerialized]
			private AndroidJavaObject m_AndroidActivity;
		#endif
		
		#if UNITY_ANDROID
			[System.NonSerialized]
			private AndroidJavaObject m_AndroidDeviceInfo;
		#endif

		#if UNITY_ANDROID
			[System.NonSerialized]
			private AndroidJavaObject m_AndroidDebug;
		#endif

		#if UNITY_ANDROID
			[System.NonSerialized]
			private AndroidJavaObject m_AndroidMixpanel;
		#endif

		#if UNITY_ANDROID
			[System.NonSerialized]
			private AndroidJavaObject m_AndroidPromoWebView;

			[System.NonSerialized]
			private AndroidJavaObject m_AndroidPromoWebViewAsync;
		#endif
		
		#endregion
		
		
		#region Android Helper Methods
		
		#if UNITY_ANDROID
			private AndroidJavaObject AndroidActivity {
				get {
					#if UNITY_ANDROID && !UNITY_EDITOR
						if (m_AndroidActivity == null) {
							using (AndroidJavaClass javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
								m_AndroidActivity = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
							}
						}
						return m_AndroidActivity;
					#else
						return null;
					#endif
				}
			}
		#endif

		#if UNITY_ANDROID
		private AndroidJavaObject AndroidMixpanel {
			get {
				#if UNITY_ANDROID && !UNITY_EDITOR
					m_AndroidMixpanel = m_AndroidMixpanel ?? new AndroidJavaClass("com.sagosago.sagomixpanel.Mixpanel");
					return m_AndroidMixpanel;
				#else
					return null;
				#endif
			}
		}
		#endif

		#if UNITY_ANDROID
			private AndroidJavaObject AndroidDeviceInfo {
				get {
					#if UNITY_ANDROID && !UNITY_EDITOR
						m_AndroidDeviceInfo = m_AndroidDeviceInfo ?? new AndroidJavaClass("com.sagosago.sagobiz.DeviceInfo");
						return m_AndroidDeviceInfo;
					#else
						return null;
					#endif
				}
			}
		#endif
		
		#if UNITY_ANDROID
			private AndroidJavaObject AndroidPromoWebView {
				get {
					#if !UNITY_EDITOR
						if (m_AndroidPromoWebView == null) {
							m_AndroidPromoWebView =  new AndroidJavaObject(
								"com.sagosago.sagobiz.PromoWebView", 
								this.AndroidActivity);
						}
						return m_AndroidPromoWebView;
					#else
						return null;
					#endif
				}
			}

			private AndroidJavaObject AndroidPromoWebViewAsync {
				get {
					#if !UNITY_EDITOR
						Debug.Log("AndroidPromoWebViewAsync was accessed");
						if (m_AndroidPromoWebViewAsync == null) {
							m_AndroidPromoWebViewAsync =  new AndroidJavaObject(
								"com.sagosago.sagobiz.PromoWebViewAsync", 
								this.AndroidActivity);
						}
						return m_AndroidPromoWebViewAsync;
					#else
						return null;
					#endif
				}
			}

			private AndroidJavaObject AndroidDebug {
				get {
					#if !UNITY_EDITOR && UNITY_ANDROID
						if (m_AndroidDebug == null) {
						m_AndroidDebug =  new AndroidJavaClass("com.sagosago.sagobiz.SagoBizDebug");
								}
						return m_AndroidDebug;
					#else
						return null;
					#endif
				}
			}
		#endif
		
		#endregion
		

		#region Apple Search Ad

		public void InitializeAppleSearchAd() {
			#if SAGO_IOS && !UNITY_EDITOR
				_InitializeAppleSearchAd();
			#endif
		}

		#endregion

	}
	
}