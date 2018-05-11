namespace SagoUtilsEditor {
	
	using SagoCore.Submodules;
	using SagoUtils;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using UnityEngine;
	using UnityEditor;
	using Debug = UnityEngine.Debug;
	
	
	/// <summary>
	/// The OnInstallGitHooksExample class shows how to use OnInstallGitHooksAttribute.
	/// </summary>
	public class OnInstallGitHooksExample : MonoBehaviour {
		
		/// <summary>
		/// Called before GitUtil.InstallHooks() searches for hooks to install. 
		/// If you add a directory to the list of paths, that directory will be 
		/// searched for hooks to install. If you add a file to the list of paths, 
		/// that file will be installed. Paths must be absolute.
		/// </summary>
		[OnInstallGitHooks]
		public static void OnInstallGitHooks(List<string> paths) {
			
			string submodulePath;
			submodulePath = SubmoduleMap.GetAbsoluteSubmodulePath(typeof(SagoUtils.SubmoduleInfo));
			
			// add a directory (any files in the directory that end with .hook.rb will be installed)
			// paths.Add(Path.Combine(submodulePath, ".githooks"));
			
			// add a file (if the file ends with .hook.rb, it will be installed)
			// paths.Add(Path.Combine(submodulePath, ".githooks/mysubmodule.hook.rb"));
			
			paths.Add(Path.Combine(submodulePath, ".githooks/sago_debug.hook.rb"));
			paths.Add(Path.Combine(submodulePath, ".githooks/sago_demo.hook.rb"));
			paths.Add(Path.Combine(submodulePath, ".githooks/sago_seasonal.hook.rb"));
			
		}
		
	}
	
	
	/// <summary>
	/// The OnInstallGitHooksAttribute allows other submodules to add to the 
	/// list of directories to search when installing git hooks.
	/// </summary>
	public class OnInstallGitHooksAttribute : CallbackOrderAttribute {
		
		/// <summary>
		/// Invoke all methods that have the OnInstallGitHooksAttribute 
		/// and passes the list of paths to each method.
		/// </summary>
		public static void Invoke(List<string> paths) {
			CallbackOrderAttribute.Invoke<OnInstallGitHooksAttribute>(new object[] { paths });
		}
		
		/// <summary>
		/// Creates an OnInstallGitHooksAttribute object.
		/// </summary>
		public OnInstallGitHooksAttribute(int priority = 0) : base(priority) {
			
		}
		
	}
	
	
	/// <summary>
	/// The GitUtil class provides methods for common git-related tasks.
	/// </summary>
	[InitializeOnLoad]
	public static class GitUtil {
		
		
		#region Static Constructor
		
		static GitUtil() {
			#if !(UNITY_CLOUD_BUILD || TEAMCITY_BUILD) && !UNITY_EDITOR_WIN
				EditorApplication.update += AutoInstallHooks;
			#endif
		}
		
		#endregion
		
		
		#region Static Hook Methods
		
		static void AutoInstallHooks() {
			InstallHooks(false);
			EditorApplication.update -= AutoInstallHooks;
		}
		
		#if !UNITY_EDITOR_WIN
		[MenuItem("Sago/Utils/Git/Install Hooks")]
		static void MenuInstallHooks() {
			InstallHooks(true);
		}
		#endif
		
		static void InstallHooks(bool throwExceptions) {
			#if !UNITY_EDITOR_WIN
			
			// get project path
			string projectPath = null;
			{
				Process p = new Process();
				p.StartInfo.FileName = "git";
				p.StartInfo.Arguments = "rev-parse --show-toplevel";
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.UseShellExecute = false;
				p.Start();
				p.WaitForExit();
				
				string error = p.StandardError.ReadToEnd().Trim();
				string output = p.StandardOutput.ReadToEnd().Trim();
				
				if (!string.IsNullOrEmpty(output) && string.IsNullOrEmpty(error)) {
					projectPath = output;
				} else if (throwExceptions) {
					UnityEngine.Debug.LogError(error);
					throw new System.Exception("Could not install hooks.");
				} else {
					return;
				}
				
			}
			
			// get other paths
			List<string> otherPaths = null;
			{
				otherPaths = new List<string>();
				try {
					OnInstallGitHooksAttribute.Invoke(otherPaths);
				} catch (System.Exception e) {
					if (throwExceptions) {
						throw e;
					} else {
						return;
					}
				}
			}
			
			// install hooks
			{	
				List<string> arguments = new List<string>();
				arguments.Add("--debug");
				arguments.Add("--install");
				arguments.Add(projectPath);
				arguments.AddRange(otherPaths);
				
				string submodulePath;
				submodulePath = SubmoduleMap.GetAbsoluteSubmodulePath(typeof(SagoUtils.SubmoduleInfo));
					
				Process p = new Process();
				p.StartInfo.FileName = Path.Combine(submodulePath, ".githooks/hook");
				p.StartInfo.Arguments = string.Join(" ", arguments.ToArray());
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.UseShellExecute = false;
				p.Start();
				p.WaitForExit();
				
				string error = p.StandardError.ReadToEnd().Trim();
				if (!string.IsNullOrEmpty(error)) {
					if (throwExceptions) {
						Debug.LogError(error);
						throw new System.Exception("Could not install hooks.");
					} else {
						return;
					}
				}
				
			}
			
			// open hooks folder in finder
			if (throwExceptions) {
				Process p = new Process();
				p.StartInfo.FileName = "open";
				p.StartInfo.Arguments = Path.Combine(projectPath, ".git/hooks");
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.UseShellExecute = false;
				p.Start();
				p.WaitForExit();
			}
			
			#endif
		}
		
		#endregion
		
		
	}
	
}