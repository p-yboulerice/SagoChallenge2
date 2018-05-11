namespace SagoBuildEditor.SamsungMilk {
	
	using SagoBuildEditor.Android;
	using SagoBuildEditor.Core;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The SamsungMilkBuildRunner class provides build runner functionality for Samsung Milk builds.
	/// </summary>
	public class SamsungMilkBuildRunner : AndroidBuildRunner {
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new SamsungMilkBuildRunner instance.
		/// </summary>
		public SamsungMilkBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/SamsungMilk/SamsungMilk.apk");
		}
		
		#endregion
		
	}
	
}