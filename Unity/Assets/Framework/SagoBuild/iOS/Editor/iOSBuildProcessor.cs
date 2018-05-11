#if UNITY_IOS || UNITY_TVOS

namespace SagoBuildEditor.iOS {
	
	using PlistCS;
	using SagoBuildEditor.Core;
	using SagoCore.Submodules;
	using SagoPlatform;
	using SagoPlatformEditor;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	using UnityEngine;
	using UnityEngine.CloudBuild;
	using Version = System.Version;
	
	/// <summary>
	/// The iOSBuildProcessor class implements functionality for processing iOS builds.
	/// </summary>
	public class iOSBuildProcessor : BuildProcessor {


		#region Constants

		/// <summary>
		/// The code signing identity to use for AdHoc builds.
		/// </summary>

		#if TEAMCITY_BUILD
			protected const string AdHocCodeSigningIdentity = "";
		#else
			protected const string AdHocCodeSigningIdentity = "iPhone Distribution: Toca Boca Inc.";
		#endif
		
		/// <summary>
		/// The provisioning profile to use for AdHoc builds.
		/// </summary>
		#if TEAMCITY_BUILD
			protected const string AdHocProvisioningProfile = "";
		#else
			protected const string AdHocProvisioningProfile = "ad21062e-a1bc-48fa-bfdb-9a7cd77e95ba";
		#endif
		
		/// <summary>
		/// The code signing identity to use for app store builds.
		/// </summary>
		#if TEAMCITY_BUILD
			protected const string AppStoreCodeSigningIdentity = "";
		#else
			protected const string AppStoreCodeSigningIdentity = "iPhone Distribution: Sago Sago Toys Inc. (SF8M4XHC7B)";
		#endif
		
		/// <summary>
		/// The provisioning profile to use for AppStore builds.
		/// </summary>
		protected static string AppStoreProvisioningProfile {
			get {
				iOSProductInfo info = PlatformUtil.GetSettings<iOSProductInfo>();
				return info ? info.AppStoreProvisioningProfile : null;
			}
		}
		
		/// <summary>
		/// The code signing identity to use for developer builds.
		/// </summary>
		#if TEAMCITY_BUILD
			protected const string DeveloperCodeSigningIdentity = "";
		#else
			protected const string DeveloperCodeSigningIdentity = "iPhone Developer";
		#endif
		
		/// <summary>
		/// The provisioning profile to use for developer builds.
		/// </summary>
		protected const string DeveloperProvisioningProfile = "";
		
		#endregion
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project and runs it on an iOS device.
		/// </summary>
		[MenuItem("Sago/Build/iOS: Build and Run (Device)", false, 9000)]
		public static void BuildAndRunDevice() {
			iOSBuildRunner runner = new iOSBuildRunner();
			runner.BuildOptions = (
				BuildOptions.SymlinkLibraries 
			);
			runner.BuildAction = iOSBuildAction.BuildAndRun;
			runner.BuildSdkVersion = iOSSdkVersion.DeviceSDK;
			runner.OnPreprocess = iOSBuildProcessor.OnPreprocessDevice;
			runner.OnPostprocess = iOSBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="BuildAndRunDevice" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/iOS: Build and Run (Device)", true)]
		public static bool ValidateBuildAndRunDevice() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.iOS);
		}
		
		/// <summary>
		/// Builds the project and runs it in the iOS simulator.
		/// </summary>
		[MenuItem("Sago/Build/iOS: Build and Run (Simulator)", false, 9000)]
		public static void BuildAndRunSimulator() {
			iOSBuildRunner runner = new iOSBuildRunner();
			runner.BuildOptions = (
				BuildOptions.SymlinkLibraries 
			);
			runner.BuildAction = iOSBuildAction.BuildAndRun;
			runner.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/iOSSimulator");
			runner.BuildSdkVersion = iOSSdkVersion.SimulatorSDK;
			runner.OnPreprocess = iOSBuildProcessor.OnPreprocessSimulator;
			runner.OnPostprocess = iOSBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="BuildAndRunSimulator" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/iOS: Build and Run (Simulator)", true)]
		public static bool ValidateBuildAndRunSimulator() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.iOS);
		}
		
		/// <summary>
		/// Builds the project and runs it on an iOS device with Development and Profiler flags.
		/// </summary>
		[MenuItem("Sago/Build/iOS: Build and Run (Profile)", false, 9000)]
		public static void BuildAndRunDeviceWithProfile() {
			iOSBuildRunner runner = new iOSBuildRunner();
			runner.BuildOptions = (
				BuildOptions.SymlinkLibraries |
				BuildOptions.Development |
				BuildOptions.ConnectWithProfiler |
				BuildOptions.AllowDebugging
			);
			runner.BuildAction = iOSBuildAction.BuildAndRun;
			runner.BuildSdkVersion = iOSSdkVersion.DeviceSDK;
			runner.OnPreprocess = iOSBuildProcessor.OnPreprocessDevice;
			runner.OnPostprocess = iOSBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="BuildAndRunDeviceWithProfile" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/iOS: Build and Run (Profile)", true)]
		public static bool ValidateBuildAndRunDeviceWithProfile() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.iOS);
		}
		
		/// <summary>
		/// Builds the project so it's ready to archive for AdHoc.
		/// </summary>
		[MenuItem("Sago/Build/iOS: Build and Archive (Ad Hoc)", false, 9000)]
		public static void BuildAndArchiveAdHoc() {
			iOSBuildRunner runner = new iOSBuildRunner();
			runner.BuildOptions = (
				BuildOptions.SymlinkLibraries 
			);
			runner.BuildAction = iOSBuildAction.BuildAndArchive;
			runner.BuildSdkVersion = iOSSdkVersion.DeviceSDK;
			runner.OnPreprocess = iOSBuildProcessor.OnPreprocessAdHoc;
			runner.OnPostprocess = iOSBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="BuildAndArchiveAdHoc" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/iOS: Build and Archive (Ad Hoc)", true)]
		public static bool ValidateBuildArchiveAdHoc() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.iOS);
		}
		
		/// <summary>
		/// Builds the project so it's ready to archive for AppStore.
		/// </summary>
		[MenuItem("Sago/Build/iOS: Build and Archive (App Store)", false, 9000)]
		public static void BuildAndArchiveAppStore() {
			iOSBuildRunner runner = new iOSBuildRunner();
			runner.BuildOptions = (
				BuildOptions.SymlinkLibraries 
			);
			runner.BuildAction = iOSBuildAction.BuildAndArchive;
			runner.BuildSdkVersion = iOSSdkVersion.DeviceSDK;
			runner.OnPreprocess = iOSBuildProcessor.OnPreprocessAppStore;
			runner.OnPostprocess = iOSBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="BuildAndArchiveAppStore" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/iOS: Build and Archive (App Store)", true)]
		public static bool ValidateBuildArchiveAppStore() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.iOS);
		}
		
		#endregion
		
		
		#region Build Callbacks

		/// <summary>
		/// Static preprocess callback method for AdHoc builds.
		/// </summary>
		public static void OnPreprocessAdHoc(BuildManifestObject manifest) {
			#if (UNITY_CLOUD_BUILD || TEAMCITY_BUILD) && SAGO_DEVELOPMENT
				EditorUserBuildSettings.development = true;
				EditorUserBuildSettings.allowDebugging = true;
			#endif
			iOSBuildProcessor processor = new iOSBuildProcessor();
			processor.BuildManifest = manifest;
			processor.BuildType = iOSBuildType.AdHoc;
			processor.ReleaseCodeSigningIdentity = AdHocCodeSigningIdentity;
			processor.ReleaseProvisioningProfile = AdHocProvisioningProfile;
			OnPreprocess(processor);
		}
		
		/// <summary>
		/// Static preprocess callback method for AppStore builds.
		/// </summary>
		public static void OnPreprocessAppStore(BuildManifestObject manifest) {
			iOSBuildProcessor processor = new iOSBuildProcessor();
			processor.BuildManifest = manifest;
			processor.BuildType = iOSBuildType.AppStore;
			processor.ReleaseCodeSigningIdentity = AppStoreCodeSigningIdentity;
			processor.ReleaseProvisioningProfile = AppStoreProvisioningProfile;
			OnPreprocess(processor);
		}
		
		/// <summary>
		/// Static preprocess callback method for device builds.
		/// </summary>
		public static void OnPreprocessDevice(BuildManifestObject manifest) {
			iOSBuildProcessor processor = new iOSBuildProcessor();
			processor.BuildManifest = manifest;
			processor.BuildType = iOSBuildType.Device;
			processor.ReleaseCodeSigningIdentity = DeveloperCodeSigningIdentity;
			processor.ReleaseProvisioningProfile = DeveloperProvisioningProfile;
			OnPreprocess(processor);
		}
		
		/// <summary>
		/// Static preprocess callback method for simulator builds.
		/// </summary>
		public static void OnPreprocessSimulator(BuildManifestObject manifest) {
			iOSBuildProcessor processor = new iOSBuildProcessor();
			processor.BuildManifest = manifest;
			processor.BuildType = iOSBuildType.Simulator;
			processor.ReleaseCodeSigningIdentity = DeveloperCodeSigningIdentity;
			processor.ReleaseProvisioningProfile = DeveloperProvisioningProfile;
			OnPreprocess(processor);
		}
		
		#endregion
		
		
		#region Public Methods
		
		/// <summary>
		/// Reads the plist from the default location.	After modifying it,
		/// save it back with <see cref="WritePlist()"/>.
		/// </summary>
		/// <returns>The plist.</returns>
		public Dictionary<string,object> ReadPlist() {
			
			return (Dictionary<string, object>)Plist.readPlist(this.XcodePlistPath);
		}
		
		/// <summary>
		/// Writes the plist to the default location.  See <see cref="ReadPlist()"/>.
		/// </summary>
		/// <param name="dict">Modified dictionary returned from <see cref="ReadPlist()"/></param>
		public void WritePlist(Dictionary<string,object> dict) {
			
			Plist.writeXml(dict, this.XcodePlistPath);
		}
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets the build type.
		/// </summary>
		protected virtual iOSBuildType BuildType {
			get;
			set;
		}
		
		/// <summary>
		/// Gets the short bundle version.
		/// </summary>
		protected virtual string BundleShortVersion {
			get {
				
				// NOTE: (2017-05-03) 
				// We decided to change the BundleShortVersion to include the build number 
				// so that we can tell the difference builds in Unity's crash reporting. 
				// One side effect is that our version numbers in iTunes Connect and the 
				// app store will also include the build number. It's not ideal, but the
				// utility we get from the crash reporting is worth it.
				
				// Version value;
				// value = new Version(this.BundleVersion);
				// value = new Version(value.Major, value.Minor);
				// return value.ToString();
				
				return this.BundleVersion;
				
			}
		}
		
		/// <summary>
		/// Gets the bundle version.
		/// </summary>
		protected virtual string BundleVersion {
			get {
			
				ProductInfo info;
				info = PlatformUtil.GetSettings<ProductInfo>();
				
				Version infoVersion;
				infoVersion = new Version(info ? info.Version : "0.0.0");
				infoVersion = new Version(
					Mathf.Max(infoVersion.Major, 1), 
					Mathf.Max(infoVersion.Minor, 0),
					Mathf.Max(info ? info.Build : 0, 0)
				);
				
				Version playerVersion;
				playerVersion = new Version(PlayerSettings.bundleVersion);
				playerVersion = new Version(
					Mathf.Max(playerVersion.Major, 1), 
					Mathf.Max(playerVersion.Minor, 0), 
					Mathf.Max(playerVersion.Build, 0)
				);
				
				Version value = new Version(
					info ? infoVersion.Major : playerVersion.Major,
					info ? infoVersion.Minor : playerVersion.Minor,
					info ? infoVersion.Build : playerVersion.Build
				);
				
				return value.ToString();
				
			}
		}
		
		/// <summary>
		/// Gets the absolute path to the external native assets (i.e. the native
		/// assets that are checked into the SagoBuild repo).
		/// </summary>
		virtual public string ExternalNativeAssetsPath {
			get { return Path.Combine(this.ProjectPath, Path.Combine(SubmoduleMap.GetSubmodulePath(typeof(SagoBuild.SubmoduleInfo)), "iOS/.Native")); }
		}
		
		/// <summary>
		/// Gets the absolute path to the project native assets (i.e. the native
		/// assets that are checked into the project repo).
		/// </summary>
		virtual public string ProjectNativeAssetsPath {
			get { return Path.Combine(this.ProjectPath, "Assets/Build/iOS/.Native"); }
		}
		
		/// <summary>
		/// Gets or sets the codesigning identity to use for the Xcode project's debug build configuration.
		/// </summary>
		virtual public string DebugCodeSigningIdentity {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the provisioning profile to use for the Xcode project's debug build configuration.
		/// </summary>
		virtual public string DebugProvisioningProfile {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the codesigning identity to use for the Xcode project's release build configuration.
		/// </summary>
		virtual public string ReleaseCodeSigningIdentity {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the provisioning profile to use for the Xcode project's release build configuration.
		/// </summary>
		virtual public string ReleaseProvisioningProfile {
			get; set;
		}
		
		/// <summary>
		/// Gets the path to the group in the Xcode project that assets should be added to.
		/// </summary>
		virtual public string XcodeGroupPath {
			get { return "SagoBuild"; }
		}
		
		/// <summary>
		/// Gets the path Xcode project's info plist.
		/// </summary>
		virtual public string XcodePlistPath {
			get { return string.IsNullOrEmpty(this.BuildPath) ? null : Path.Combine(this.BuildPath, "Info.plist"); }
		}
		
		/// <summary>
		/// Gets the path to the Xcode project.
		/// </summary>
		virtual public string XcodeProjectPath {
			get { return string.IsNullOrEmpty(this.BuildPath) ? null : this.BuildPath; }
		}
		
		#endregion
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new iOSBuildProcessor instance.
		/// </summary>
		public iOSBuildProcessor() {
			this.BuildPlatform = Platform.iOS;
			this.DebugCodeSigningIdentity = DeveloperCodeSigningIdentity;
			this.DebugProvisioningProfile = DeveloperProvisioningProfile;
			this.ReleaseCodeSigningIdentity = string.Empty;
			this.ReleaseProvisioningProfile = string.Empty;
		}
		
		#endregion
		
		
		#region IBuildProcessor Methods
		
		/// <summary>
		/// See <see cref="BuildProcessor.Preprocess" />.
		/// </summary>
		override public void Preprocess() {
			base.Preprocess();
			this.PreprocessNativeAssets();
			this.PreprocessPlayerSettings();
		}
		
		/// <summary>
		/// See <see cref="BuildProcessor.Postprocess" />.
		/// </summary>
		override public void Postprocess(string buildPath) {
			base.Postprocess(buildPath);
			this.PostprocessRegisterMonoModules();
			this.PostprocessXcodeProject();
			this.PostprocessXcodePlist();
		}
		
		#endregion
		
		
		#region Internal Methods
		
		/// <summary>
		/// Removes the specified launch screen image.
		/// </summary>
		protected virtual void DeleteAndRemoveFromProject(string path, PBXProject project, string projectPath) {
			File.Delete(path);
			project.RemoveFile(project.FindFileGuidByProjectPath(projectPath));
		}
		
		/// <summary>
		/// Copies native assets from the submodule to the project.
		/// </summary>
		protected virtual void PreprocessNativeAssets() {
			CopyAsset(this.ExternalNativeAssetsPath, this.ProjectNativeAssetsPath, "GrowToolSettings.plist");
			CopyAsset(this.ExternalNativeAssetsPath, this.ProjectNativeAssetsPath, "Images.xcassets");
			CopyAsset(this.ExternalNativeAssetsPath, this.ProjectNativeAssetsPath, "Settings.bundle");
			CopyAsset(this.ExternalNativeAssetsPath, this.ProjectNativeAssetsPath, "Native.md");
			AssetDatabase.Refresh();
		}
		
		protected virtual void PreprocessPlayerSettings() {
			
			iOSProductInfo info = PlatformUtil.GetSettings<iOSProductInfo>();
			
			#if !SAGO_BUILD_DO_NOT_USE_VERSION_SERVICE
				if (!VersionService.Bump(info)) {
					Debug.LogWarning("Could not bump build number.", info);
					switch (this.BuildType) {
						case iOSBuildType.Simulator:
						case iOSBuildType.Device:
							Debug.LogWarning("Could not bump build number");
							break;
						default:
							throw new System.InvalidOperationException("Could not bump build number");
					}
				}
			#endif
			
			PlayerSettings.productName = info.DisplayName;
			PlayerSettings.applicationIdentifier = info.Identifier;
			PlayerSettings.bundleVersion = this.BundleVersion;
			
			if (string.IsNullOrEmpty(info.UrlScheme)) {
				throw new System.Exception("UrlScheme property is missing from iOS platform prefab");
			}
			
			// short bundle version
			// the short bundle version property was added in Unity 4.6.3, so 
			// set it via reflection to play nice with older versions of Unity.
			PropertyInfo shortBundleVersion = typeof(PlayerSettings).GetProperty(
				"shortBundleVersion", 
				BindingFlags.Public | BindingFlags.Static
			);
			if (shortBundleVersion != null) {
				shortBundleVersion.SetValue(null, this.BundleShortVersion, null);
			}
			
		}
		
		/// <summary>
		/// Updates RegisterMonoModules.cpp so that native plugins work in the simulator.
		/// This fix is only required when using the Mono scripting backend, not IL2CPP
		/// http://answers.unity3d.com/questions/768959/why-are-unity-plugins-disabled-on-the-ios-simulato.html
		/// </summary>
		protected virtual void PostprocessRegisterMonoModules() {
			#if UNITY_IOS && !(UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
				if (PlayerSettings.iOS.sdkVersion == iOSSdkVersion.SimulatorSDK) {
				
					string path;
					path = Path.Combine(this.XcodeProjectPath, "Libraries/RegisterMonoModules.cpp");
					
					List<string> lines;
					lines = new List<string>(File.ReadAllLines(path));
					
					bool requiresFix;
					requiresFix = lines.FindIndex(line => line.StartsWith(@"#if !(TARGET_IPHONE_SIMULATOR)")) != -1;
					
					if (requiresFix) {
						int startIndex = 0;
						int srcIndex = lines.FindIndex(startIndex, line => line.EndsWith(@"mono_dl_register_symbol (const char* name, void *addr);"));
						string temp = Regex.Replace(lines[srcIndex], @"\t+", "\t");
						lines.RemoveAt(srcIndex);
						int dstIndex = lines.FindIndex(startIndex, line => line.StartsWith(@"#endif // !(TARGET_IPHONE_SIMULATOR)")) + 1;
						lines.Insert(dstIndex, temp);
					}
					
					if (requiresFix) {
						int startIndex = lines.FindIndex(line => line.Equals("DLL_EXPORT void RegisterMonoModules()"));
						int srcIndex = lines.FindIndex(startIndex, line => line.StartsWith(@"#endif // !(TARGET_IPHONE_SIMULATOR)"));
						string temp = lines[srcIndex];
						lines.RemoveAt(srcIndex);
						int dstIndex = lines.FindIndex(startIndex, line => line.Equals(@""));
						lines.Insert(dstIndex, temp);
					}
					
					if (requiresFix) {
						File.WriteAllLines(path, lines.ToArray());
					}
					
				}
			#endif
		}
		
		/// <summary>
		/// Updates the Xcode project by adding the grow tool and updating the build settings.
		/// </summary>
		protected virtual void PostprocessXcodeProject() {
			PBXProject project;
			project = new PBXProject();
			project.ReadFromFile(PBXProject.GetPBXProjectPath(this.XcodeProjectPath));
			
			string target;
			target = project.TargetGuidByName(PBXProject.GetUnityTargetName());
			
			// add frameworks
			project.AddFrameworkToProject(target, "Accelerate.framework", false);
			project.AddFrameworkToProject(target, "CoreGraphics.framework", false);
			project.AddFrameworkToProject(target, "CoreTelephony.framework", false);
			project.AddFrameworkToProject(target, "CoreText.framework", false);
			project.AddFrameworkToProject(target, "Foundation.framework", false);
			project.AddFrameworkToProject(target, "MediaPlayer.framework", false);
			project.AddFrameworkToProject(target, "QuartzCore.framework", false);
			project.AddFrameworkToProject(target, "SystemConfiguration.framework", false);
			project.AddFrameworkToProject(target, "UIKit.framework", false);
			project.AddFrameworkToProject(target, "MessageUI.framework", false);

			// add files (project)
			project.AddFileToBuild(target, project.AddFile(
				Path.Combine(this.ProjectNativeAssetsPath, "Settings.bundle"),
				Path.Combine(this.XcodeGroupPath, "Settings.bundle"),
				PBXSourceTree.Source
			));

			// add localized app names to project
			string appResources;
			appResources = Path.Combine(this.ProjectNativeAssetsPath, "LocalizedResources");
			if (Directory.Exists(appResources)) {
				foreach (var langFolder in Directory.GetDirectories(appResources)) {
					project.AddFileToBuild(target, project.AddFile(
						langFolder,
						Path.GetFileName(langFolder)
					));
				}
			} else {
				string appNames;
				appNames = Path.Combine(this.ProjectNativeAssetsPath, "LocalizedAppName");
				foreach (var langFolder in Directory.GetDirectories(appNames)) {
					project.AddFileToBuild(target, project.AddFile(
						langFolder,
						Path.GetFileName(langFolder)
					));
				}
			}

			// replace files (project)
			CopyAsset(
				this.ProjectNativeAssetsPath, 
				Path.Combine(this.XcodeProjectPath, PBXProject.GetUnityTargetName()), 
				"Images.xcassets",
				true
			);

			// remove default launch screen
			project.RemoveFile(project.FindFileGuidByProjectPath("LaunchScreen.xib"));
			project.RemoveFile(project.FindFileGuidByProjectPath("LaunchScreenImage-Landscape.png"));
			project.RemoveFile(project.FindFileGuidByProjectPath("LaunchScreenImage-Portrait.png"));
			File.Delete(Path.Combine(this.XcodeProjectPath, "LaunchScreen.xib"));
			File.Delete(Path.Combine(this.XcodeProjectPath, "LaunchScreenImage-Landscape.png"));
			File.Delete(Path.Combine(this.XcodeProjectPath, "LaunchScreenImage-Portrait.png"));
			
			// add files (external)
			project.AddFileToBuild(target, project.AddFile(
				Path.Combine(this.ExternalNativeAssetsPath, "LaunchScreen.xib"),
				"LaunchScreen.xib",
				PBXSourceTree.Source
			));

			// update build settings
			project.SetBuildProperty(target, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
			project.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
			
			// set the development team when building locally (this should also be fine 
			// on Cloud Build, but Cloud Build seems to work fine without out it).
			#if !(UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
				project.SetBuildProperty(target, "DEVELOPMENT_TEAM", "SF8M4XHC7B");
			#endif

			#if SAGO_DISABLE_BITCODE
				project.SetBuildProperty(target, "ENABLE_BITCODE", "No");
			#endif

			// NOTE: Xcode splits the values for FRAMEWORK_SEARCH_PATHS on spaces, so if 
			// a value contains spaces it needs to be quoted. However, AddBuildProperty 
			// and SetBuildProperty will strip quotes from the value if it begins and ends 
			// with a quote. To prevent the quotes from being stripped, use string.Format
			// with $(inherited) at the beginning instead of using separate calls to 
			// AddBuildProperty.
			project.SetBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", string.Format(
				"$(inherited) \"{0}\" \"{1}\"", 
				this.ProjectNativeAssetsPath, 
				this.ExternalNativeAssetsPath
			));
			
			// code signing and provisioning profiles
			string debugBuildConfig = project.BuildConfigByName(target, "Debug");
			if (!this.IsCloudBuild) {
				project.SetBuildPropertyForConfig(debugBuildConfig, "CODE_SIGN_IDENTITY[sdk=iphoneos*]", this.DebugCodeSigningIdentity);
				project.SetBuildPropertyForConfig(debugBuildConfig, "PROVISIONING_PROFILE", this.DebugProvisioningProfile);
			}
			
			// debug preprocessor macro (run TBGrowTool in development mode using the 
			// development Mixpanel token so we can confirm that Mixpanel is receiving 
			// data for all events).
			project.SetBuildPropertyForConfig(debugBuildConfig, "GCC_PREPROCESSOR_DEFINITIONS", "$(inherited)");
			project.SetBuildPropertyForConfig(debugBuildConfig, "GCC_PREPROCESSOR_DEFINITIONS", "DEBUG");
			
			string releaseBuildConfig = project.BuildConfigByName(target, "Release");
			if (!this.IsCloudBuild) {
				project.SetBuildPropertyForConfig(releaseBuildConfig, "CODE_SIGN_IDENTITY[sdk=iphoneos*]", this.ReleaseCodeSigningIdentity);
				project.SetBuildPropertyForConfig(releaseBuildConfig, "PROVISIONING_PROFILE", this.ReleaseProvisioningProfile);
			}
			
			#if SAGO_DEBUG && !(UNITY_CLOUD_BUILD || TEAMCITY_BUILD)
				// make local debug builds faster by only building the active architecture 
				project.SetBuildPropertyForConfig(debugBuildConfig, "ONLY_ACTIVE_ARCH", "YES");
				project.SetBuildPropertyForConfig(releaseBuildConfig, "ONLY_ACTIVE_ARCH", "YES");
			#endif
			
			// write xcode project
			project.WriteToFile(PBXProject.GetPBXProjectPath(this.XcodeProjectPath));
			
		}
		
		/// <summary>
		/// Updates the info plist.
		/// </summary>
		protected virtual void PostprocessXcodePlist() {
			
			// read plist
			Dictionary<string, object> dict;
			dict = ReadPlist();
			
			// cleanup plist
			dict.Remove("CFBundleIconFiles");

			// remove default launch image entries from the plist -
			// otherwise we get a black launch screen.
			// without these values, launch uses our LaunchScreen.xib file (white screen)
			dict.Remove("UILaunchStoryboardName~ipad");
			dict.Remove("UILaunchStoryboardName~iphone");
			dict.Remove("UILaunchStoryboardName~ipod");
			
			// update plist
			dict["UIPrerenderedIcon"] = true;
			dict["CFBundleAllowMixedLocalizations"] = true;

			iOSProductInfo productInfo = PlatformUtil.GetSettings<iOSProductInfo>(SagoPlatform.PlatformUtil.ActivePlatform);
			object urlScheme = productInfo.UrlScheme;

			dict["CFBundleURLTypes"] = new List<object> {
				new Dictionary<string,object> {
                    { "CFBundleURLName", PlayerSettings.applicationIdentifier },
					{ "CFBundleURLSchemes", new List<object> { urlScheme } }
				}
			};

			dict["LSApplicationQueriesSchemes"] = iOSLSApplicationQueriesSchemes.Schemes;
			dict["CFBundleShortVersionString"] = this.BundleShortVersion;
			dict["CFBundleVersion"] = this.BundleVersion;
			dict["UILaunchStoryboardName"] = "LaunchScreen";

			// Inform App Store submission/Testflight that we do not use any non-standard encryption.
			dict["ITSAppUsesNonExemptEncryption"] = false;
			
			dict["NSPhotoLibraryUsageDescription"] = "This app would like to save photos to your camera roll.";
			
			dict["LSHasLocalizedDisplayName"] = true;

			// write plist
			WritePlist(dict);
			
		}
		
		#endregion
		
		
	}
	
}

#endif
