namespace SagoBuildEditor.PanasonicEx {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The PanasonicExBuildProcessor class implements functionality for processing PanasonicEx builds.
	/// </summary>
	public class PanasonicExBuildProcessor : AndroidBuildProcessor {

		#region Menu Items

		/// <summary>
		/// Builds the project for PanasonicEx.
		/// </summary>
		[MenuItem("Sago/Build/PanasonicEx: Build", false, 12000)]
		public static void Build() {
			BuildRunner runner = new PanasonicExBuildRunner();
			runner.OnPreprocess = PanasonicExBuildProcessor.OnPreprocess;
			runner.OnPostprocess = PanasonicExBuildProcessor.OnPostprocess;
			runner.Run();
		}

		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/PanasonicEx: Build", true)]
		protected static bool ValidateBuild() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.PanasonicEx);
		}

		/// <summary>
		/// Builds the project for PanasonicEx.
		/// </summary>
		[MenuItem("Sago/Build/PanasonicEx: Build And Run", false, 12000)]
		public static void BuildAndRun() {
			BuildRunner runner = new PanasonicExBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer
			);
			runner.OnPreprocess = PanasonicExBuildProcessor.OnPreprocess;
			runner.OnPostprocess = PanasonicExBuildProcessor.OnPostprocess;
			runner.Run();
		}

		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/PanasonicEx: Build And Run", true)]
		protected static bool ValidateBuildAndRun() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.PanasonicEx);
		}

		#endregion


		#region Build Callbacks

		/// <summary>
		/// Static preprocess callback method.
		/// </summary>
		public static void OnPreprocess(BuildManifestObject manifest) {
			PanasonicExBuildProcessor processor = new PanasonicExBuildProcessor();
			processor.BuildManifest = manifest;
			BuildProcessor.OnPreprocess(processor);
		}

		#endregion


		#region Constructors

		public PanasonicExBuildProcessor() : base() {
			this.BuildPlatform = Platform.PanasonicEx;
		}

		#endregion
		
	}
	
}
