namespace SagoAppEditor {
	
	using SagoCore.Submodules;
	using SagoPlatform;
	using SagoPlatformEditor;
	using SagoUtilsEditor;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	public static class PluginUtil {
		
		
		#region Static Methods
		
		[OnPlatformDidChange(0)]
		public static void SymlinkAssets(Platform oldPlatform, Platform newPlatform) {
			
			AssetDatabase.StartAssetEditing();
			
			string submodulePath;
			submodulePath = SubmoduleMap.GetSubmodulePath(typeof(SagoApp.SubmoduleInfo));

			bool symlinkGooglePlayDownloader = newPlatform == Platform.GooglePlay || newPlatform == Platform.GooglePlayFree;

			{
				string path = Path.Combine(
					submodulePath,
					"Plugins/Android/.Native/dist/googlePlayDownloader-release.aar"
				);
				SymlinkAARfile(path, symlinkGooglePlayDownloader);
			}

			{
				string path = Path.Combine(
					submodulePath,
					"Plugins/Android/.Native/dist/sagoApp-release.aar"
				);
				SymlinkAARfile(path, newPlatform.ToBuildTargetGroup() == BuildTargetGroup.Android);
			}

			{
				string path = Path.Combine(
					submodulePath,
					"Plugins/Android/.Native/dist/sagoPermission-release.aar"
				);
				SymlinkAARfile(path, newPlatform.ToBuildTargetGroup() == BuildTargetGroup.Android);
			}

			{
				string path = Path.Combine(
					submodulePath,
					"Plugins/Android/.Native/dist/support-compat-25.1.1.aar"
				);
				SymlinkAARfile(path, newPlatform.ToBuildTargetGroup() == BuildTargetGroup.Android);
			}

			{
				string path = Path.Combine(
					submodulePath,
					"Plugins/Android/.Native/dist/support-fragment-25.1.1.aar"
				);
				SymlinkAARfile(path, newPlatform.ToBuildTargetGroup() == BuildTargetGroup.Android);
			}
			
			{
				string path = Path.Combine(
					submodulePath,
					"Plugins/Android/.Native/dist/support-core-utils-25.1.1.aar"
				);
				SymlinkAARfile(path, newPlatform.ToBuildTargetGroup() == BuildTargetGroup.Android);
			}

			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
			
		}
		
		public static void SymlinkAARfile(string aarFilePath, bool symlinkCondition) {
			if (!string.IsNullOrEmpty(aarFilePath) && File.Exists(aarFilePath)) {
				
				string srcName = Path.GetFileName(aarFilePath);
				string dstPath = Path.Combine("Assets/Plugins/Android", srcName);
				
				if (symlinkCondition) {
					AssetUtil.SymlinkAsset(aarFilePath, dstPath, false);
				} else {
					AssetUtil.UnlinkAsset(dstPath);
				}
			}
		}
		
		#endregion
		
		
	}
	
}
