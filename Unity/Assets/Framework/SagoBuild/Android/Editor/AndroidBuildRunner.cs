namespace SagoBuildEditor.Android {
	
	using SagoBuildEditor.Core;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	public enum AndroidBuildAction {
		Build,
		BuildAndRun
	}
	
	/// <summary>
	/// The AndroidBuildRunner class provides build runner functionality for Android builds.
	/// </summary>
	public class AndroidBuildRunner : BuildRunner {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new AndroidBuildRunner instance.
		/// </summary>
		public AndroidBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/Android/Android.apk");
			this.BuildTarget = BuildTarget.Android;
			this.BuildTargetGroup = BuildTargetGroup.Android;
		}
		
		#endregion
		
		
	}
	
}