namespace SagoBuildEditor.Bemobi {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The BemobiBuildProcessor class implements functionality for processing Bemobi builds.
	/// </summary>
	public class BemobiBuildProcessor : AndroidBuildProcessor {
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project for Bemobi.
		/// </summary>
		[MenuItem("Sago/Build/Bemobi: Build", false, 12100)]
		public static void Build() {
			BuildRunner runner = new BemobiBuildRunner();
			runner.OnPreprocess = BemobiBuildProcessor.OnPreprocess;
			runner.OnPostprocess = BemobiBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/Bemobi: Build", true)]
		protected static bool ValidateBuild() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.Bemobi);
		}
		
		/// <summary>
		/// Builds the project for Bemobi.
		/// </summary>
		[MenuItem("Sago/Build/Bemobi: Build And Run", false, 12100)]
		public static void BuildAndRun() {
			BuildRunner runner = new BemobiBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer
			);
			runner.OnPreprocess = BemobiBuildProcessor.OnPreprocess;
			runner.OnPostprocess = BemobiBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/Bemobi: Build And Run", true)]
		protected static bool ValidateBuildAndRun() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.Bemobi);
		}
		
		#endregion
		
		
		#region Build Callbacks
		
		/// <summary>
		/// Static preprocess callback method.
		/// </summary>
		public static void OnPreprocess(BuildManifestObject manifest) {
			BemobiBuildProcessor processor = new BemobiBuildProcessor();
			processor.BuildManifest = manifest;
			BuildProcessor.OnPreprocess(processor);
		}
		
		#endregion
		
		
		#region Constructors
		
		public BemobiBuildProcessor() : base() {
			this.BuildPlatform = Platform.Bemobi;
		}
		
		#endregion
		
		
	}
	
}