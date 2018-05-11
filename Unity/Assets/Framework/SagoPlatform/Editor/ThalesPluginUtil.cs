namespace SagoPlatformEditor {
	
	using SagoUtilsEditor;
	using SagoPlatform;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	public static class ThalesPluginUtil {
		
		
		#region Static Methods
		
		[OnPlatformDidChange(0)]
		public static void SymlinkAssets(Platform oldPlatform, Platform newPlatform) {

			AssetDatabase.StartAssetEditing();

			SymlinkAARfile("Assets/External/SagoPlatform/Plugins/Android/.Thales/.src/thales/build/outputs/aar/thales-release.aar", newPlatform == Platform.Thales);

			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
			
		}

		public static void SymlinkAARfile(string aarFilePath, bool symlinkCondition) {
			if (!string.IsNullOrEmpty(aarFilePath) && File.Exists(aarFilePath)) {
				
				string srcName = Path.GetFileName(aarFilePath);
				string dstPath = Path.Combine("Assets/Plugins/Android/", srcName);
				
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