namespace SagoBuildEditor.Android {
	
	using SagoPlatform;
	using SagoBuildEditor.Core;
	using UnityEditor;
	using SagoDebugEditor;

	/// <summary>
	/// The AndroidBuildProcessor class implements functionality for processing Android builds.
	/// </summary>
	public class AndroidBuildProcessor : BuildProcessor {
		
		#region Fields

		/// <summary>
		/// The name of the product.
		/// </summary>
		private string ProductName;

		/// <summary>
		/// The build platform being targeted
		/// </summary>
		private BuildTargetGroup BuildTargetGroup;
		
		/// <summary>
		/// The bundle identifier.
		/// </summary>
		private string BundleIdentifier;

		/// <summary>
		/// The bundle version.
		/// </summary>
		private string BundleVersion;

		/// <summary>
		/// The bundle version code.
		/// </summary>
		private int BundleVersionCode;

		/// <summary>
		/// The sdk version.
		/// </summary>
		private AndroidSdkVersions SdkVersion;
		
		#endregion
		
		
		#region IBuildProcessor Methods
		
		/// <summary>
		/// Overrides the preprocess method to apply AndroidProductInfo to the PlayerSettings.
		/// <summary>
		override public void Preprocess() {
			
			base.Preprocess();
			
			AndroidProductInfo info;
			info = PlatformUtil.GetSettings<AndroidProductInfo>();
			
			#if !SAGO_BUILD_DO_NOT_USE_VERSION_SERVICE
				if (!VersionService.Bump(info)) {
					throw new System.InvalidOperationException("Could not bump build number.");
				}
			#endif
			
			if (info) {
				
				// store the player settings
				this.ProductName = PlayerSettings.productName;
				this.BuildTargetGroup = EditorUserBuildSettings.activeBuildTarget.ConvertToGroup();
				this.BundleIdentifier = PlayerSettings.applicationIdentifier;
				this.BundleVersion = PlayerSettings.bundleVersion;
				this.BundleVersionCode = PlayerSettings.Android.bundleVersionCode;
				this.SdkVersion = PlayerSettings.Android.minSdkVersion;
				
				// copy product info to player settings
				PlayerSettings.productName = info.DisplayName;
				PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, info.Identifier);
				PlayerSettings.bundleVersion = ProductInfo.CheckVersion(info.Version);
				PlayerSettings.Android.bundleVersionCode = ProductInfo.CheckBuild(info.Build);
				PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)info.SdkVersion;
				
			}

			#if !UNITY_CLOUD_BUILD && TEAMCITY_BUILD
				#if SAGO_ANDROID_USE_GRADLE_BUILD_SYSTEM
					EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
				#else
					EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
				#endif
			#endif
			
		}

		/// <summary>
		/// Overrides the postprocess method to revert the AndroidProductInfo changes to the PlayerSettings.
		/// <summary>
		override public void Postprocess(string buildPath) {
			
			base.Postprocess(buildPath);
			
			AndroidProductInfo info;
			info = PlatformUtil.GetSettings<AndroidProductInfo>();
			
			if (info) {
			
				// restore player settings
				PlayerSettings.productName = this.ProductName;
				PlayerSettings.SetApplicationIdentifier(BuildTargetGroup, this.BundleIdentifier);
				PlayerSettings.bundleVersion = this.BundleVersion;
				PlayerSettings.Android.bundleVersionCode = this.BundleVersionCode;
				PlayerSettings.Android.minSdkVersion = this.SdkVersion;
				
				// clear player settings
				this.ProductName = null;
				this.BundleIdentifier = null;
				this.BundleVersion = null;
				this.BundleVersionCode = 0;
				this.SdkVersion = default(AndroidSdkVersions);
				
			}
			
		}
		
		#endregion
		
	}
	
}