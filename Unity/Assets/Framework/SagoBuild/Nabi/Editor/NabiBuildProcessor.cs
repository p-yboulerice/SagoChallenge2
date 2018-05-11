namespace SagoBuildEditor.Nabi {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The NabiBuildProcessor class implements functionality for processing Nabi builds.
	/// </summary>
	public class NabiBuildProcessor : AndroidBuildProcessor {
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project for Nabi.
		/// </summary>
		[MenuItem("Sago/Build/Nabi: Build", false, 14000)]
		public static void Build() {
			BuildRunner runner = new NabiBuildRunner();
			runner.OnPreprocess = NabiBuildProcessor.OnPreprocess;
			runner.OnPostprocess = NabiBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/Nabi: Build", true)]
		protected static bool ValidateBuild() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.Nabi);
		}
		
		/// <summary>
		/// Builds the project for Nabi.
		/// </summary>
		[MenuItem("Sago/Build/Nabi: Build And Run", false, 14000)]
		public static void BuildAndRun() {
			BuildRunner runner = new NabiBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer
			);
			runner.OnPreprocess = NabiBuildProcessor.OnPreprocess;
			runner.OnPostprocess = NabiBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/Nabi: Build And Run", true)]
		protected static bool ValidateBuildAndRun() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.Nabi);
		}
		
		#endregion
		
		
		#region Build Callbacks
		
		/// <summary>
		/// Static preprocess callback method.
		/// </summary>
		public static void OnPreprocess(BuildManifestObject manifest) {
			NabiBuildProcessor processor = new NabiBuildProcessor();
			processor.BuildManifest = manifest;
			BuildProcessor.OnPreprocess(processor);
		}
		
		#endregion
		
		
		#region Constructors
		
		public NabiBuildProcessor() : base() {
			this.BuildPlatform = Platform.Nabi;
		}
		
		#endregion
		
		
	}
	
}