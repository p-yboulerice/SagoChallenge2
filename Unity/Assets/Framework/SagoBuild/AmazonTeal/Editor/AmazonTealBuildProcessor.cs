namespace SagoBuildEditor.AmazonTeal {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The AmazonTealBuildProcessor class implements functionality for processing Amazon Teal builds.
	/// </summary>
	public class AmazonTealBuildProcessor : AndroidBuildProcessor {
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project for Amazon Teal.
		/// </summary>
		[MenuItem("Sago/Build/Amazon Teal: Build", false, 15000)]
		public static void Build() {
			BuildRunner runner = new AmazonTealBuildRunner();
			runner.OnPreprocess = AmazonTealBuildProcessor.OnPreprocess;
			runner.OnPostprocess = AmazonTealBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/Amazon Teal: Build", true)]
		protected static bool ValidateBuild() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.AmazonTeal);
		}
		
		/// <summary>
		/// Builds the project for Amazon Teal.
		/// </summary>
		[MenuItem("Sago/Build/Amazon Teal: Build And Run", false, 15000)]
		public static void BuildAndRun() {
			BuildRunner runner = new AmazonTealBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer
			);
			runner.OnPreprocess = AmazonTealBuildProcessor.OnPreprocess;
			runner.OnPostprocess = AmazonTealBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/Amazon Teal: Build And Run", true)]
		protected static bool ValidateBuildAndRun() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.AmazonTeal);
		}
		
		#endregion
		
		
		#region Build Callbacks
		
		/// <summary>
		/// Static preprocess callback method.
		/// </summary>
		public static void OnPreprocess(BuildManifestObject manifest) {
			AmazonTealBuildProcessor processor = new AmazonTealBuildProcessor();
			processor.BuildManifest = manifest;
			BuildProcessor.OnPreprocess(processor);
		}
		
		#endregion
		
		
		#region Constructors
		
		public AmazonTealBuildProcessor() : base() {
			this.BuildPlatform = Platform.AmazonTeal;
		}
		
		#endregion
		
		
	}
	
}