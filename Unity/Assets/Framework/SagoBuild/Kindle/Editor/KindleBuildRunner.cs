namespace SagoBuildEditor.Kindle {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The KindleBuildRunner class provides build runner functionality for Kindle builds.
	/// </summary>
	public class KindleBuildRunner : AndroidBuildRunner {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new KindleBuildRunner instance.
		/// </summary>
		public KindleBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/Kindle/Kindle.apk");
		}
		
		#endregion
		
		
	}
	
}