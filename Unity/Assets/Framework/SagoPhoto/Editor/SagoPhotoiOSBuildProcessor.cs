#if UNITY_IOS
namespace SagoPhotoEditor {
	
	using PlistCS;
	using SagoBuildEditor.Core;
	using SagoBuildEditor.iOS;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	using UnityEngine;
	using Version = System.Version;
	using SagoCore.Submodules;
	
	public static class SagoPhotoiOSBuildProcessor {

		[OnBuildPostprocess]
		public static void OnBuildPostprocess(IBuildProcessor buildProcessor) {
			if (buildProcessor is iOSBuildProcessor) {
				OnBuildPostprocess(buildProcessor as iOSBuildProcessor);
			}
		}
		
		private static void OnBuildPostprocess(iOSBuildProcessor processor) {
			
			// read project
			PBXProject project;
			project = new PBXProject();
			project.ReadFromFile(PBXProject.GetPBXProjectPath(processor.XcodeProjectPath));
			
			// read target
			string target;
			target = project.TargetGuidByName(PBXProject.GetUnityTargetName());
			
			// define files
			List<string> filenames = new List<string> {
				"PhotoUtil.h",
				"PhotoUtil.mm",
			};

			// get the native path
			string nativePath;
			nativePath = SubmoduleMap.GetAbsoluteSubmodulePath(typeof(SagoPhoto.SubmoduleInfo));
			nativePath += "/Build/iOS/.Native";

			
			// add files
			foreach (string filename in filenames) {
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativePath, filename),
					Path.Combine(processor.XcodeGroupPath, filename),
					PBXSourceTree.Source
				));
			}

			// Starting in Unity 5.x, ARC is enabled by default, but PhotoUtil.mm 
			// has non-ARC code, so we have to turn off arc for PhotoUtil.mm
			if (BuildProcessor.UnityVersion.Major >= 5) {
				string file = project.FindFileGuidByProjectPath(Path.Combine(processor.XcodeGroupPath, "PhotoUtil.mm"));
				var flags = project.GetCompileFlagsForFile(target, file);
				flags.Add("-fno-objc-arc");
				project.SetCompileFlagsForFile(target, file, flags);	
			}

			// write xcode project
			project.WriteToFile(PBXProject.GetPBXProjectPath(processor.XcodeProjectPath));
			
		}
				
	}
	
}
#endif