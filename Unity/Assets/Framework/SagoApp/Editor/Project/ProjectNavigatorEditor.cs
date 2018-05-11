namespace SagoAppEditor.Project {
	
	using SagoApp.Project;
	using SagoCore.AssetBundles;
	using SagoCore.Scenes;
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	
	[CustomEditor(typeof(ProjectNavigator))]
	public class ProjectNavigatorEditor : Editor {
		
		
		#region Constants
		
		private const string LastScenePathKey = "SagoAppEditor.Project.ProjectNavigatorEditor.LastScenePath";
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Registers editor callbacks.
		/// </summary>
		[InitializeOnLoadMethod]
		private static void Init() {
			EditorApplication.playModeStateChanged += (PlayModeStateChange playState) => {
				if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying) {
					OnEditorApplicationWillStartPlaying();
				} else if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying) {
					OnEditorApplicationDidStopPlaying();
				}
			};
		}
		
		/// <summary>
		/// Prompts the user if it's not safe to play from the active scene.
		/// </summary>
		/// <remarks>
		/// <para>
		/// In the editor, if you try to play directly from a scene that's part of an 
		/// asset bundle and <c>UseAssetBundlesInEditor</c> is enabled, it causes two 
		/// problems. 
		/// </para>
		/// <para>
		/// First, the scene itself isn't being loaded from the asset bundle. If the 
		/// scene in the project and the scene in the asset bundle are out of sync, 
		/// it's very confusing.
		/// </para>
		/// <para>
		/// Second, since the resource asset bundle the scene depends on isn't already 
		/// loaded when the scene loads, the scene may not be able to load it's resources, 
		/// or the performance may be much worse, since it'll have to load the resources 
		/// asset bundle.
		/// </para>
		/// </remarks>
		private static void OnEditorApplicationWillStartPlaying() {
			if (AssetBundleOptions.UseAssetBundlesInEditor) {
				
				string scenePath;
				scenePath = EditorSceneManager.GetActiveScene().path;
				
				string sceneBundle;
				sceneBundle = AssetImporter.GetAtPath(scenePath).assetBundleName;
				
				if (!string.IsNullOrEmpty(sceneBundle)) {
					
					string title;
					title = "Cannot Play Scene";
					
					string message;
					message = "The active scene is part of an asset bundle and UseAssetBundlesInEditor is enabled.";
					
					string ok;
					ok = "Play From The Main Scene";
					
					string cancel;
					cancel = "Don't Use Asset Bundles";
					
					if (EditorUtility.DisplayDialog(title, message, ok, cancel)) {
						EditorSceneManager.OpenScene(ProjectInfo.Instance.MainScene.AssetPath, OpenSceneMode.Single);
						EditorPrefs.SetString(LastScenePathKey, scenePath);
					} else {
						AssetBundleOptions.UseAssetBundlesInEditor = false;
						EditorPrefs.DeleteKey(LastScenePathKey);
					}
					
				}
				
			}
		}
		
		/// <summary>
		/// Opens the last scene if the user chose to play from the main scene.
		/// </summary>
		private static void OnEditorApplicationDidStopPlaying() {
			if (AssetBundleOptions.UseAssetBundlesInEditor && EditorPrefs.HasKey(LastScenePathKey)) {
				EditorSceneManager.OpenScene(EditorPrefs.GetString(LastScenePathKey), OpenSceneMode.Single);
				EditorPrefs.DeleteKey(LastScenePathKey);
			}
		}
		
		#endregion
		
		
		#region Methods
		
		override public void OnInspectorGUI() {
			EditorGUI.BeginDisabledGroup(true);
			DrawDefaultInspector();
			EditorGUI.EndDisabledGroup();
		}
		
		#endregion
		
		
	}
	
}