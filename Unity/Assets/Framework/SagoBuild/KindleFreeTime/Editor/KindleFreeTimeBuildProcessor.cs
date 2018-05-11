namespace SagoBuildEditor.KindleFreeTime {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The KindleFreeTimeBuildProcessor class implements functionality for processing KindleFreeTime builds.
	/// </summary>
	public class KindleFreeTimeBuildProcessor : AndroidBuildProcessor {
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project for KindleFreeTime.
		/// </summary>
		[MenuItem("Sago/Build/KindleFreeTime: Build", false, 12000)]
		public static void Build() {
			BuildRunner runner = new KindleFreeTimeBuildRunner();
			runner.OnPreprocess = KindleFreeTimeBuildProcessor.OnPreprocess;
			runner.OnPostprocess = KindleFreeTimeBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/KindleFreeTime: Build", true)]
		protected static bool ValidateBuild() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.KindleFreeTime);
		}
		
		/// <summary>
		/// Builds the project for KindleFreeTime.
		/// </summary>
		[MenuItem("Sago/Build/KindleFreeTime: Build And Run", false, 12000)]
		public static void BuildAndRun() {
			BuildRunner runner = new KindleFreeTimeBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer
			);
			runner.OnPreprocess = KindleFreeTimeBuildProcessor.OnPreprocess;
			runner.OnPostprocess = KindleFreeTimeBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/KindleFreeTime: Build And Run", true)]
		protected static bool ValidateBuildAndRun() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.KindleFreeTime);
		}
		
		#endregion
		
		
		#region Build Callbacks
		
		/// <summary>
		/// Static preprocess callback method.
		/// </summary>
		public static void OnPreprocess(BuildManifestObject manifest) {
			KindleFreeTimeBuildProcessor processor = new KindleFreeTimeBuildProcessor();
			processor.BuildManifest = manifest;
			BuildProcessor.OnPreprocess(processor);
		}
		
		#endregion
		
		
		#region Constructors
		
		public KindleFreeTimeBuildProcessor() : base() {
			this.BuildPlatform = Platform.KindleFreeTime;
		}
		
		#endregion
		
		
	}
	
}