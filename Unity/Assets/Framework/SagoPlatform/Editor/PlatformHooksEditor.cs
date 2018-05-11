namespace SagoPlatformEditor {
	
	using SagoCore.Submodules;
	using SagoUtilsEditor;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;
	using UnityEditor;
	
	public class OnInstallGitHooksExample : MonoBehaviour {
		
		[OnInstallGitHooks]
		public static void OnInstallGitHooks(List<string> paths) {
			paths.Add(Path.Combine(SubmoduleMap.GetAbsoluteSubmodulePath(typeof(SagoPlatform.SubmoduleInfo)), ".githooks"));
		}
		
	}
	
}