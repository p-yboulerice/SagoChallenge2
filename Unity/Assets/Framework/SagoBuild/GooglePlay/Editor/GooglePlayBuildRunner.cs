namespace SagoBuildEditor.GooglePlay {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The GooglePlayBuildRunner class provides build runner functionality for GooglePlay builds.
	/// </summary>
	public class GooglePlayBuildRunner : AndroidBuildRunner {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new GooglePlayBuildRunner instance.
		/// </summary>
		public GooglePlayBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/GooglePlay/GooglePlay.apk");
		}
		
		#endregion
		
		
	}
	
}