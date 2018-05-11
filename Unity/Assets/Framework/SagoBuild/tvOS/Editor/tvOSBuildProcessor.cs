#if UNITY_TVOS

namespace SagoBuildEditor.tvOS {
	
	using PlistCS;
	using SagoCore.Submodules;
	using SagoBuildEditor.Core;
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
	/// The tvOSBuildProcessor class implements functionality for processing tvOS builds.
	/// </summary>
	public class tvOSBuildProcessor : BuildProcessor {
		
		
		#region Constants
		
		/// <summary>
		/// The code signing identity to use for AdHoc builds.
		/// </summary>
		protected const string AdHocCodeSigningIdentity = "iPhone Distribution: Toca Boca Inc.";
		
		/// <summary>
		/// The provisioning profile to use for AdHoc builds.
		/// </summary>
		protected const string AdHocProvisioningProfile = "ad21062e-a1bc-48fa-bfdb-9a7cd77e95ba";
		
		/// <summary>
		/// The code signing identity to use for app store builds.
		/// </summary>
		protected const string AppStoreCodeSigningIdentity = "iPhone Distribution: Sago Sago Toys Inc. (SF8M4XHC7B)";
		
		/// <summary>
		/// The provisioning profile to use for AppStore builds.
		/// </summary>
		protected static string AppStoreProvisioningProfile {
			get {
				tvOSProductInfo info = PlatformUtil.GetSettings<tvOSProductInfo>();
				return info ? info.AppStoreProvisioningProfile : null;
			}
		}
		
		/// <summary>
		/// The code signing identity to use for developer builds.
		/// </summary>
		protected const string DeveloperCodeSigningIdentity = "iPhone Developer";
		
		/// <summary>
		/// The provisioning profile to use for developer builds.
		/// </summary>
		protected const string DeveloperProvisioningProfile = "";
		
		#endregion
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project and runs it on an tvOS device.
		/// </summary>
		[MenuItem("Sago/Build/tvOS: Build and Run (Device)", false, 16000)]
		public static void BuildAndRunDevice() {
			tvOSBuildRunner runner = new tvOSBuildRunner();
			runner.BuildOptions = (
				BuildOptions.SymlinkLibraries 
			);
			runner.BuildAction = tvOSBuildAction.BuildAndRun;
			runner.BuildSdkVersion = iOSSdkVersion.DeviceSDK;
			runner.OnPreprocess = tvOSBuildProcessor.OnPreprocessDevice;
			runner.OnPostprocess = tvOSBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="BuildAndRunDevice" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/tvOS: Build and Run (Device)", true)]
		public static bool ValidateBuildAndRunDevice() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.tvOS);
		}
		
		/// <summary>
		/// Builds the project and runs it in the tvOS simulator.
		/// </summary>
		[MenuItem("Sago/Build/tvOS: Build and Run (Simulator)", false, 16000)]
		public static void BuildAndRunSimulator() {
			tvOSBuildRunner runner = new tvOSBuildRunner();
			runner.BuildOptions = (
				BuildOptions.SymlinkLibraries 
			);
			runner.BuildAction = tvOSBuildAction.BuildAndRun;
			runner.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/tvOSSimulator");
			runner.BuildSdkVersion = iOSSdkVersion.SimulatorSDK;
			runner.OnPreprocess = tvOSBuildProcessor.OnPreprocessSimulator;
			runner.OnPostprocess = tvOSBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="BuildAndRunSimulator" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/tvOS: Build and Run (Simulator)", true)]
		public static bool ValidateBuildAndRunSimulator() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.tvOS);
		}
		
		/// <summary>
		/// Builds the project and runs it on an tvOS device with Development and Profiler flags.
		/// </summary>
		[MenuItem("Sago/Build/tvOS: Build and Run (Profile)", false, 16000)]
		public static void BuildAndRunDeviceWithProfile() {
			tvOSBuildRunner runner = new tvOSBuildRunner();
			runner.BuildOptions = (
				BuildOptions.SymlinkLibraries |
				BuildOptions.Development |
				BuildOptions.ConnectWithProfiler
			);
			runner.BuildAction = tvOSBuildAction.BuildAndRun;
			runner.BuildSdkVersion = iOSSdkVersion.DeviceSDK;
			runner.OnPreprocess = tvOSBuildProcessor.OnPreprocessDevice;
			runner.OnPostprocess = tvOSBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="BuildAndRunDeviceWithProfile" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/tvOS: Build and Run (Profile)", true)]
		public static bool ValidateBuildAndRunDeviceWithProfile() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.tvOS);
		}
		
		/// <summary>
		/// Builds the project so it's ready to archive for AdHoc.
		/// </summary>
		[MenuItem("Sago/Build/tvOS: Build and Archive (Ad Hoc)", false, 16000)]
		public static void BuildAndArchiveAdHoc() {
			tvOSBuildRunner runner = new tvOSBuildRunner();
			runner.BuildOptions = (
				BuildOptions.SymlinkLibraries 
			);
			runner.BuildAction = tvOSBuildAction.BuildAndArchive;
			runner.BuildSdkVersion = iOSSdkVersion.DeviceSDK;
			runner.OnPreprocess = tvOSBuildProcessor.OnPreprocessAdHoc;
			runner.OnPostprocess = tvOSBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="BuildAndArchiveAdHoc" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/tvOS: Build and Archive (Ad Hoc)", true)]
		public static bool ValidateBuildArchiveAdHoc() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.tvOS);
		}
		
		/// <summary>
		/// Builds the project so it's ready to archive for AppStore.
		/// </summary>
		[MenuItem("Sago/Build/tvOS: Build and Archive (App Store)", false, 16000)]
		public static void BuildAndArchiveAppStore() {
			tvOSBuildRunner runner = new tvOSBuildRunner();
			runner.BuildOptions = (
				BuildOptions.SymlinkLibraries 
			);
			runner.BuildAction = tvOSBuildAction.BuildAndArchive;
			runner.BuildSdkVersion = iOSSdkVersion.DeviceSDK;
			runner.OnPreprocess = tvOSBuildProcessor.OnPreprocessAppStore;
			runner.OnPostprocess = tvOSBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="BuildAndArchiveAppStore" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/tvOS: Build and Archive (App Store)", true)]
		public static bool ValidateBuildArchiveAppStore() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.tvOS);
		}
		
		#endregion
		
		
		#region Build Callbacks
		
		/// <summary>
		/// Static preprocess callback method for AdHoc builds.
		/// </summary>
		public static void OnPreprocessAdHoc(BuildManifestObject manifest = null) {
			tvOSBuildProcessor processor = new tvOSBuildProcessor();
			processor.BuildManifest = manifest;
			processor.BuildType = tvOSBuildType.AdHoc;
			processor.ReleaseCodeSigningIdentity = AdHocCodeSigningIdentity;
			processor.ReleaseProvisioningProfile = AdHocProvisioningProfile;
			OnPreprocess(processor);
		}
		
		/// <summary>
		/// Static preprocess callback method for AppStore builds.
		/// </summary>
		public static void OnPreprocessAppStore(BuildManifestObject manifest = null) {
			tvOSBuildProcessor processor = new tvOSBuildProcessor();
			processor.BuildManifest = manifest;
			processor.BuildType = tvOSBuildType.AppStore;
			processor.ReleaseCodeSigningIdentity = AppStoreCodeSigningIdentity;
			processor.ReleaseProvisioningProfile = AppStoreProvisioningProfile;
			OnPreprocess(processor);
		}
		
		/// <summary>
		/// Static preprocess callback method for device builds.
		/// </summary>
		public static void OnPreprocessDevice(BuildManifestObject manifest = null) {
			tvOSBuildProcessor processor = new tvOSBuildProcessor();
			processor.BuildManifest = manifest;
			processor.BuildType = tvOSBuildType.Device;
			processor.ReleaseCodeSigningIdentity = DeveloperCodeSigningIdentity;
			processor.ReleaseProvisioningProfile = DeveloperProvisioningProfile;
			OnPreprocess(processor);
		}
		
		/// <summary>
		/// Static preprocess callback method for simulator builds.
		/// </summary>
		public static void OnPreprocessSimulator(BuildManifestObject manifest = null) {
			tvOSBuildProcessor processor = new tvOSBuildProcessor();
			processor.BuildManifest = manifest;
			processor.BuildType = tvOSBuildType.Simulator;
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
		/// Gets and sets the build type.
		/// </summary>
		protected virtual tvOSBuildType BuildType {
			get;
			set;
		}
		
		/// <summary>
		/// Gets the short bundle version.
		/// </summary>
		protected virtual string BundleShortVersion {
			get {
				Version value;
				value = new Version(this.BundleVersion);
				value = new Version(value.Major, value.Minor);
				return value.ToString();
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
			get { return Path.Combine(this.ProjectPath, Path.Combine(SubmoduleMapEditorAdaptor.GetSubmodulePath(typeof(SagoBuild.SubmoduleInfo)), "tvOS/.Native")); }
		}
		
		/// <summary>
		/// Gets the absolute path to the project native assets (i.e. the native
		/// assets that are checked into the project repo).
		/// </summary>
		virtual public string ProjectNativeAssetsPath {
			get { return Path.Combine(this.ProjectPath, "Assets/Build/tvOS/.Native"); }
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
		/// Creates a new tvOSBuildProcessor instance.
		/// </summary>
		public tvOSBuildProcessor() {
			this.BuildPlatform = Platform.tvOS;
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
			this.PostprocessXcodeProject();
			this.PostprocessXcodePlist();
			this.PostprocessXcodeSources();
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
			CopyAsset(this.ExternalNativeAssetsPath, this.ProjectNativeAssetsPath, "Images.xcassets");
			AssetDatabase.Refresh();
		}
		
		protected virtual void PreprocessPlayerSettings() {
			
			tvOSProductInfo info = PlatformUtil.GetSettings<tvOSProductInfo>();
			
			#if !SAGO_BUILD_DO_NOT_USE_VERSION_SERVICE
				if (!VersionService.Bump(info)) {
					Debug.LogWarning("Could not bump build number.", info);
					switch (this.BuildType) {
						case tvOSBuildType.Simulator:
						case tvOSBuildType.Device:
							Debug.LogWarning("Could not bump build number");
							break;
						default:
							throw new System.InvalidOperationException("Could not bump build number");
					}
				}
			#endif
			
			PlayerSettings.productName = info.DisplayName;
			PlayerSettings.bundleIdentifier = info.Identifier;
			PlayerSettings.bundleVersion = this.BundleVersion;
			
			if (string.IsNullOrEmpty(info.UrlScheme)) {
				throw new System.Exception("UrlScheme property is missing from tvOS platform prefab");
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
			project.AddFrameworkToProject(target, "CoreText.framework", false);
			project.AddFrameworkToProject(target, "Foundation.framework", false);
			project.AddFrameworkToProject(target, "MediaPlayer.framework", false);
			project.AddFrameworkToProject(target, "QuartzCore.framework", false);
			project.AddFrameworkToProject(target, "SystemConfiguration.framework", false);
			project.AddFrameworkToProject(target, "UIKit.framework", false);

			// add files (project)
			if (Directory.Exists(Path.Combine(this.ProjectNativeAssetsPath, "Settings.bundle"))) {
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(this.ProjectNativeAssetsPath, "Settings.bundle"),
					Path.Combine(this.XcodeGroupPath, "Settings.bundle"),
					PBXSourceTree.Source
				));
			}

			// replace files (project)
			CopyAsset(
				this.ProjectNativeAssetsPath, 
				Path.Combine(this.XcodeProjectPath, PBXProject.GetUnityTargetName()), 
				"Images.xcassets",
				true
			);

			// update build settings
			project.SetBuildProperty(target, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
			project.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
			
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
			
			// update plist
			dict["UIPrerenderedIcon"] = true;
			dict["CFBundleAllowMixedLocalizations"] = true;

			tvOSProductInfo productInfo = PlatformUtil.GetSettings<tvOSProductInfo>(SagoPlatform.PlatformUtil.ActivePlatform);
			object urlScheme = productInfo.UrlScheme;

			dict["CFBundleURLTypes"] = new List<object> {
				new Dictionary<string,object> {
					{ "CFBundleURLName", PlayerSettings.iPhoneBundleIdentifier },
					{ "CFBundleURLSchemes", new List<object> { urlScheme } }
				}
			};

			dict["LSApplicationQueriesSchemes"] = SagoBuildEditor.iOS.iOSLSApplicationQueriesSchemes.Schemes;
			dict["CFBundleShortVersionString"] = this.BundleShortVersion;
			dict["CFBundleVersion"] = this.BundleVersion;
			dict["UILaunchStoryboardName"] = "LaunchScreen";


			// Inform App Store submission/Testflight that we do not use any non-standard encryption.
			dict["ITSAppUsesNonExemptEncryption"] = false;

			// write plist
			WritePlist(dict);
			
		}
		
		/// <summary>
		/// Updates the c++ source files.
		/// </summary>
		protected virtual void PostprocessXcodeSources() {
			
			// DisplayManager.mm
			// As of Unity 5.3.0f2 seed 2, DisplayManager.mm throws a compiler error in Xcode 7.2 beta 4.
			// The code below comments out the line that throws the error.
			{
				string path = Path.Combine(this.XcodeProjectPath, "Classes/Unity/DisplayManager.mm");
				string[] lines = File.ReadAllLines(path);
				bool dirty = false;
				
				for (int index = 0; index < lines.Length; index++) {
					string line = lines[index];
					if (line.Contains("preferredMode")) {
						string commented = string.Format("// {0}", line);
						lines[index] = commented;
						dirty = true;
					}
				}
				
				if (dirty) {
					File.WriteAllLines(path, lines);
				}
				
			}
			
		}
		
		#endregion
		
		
	}
	
}

#endif