namespace SagoBuildEditor.Core {
	
	using SagoUtilsEditor;
	using SagoPlatform;
	using SagoPlatformEditor;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The BuildRunner class provides Cloud Build-like functionality in the 
	/// Unity editor (when building in the cloud, Cloud Build takes care of 
	/// setting custom define symbols, calling the static preprocess and 
	/// postprocess callbacks and actually doing the build).
	///
	/// Build processor classes must not depend on the build runner. 
	/// When building in the cloud, Cloud Build will call the build 
	/// processor directly and the build runner will not exist.
	/// </summary>
	public class BuildRunner {
		
		
		#region Types
		
		/// <summary>
		/// Delegate for <see cref="OnPreprocess" />.
		/// <summary>
		public delegate void PreprocessCallback(UnityEngine.CloudBuild.BuildManifestObject manifest);
		
		/// <summary>
		/// Delegate for <see cref="OnPostprocess" />.
		/// </summary>
		public delegate void PostprocessCallback(string buildPath);
		
		#endregion
		
		
		#region Static Properties
		
		/// <summary>
		/// Gets the absolute path to the Unity project.
		/// </summary>
		public static string ProjectPath {
			get { return Path.GetDirectoryName(Application.dataPath); }
		}
		
		/// <summary>
		/// Gets or sets a reference to the build runner that is currently running.
		/// </summary>
		public static BuildRunner Runner {
			get; protected set;
		}
		
		#endregion
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new <see cref="BuildRunner" /> instance.
		/// </summary>
		public BuildRunner() {
			this.BuildOptions = BuildOptions.None;
			this.DefineSymbols = new HashSet<string>();
		}
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets or sets the build options that will be passed to <see cref="BuildPipeline.BuildPlayer" />.
		/// </summary>
		virtual public BuildOptions BuildOptions {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the build path. Must be an absolue path to a directory 
		/// outside of the Unity project. If it doesn't exist, it will be created 
		/// before the build process begins.
		/// </summary>
		virtual public string BuildPath {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the list of scenes to build. The default implementation 
		/// reads the list of scenes from Unity's build settings.
		/// </summary>
		virtual public List<string> BuildScenes {
			get { return this.GetBuildScenes(); }
			set { }
		}
		
		/// <summary>
		/// The build target that will be passed to <see cref="BuildPipeline.BuildPlayer" />.
		/// </summary>
		virtual public BuildTarget BuildTarget {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the build target group that the <see cref="DefineSymbols" /> 
		/// will be applied to. Must match the <see cref="BuildTarget" />.
		/// </summary>
		virtual public BuildTargetGroup BuildTargetGroup {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the set of define symbols that will be added before 
		/// the build and then removed afterwards.
		/// </summary>
		virtual public HashSet<string> DefineSymbols {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the preprocess callback to call during the build process.
		/// </summary>
		virtual public PreprocessCallback OnPreprocess {
			get; set;
		}
		
		/// <summary>
		/// Gets or sets the postprocess callback to call during the build process.
		/// </summary>
		virtual public PostprocessCallback OnPostprocess {
			get; set;
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Runs the build process.
		/// </summary>
		virtual public void Run() {
			
			// set the active runner
			BuildRunner.Runner = this;
			
			// validate the build path
			if (string.IsNullOrEmpty(this.BuildPath)) {
				Debug.LogError(string.Format("Invalid build path: {0}", this.BuildPath));
				return;
			}
			
			// create build directory
			Directory.CreateDirectory(Path.GetDirectoryName(this.BuildPath));
			
			// build
			this.Build();
			
			// clear the active runner
			BuildRunner.Runner = null;
			
		}
		
		/// <summary>
		/// Implements the build logic. Subclasses should override this method 
		/// to implement custom runner functionality.
		/// </summary>
		virtual protected void Build() {
			
			// add define symbols
			DefineSymbolUtil.AddDefineSymbols(this.BuildTargetGroup, this.DefineSymbols);
			
			// preprocess
			this.OnPreprocess(null);
			
			// build
			BuildPipeline.BuildPlayer(
				this.BuildScenes.ToArray(),
				this.BuildPath,
				this.BuildTarget,
				this.BuildOptions
			);
			
			// postprocess
			this.OnPostprocess(this.BuildPath);
			
			// remove defined symbols
			DefineSymbolUtil.RemoveDefineSymbols(this.BuildTargetGroup, this.DefineSymbols);
			
		}
		
		#endregion
		
		
		#region Helper Methods
		
		/// <summary>
		/// Gets the list of scenes to build from Unity's build settings.
		/// </summary>
		virtual protected List<string> GetBuildScenes() {
			List<string> list = new List<string>();
			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
				if (scene.enabled) {
					list.Add(scene.path);
				}
			}
			return list;
		}
		
		#endregion
		
		
	}
	
}