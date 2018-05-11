namespace SagoBuildEditor.PanasonicEx {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// 
	/// </summary>
	public class PanasonicExBuildRunner : AndroidBuildRunner {

		#region Constructor

		/// <summary>
		/// Creates a new KindleFreeTimeBuildRunner instance.
		/// </summary>
		public PanasonicExBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/PanasonicEx/PanasonicEx.apk");
		}

		#endregion
		
	}
	
}
