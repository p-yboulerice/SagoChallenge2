#if UNITY_IOS || UNITY_TVOS

namespace SagoBuildEditor.iOS {
	
	using SagoBuildEditor.Core;
	using SagoCore.Submodules;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using UnityEditor;
	using UnityEditor.iOS.Xcode;
	using UnityEngine;
	
	public enum iOSBuildAction {
		Build,
		BuildAndRun,
		BuildAndArchive
	}
	
	public enum iOSBuildType {
		Simulator,
		Device,
		AdHoc,
		AppStore
	}
	
	public class iOSBuildRunner : BuildRunner {
		
		
		#region Properties
		
		public iOSBuildAction BuildAction {
			get; set;
		}
		
		public iOSSdkVersion BuildSdkVersion {
			get; set;
		}
		
		#endregion
		
		
		#region Constructor
		
		public iOSBuildRunner() : base() {
			this.BuildPath = Path.Combine(BuildRunner.ProjectPath, "../Build/iOS");
			this.BuildTarget = BuildTarget.iOS;
			this.BuildTargetGroup = BuildTargetGroup.iOS;
			this.BuildSdkVersion = iOSSdkVersion.DeviceSDK;
		}
		
		#endregion
		
		
		#region Methods
		
		override protected void Build() {
			
			// quit xcode so the user isn't prompted to reload the project
			switch (this.BuildAction) {
				case iOSBuildAction.BuildAndRun:
				case iOSBuildAction.BuildAndArchive:
					this.XcodeHelper("Quit");
				break;
			}
			
			// store intial and target sdk versions
			iOSSdkVersion initialSdkVersion = PlayerSettings.iOS.sdkVersion;
			iOSSdkVersion targetSdkVersion = this.BuildSdkVersion;
			
			// switch to the target sdk (device or simulator)
			if (targetSdkVersion != initialSdkVersion) {
				PlayerSettings.iOS.sdkVersion = targetSdkVersion;
			}
			
			// build
			base.Build();
			
			// revert to the inital sdk
			if (targetSdkVersion != initialSdkVersion) {
				PlayerSettings.iOS.sdkVersion = initialSdkVersion;
			}
			
			// execute the build action via applescript
			switch (this.BuildAction) {
				case iOSBuildAction.BuildAndRun:
					this.XcodeHelper("Run");
				break;
				case iOSBuildAction.BuildAndArchive:
					this.XcodeHelper("Archive");
				break;
			}
			
		}
		
		#endregion
		
		#region Helper Methods
		
		private void XcodeHelper(string action) {
			
			string xcodeHelperPath;
			xcodeHelperPath = Path.Combine(ProjectPath, Path.Combine(SubmoduleMap.GetSubmodulePath(typeof(SagoBuild.SubmoduleInfo)), "iOS/.Native/xcode_helper"));
			
			string xcodeProjectPath;
			xcodeProjectPath = Path.GetDirectoryName(PBXProject.GetPBXProjectPath(this.BuildPath));
			
			Process p = new Process();
			p.StartInfo.Arguments = string.Format("'{0}' '{1}'", xcodeProjectPath, action);
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			p.StartInfo.FileName = xcodeHelperPath;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.UseShellExecute = false;
			p.Start();
			p.WaitForExit();
			
		}
		
		#endregion
		
		
	}
	
}

#endif
