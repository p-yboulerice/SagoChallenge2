namespace SagoBuildEditor.Kindle {

	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The KindleBuildProcessor class implements functionality for processing Kindle builds.
	/// </summary>
	public class KindleBuildProcessor : AndroidBuildProcessor {
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project for Kindle.
		/// </summary>
		[MenuItem("Sago/Build/Kindle: Build", false, 11000)]
		public static void Build() {
			BuildRunner runner = new KindleBuildRunner();
			runner.OnPreprocess = KindleBuildProcessor.OnPreprocess;
			runner.OnPostprocess = KindleBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/Kindle: Build", true)]
		protected static bool ValidateBuild() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.Kindle);
		}
		
		/// <summary>
		/// Builds the project for Kindle.
		/// </summary>
		[MenuItem("Sago/Build/Kindle: Build And Run", false, 11000)]
		public static void BuildAndRun() {
			BuildRunner runner = new KindleBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer
			);
			runner.OnPreprocess = KindleBuildProcessor.OnPreprocess;
			runner.OnPostprocess = KindleBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/Kindle: Build And Run", true)]
		protected static bool ValidateBuildAndRun() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.Kindle);
		}

		/// <summary>
		/// Builds the project for Kindle.
		/// </summary>
		[MenuItem("Sago/Build/Kindle: Build And Run (Profiler)", false, 11000)]
		public static void BuildAndRunProfiler() {
			BuildRunner runner = new KindleBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer |
				BuildOptions.Development |
				BuildOptions.ConnectWithProfiler |
				BuildOptions.AllowDebugging
				);
			runner.OnPreprocess = KindleBuildProcessor.OnPreprocess;
			runner.OnPostprocess = KindleBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/Kindle: Build And Run (Profiler)", true)]
		protected static bool ValidateBuildAndRunProfiler() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.Kindle);
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
			KindleBuildProcessor processor = new KindleBuildProcessor();
			processor.BuildManifest = manifest;
			BuildProcessor.OnPreprocess(processor);
		}
		
		#endregion
		
		
		#region Constructors
		
		public KindleBuildProcessor() : base() {
			this.BuildPlatform = Platform.Kindle;
		}
		
		#endregion
		
		
	}
	
}