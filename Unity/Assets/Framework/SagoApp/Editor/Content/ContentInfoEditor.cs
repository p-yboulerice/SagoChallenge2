namespace SagoAppEditor.Content {
	
	using SagoApp.Content;
	using SagoCore.Submodules;
	using SagoCore.Resources;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using Debug = UnityEngine.Debug;
	using Stopwatch = System.Diagnostics.Stopwatch;
	
	/// <summary>
	/// The ResourceAssetBundleInfo is the data structure for storing info returned by the <c>resource_asset_bundle_info</c> ruby script.
	/// </summary>
	[System.Serializable]
	struct ResourceAssetBundleInfo {
		
		[SerializeField]
		public string[] AddToResourceAssetBundle;
		
		[SerializeField]
		public string[] RemoveFromResourceAssetBundle;
		
	}
	
	/// <summary>
	/// The ContentInfoEditor class provides a custom inspector for <see cref="ContentInfo" /> assets.
	/// </summary>
	[CustomEditor(typeof(ContentInfo), true)]
	public class ContentInfoEditor : Editor {
		
		
		#region Static Methods - Context Menu
		
		[MenuItem("CONTEXT/ContentInfo/Update", false, 1000)]
		private static void UpdateContextMenuItem(MenuCommand command) {
			UpdateContentInfo(command.context.GetType());
		}
		
		[MenuItem("CONTEXT/ContentInfo/Apply Asset Bundle Names", false, 1100)]
		private static void ApplyAssetBundleNamesContextMenuItem(MenuCommand command) {
			ApplyAssetBundleNames(command.context.GetType());
		}
		
		[MenuItem("CONTEXT/ContentInfo/Import Settings From Project...", false, 1100)]
		private static void ImportSettingsFromProjectContextMenuItem(MenuCommand command) {
			
			ContentInfo contentInfo;
			contentInfo = UpdateContentInfo(command.context.GetType());
			
			string projectPath;
			projectPath = EditorUtility.OpenFolderPanel("Import Settings From Project", null, null);
			
			if (!string.IsNullOrEmpty(projectPath)) {
				DynamicsManagerEditor.ImportDynamicsManagerFromProject(contentInfo.DynamicsManager, projectPath);
				GraphicsSettingsEditor.ImportGraphicsSettingsFromProject(contentInfo.GraphicsSettings, projectPath);
				Physics2DSettingsEditor.ImportPhysics2DSettingsFromProject(contentInfo.Physics2DSettings, projectPath);
				TagManagerEditor.ImportTagManagerFromProject(contentInfo.TagManager, projectPath);
				TimeManagerEditor.ImportTimeManagerFromProject(contentInfo.TimeManager, projectPath);
			}
			
		}
		
		[MenuItem("CONTEXT/ContentInfo/Update Legacy Mesh Animators", false, 1100)]
		private static void UpdateMeshAnimatorsToAssetGuids(MenuCommand command) {
			
			string submodulePath;
			submodulePath = SubmoduleMapEditorAdaptor.GetSubmodulePath(command.context.GetType());
			
			if (!string.IsNullOrEmpty(submodulePath)) {
				UpdateMeshAnimatorsInPrefabsToAssetGuids(submodulePath);
				UpdateMeshAnimatorsInScenesToAssetGuids(submodulePath);
			}
			
		}
		
		private static void UpdateMeshAnimatorsInPrefabsToAssetGuids(string submodulePath) {
			
			string[] prefabPaths = (
				AssetDatabase
				.GetAllAssetPaths()
				.Where(path => path.StartsWith(submodulePath) && path.EndsWith(".prefab"))
				.ToArray()
			);
			
			
			int updatedAnimatorCount;
			updatedAnimatorCount = 0;
			
			foreach (string path in prefabPaths) {
				
				GameObject prefab;
				prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
				
				SagoEngine.MeshAnimator[] animators;
				animators = prefab.GetComponentsInChildren<SagoEngine.MeshAnimator>(true);
				
				if (animators.Length == 0) {
					continue;
				}
				
				foreach (SagoEngine.MeshAnimator animator in animators) {
					
					bool hasAnimation = !(animator.AnimationAsset == null);
					bool hasPath = !string.IsNullOrEmpty(animator.AnimationAssetPath);
					bool hasGuid = !string.IsNullOrEmpty(animator.AnimationAssetGuid);
					
					if (hasAnimation && !hasGuid) {
						
						string animationAssetPath;
						animationAssetPath = AssetDatabase.GetAssetPath(animator.AnimationAsset);
						
						string animationAssetGuid;
						animationAssetGuid = AssetDatabase.AssetPathToGUID(animationAssetPath);
						
						animator.AnimationAssetGuid = animationAssetGuid;
						EditorUtility.SetDirty(animator);
						EditorUtility.SetDirty(prefab);
						updatedAnimatorCount++;
						
					} else if (hasPath && !hasGuid) {
						
						string animationAssetPath;
						animationAssetPath = Path.Combine(submodulePath, Path.Combine(
							"_Resources_/MeshAnimations", 
							Path.GetFileName(animator.AnimationAssetPath
						)));
						
						string animationAssetGuid;
						animationAssetGuid = AssetDatabase.AssetPathToGUID(animationAssetPath);
						
						animator.AnimationAssetGuid = animationAssetGuid;
						EditorUtility.SetDirty(animator);
						EditorUtility.SetDirty(prefab);
						updatedAnimatorCount++;
						
					}
					
				}
				
			}
			AssetDatabase.SaveAssets();
			
			Debug.LogFormat("Updated {0} mesh animator(s) in {1} prefab(s).", updatedAnimatorCount, prefabPaths.Length);
			
		}
		
		private static void UpdateMeshAnimatorsInScenesToAssetGuids(string submodulePath) {
			
			string[] scenePaths = (
				AssetDatabase
				.GetAllAssetPaths()
				.Where(path => path.StartsWith(submodulePath) && path.EndsWith(".unity"))
				.ToArray()
			);
			
			int updatedAnimatorCount;
			updatedAnimatorCount = 0;
			
			foreach (string path in scenePaths) {
				
				Scene scene;
				scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
				
				SagoEngine.MeshAnimator[] animators;
				animators = (
					scene
					.GetRootGameObjects()
					.SelectMany(gameObject => gameObject.GetComponentsInChildren<SagoEngine.MeshAnimator>(true))
					.ToArray()
				);
				
				if (animators.Length == 0) {
					continue;
				}
				
				foreach (SagoEngine.MeshAnimator animator in animators) {
					
					bool hasAnimation = !(animator.AnimationAsset == null);
					bool hasPath = !string.IsNullOrEmpty(animator.AnimationAssetPath);
					bool hasGuid = !string.IsNullOrEmpty(animator.AnimationAssetGuid);
					
					if (hasAnimation && !hasGuid) {
						
						string animationAssetPath;
						animationAssetPath = AssetDatabase.GetAssetPath(animator.AnimationAsset);
						
						string animationAssetGuid;
						animationAssetGuid = AssetDatabase.AssetPathToGUID(animationAssetPath);
						
						animator.AnimationAssetGuid = animationAssetGuid;
						EditorUtility.SetDirty(animator);
						EditorSceneManager.MarkSceneDirty(scene);
						updatedAnimatorCount++;
						
					} else if (hasPath && !hasGuid) {
						
						string animationAssetPath;
						animationAssetPath = Path.Combine(submodulePath, Path.Combine(
							"_Resources_/MeshAnimations", 
							Path.GetFileName(animator.AnimationAssetPath)
						));
						
						string animationAssetGuid;
						animationAssetGuid = AssetDatabase.AssetPathToGUID(animationAssetPath);
						
						animator.AnimationAssetGuid = animationAssetGuid;
						EditorUtility.SetDirty(animator);
						EditorSceneManager.MarkSceneDirty(scene);
						updatedAnimatorCount++;
						
					}
					
				}
				
				if (scene.isDirty) {
					EditorSceneManager.SaveScene(scene, scene.path, false);
				}
				
				
			}
			
			Debug.LogFormat("Updated {0} mesh animator(s) in {1} scene(s).", updatedAnimatorCount, scenePaths.Length);
			
		}
		
		#endregion
		
		
		#region Static Methods - Content Info
		
		/// <summary>
		/// Gets the list of <see cref="ContentInfo" /> types in the project.
		/// </summary>
		public static System.Type[] ContentInfoTypes {
			get {
				return (
					System
					.AppDomain
					.CurrentDomain
					.GetAssemblies()
					.SelectMany(assembly => assembly.GetTypes())
					.Where(type => type.BaseType == typeof(ContentInfo))
					.ToArray()
				);
			}
		}
		
		/// <summary>
		/// Creates a new <see cref="ContentInfo" /> asset for the specified content submodule.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </typeparam>
		public static ContentInfo CreateContentInfo<T>() where T : ContentInfo {
			return CreateContentInfo(typeof(T));
		}
		
		/// <summary>
		/// Creates a new <see cref="ContentInfo" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static ContentInfo CreateContentInfo(System.Type type) {
			string path = GetContentInfoPath(type);
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(type), path);
			return FindContentInfo(type);
		}
		
		/// <summary>
		/// Finds the existing <see cref="ContentInfo" /> asset for the specified content submodule.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </typeparam>
		public static ContentInfo FindContentInfo<T>() where T : ContentInfo {
			return FindContentInfo(typeof(T));
		}
		
		/// <summary>
		/// Finds the existing <see cref="ContentInfo" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static ContentInfo FindContentInfo(System.Type type) {
			return AssetDatabase.LoadAssetAtPath(GetContentInfoPath(type), type) as ContentInfo;
		}
		
		/// <summary>
		/// Finds the existing <see cref="ContentInfo" /> asset or creates a new <see cref="ContentInfo" /> asset for the specified content submodule.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </typeparam>
		public static ContentInfo FindOrCreateContentInfo<T>() where T : ContentInfo {
			return FindOrCreateContentInfo(typeof(T));
		}
		
		/// <summary>
		/// Finds the existing <see cref="ContentInfo" /> asset or creates a new <see cref="ContentInfo" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static ContentInfo FindOrCreateContentInfo(System.Type type) {
			return FindContentInfo(type) ?? CreateContentInfo(type);
		}
		
		/// <summary>
		/// Gets the path to the <see cref="ContentInfo" /> asset for the specified content submodule.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </typeparam>
		public static string GetContentInfoPath<T>() where T : ContentInfo {
			return GetContentInfoPath(typeof(T));
		}
		
		/// <summary>
		/// Gets the path to the <see cref="ContentInfo" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static string GetContentInfoPath(System.Type type) {
			return Path.Combine(SubmoduleMapEditorAdaptor.GetSubmodulePath(type), Path.Combine("Resources", Path.Combine(SubmoduleMapEditorAdaptor.GetSubmoduleName(type),"ContentInfo.asset")));
		}
		
		/// <summary>
		/// Updates the <see cref="ContentInfo" /> asset for the specified content submodule.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </typeparam>
		public static ContentInfo UpdateContentInfo<T>() where T : ContentInfo {
			return UpdateContentInfo(typeof(T));
		}
		
		/// <summary>
		/// Updates the <see cref="ContentInfo" /> asset for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static ContentInfo UpdateContentInfo(System.Type type) {
			
			ContentInfo contentInfo;
			contentInfo = FindOrCreateContentInfo(type);
			
			SerializedObject serializedObject;
			serializedObject = new SerializedObject(contentInfo);
			
			serializedObject.FindProperty("m_ResourceAssetBundleName").stringValue = (
				string.IsNullOrEmpty(contentInfo.ResourceAssetBundleName) ? 
				GetResourceAssetBundleName(type) :
				contentInfo.ResourceAssetBundleName
			);
				
			serializedObject.FindProperty("m_SceneAssetBundleName").stringValue = (
				string.IsNullOrEmpty(contentInfo.SceneAssetBundleName) ? 
				GetSceneAssetBundleName(type) :
				contentInfo.SceneAssetBundleName
			);
				
			serializedObject.FindProperty("m_DynamicsManager").objectReferenceValue = DynamicsManagerEditor.UpdateDynamicsManager(type);
			serializedObject.FindProperty("m_GraphicsSettings").objectReferenceValue = GraphicsSettingsEditor.UpdateGraphicsSettings(type);
			serializedObject.FindProperty("m_Physics2DSettings").objectReferenceValue = Physics2DSettingsEditor.UpdatePhysics2DSettings(type);
			serializedObject.FindProperty("m_TagManager").objectReferenceValue = TagManagerEditor.UpdateTagManager(type);
			serializedObject.FindProperty("m_TimeManager").objectReferenceValue = TimeManagerEditor.UpdateTimeManager(type);
			
			serializedObject.ApplyModifiedPropertiesWithoutUndo();
			
			return FindContentInfo(type);
			
		}
		
		#endregion
		
		
		#region Static Methods - Asset Bundles
		
		/// <summary>
		/// Applies asset bundle names to each asset in the specified content submodule.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </typeparam>
		public static void ApplyAssetBundleNames<T>() where T : ContentInfo {
			ApplyAssetBundleNames(typeof(T));
		}
		
		/// <summary>
		/// Applies asset bundle names to each asset in the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		public static void ApplyAssetBundleNames(System.Type type) {
			ApplyResourceAssetBundleName(type);
			ApplySceneAssetBundleName(type);
		}
		
		/// <summary>
		/// Applies the <see cref="ContentInfo.ResourceAssetBundleName" /> to every 
		/// asset that should be included in the resource asset bundle for the 
		/// specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		private static void ApplyResourceAssetBundleName(System.Type type) {
			
			// To make sure all of the required assets get added to the resource 
			// asset bundle, we need to find:
			//   
			//   * All assets in any _Resources_ folder
			//   * All dependencies of assets in any _Resources_ folder
			//   * All dependencies of all scenes
			//   
			// We can get the info in C#, but it's very, very slow because Unity 
			// has to actually load all the assets before it can figure out the 
			// dependencies.
			// 
			// Instead, we're calling out to a ruby script to do the heavy lifting. 
			// It crawls through the all of the asset files in the submodule and 
			// figures out which assets need to be in the resource asset bundle. 
			// 
			// However, the script can't add the asset bundle name to the meta files 
			// directly (modifying the meta files causes Unity to reimport the asset, 
			// which is really slow for things like sounds).
			// 
			// Instead, the script reads the meta files for all of the assets in the 
			// submodule and builds two lists and returns them as JSON data:
			//   
			//   * Assets that need the asset bundle name added
			//   * Assets that need the asset bundle name removed
			//   
			// Then, in C#, we can just loop through the two lists to add or remove 
			// the asset bundle name where necessary using the AssetImporter (which
			// doesn't cause Unity to reimport the asset).
			
			ContentInfo contentInfo;
			contentInfo = FindOrCreateContentInfo(type);
			
			string progressTitle;
			progressTitle = string.Format("ApplyAssetBundleNames");
			
			string progressInfo1;
			progressInfo1 = string.Format("Finding assets: {0}", contentInfo.ResourceAssetBundleName);
			
			string progressInfo2;
			progressInfo2 = string.Format("Applying asset bundle name: {0}", contentInfo.ResourceAssetBundleName);
			
			int progressIndex;
			progressIndex = 0;
			
			float progressTotal;
			progressTotal = 1;
			
			EditorUtility.DisplayProgressBar(progressTitle, progressInfo1, progressIndex / progressTotal);
			
			string command;
			command = Path.Combine(SubmoduleMapEditorAdaptor.GetAbsoluteSubmodulePath(typeof(SagoApp.SubmoduleInfo)), ".ruby/resource_asset_bundle_info");
			
			string[] arguments;
			arguments = new string[] {
				string.Format("--project_path {0}", Path.GetDirectoryName(Application.dataPath)),
				string.Format("--submodule_path {0}", SubmoduleMapEditorAdaptor.GetAbsoluteSubmodulePath(type)),
				string.Format("--asset_bundle_name {0}", contentInfo.ResourceAssetBundleName)
			};
			
			string error;
			error = string.Empty;
			
			string output;
			output = string.Empty;
			
			Process process = new Process();
			process.StartInfo.FileName = command;
			process.StartInfo.Arguments = string.Join(" ", arguments);
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.UseShellExecute = false;
			process.ErrorDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => {
				if (!string.IsNullOrEmpty(e.Data)) {
					error += e.Data + "\n";
				}
			});
			process.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => {
				if (!string.IsNullOrEmpty(e.Data)) {
					output += e.Data + "\n";
				}
			});
			process.Start();
			process.BeginErrorReadLine();
			process.BeginOutputReadLine();
			process.WaitForExit();
			
			if (!string.IsNullOrEmpty(error)) {
				throw new System.InvalidOperationException(error);
			}
			
			ResourceAssetBundleInfo info;
			info = JsonUtility.FromJson<ResourceAssetBundleInfo>(output);
			
			progressIndex = 0;
			progressTotal = info.AddToResourceAssetBundle.Length + info.RemoveFromResourceAssetBundle.Length - 1;
			
			AssetDatabase.StartAssetEditing();
			foreach (string assetPath in info.AddToResourceAssetBundle) {
			
				EditorUtility.DisplayProgressBar(progressTitle, progressInfo2, progressIndex / progressTotal);
				progressIndex++;
				
				AssetImporter importer;
				importer = AssetImporter.GetAtPath(assetPath);
				importer.SetAssetBundleNameAndVariant(contentInfo.ResourceAssetBundleName, string.Empty);
				EditorUtility.SetDirty(importer);
				AssetDatabase.WriteImportSettingsIfDirty(assetPath);
				
			}
			foreach (string assetPath in info.RemoveFromResourceAssetBundle) {
			
				EditorUtility.DisplayProgressBar(progressTitle, progressInfo2, progressIndex++ / progressTotal);
				progressIndex++;
				
				AssetImporter importer;
				importer = AssetImporter.GetAtPath(assetPath);
				importer.SetAssetBundleNameAndVariant(string.Empty, string.Empty);
				EditorUtility.SetDirty(importer);
				AssetDatabase.WriteImportSettingsIfDirty(assetPath);
				
			}
			AssetDatabase.StopAssetEditing();
			AssetDatabase.RemoveUnusedAssetBundleNames();
			EditorUtility.ClearProgressBar();
			
		}
		
		/// <summary>
		/// Applies the <see cref="ContentInfo.SceneAssetBundleName" /> to every 
		/// asset that should be included in the scene asset bundle for the 
		/// content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		private static void ApplySceneAssetBundleName(System.Type type) {
			
			ContentInfo contentInfo;
			contentInfo = FindOrCreateContentInfo(type);
			
			string progressTitle;
			progressTitle = string.Format("ApplyAssetBundleNames");
			
			string progressInfo1;
			progressInfo1 = string.Format("Finding scenes: {0}", contentInfo.SceneAssetBundleName);
			
			string progressInfo2;
			progressInfo2 = string.Format("Applying asset bundle name: {0}", contentInfo.SceneAssetBundleName);
			
			int progressIndex;
			progressIndex = 0;
			
			float progressTotal;
			progressTotal = 1;
			
			EditorUtility.DisplayProgressBar(progressTitle, progressInfo1, progressIndex / progressTotal);
			
			string submodulePath;
			submodulePath = SubmoduleMapEditorAdaptor.GetSubmodulePath(type);
			
			string[] allScenePaths;
			allScenePaths = (
				AssetDatabase
				.FindAssets("t:SceneAsset", new string[]{ submodulePath })
				.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
				.ToArray()
			);
			
			string[] scenePaths;
			scenePaths = (
				AssetDatabase
				.FindAssets("t:SceneAsset", new string[]{ submodulePath })
				.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
				.Where(path => path.Contains("_Scenes_"))
				.Where(path => File.Exists(path))
				.ToArray()
			);
			
			progressIndex = 0;
			progressTotal = allScenePaths.Length - 1;
			
			foreach (string scenePath in allScenePaths) {
				
				EditorUtility.DisplayProgressBar(progressTitle, progressInfo2, progressIndex / progressTotal);
				progressIndex++;
				
				string assetBundleName;
				assetBundleName = scenePaths.Contains(scenePath) ? contentInfo.SceneAssetBundleName : null;
				
				AssetImporter importer;
				importer = AssetImporter.GetAtPath(scenePath);
				importer.SetAssetBundleNameAndVariant(assetBundleName, null);
				EditorUtility.SetDirty(importer);
				AssetDatabase.WriteImportSettingsIfDirty(scenePath);
				
			}
			
			AssetDatabase.RemoveUnusedAssetBundleNames();
			EditorUtility.ClearProgressBar();
			
		}
		
		/// <summary>
		/// Gets the <see cref="ContentInfo.ResourceAssetBundleName" /> for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		private static string GetResourceAssetBundleName(System.Type type) {
			return string.Format("{0}_resources", SubmoduleMapEditorAdaptor.GetSubmoduleName(type).ToLower());
		}
		
		/// <summary>
		/// Gets the <see cref="ContentInfo.SceneAssetBundleName" /> for the specified content submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="ContentInfo" /> subclass for the content submodule.
		/// </param>
		private static string GetSceneAssetBundleName(System.Type type) {
			return string.Format("{0}_scenes", SubmoduleMapEditorAdaptor.GetSubmoduleName(type).ToLower());
		}
		
		#endregion
		
		
	}
	
}