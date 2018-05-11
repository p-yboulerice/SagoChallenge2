namespace SagoBuildEditor.Nabi {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The NabiBuildRunner class provides build runner functionality for Nabi builds.
	/// </summary>
	public class NabiBuildRunner : AndroidBuildRunner {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new NabiBuildRunner instance.
		/// </summary>
		public NabiBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/Nabi/Nabi.apk");
		}
		
		#endregion
		
		
	}
	
}