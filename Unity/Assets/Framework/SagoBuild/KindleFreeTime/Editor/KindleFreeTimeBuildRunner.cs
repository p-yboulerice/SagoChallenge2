namespace SagoBuildEditor.KindleFreeTime {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The KindleFreeTimeBuildRunner class provides build runner functionality for Kindle Free Time builds.
	/// </summary>
	public class KindleFreeTimeBuildRunner : AndroidBuildRunner {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new KindleFreeTimeBuildRunner instance.
		/// </summary>
		public KindleFreeTimeBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/KindleFreeTime/KindleFreeTime.apk");
		}
		
		#endregion
		
		
	}
	
}