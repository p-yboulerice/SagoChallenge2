#if UNITY_IOS || UNITY_TVOS

namespace SagoBizEditor {
	
	using PlistCS;
	using SagoBuildEditor.Core;
	using SagoCore.Submodules;
	using SagoBiz;
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

	public class SagoBiziOSBuildProcessor {

		#region Properties

		/// <summary>
		/// Gets the path to the group in the Xcode project that assets should be added to.
		/// </summary>
		public static string XcodeGroupPath {
			get { return "SagoBiz"; }
		}

		#endregion

		#region Internal Methods

		/// <summary>
		/// Updates the Xcode project by adding the grow tool and updating the build settings.
		/// </summary>
		[OnBuildPostprocess]
		public static void PostprocessXcodeProject(IBuildProcessor processor) {
			#if !SAGO_DISABLE_SAGOBIZ
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
				externalNativeAssetsPath = Path.Combine(ios.ProjectPath, Path.Combine(SubmoduleMap.GetSubmodulePath(typeof(SagoBiz.SubmoduleInfo)), "Plugins/iOS/.Native"));
				
				string target;
				target = project.TargetGuidByName(PBXProject.GetUnityTargetName());
				
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(externalNativeAssetsPath, "SBNativeBindings.h"),
					Path.Combine(XcodeGroupPath, "SBNativeBindings.h"),
					PBXSourceTree.Source
					));
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(externalNativeAssetsPath, "SBNativeBindings.mm"),
					Path.Combine(XcodeGroupPath, "SBNativeBindings.mm"),
					PBXSourceTree.Source
					));
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(externalNativeAssetsPath, "SagoBiz.framework"),
					Path.Combine(XcodeGroupPath, "SagoBiz.framework"),
					PBXSourceTree.Source
					));
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(externalNativeAssetsPath, "SagoBiz.framework/Resources/SagoBiz.bundle"),
					Path.Combine(XcodeGroupPath, "SagoBiz.bundle"),
					PBXSourceTree.Source
					));
								
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(externalNativeAssetsPath, "SBStoreReviewController.h"),
					Path.Combine(XcodeGroupPath, "SBStoreReviewController.h"),
					PBXSourceTree.Source
				));

				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(externalNativeAssetsPath, "SBStoreReviewController.mm"),
					Path.Combine(XcodeGroupPath, "SBStoreReviewController.mm"),
					PBXSourceTree.Source
				));

				// Read our AdjustConfig.plist template in externalNativeAssetsPath folder.
				AdjustOptions adjustOptions = SagoPlatform.PlatformUtil.GetSettings<AdjustOptions>();
				if (adjustOptions != null) {
					Dictionary<string, object> adjConfigDict = (Dictionary<string, object>)Plist.readPlist(Path.Combine(externalNativeAssetsPath, "SBAdjust.plist"));
					adjConfigDict["AdjustToken"] = adjustOptions.AppToken;
					adjConfigDict["AdjustDebug"] = adjustOptions.IsDebug;
					adjConfigDict["AdjustLogLevel"] = adjustOptions.LogLevel;

					// Copy AdjustConfig.plist to iOS Xcode project path and add to Xcode project.
					Plist.writeXml(adjConfigDict, Path.Combine(ios.XcodeProjectPath, "SBAdjust.plist"));
					project.AddFileToBuild(target, project.AddFile(
						Path.Combine(ios.XcodeProjectPath, "SBAdjust.plist"),
						Path.Combine(XcodeGroupPath, "SBAdjust.plist"),
						PBXSourceTree.Source
					));
				}
				
				project.UpdateBuildProperty(target, "HEADER_SEARCH_PATHS", new[] {
					string.Format("\"{0}\"/SagoBiz.framework/Headers", externalNativeAssetsPath)
				}, null);

				project.AddBuildProperty(target, "OTHER_LDFLAGS", "-licucore");

				// Adjust runtime "unrecognized selector sent to instance" error fix.
				// Seems like an issue with static framework being added.
				project.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
				
				// Starting in Unity 5.x, ARC is enabled by default, but SBNativeBindings.mm 
				// has non-ARC code, so we have to turn off arc for SBNativeBindings.mm
				if (BuildProcessor.UnityVersion.Major >= 5) {
					string file = project.FindFileGuidByProjectPath(Path.Combine(XcodeGroupPath, "SBNativeBindings.mm"));
					var flags = project.GetCompileFlagsForFile(target, file);
					flags.Add("-fno-objc-arc");
					project.SetCompileFlagsForFile(target, file, flags);	
				}

				// NOTE: Xcode splits the values for FRAMEWORK_SEARCH_PATHS on spaces, so if 
				// a value contains spaces it needs to be quoted. However, AddBuildProperty 
				// and SetBuildProperty will strip quotes from the value if it begins and ends 
				// with a quote. To prevent the quotes from being stripped, use string.Format
				// with $(inherited) at the beginning instead of using separate calls to 
				// AddBuildProperty.
				project.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS", string.Format("\"{0}\"", externalNativeAssetsPath));

				// write xcode project
				project.WriteToFile(pbxPath);
			#endif
		}

		#endregion
		
		
	}
	
}

#endif
