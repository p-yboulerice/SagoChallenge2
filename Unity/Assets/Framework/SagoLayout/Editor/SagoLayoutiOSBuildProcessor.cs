#if UNITY_IOS || UNITY_TVOS

namespace SagoLayoutEditor {
	
	using PlistCS;
	using SagoBuildEditor.Core;
	using SagoCore.Submodules;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	using UnityEngine;
	using UnityEngine.CloudBuild;
	using Version = System.Version;
	using SagoBuildEditor;
	using SagoBuildEditor.iOS;

	public class SagoLayoutiOSBuildProcessor {

		#region Properties

		/// <summary>
		/// Gets the path to the group in the Xcode project that assets should be added to.
		/// </summary>
		public static string XcodeGroupPath {
			get { return "SagoLayout"; }
		}

		#endregion

		#region Internal Methods

		/// <summary>
		/// Updates the Xcode project by adding the native file that calculates the Safe Area.
		/// </summary>
		[OnBuildPostprocess]
		public static void PostprocessXcodeProject(IBuildProcessor processor) {
			
			iOSBuildProcessor ios;
			ios = processor as iOSBuildProcessor;
			
			if (ios == null) {
				return;
			}
			
			string pbxPath;
			pbxPath = Path.Combine(ios.XcodeProjectPath, "Unity-iPhone.xcodeproj/project.pbxproj");
			
			PBXProject project;
			project = new PBXProject();
			project.ReadFromFile(pbxPath);
			
			string externalNativeAssetsPath;
			externalNativeAssetsPath = Path.Combine(ios.ProjectPath, Path.Combine(SubmoduleMap.GetSubmodulePath(typeof(SagoLayout.SubmoduleInfo)), "Plugins/iOS/.Native"));
			
			string target;
			target = project.TargetGuidByName(PBXProject.GetUnityTargetName());
			
			project.AddFileToBuild(target, project.AddFile(
				Path.Combine(externalNativeAssetsPath, "SLSafeArea.mm"),
				Path.Combine(XcodeGroupPath, "SLSafeArea.mm"),
				PBXSourceTree.Source
				));

			// write xcode project
			project.WriteToFile(pbxPath);
		}

		#endregion
	}
}

#endif
