namespace SagoBizEditor {
	
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
	
	public class SagoAppAndroidBuildProcessor {
		
		
		#region Internal Methods
		
		[OnBuildPreprocess]
		public static void PreprocessAndroidManifest(IBuildProcessor processor) {
			if (processor is AndroidBuildProcessor) {
				try {
					
					string submodulePath;
					submodulePath = SubmoduleMap.GetSubmodulePath(typeof(SagoPlatform.SubmoduleInfo));
					
					string srcRoot;
					srcRoot = Path.Combine(submodulePath, "Plugins/Android/");
					
					string dstRoot;
					dstRoot = "Assets/Plugins/Android/";
					
					string srcPath;
					srcPath = srcRoot + "." + PlatformUtil.ActivePlatform.ToString() + "/";
					
					string dstPath;
					dstPath = dstRoot + "." + PlatformUtil.ActivePlatform.ToString() + "/";
					
					Debug.Log("Copying AndroidManifest.xml from " + srcPath + " to " + dstPath);
					
					if (Directory.Exists(dstPath) && Directory.Exists(dstPath)) {
						File.Copy(srcPath + "AndroidManifest.xml", dstPath + "AndroidManifest.xml", true);
					}
					
				} catch (System.Exception e) {
					Debug.LogException(e);
				}
			}
		}
		
		#endregion
		
		
	}
	
}