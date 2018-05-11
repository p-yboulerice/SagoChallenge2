namespace SagoBuildEditor.Core {
	
	using SagoPlatform;
	using SagoPlatformEditor;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.CloudBuild;
	
	/// <summary>
	/// The IBuildProcessor interface defines methods required to implement a build processor.
	/// </summary>
	public interface IBuildProcessor {
		
		/// <summary>
		/// Called before <see cref="Preprocess" />.
		/// </summary>
		void PreprocessBegin();
		
		/// <summary>
		/// Called before the build starts.
		/// </summary>
		void Preprocess();
		
		/// <summary>
		/// Called after <see cref="Preprocess" />.
		/// </summary>
		void PreprocessEnd();
		
		/// <summary>
		/// Called before <see cref="Postprocess" />.
		/// </summary>
		void PostprocessBegin(string buildPath);
		
		/// <summary>
		/// Called after the build finishes.
		/// <summary>
		/// <param name="buildPath">
		/// The path to the build directory. When running in the editor, the value 
		/// will be an absolute path. When running in the cloud, the value may be 
		/// relative or absolute (the Cloud Build documentation isn't clear).
		/// </param>
		void Postprocess(string buildPath);
		
		/// <summary>
		/// Called after <see cref="Postprocess" />
		/// </summary>
		void PostprocessEnd(string buildPath);
		
	}
	
	/// <summary>
	/// The BuildProcessor class is the base class for build processors.
	/// </summary>
	public class BuildProcessor : IBuildProcessor {
		
		
		#region Static Properties
		
		/// <summary>
		/// Gets the unity version.
		/// </summary>
		public static System.Version UnityVersion {
			get { return new System.Version(Regex.Replace(Application.unityVersion, @"\D+", ".")); }
		}
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// 
		/// </summary>
		virtual public BuildManifestObject BuildManifest {
			get; set;
		}
		
		/// <summary>
		/// Gets the path to the build. When running in the editor, the value 
		/// will be an absolute path. When running in the cloud, the value may be 
		/// relative or absolute (the Cloud Build documentation isn't clear). The 
		/// value will be null until the <see cref="Postprocess" /> method is 
		/// called. 
		/// </summary>
		virtual public string BuildPath {
			get; protected set;
		}
		
		/// <summary>
		/// TODO:
		/// </summary>
		virtual public Platform BuildPlatform {
			get; protected set;
		}
		
		/// <summary>
		/// Gets a flag indicating whether the build processor is running in the cloud.
		/// </summary>
		virtual public bool IsCloudBuild {
			get { return BuildRunner.Runner == null; }
		}
		
		/// <summary>
		/// Gets the absolute path to the Unity project.
		/// </summary>
		virtual public string ProjectPath {
			get { return Path.GetDirectoryName(Application.dataPath); }
		}
		
		/// <summary>
		/// TODO: 
		/// </summary>
		virtual public Platform ProjectPlatform {
			get; protected set;
		}
		
		#endregion
		
		
		#region Constructor
		
		public BuildProcessor() {
			
		}
		
		#endregion
		
		
		#region IBuildProcessor Methods
		
		virtual public void PreprocessBegin() {
			
			this.ApplyBuildPlatform();
			this.CheckBuildPlatform();
			
			Debug.Log(string.Format(
				"SagoBuild: Platform = {0}", 
				PlatformUtil.ActivePlatform
			));
			
			Debug.Log(string.Format(
				"SagoBuild: Define Symbols = {0}",
				PlayerSettings.GetScriptingDefineSymbolsForGroup(PlatformUtil.ActivePlatform.ToBuildTargetGroup())
			));
			
		}
		
		/// <summary>
		/// Called before the build starts. Subclasses should override this method 
		/// to implement custom preprocessing functionality.
		/// </summary>
		virtual public void Preprocess() {
			
		}
		
		virtual public void PreprocessEnd() {
			
		}
		
		virtual public void PostprocessBegin(string buildPath) {
			
		}
		
		/// <summary>
		/// Called after the build finishes. Subclasses should override this method
		/// to implement custom postprocessing functionality. 
		/// <summary>
		/// <param name="buildPath">
		/// The path to the build directory. When running in the editor, the value 
		/// will be an absolute path. When running in the cloud, the value may be 
		/// relative or absolute (the Cloud Build documentation isn't clear).
		/// </param>
		virtual public void Postprocess(string buildPath) {
			this.BuildPath = buildPath;
		}
		
		virtual public void PostprocessEnd(string buildPath) {
			this.RevertBuildPlatform();
		}
		
		#endregion
		
		
		#region Callback Methods
		
		/// <summary>
		/// Gets or sets a reference to the build processor that is currently running. 
		/// The static OnPreprocess method sets the reference and the static OnPostprocess
		/// method gets the reference, which allows both static methods to use the
		/// same build processor instance.
		/// </summary>
		private static IBuildProcessor Processor {
			get; set;
		}
		
		/// <summary>
		/// A helper method subclasses can use to implement the <see cref="Processor"/ > logic.
		/// </summary>
		/// <param name="processor">
		/// The build processor instance to assign to the <see cref="Processor"/ > property.
		/// </param>
		public static void OnPreprocess(IBuildProcessor processor) {
			
			if (BuildProcessor.Processor != null) {
				Debug.LogError(string.Format("Invalid operation: Processor already exists: {0}", BuildProcessor.Processor));
				return;
			}
			
			if (processor == null) {
				Debug.LogError(string.Format("Invalid processor: {0}", processor));
			}
			
			BuildProcessor.Processor = processor;
			try {
				BuildProcessor.Processor.PreprocessBegin();
				BuildProcessor.Processor.Preprocess();
				OnBuildPreprocessAttribute.Invoke(BuildProcessor.Processor);
				BuildProcessor.Processor.PreprocessEnd();
			} catch (System.Exception e) {
				Debug.LogException(e);
				BuildProcessor.Processor = null;
				throw e;
			}
			
		}
		
		
		/// <summary>
		/// A helper method subclasses can use to implement the <see cref="Processor" /> logic.
		/// </summary>
		/// <param name="buildPath">
		/// The path to the build directory. When running in the editor, the value 
		/// will be an absolute path. When running in the cloud, the value may be 
		/// relative or absolute (the Cloud Build documentation isn't clear).
		/// </param>
		public static void OnPostprocess(string buildPath) {
			
			if (BuildProcessor.Processor == null) {
				Debug.LogError(string.Format("Invalid operation: Processor does not exist: {0}", BuildProcessor.Processor));
				return;
			}

			try {
				BuildProcessor.Processor.PostprocessBegin(buildPath);
				BuildProcessor.Processor.Postprocess(buildPath);
				OnBuildPostprocessAttribute.Invoke(BuildProcessor.Processor);
				BuildProcessor.Processor.PostprocessEnd(buildPath);
			} finally {
				BuildProcessor.Processor = null;
			}
		}
		
		#endregion
		
		
		#region Helper Methods
		
		public void ApplyBuildPlatform() {
			this.ProjectPlatform = PlatformUtil.ActivePlatform;
			PlatformUtilEditor.Activate(this.BuildPlatform);
		}
		
		public void CheckBuildPlatform() {
			
			Platform activePlatform;
			activePlatform = PlatformUtil.ActivePlatform;
			
			if (this.BuildPlatform != activePlatform) {
				throw new System.InvalidOperationException(string.Format(
					"Invalid platform (active = {0}, target = {1}", 
					activePlatform,
					this.BuildPlatform
				));
			}
			
		}
		
		public void RevertBuildPlatform() {
			PlatformUtilEditor.Activate(this.ProjectPlatform);
			this.ProjectPlatform = default(Platform);
		}
		
		/// <summary>
		/// A helper method for copying assets from one directory to another. If 
		/// the asset already exists in the destination directory, it will NOT be 
		/// overwritten.
		/// <summary>
		/// <param name="srcRoot">
		/// The path to the source directory.
		/// </param>
		/// <param name="dstRoot">
		/// The path to the destination directory.
		/// </param>
		/// <param name="path">
		/// The relative path to a file or directory to be copied from the 
		/// source directory to the destination directory.
		/// </param>
		public static void CopyAsset(string srcRoot, string dstRoot, string path) {
			CopyAsset(srcRoot, dstRoot, path, false);
		}
		
		/// <summary>
		/// A helper method for copying assets from one directory to another.
		/// <summary>
		/// <param name="srcRoot">
		/// The path to the source directory.
		/// </param>
		/// <param name="dstRoot">
		/// The path to the destination directory.
		/// </param>
		/// <param name="path">
		/// The relative path to a file or directory to be copied from the 
		/// source directory to the destination directory.
		/// </param>
		/// <param name="overwrite">
		/// A flag indicating the operation should overwrite existing assets 
		/// in the destination directory.
		/// </param>
		public static void CopyAsset(string srcRoot, string dstRoot, string path, bool overwrite) {
			CopyAsset(srcRoot, path, dstRoot, path, overwrite);
		}

		/// <summary>
		/// A helper method for copying assets from one directory to another.
		/// <summary>
		/// <param name="srcRoot">
		/// The path to the source directory.
		/// </param>
		/// <param name="dstRoot">
		/// The path to the destination directory.
		/// </param>
		/// <param name="srcRelativePath">
		/// The relative path to a file or directory to be copied from the 
		/// source directory.
		/// </param>
		/// <param name="dstRelativePath">
		/// The relative path to a file or directory to be copied from the 
		/// source directory.
		/// </param>
		/// <param name="overwrite">
		/// A flag indicating the operation should overwrite existing assets 
		/// in the destination directory.
		/// </param>
		public static void CopyAsset(string srcRoot, string srcRelativePath, string dstRoot, string dstRelativePath, bool overwrite) {

			string srcPath = Path.Combine(srcRoot, srcRelativePath);
			string dstPath = Path.Combine(dstRoot, dstRelativePath);

			if (Directory.Exists(srcPath) && (overwrite || !Directory.Exists(dstPath))) {
				CopyAssetDirectory(srcPath, dstPath);
			}
			
			if (File.Exists(srcPath) && (overwrite || !File.Exists(dstPath))) {
				CopyAssetFile(srcPath, dstPath);
			}
			
		}

		/// <summary>
		/// A helper method for copying files.
		/// </summary>
		private static void CopyAssetFile(string srcPath, string dstPath) {
			
			// create the directory
			Directory.CreateDirectory(Path.GetDirectoryName(dstPath));
			
			// copy the file
			File.Copy(srcPath, dstPath);
			
		}
		
		/// <summary>
		/// A helper method for copying directories.
		/// </summary>
		private static void CopyAssetDirectory(string srcPath, string dstPath) {
			
			// replace the directory
			if (Directory.Exists(dstPath)) {
				Directory.Delete(dstPath, true);
			}
			Directory.CreateDirectory(dstPath);
			
			// copy files
			foreach (var file in Directory.GetFiles(srcPath)) {
				File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
			}
			
			// copy subdirectories
			foreach (var dir in Directory.GetDirectories(srcPath)) {
				CopyAssetDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
			}
			
		}
		
		#endregion
		
		
	}
	
}