namespace SagoPhotoEditor {

	using PlistCS;
	using SagoBuildEditor.Core;
	using SagoCore.Submodules;
	using SagoPlatform;
	using SagoPlatformEditor;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEngine;
	using UnityEngine.CloudBuild;
	using Version = System.Version;
	using SagoBuildEditor;
	using SagoBuildEditor.Android;
	using SagoUtilsEditor;

	public class SagoPhotoAndroidBuildProcessor {


		#region Internal Methods

		[OnPlatformDidChange]
		public static void SymlinkLibrary(Platform oldPlatform, Platform newPlatform) {
			try {

				string submodulePath;
				submodulePath = SubmoduleMap.GetSubmodulePath(typeof(SagoPhoto.SubmoduleInfo));

				string srcPath;
				srcPath = Path.Combine(submodulePath, "Plugins/Android/.Native/");

				string dstRoot;
				dstRoot = "Assets/Plugins/Android/";

				string dstPath;
				dstPath = dstRoot;

				Debug.Log("Copying SaveScreenshot.jar from " + srcPath + " to " + dstPath);

				if (Directory.Exists(srcPath) && Directory.Exists(dstPath)) {
					if (newPlatform.ToBuildTargetGroup() == BuildTargetGroup.Android) {
						AssetUtil.SymlinkAsset(srcPath + "SaveScreenshot.jar", dstPath + "SaveScreenshot.jar", false);
					} else {
						AssetUtil.UnlinkAsset(dstPath + "SaveScreenshot.jar");
					}
				}

			} catch (System.Exception e) {
				Debug.LogException(e);
			}
		}

		#endregion


	}

}