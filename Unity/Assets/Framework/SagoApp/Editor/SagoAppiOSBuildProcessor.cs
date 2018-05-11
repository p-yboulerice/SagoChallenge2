#if UNITY_IOS || UNITY_TVOS
namespace SagoAppEditor {

	using SagoApp.Project;
	using SagoApp.Content;
	using SagoCore.Submodules;
	using SagoBuildEditor.Core;
	using SagoBuildEditor.iOS;
	using System.IO;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.iOS.Xcode;

	public class SagoAppiOSBuildProcessor : MonoBehaviour {
		
		static bool HasOnDemandResourcesAssetBundle {
			get {
				foreach (ContentInfo content in ProjectInfo.Instance.ContentInfo) {

					if (AssetBundleAdaptorMap.Instance) {

						AssetBundleAdaptorType resourceAssetBundleAdaptorType = AssetBundleAdaptorMap.Instance.GetAdaptorType(content.ResourceAssetBundleName);
						AssetBundleAdaptorType sceneAssetBundleAdaptorType = AssetBundleAdaptorMap.Instance.GetAdaptorType(content.SceneAssetBundleName);

						if (resourceAssetBundleAdaptorType == AssetBundleAdaptorType.OnDemandResources ||
							sceneAssetBundleAdaptorType == AssetBundleAdaptorType.OnDemandResources) {
							return true;
						}
					}
				}
				return false;
			}
		}

		[OnBuildPostprocess]
		public static void OnBuildPostprocess(IBuildProcessor processor) {
			if (processor is iOSBuildProcessor) {

				iOSBuildProcessor ios;
				ios = processor as iOSBuildProcessor;

				string projectPath;
				projectPath = Path.Combine(ios.XcodeProjectPath, "Unity-iPhone.xcodeproj/project.pbxproj");

				PBXProject project;
				project = new PBXProject();
				project.ReadFromFile(projectPath);

				string submodulePath;
				submodulePath = SubmoduleMap.GetSubmodulePath(typeof(SagoApp.SubmoduleInfo));

				string nativeAssetsPath;
				nativeAssetsPath = Path.Combine(ios.ProjectPath, Path.Combine(submodulePath, "Plugins/iOS/.Native"));

				string target;
				target = project.TargetGuidByName(PBXProject.GetUnityTargetName());

				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "AudioSessionUtil.h"),
					Path.Combine("SagoApp", "AudioSessionUtil.h"),
					PBXSourceTree.Source
				));

				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "AudioSessionUtil.mm"),
					Path.Combine("SagoApp", "AudioSessionUtil.mm"),
					PBXSourceTree.Source
				));

				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "SAOnDemandResourceUtil.h"),
					Path.Combine("SagoApp", "SAOnDemandResourceUtil.h"),
					PBXSourceTree.Source
				));

				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "SAOnDemandResourceUtil.mm"),
					Path.Combine("SagoApp", "SAOnDemandResourceUtil.mm"),
					PBXSourceTree.Source
				));

				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "SAInstalledAppsHelper.h"),
					Path.Combine("SagoApp", "SAInstalledAppsHelper.h"),
					PBXSourceTree.Source
				));

				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "SAInstalledAppsHelper.mm"),
					Path.Combine("SagoApp", "SAInstalledAppsHelper.mm"),
					PBXSourceTree.Source
				));

				// Open source third-party library for Unity for storing data in iOS keychain.
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "UICKeyChainStore.h"),
					Path.Combine("SagoApp", "UICKeyChainStore.h"),
					PBXSourceTree.Source
				));
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "UICKeyChainStore.m"),
					Path.Combine("SagoApp", "UICKeyChainStore.m"),
					PBXSourceTree.Source
				));

				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "SAKeychainStore.h"),
					Path.Combine("SagoApp", "SAKeychainStore.h"),
					PBXSourceTree.Source
				));

				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "SAKeychainStore.mm"),
					Path.Combine("SagoApp", "SAKeychainStore.mm"),
					PBXSourceTree.Source
				));

				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "UnityViewController+DeferSystemGestures.mm"),
					Path.Combine("SagoApp", "UnityViewController+DeferSystemGestures.mm"),
					PBXSourceTree.Source
				));
				
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "SendEmailUtil.h"),
					Path.Combine("SagoApp", "SendEmailUtil.h"),
					PBXSourceTree.Source
				));
				
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "SendEmailUtil.mm"),
					Path.Combine("SagoApp", "SendEmailUtil.mm"),
					PBXSourceTree.Source
				));

				// Following section of code is to fix error caused when disposing Unity's OnDemandResourcesRequest
				// even though ODR download is incomplete.

				// First find Unity's original copy of OnDemandREsources.mm file and remove it.
				string onDemandResourcesFileGuid = project.FindFileGuidByProjectPath("Classes/Unity/OnDemandResources.mm");
				project.RemoveFile(onDemandResourcesFileGuid);
				// Now add our updated implmentation of OnDemandResources.mm file.
				project.AddFileToBuild(target, project.AddFile(
					Path.Combine(nativeAssetsPath, "OnDemandResources.mm"),
					Path.Combine("Classes/Unity", "OnDemandResources.mm"),
					PBXSourceTree.Source
				));

				// Adding odr_update_bug_fix.txt & odr_update_bug_fix_v2.txt file to Xcode project and marking
				// it with initial install tag to fix ODR update bug.
				//
				// Jira: https://sagosago.atlassian.net/browse/SW-168
				#if ENABLE_IOS_ON_DEMAND_RESOURCES
					bool hasOdrAssetBundle = HasOnDemandResourcesAssetBundle;
					if (hasOdrAssetBundle) {
						// odr_update_bug_fix.txt
						string odrUpdateBugFixResTag = "odr_update_bug_fix";
						string odrUpdateBugFixResFileName = "odr_update_bug_fix.txt";
						string targetGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
						string odrUpdateBugFixResFileGuid = null;
						project.AddFileToBuild(target, project.AddFile(
							Path.Combine(nativeAssetsPath, odrUpdateBugFixResFileName),
							Path.Combine("SagoApp", odrUpdateBugFixResFileName),
							PBXSourceTree.Source
						));
						odrUpdateBugFixResFileGuid = project.FindFileGuidByProjectPath(Path.Combine("SagoApp", odrUpdateBugFixResFileName));
						project.AddAssetTagForFile(targetGuid, odrUpdateBugFixResFileGuid, odrUpdateBugFixResTag);
						project.AddAssetTagToDefaultInstall(targetGuid, odrUpdateBugFixResTag);

						// odr_update_bug_fix_v2.txt
						odrUpdateBugFixResTag = "odr_update_bug_fix_v2";
						odrUpdateBugFixResFileName = "odr_update_bug_fix_v2.txt";
						targetGuid = project.TargetGuidByName(PBXProject.GetUnityTargetName());
						odrUpdateBugFixResFileGuid = null;
						project.AddFileToBuild(target, project.AddFile(
							Path.Combine(nativeAssetsPath, odrUpdateBugFixResFileName),
							Path.Combine("SagoApp", odrUpdateBugFixResFileName),
							PBXSourceTree.Source
						));
						odrUpdateBugFixResFileGuid = project.FindFileGuidByProjectPath(Path.Combine("SagoApp", odrUpdateBugFixResFileName));
						project.AddAssetTagForFile(targetGuid, odrUpdateBugFixResFileGuid, odrUpdateBugFixResTag);
						project.AddAssetTagToDefaultInstall(targetGuid, odrUpdateBugFixResTag);
					}
				#endif

				project.WriteToFile(projectPath);

				// There is a bug with Unity's XcodeAPI with how it adds resources to initial install tags.
				// It adds to "knownAssetTags" instead of "KnownAssetTags".
				// So we are actually modifying .pbxproj file by search and replace as a work around.
				#if ENABLE_IOS_ON_DEMAND_RESOURCES
					if (hasOdrAssetBundle) {
						string pbxProjFileText = File.ReadAllText(projectPath);
						if (pbxProjFileText.Contains("KnownAssetTags")) {
							throw new System.Exception(string.Format(@"Failed to search and replace 'knownAssetTags' with 'KnownAssetTags'
								in file {0}. Try merging existing 'KnownAssetTags' value instead.", projectPath));
						}
						pbxProjFileText = pbxProjFileText.Replace("knownAssetTags", "KnownAssetTags");
						File.WriteAllText(projectPath, pbxProjFileText);
					}
				#endif


				//Adding few permissions to Info.plist for iOS platform.

				// Get plist
				string plistPath = ios.XcodeProjectPath + "/Info.plist";
				PlistDocument plist = new PlistDocument();
				plist.ReadFromString(File.ReadAllText(plistPath));

				// Get root
				PlistElementDict rootDict = plist.root;

				// Adding iOS permission string values for several permissions that our apps possibly need to Info.plist.
				rootDict.SetString("NSPhotoLibraryUsageDescription", "This app would like to save photos to your camera roll.");
				rootDict.SetString("NSCameraUsageDescription", "This activity requires secure access to your camera.");

				// Apple did not reject app submissions even though the following permissions have their values left empty,
				// so we are just going to leave them empty.
				rootDict.SetString("NSMicrophoneUsageDescription", "");
				rootDict.SetString("NSMotionUsageDescription", "");

				// Leaving this permission's string value empty as well.
				// This is a new permission for iOS 11.
				// Any addtional permissions that needs to be added, refer to:
				// https://developer.apple.com/library/content/documentation/General/Reference/InfoPlistKeyReference/Articles/CocoaKeys.html
				rootDict.SetString("NSPhotoLibraryAddUsageDescription", "");

				// Write to file
				File.WriteAllText(plistPath, plist.WriteToString());
			}
		}

	}

}
#endif
