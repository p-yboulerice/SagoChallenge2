namespace SagoPlatformEditor {
	
	using SagoUtilsEditor;
	using SagoCore.Submodules;
	using SagoPlatform;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	public static class PluginUtilEditor {
		
		#region Editor Callbacks
		
		
		[OnPlatformDidChange(0)]
		public static void CopyAssets(Platform oldPlatform, Platform newPlatform) {
			
			// start editing
			AssetDatabase.StartAssetEditing();
			
			string submodulePath;
			submodulePath = SubmoduleMap.GetSubmodulePath(typeof(SagoPlatform.SubmoduleInfo));
			
			// copy manifest
			AssetUtil.CopyAsset(
				Path.Combine(submodulePath, "Plugins/Android/AndroidManifest.xml"),
				"Assets/Plugins/Android/AndroidManifest.xml",
				true
			);
			
			// copy plugins to dot folders
			foreach (Platform platform in PlatformUtilEditor.AndroidPlatforms) {
				
				string srcPath;
				srcPath = string.Format(
					Path.Combine(submodulePath, "Plugins/{0}/.{1}"), 
					platform.ToBuildTargetGroup().ToString(),
					platform.ToString()
				);
				
				string dstPath;
				dstPath = string.Format(
					"Assets/Plugins/{0}/.{1}", 
					platform.ToBuildTargetGroup().ToString(),
					platform.ToString()
				);
				
				AssetUtil.CopyAsset(srcPath, dstPath, false);
				
			}
			
			// stop editing
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
			
		}

		[OnPlatformDidChange(1)]
		public static void CleanUpLegacyNaming(Platform oldPlatform, Platform newPlatform) {
			CleanUpLegacySymlinks(oldPlatform, newPlatform);
			CleanUpLegacyCopiedAssets();
		}
		
		[OnPlatformDidChange(2)]
		public static void SymlinkAssets(Platform oldPlatform, Platform newPlatform) {
			
			// start editing
			AssetDatabase.StartAssetEditing();
			
			// update symlinks to dot folders
			foreach (Platform platform in PlatformUtilEditor.AndroidPlatforms) {
				
				string srcPath;
				srcPath = GetPluginPathForPlatform(platform);
				
				string dstPath;
				dstPath = string.Format(
					"Assets/Plugins/{0}/{1}", 
					platform.ToBuildTargetGroup().ToString(), 
					platform.ToSagoUniqueString()
				);
				
				if (platform == newPlatform) {
					AssetUtil.SymlinkAsset(srcPath, dstPath, false);
				} else {
					AssetUtil.UnlinkAsset(dstPath);
				}
			}
			
			// stop editing
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
			
		}
		
		#endregion
		
		
		#region Helper Methods

		public static string GetPluginPathForPlatform(Platform platform) {
			return string.Format(
				"Assets/Plugins/{0}/.{1}", 
				platform.ToBuildTargetGroup().ToString(), 
				platform.ToString()
			);
		}
		
		public static void SymlinkAndroidLibraryProject(Platform oldPlatform, Platform newPlatform, string srcRoot) {
			foreach (string libraryPath in Directory.GetFiles(srcRoot, "*.aar")) {
				
				string srcName = Path.GetFileName(libraryPath);
				string srcPath = Path.Combine(srcRoot, srcName);
				string dstPath = Path.Combine("Assets/Plugins/Android", srcName);
				
				if (!string.IsNullOrEmpty(srcPath) && File.Exists(srcPath)) {
					if (newPlatform.ToBuildTargetGroup() == BuildTargetGroup.Android) {
						AssetUtil.SymlinkAsset(srcPath, dstPath, false);
					} else {
						AssetUtil.UnlinkAsset(dstPath);
					}
				}
				
			}
		}
		
		public static void SymlinkEclipseProject(Platform oldPlatform, Platform newPlatform, string srcPath) {
			
			string dstName = Path.GetFileName(srcPath).TrimStart('.');
			string dstPath = Path.Combine("Assets/Plugins/Android", dstName);
			
			if (!string.IsNullOrEmpty(srcPath) && Directory.Exists(srcPath)) {
				if (newPlatform.ToBuildTargetGroup() == BuildTargetGroup.Android) {
					AssetUtil.SymlinkAsset(srcPath, dstPath, false);
				} else {
					AssetUtil.UnlinkAsset(dstPath);
				}
			}
			
		}
		
		/// <summary>
		/// Cleans up symlinks that were created in working copies before commit 
		/// eef9c667807adb537d4677fdd97300daa576e9b2. Before the switch to sago prefixed symlinks.
		/// </summary>
		private static void CleanUpLegacySymlinks(Platform oldPlatform, Platform newPlatform) {
			// start editing
			AssetDatabase.StartAssetEditing();
			
			// Remove any symlinks that still exist from the legacy naming convention before
			// the directories were named based off platform.ToSagoUniqueString()
			foreach (Platform platform in PlatformUtilEditor.AndroidPlatforms) {
				
				string legacyLinkPath;
				legacyLinkPath = string.Format(
					"Assets/Plugins/{0}/{1}", 
					platform.ToBuildTargetGroup().ToString(), 
					platform.ToString()
				);
				
				AssetUtil.UnlinkAsset(legacyLinkPath);
			}
			
			// stop editing
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Cleans up .sago{platform] specific folders that were accidentally introduced by changes in commit
		/// eef9c667807adb537d4677fdd97300daa576e9b2. Very few working copies should ever have this.
		/// </summary>
		private static void CleanUpLegacyCopiedAssets() {
			// start editing
			AssetDatabase.StartAssetEditing();
			
			// Remove any .sago{platform} directories that were accidentally created in the past
			foreach (Platform platform in PlatformUtilEditor.AndroidPlatforms) {
				
				string legacyCopiedAssetPath;
				legacyCopiedAssetPath = string.Format(
					"Assets/Plugins/{0}/.{1}", 
					platform.ToBuildTargetGroup().ToString(),
					platform.ToSagoUniqueString()
				);

				if (Directory.Exists(legacyCopiedAssetPath)) {
					Debug.Log("Clean up legacy platform folder: " + legacyCopiedAssetPath);
					FileUtil.DeleteFileOrDirectory(legacyCopiedAssetPath);
				}
			}
			
			// stop editing
			AssetDatabase.StopAssetEditing();
			AssetDatabase.Refresh();
		}
		
		#endregion
		
	}
	
}