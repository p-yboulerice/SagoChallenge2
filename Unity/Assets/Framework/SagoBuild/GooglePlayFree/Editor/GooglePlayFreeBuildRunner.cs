namespace SagoBuildEditor.GooglePlayFree {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The GooglePlayFreeBuildRunner class provides build runner functionality for GooglePlay Free builds.
	/// </summary>
	public class GooglePlayFreeBuildRunner : AndroidBuildRunner {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new GooglePlayBuildRunner instance.
		/// </summary>
		public GooglePlayFreeBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/GooglePlay/GooglePlayFree.apk");
		}
		
		#endregion
		
		
	}
	
}