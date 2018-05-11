namespace SagoPlatformEditor {
	
	using SagoUtilsEditor;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	public class PlatformUtilEditor : MonoBehaviour {
		
		
		#region Bootstrap Menu Item
		
		[MenuItem("Sago/Platform/Bootstrap", false, 0)]
		private static void BootstrapMenuItem() {
			Bootstrap();
		}
		
		#endregion
		
		
		#region Platform Menu Items
		
		[MenuItem("Sago/Platform/Unknown", false, 100)]
		private static void UnknownMenuItem() {
			PlatformUtilEditor.Activate(Platform.Unknown);
		}
		
		[MenuItem("Sago/Platform/Unknown", true)]
		private static bool ValidateUnknownMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.Unknown;
		}
		
		[MenuItem("Sago/Platform/iOS", false, 100)]
		private static void iOSMenuItem() {
			PlatformUtilEditor.Activate(Platform.iOS);
		}
		
		[MenuItem("Sago/Platform/iOS", true)]
		private static bool ValidateiOSMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.iOS;
		}
		
		[MenuItem("Sago/Platform/tvOS", false, 100)]
		private static void tvOSMenuItem() {
			PlatformUtilEditor.Activate(Platform.tvOS);
		}
		
		[MenuItem("Sago/Platform/tvOS", true)]
		private static bool ValidatetvOSMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.tvOS;
		}
		
		[MenuItem("Sago/Platform/GooglePlay", false, 100)]
		private static void GooglePlayMenuItem() {
			PlatformUtilEditor.Activate(Platform.GooglePlay);
		}
		
		[MenuItem("Sago/Platform/GooglePlay", true)]
		private static bool ValidateGooglePlayMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.GooglePlay;
		}

		[MenuItem("Sago/Platform/GooglePlayFree", false, 100)]
		private static void GooglePlayFreeMenuItem() {
			PlatformUtilEditor.Activate(Platform.GooglePlayFree);
		}
		
		[MenuItem("Sago/Platform/GooglePlayFree", true)]
		private static bool ValidateGooglePlayFreeMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.GooglePlayFree;
		}

		[MenuItem("Sago/Platform/Kindle", false, 100)]
		private static void KindleMenuItem() {
			PlatformUtilEditor.Activate(Platform.Kindle);
		}
		
		[MenuItem("Sago/Platform/Kindle", true)]
		private static bool ValidateKindleMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.Kindle;
		}
		
		[MenuItem("Sago/Platform/KindleFreeTime", false, 100)]
		private static void KindleFreeTimeMenuItem() {
			PlatformUtilEditor.Activate(Platform.KindleFreeTime);
		}
		
		[MenuItem("Sago/Platform/KindleFreeTime", true)]
		private static bool ValidateKindleFreeTimeMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.KindleFreeTime;
		}

		[MenuItem("Sago/Platform/WindowsStore", false, 100)]
		private static void WindowsStoreMenuItem() {
			PlatformUtilEditor.Activate(Platform.WindowsStore);
		}
		
		[MenuItem("Sago/Platform/WindowsStore", true)]
		private static bool ValidateWindowsStoreMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.WindowsStore;
		}

		[MenuItem("Sago/Platform/Nabi", false, 100)]
		private static void NabiMenuItem() {
			PlatformUtilEditor.Activate(Platform.Nabi);
		}
		
		[MenuItem("Sago/Platform/Nabi", true)]
		private static bool ValidateNabiMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.Nabi;
		}

		[MenuItem("Sago/Platform/Amazon Teal", false, 100)]
		private static void AmazonTealMenuItem() {
			PlatformUtilEditor.Activate(Platform.AmazonTeal);
		}
		
		[MenuItem("Sago/Platform/Amazon Teal", true)]
		private static bool ValidateAmazonTealMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.AmazonTeal;
		}
		
		[MenuItem("Sago/Platform/Samsung Milk", false, 100)]
		private static void SamsungMilkMenuItem() {
			PlatformUtilEditor.Activate(Platform.SamsungMilk);
		}
		
		[MenuItem("Sago/Platform/Samsung Milk", true)]
		private static bool ValidateSamsungMilkMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.SamsungMilk;
		}

		[MenuItem("Sago/Platform/Smart Education", false, 100)]
		private static void SmartEducationMenuItem() {
			PlatformUtilEditor.Activate(Platform.SmartEducation);
		}
		
		[MenuItem("Sago/Platform/Smart Education", true)]
		private static bool ValidateSmartEducationMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.SmartEducation;
		}

		[MenuItem("Sago/Platform/Bemobi", false, 100)]
		private static void BemobiMenuItem() {
			PlatformUtilEditor.Activate(Platform.Bemobi);
		}

		[MenuItem("Sago/Platform/Bemobi", true)]
		private static bool ValidateBemobiMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.Bemobi;
		}

		[MenuItem("Sago/Platform/Thales", false, 100)]
		private static void ThalesMenuItem() {
			PlatformUtilEditor.Activate(Platform.Thales);
		}

		[MenuItem("Sago/Platform/Thales", true)]
		private static bool ValidateThalesMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.Thales;
		}

		[MenuItem("Sago/Platform/Panasonic Ex", false, 100)]
		private static void PanasonicExMenuItem() {
			PlatformUtilEditor.Activate(Platform.PanasonicEx);
		}

		[MenuItem("Sago/Platform/Panasonic Ex", true)]
		private static bool ValidatePanasonicExMenuItem() {
			return PlatformUtil.ActivePlatform != Platform.PanasonicEx;
		}

		[MenuItem("Sago/Current Platform Settings")]
		private static void PlatformSettingMenuItem() {
			Selection.activeInstanceID = PlatformSettingsMultiplexer.Instance.GetPrefab().GetInstanceID();
		}
		#endregion
		
		
		#region Static Properties
		
		public static Platform[] AndroidPlatforms {
			get {
				return PlatformUtil
					.AllPlatforms
					.Where(p => p.ToBuildTargetGroup() == BuildTargetGroup.Android)
					.ToArray();
			}
		}
		
		public static Platform[] iOSPlatforms {
			get {
				return PlatformUtil
					.AllPlatforms
					.Where(p => p.ToBuildTargetGroup() == BuildTargetGroup.iOS)
					.ToArray();
			}
		}
		
		public static Platform[] tvOSPlatforms {
			get {
				return PlatformUtil
					.AllPlatforms
					.Where(p => p.ToBuildTargetGroup() == BuildTargetGroup.tvOS)
					.ToArray();
			}
		}
		
		public static Platform[] WindowsPlatforms {
			get {
				return PlatformUtil
					.AllPlatforms
					.Where(p => {
						return (
							p.ToBuildTargetGroup() == BuildTargetGroup.WSA // Unity 5.3+
						);
					})
					.ToArray();
			}
		}
		
		#endregion
		
		
		#region Static Methods
		
		public static void Bootstrap() {
			OnPlatformBootstrapAttribute.Invoke();
		}
		
		/// <summary>
		/// Switches the active build target, adds the define symbol for the 
		/// specified platform and removes the defines symbols for all other 
		/// platforms.
		/// </summary>
		public static bool Activate(Platform platform) {
			
			// bootstrap
			Bootstrap();
			
			// get platforms
			Platform newPlatform = platform;
			Platform oldPlatform = PlatformUtil.ActivePlatform;
			
			// get build targets
			// BuildTarget oldBuildTarget = oldPlatform.ToBuildTarget();
			
			// change build target
			#if !(UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
				BuildTarget newBuildTarget = newPlatform.ToBuildTarget();
				if (EditorUserBuildSettings.activeBuildTarget != newBuildTarget) {
					if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(newBuildTarget), newBuildTarget)) {
						Debug.LogError(string.Format("Could not switch active build target: {0}", newBuildTarget));
						return false;
					}
				}
			#endif
			
			// platform will change callback
			OnPlatformWillChangeAttribute.Invoke(oldPlatform, newPlatform);
			
			// change define symbols
			#if !(UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
			
				HashSet<int> obsoleteBuildTargets = new HashSet<int>() {
					2,  // (int)BuildTargetGroup.WebPlayer,
					5,  // (int)BuildTargetGroup.PS3,
					6,  // (int)BuildTargetGroup.XBOX360,
					15, // (int)BuildTargetGroup.WP8,
					16, // (int)BuildTargetGroup.BlackBerry,
					20, // (int)BuildTargetGroup.PSM,
					22, // (int)BuildTargetGroup.SamsungTV,
				};
				
				// Clean up the enum - Unity has left in some values which some of
				// their other APIs don't like
				IEnumerable<BuildTargetGroup> targetGroups = System.Enum.
					GetValues(typeof(BuildTargetGroup)).
					Cast<BuildTargetGroup>().
					Where(bt => !obsoleteBuildTargets.Contains((int)bt));
				
				foreach (BuildTargetGroup buildTargetGroup in targetGroups) {
					
					// find all the platforms that do not belong to the build target group
					Platform[] excludedPlatforms = System.Enum
						.GetValues(typeof(Platform))
						.Cast<Platform>()
						.Where(p => buildTargetGroup != p.ToBuildTargetGroup())
						.ToArray();
					
					// remove defines
					foreach (Platform excludedPlatform in excludedPlatforms) {
						DefineSymbolUtil.DefineSymbol(
							buildTargetGroup, 
							excludedPlatform.ToDefineSymbol(), 
							false
						);
					}
					
					// find all the platforms that belong to the build target group
					Platform[] includedPlatforms = System.Enum
						.GetValues(typeof(Platform))
						.Cast<Platform>()
						.Where(p => buildTargetGroup == p.ToBuildTargetGroup())
						.ToArray();
					
					
					// add the define for the new platform and remove the defines for all other platforms
					foreach (Platform includedPlatform in includedPlatforms) {
						DefineSymbolUtil.DefineSymbol(
							buildTargetGroup, 
							includedPlatform.ToDefineSymbol(), 
							includedPlatform == platform
						);
					}
					
				}
				AssetDatabase.SaveAssets();
			#endif
			
			// platform did change callback
			OnPlatformDidChangeAttribute.Invoke(oldPlatform, newPlatform);
			
			return true;
			
		}
		
		#endregion
		
		
	}
	
}