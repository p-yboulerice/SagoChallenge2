namespace SagoBuildEditor.Bemobi {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The BemobiBuildRunner class provides build runner functionality for Bemobi builds.
	/// </summary>
	public class BemobiBuildRunner : AndroidBuildRunner {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new BemobiBuildRunner instance.
		/// </summary>
		public BemobiBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/Bemobi/Bemobi.apk");
		}
		
		#endregion
		
		
	}
	
}