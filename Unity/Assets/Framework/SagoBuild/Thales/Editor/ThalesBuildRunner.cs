namespace SagoBuildEditor.Thales {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The ThalesBuildRunner class provides build runner functionality for Thales builds.
	/// </summary>
	public class ThalesBuildRunner : AndroidBuildRunner {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new KindleFreeTimeBuildRunner instance.
		/// </summary>
		public ThalesBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/Thales/Thales.apk");
		}
		
		#endregion
		
		
	}
	
}