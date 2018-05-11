namespace SagoBuildEditor.Thales {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The ThalesBuildProcessor class implements functionality for processing Thales builds.
	/// </summary>
	public class ThalesBuildProcessor : AndroidBuildProcessor {
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project for Thales.
		/// </summary>
		[MenuItem("Sago/Build/Thales: Build", false, 12000)]
		public static void Build() {
			BuildRunner runner = new ThalesBuildRunner();
			runner.OnPreprocess = ThalesBuildProcessor.OnPreprocess;
			runner.OnPostprocess = ThalesBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/Thales: Build", true)]
		protected static bool ValidateBuild() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.Thales);
		}
		
		/// <summary>
		/// Builds the project for Thales.
		/// </summary>
		[MenuItem("Sago/Build/Thales: Build And Run", false, 12000)]
		public static void BuildAndRun() {
			BuildRunner runner = new ThalesBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer
			);
			runner.OnPreprocess = ThalesBuildProcessor.OnPreprocess;
			runner.OnPostprocess = ThalesBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/Thales: Build And Run", true)]
		protected static bool ValidateBuildAndRun() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.Thales);
		}
		
		#endregion
		
		
		#region Build Callbacks
		
		/// <summary>
		/// Static preprocess callback method.
		/// </summary>
		public static void OnPreprocess(BuildManifestObject manifest) {
			ThalesBuildProcessor processor = new ThalesBuildProcessor();
			processor.BuildManifest = manifest;
			BuildProcessor.OnPreprocess(processor);
		}
		
		#endregion
		
		
		#region Constructors
		
		public ThalesBuildProcessor() : base() {
			this.BuildPlatform = Platform.Thales;
		}
		
		#endregion
		
		
	}
	
}