namespace SagoCoreEditor.AssetBundles {
	
	using SagoCore.AssetBundles;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// TODO: Not sure this class will stay like this, need to add documentation when it's stable.
	/// </summary>
	[InitializeOnLoad]
	public class AssetBundleScenesFix : MonoBehaviour {
		
		
		#region Static Constructor
		
		static AssetBundleScenesFix() {
			EditorApplication.delayCall += Apply;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}
		
		#endregion
		
		
		#region Static Methods

		private static void OnPlayModeStateChanged(PlayModeStateChange playState) {
			Apply();
		}
		
		private static void Apply() {
			
			string[] assetBundleScenePaths = (
				AssetDatabase
				.GetAllAssetPaths()
				.Where(p => p.EndsWith(".unity"))
				.Where(p => !string.IsNullOrEmpty(AssetImporter.GetAtPath(p).assetBundleName))
				.ToArray()
			);
			
			string[] buildSettingsScenePaths = (
				EditorBuildSettings
				.scenes
				.Select(s => s.path)
				.ToArray()
			);
			
			if (EditorApplication.isPlayingOrWillChangePlaymode && AssetBundleOptions.UseAssetDatabaseInEditor) {
				
				string[] scenePathsToAdd = (
					assetBundleScenePaths
					.Where(p => !buildSettingsScenePaths.Contains(p))
					.ToArray()
				);
				
				EditorBuildSettings.scenes = (
					EditorBuildSettings
					.scenes
					.Union(scenePathsToAdd.Select(p => new EditorBuildSettingsScene(p, true)))
					.ToArray()
				);
				
			} else {
				
				string[] scenePathsToRemove = (
					assetBundleScenePaths
					.Where(p => buildSettingsScenePaths.Contains(p))
					.ToArray()
				);
				
				EditorBuildSettings.scenes = (
					EditorBuildSettings
					.scenes
					.Where(s => !scenePathsToRemove.Contains(s.path))
					.ToArray()
				);
				
			}
			
		}
		
		#endregion
		
		
	}
	
}