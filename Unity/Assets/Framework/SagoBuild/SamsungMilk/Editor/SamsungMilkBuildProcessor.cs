namespace SagoBuildEditor.SamsungMilk {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The SamsungMilkeBuildProcessor class implements functionality for processing SamsungMilk builds.
	/// </summary>
	public class SamsungMilkBuildProcessor : AndroidBuildProcessor {
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project for SamsungMilk.
		/// </summary>
		[MenuItem("Sago/Build/SamsungMilk: Build", false, 13000)]
		public static void Build() {
			BuildRunner runner = new SamsungMilkBuildRunner();
			runner.OnPreprocess = SamsungMilkBuildProcessor.OnPreprocess;
			runner.OnPostprocess = SamsungMilkBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/SamsungMilk: Build", true)]
		protected static bool ValidateBuild() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.SamsungMilk);
		}
		
		/// <summary>
		/// Builds the project for SamsungMilk.
		/// </summary>
		[MenuItem("Sago/Build/SamsungMilk: Build And Run", false, 13000)]
		public static void BuildAndRun() {
			BuildRunner runner = new SamsungMilkBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer
			);
			runner.OnPreprocess = SamsungMilkBuildProcessor.OnPreprocess;
			runner.OnPostprocess = SamsungMilkBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/SamsungMilk: Build And Run", true)]
		protected static bool ValidateBuildAndRun() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.SamsungMilk);
		}
		
		#endregion
		
		
		#region Build Callbacks
		
		/// <summary>
		/// Static preprocess callback method.
		/// </summary>
		public static void OnPreprocess(BuildManifestObject manifest) {
			SamsungMilkBuildProcessor processor = new SamsungMilkBuildProcessor();
			processor.BuildManifest = manifest;
			BuildProcessor.OnPreprocess(processor);
		}
		
		#endregion
		
		
		#region Constructors
		
		public SamsungMilkBuildProcessor() : base() {
			this.BuildPlatform = Platform.SamsungMilk;
		}
		
		#endregion
		
		
	}
	
}