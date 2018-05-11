namespace SagoBuildEditor.GooglePlayFree {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The GooglePlayFreeBuildProcessor class implements functionality for processing GooglePlay Free builds.
	/// </summary>
	public class GooglePlayFreeBuildProcessor : AndroidBuildProcessor {
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project for GooglePlay Free.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay Free: Build", false, 10100)]
		public static void Build() {
			BuildRunner runner = new GooglePlayFreeBuildRunner();
			runner.OnPreprocess = GooglePlayFreeBuildProcessor.OnPreprocess;
			runner.OnPostprocess = GooglePlayFreeBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay Free: Build", true)]
		protected static bool ValidateBuild() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.GooglePlayFree);
		}
		
		/// <summary>
		/// Builds the project for GooglePlay Free.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay Free: Build And Run", false, 10100)]
		public static void BuildAndRun() {
			BuildRunner runner = new GooglePlayFreeBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer
			);
			runner.OnPreprocess = GooglePlayFreeBuildProcessor.OnPreprocess;
			runner.OnPostprocess = GooglePlayFreeBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay Free: Build And Run", true)]
		protected static bool ValidateBuildAndRun() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.GooglePlayFree);
		}

		/// <summary>
		/// Builds the project for GooglePlay.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay Free: Build And Run (Profiler)", false, 10100)]
		public static void BuildAndRunProfiler() {
			BuildRunner runner = new GooglePlayFreeBuildRunner();

			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer |
				BuildOptions.Development |
				BuildOptions.ConnectWithProfiler |
				BuildOptions.AllowDebugging
				);
			runner.OnPreprocess = GooglePlayFreeBuildProcessor.OnPreprocess;
			runner.OnPostprocess = GooglePlayFreeBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay Free: Build And Run (Profiler)", true)]
		protected static bool ValidateBuildAndRunProfiler() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.GooglePlayFree);
		}
		
		#endregion
		
		
		#region Build Callbacks
		
		/// <summary>
		/// Static preprocess callback method.
		/// </summary>
		public static void OnPreprocess(BuildManifestObject manifest) {
			#if (UNITY_CLOUD_BUILD || TEAMCITY_BUILD) && SAGO_DEVELOPMENT
				EditorUserBuildSettings.development = true;
				EditorUserBuildSettings.allowDebugging = true;
			#endif
			GooglePlayFreeBuildProcessor processor = new GooglePlayFreeBuildProcessor();
			processor.BuildManifest = manifest;
			BuildProcessor.OnPreprocess(processor);
		}
		
		#endregion
		
		
		#region Constructors
		
		public GooglePlayFreeBuildProcessor() : base() {
			this.BuildPlatform = Platform.GooglePlayFree;
		}
		
		#endregion

		#region IBuildProcessor Methods

		/// <summary>
		/// Overrides the preprocess method to apply AndroidProductInfo to the PlayerSettings.
		/// <summary>
		override public void Preprocess() {

			base.Preprocess();

			GooglePlayProductInfo info;
			info = PlatformUtil.GetSettings<GooglePlayProductInfo>();

			bool shouldUseExpansionFiles = ShouldUseExpansionFiles;

			if (info != null) {
				PlayerSettings.Android.useAPKExpansionFiles = shouldUseExpansionFiles;
			}

			// Check if the license key is provided.
			if (shouldUseExpansionFiles && string.IsNullOrEmpty(info.LicensePublicKey)) {
				throw new System.Exception("License Public Key is missing");
			}

			Debug.Log("GooglePlayDownloaderDebugMode=" + info.GooglePlayDownloaderDebugMode + " UseExpansionFile=" + shouldUseExpansionFiles + " LicensePublicKey=" + info.LicensePublicKey);
		}

		#endregion

		#region Helper Methods

		protected static bool ShouldUseExpansionFiles {

			get {
				bool defineSymbolSagoGooglePlayExpansionDownloader = false;
				#if SAGO_GOOGLE_PLAY_EXPANSION_DOWNLOADER
					defineSymbolSagoGooglePlayExpansionDownloader = true;
				#endif

				return defineSymbolSagoGooglePlayExpansionDownloader;
			}

		}

		#endregion
		
	}
	
}