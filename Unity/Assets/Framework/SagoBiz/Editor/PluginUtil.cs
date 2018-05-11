namespace SagoBizEditor {
	
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
			
			PluginUtilEditor.SymlinkAndroidLibraryProject(
				oldPlatform, 
				newPlatform, 
				Path.Combine(SubmoduleMap.GetSubmodulePath(typeof(SagoBiz.SubmoduleInfo)), "Plugins/Android/.Native/externalLibs")
			);
			
			SymlinkAARfile(
				oldPlatform, 
				newPlatform, 
				Path.Combine(SubmoduleMap.GetSubmodulePath(typeof(SagoBiz.SubmoduleInfo)), "Plugins/Android/.Native/dist/sagoBiz-release.aar")
			);
			
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
			
		}

		public static void SymlinkAARfile(Platform oldPlatform, Platform newPlatform, string aarFilePath) {
			
			if (!string.IsNullOrEmpty(aarFilePath) && File.Exists(aarFilePath)) {
				
				string srcName = Path.GetFileName(aarFilePath);
				string dstPath = Path.Combine("Assets/Plugins/Android", srcName);

				if (newPlatform.ToBuildTargetGroup() == BuildTargetGroup.Android) {
					
					AssetUtil.SymlinkAsset(aarFilePath, dstPath, false);
				} else {
					AssetUtil.UnlinkAsset(dstPath);
				}
			}
		}
		
		#endregion
		
		
	}
	
}