namespace SagoBuildEditor.AmazonTeal {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The AmazonTealBuildRunner class provides build runner functionality for AmazonTeal builds.
	/// </summary>
	public class AmazonTealBuildRunner : AndroidBuildRunner {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new AmazonTealBuildRunner instance.
		/// </summary>
		public AmazonTealBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/AmazonTeal/AmazonTeal.apk");
		}
		
		#endregion
		
		
	}
	
}