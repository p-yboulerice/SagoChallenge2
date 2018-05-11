namespace SagoBuildEditor.SmartEducation {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The SmartEducationBuildRunner class provides build runner functionality for Suku Suku's Smart Education builds.
	/// </summary>
	public class SmartEducationBuildRunner : AndroidBuildRunner {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new SmartEducationBuildRunner instance.
		/// </summary>
		public SmartEducationBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/SmartEducation/SmartEducation.apk");
		}
		
		#endregion
		
		
	}
	
}