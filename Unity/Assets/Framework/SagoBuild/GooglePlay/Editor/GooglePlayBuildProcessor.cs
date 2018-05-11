namespace SagoBuildEditor.GooglePlay {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The GooglePlayBuildProcessor class implements functionality for processing GooglePlay builds.
	/// </summary>
	public class GooglePlayBuildProcessor : AndroidBuildProcessor {
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project for GooglePlay.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay: Build", false, 10000)]
		public static void Build() {
			BuildRunner runner = new GooglePlayBuildRunner();
			runner.OnPreprocess = GooglePlayBuildProcessor.OnPreprocess;
			runner.OnPostprocess = GooglePlayBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay: Build", true)]
		protected static bool ValidateBuild() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.GooglePlay);
		}
		
		/// <summary>
		/// Builds the project for GooglePlay.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay: Build And Run", false, 10000)]
		public static void BuildAndRun() {
			BuildRunner runner = new GooglePlayBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer
			);
			runner.OnPreprocess = GooglePlayBuildProcessor.OnPreprocess;
			runner.OnPostprocess = GooglePlayBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay: Build And Run", true)]
		protected static bool ValidateBuildAndRun() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.GooglePlay);
		}

		/// <summary>
		/// Builds the project for GooglePlay.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay: Build And Run (Profiler)", false, 10000)]
		public static void BuildAndRunProfiler() {
			BuildRunner runner = new GooglePlayBuildRunner();

			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer |
				BuildOptions.Development |
				BuildOptions.ConnectWithProfiler |
				BuildOptions.AllowDebugging
				);
			runner.OnPreprocess = GooglePlayBuildProcessor.OnPreprocess;
			runner.OnPostprocess = GooglePlayBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/GooglePlay: Build And Run (Profiler)", true)]
		protected static bool ValidateBuildAndRunProfiler() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.GooglePlay);
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
			GooglePlayBuildProcessor processor = new GooglePlayBuildProcessor();
			processor.BuildManifest = manifest;
			BuildProcessor.OnPreprocess(processor);
		}
		
		#endregion
		
		
		#region Constructors
		
		public GooglePlayBuildProcessor() : base() {
			this.BuildPlatform = Platform.GooglePlay;
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