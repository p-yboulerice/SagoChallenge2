namespace SagoPlatform {
	
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	
	/// <summary>
	/// The PlatformUtil class provides helper methods for working with the <see cref="Platform" /> enum.
	/// </summary>
	public static class PlatformUtil {
				
		#region Static Properties
		
		/// <summary>
		/// Gets the current platform.
		/// </summary>
		public static Platform ActivePlatform {
			get {
				
				#if UNITY_EDITOR
					
					/// Make sure the active platform is valid. 
					
					/// If more than one platform is active, it means something has gone 
					/// wrong and there are multiple platform define symbols in the player 
					/// settings. This could happen if one platform define is checked into 
					/// the repo and another is added by Cloud Build.
					
					List<Platform> activePlatforms;
					activePlatforms = new List<Platform>();
					
					#if SAGO_IOS
						activePlatforms.Add(Platform.iOS);
					#endif
					#if SAGO_GOOGLE_PLAY
						activePlatforms.Add(Platform.GooglePlay);
					#endif
					#if SAGO_GOOGLE_PLAY_FREE
						activePlatforms.Add(Platform.GooglePlayFree);
					#endif
					#if SAGO_KINDLE
						activePlatforms.Add(Platform.Kindle);
					#endif
					#if SAGO_KINDLE_FREE_TIME
						activePlatforms.Add(Platform.KindleFreeTime);
					#endif
					#if SAGO_NABI
						activePlatforms.Add(Platform.Nabi);
					#endif
					#if SAGO_WINDOWS_PHONE
						activePlatforms.Add(Platform.WindowsPhone);
					#endif
					#if SAGO_WINDOWS_STORE
						activePlatforms.Add(Platform.WindowsStore);
					#endif
					#if SAGO_AMAZON_TEAL
						activePlatforms.Add(Platform.AmazonTeal);
					#endif
					#if SAGO_SAMSUNG_MILK
						activePlatforms.Add(Platform.SamsungMilk);
					#endif
					#if SAGO_TVOS
						activePlatforms.Add(Platform.tvOS);
					#endif
					#if SAGO_SMART_EDUCATION
						activePlatforms.Add(Platform.SmartEducation);
					#endif
					#if SAGO_BEMOBI
						activePlatforms.Add(Platform.Bemobi);
					#endif
					#if SAGO_THALES
						activePlatforms.Add(Platform.Thales);
					#endif
					#if SAGO_PANASONIC_EX
						activePlatforms.Add(Platform.PanasonicEx);
					#endif

					if (activePlatforms.Count > 1) {
						throw new System.InvalidOperationException(string.Format(
							"Multiple platforms defined: {0}", 
							string.Join(",", activePlatforms.Select(p => p.ToDefineSymbol()).ToArray())
						));
					}
					
				#endif
				
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
				#elif SAGO_WINDOWS_PHONE
					return Platform.WindowsPhone;
				#elif SAGO_WINDOWS_STORE
					return Platform.WindowsStore;
				#elif SAGO_NABI
					return Platform.Nabi;
				#elif SAGO_AMAZON_TEAL
					return Platform.AmazonTeal;
				#elif SAGO_SAMSUNG_MILK
					return Platform.SamsungMilk;
				#elif SAGO_TVOS
					return Platform.tvOS;
				#elif SAGO_SMART_EDUCATION
					return Platform.SmartEducation;
				#elif SAGO_BEMOBI
					return Platform.Bemobi;
				#elif SAGO_THALES
					return Platform.Thales;
				#elif SAGO_PANASONIC_EX
					return Platform.PanasonicEx;
				#else
					return Platform.Unknown;
				#endif
				
			}
		}
		
		public static Platform[] AllPlatforms {
			get { return (Platform[]) System.Enum.GetValues(typeof(Platform)); }
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Converts an <see cref="int" /> value to a <see cref="Platform" /> value.
		/// </summary>
		public static Platform IntToPlatform(int value) {
			return (Platform)System.Enum.ToObject(typeof(Platform) , value);
		}
		
		/// <summary>
		/// Converts a <see cref="Platform" /> value to an <see cref="int" /> value.
		/// </summary>
		public static int PlatformToInt(Platform value) {
			return (int)value;
		}
		
		/// <summary>
		/// Gets the component for the active platform.
		/// </summary>
		public static T GetSettings<T>() where T : Component {
			return (
				PlatformSettingsMultiplexer.Instance ? 
				PlatformSettingsMultiplexer.Instance.GetPrefabComponent<T>() : 
				null
			);
		}
		
		/// <summary>
		/// Gets the component for the specified platform.
		/// </summary>
		public static T GetSettings<T>(Platform platform) where T : Component {
			return (
				PlatformSettingsMultiplexer.Instance ? 
				PlatformSettingsMultiplexer.Instance.GetPrefabComponent<T>(platform) : 
				null
			);
		}

		public static string AppendVersion(string value) {
			var productInfo = GetSettings<ProductInfo>();
			if (productInfo != null) {
				return string.Format("{0}-{1}", value, productInfo.Build.ToString());
			} else {
				return value;
			}
		}
		
		#endregion
		
		
	}
	
}