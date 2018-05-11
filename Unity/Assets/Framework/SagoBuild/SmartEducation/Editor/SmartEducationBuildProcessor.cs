namespace SagoBuildEditor.SmartEducation {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using UnityEditor;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The SmartEducationBuildProcessor class implements functionality for processing SmartEducation builds.
	/// </summary>
	public class SmartEducationBuildProcessor : AndroidBuildProcessor {
		
		
		#region Menu Items
		
		/// <summary>
		/// Builds the project for SmartEducation.
		/// </summary>
		[MenuItem("Sago/Build/SmartEducation: Build", false, 12200)]
		public static void Build() {
			BuildRunner runner = new SmartEducationBuildRunner();
			runner.OnPreprocess = SmartEducationBuildProcessor.OnPreprocess;
			runner.OnPostprocess = SmartEducationBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/SmartEducation: Build", true)]
		protected static bool ValidateBuild() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.SmartEducation);
		}
		
		/// <summary>
		/// Builds the project for SmartEducation.
		/// </summary>
		[MenuItem("Sago/Build/SmartEducation: Build And Run", false, 12200)]
		public static void BuildAndRun() {
			BuildRunner runner = new SmartEducationBuildRunner();
			runner.BuildOptions = (
				BuildOptions.AutoRunPlayer
			);
			runner.OnPreprocess = SmartEducationBuildProcessor.OnPreprocess;
			runner.OnPostprocess = SmartEducationBuildProcessor.OnPostprocess;
			runner.Run();
		}
		
		/// <summary>
		/// Validates the <see cref="Build" /> menu item.
		/// </summary>
		[MenuItem("Sago/Build/SmartEducation: Build And Run", true)]
		protected static bool ValidateBuildAndRun() {
			return MenuItemUtil.IsMenuItemEnabledForBuildPlatform(Platform.SmartEducation);
		}
		
		#endregion
		
		
		#region Build Callbacks
		
		/// <summary>
		/// Static preprocess callback method.
		/// </summary>
		public static void OnPreprocess(BuildManifestObject manifest) {
			SmartEducationBuildProcessor processor = new SmartEducationBuildProcessor();
			processor.BuildManifest = manifest;
			BuildProcessor.OnPreprocess(processor);
		}
		
		#endregion
		
		
		#region Constructors
		
		public SmartEducationBuildProcessor() : base() {
			this.BuildPlatform = Platform.SmartEducation;
		}
		
		#endregion
		
		
	}
	
}