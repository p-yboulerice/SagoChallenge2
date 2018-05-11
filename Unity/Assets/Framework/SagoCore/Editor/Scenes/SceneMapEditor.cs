namespace SagoCoreEditor.Scenes {
	
	using SagoCore.Scenes;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The SceneMapEditor class is responsible for creating and updating the singleton <see cref="SceneMap" /> asset.
	/// </summary>
	public class SceneMapEditor : Editor {
		
		
		#region Menu
		
		[MenuItem("CONTEXT/SceneMap/Update")]
		private static void UpdateSceneMapContextMenuItem() {
			UpdateSceneMap();
		}
		
		#endregion
		
		
		#region Constants
		
		public const string SceneMapPath = "Assets/Project/Resources/SceneMap.asset";
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Creates a new <see cref="SceneMap" /> asset.
		/// </summary>
		public static SceneMap CreateSceneMap() {
			Directory.CreateDirectory(Path.GetDirectoryName(SceneMapPath));
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SceneMap>(), SceneMapPath);
			return FindSceneMap();
		}
		
		/// <summary>
		/// Creates the existing <see cref="SceneMap" /> asset.
		/// </summary>
		public static SceneMap FindSceneMap() {
			return AssetDatabase.LoadAssetAtPath(SceneMapPath, typeof(SceneMap)) as SceneMap;
		}
		
		/// <summary>
		/// Finds the existing <see cref="SceneMap" /> asset, or creates a 
		/// new <see cref="SceneMap" /> asset if one doesn't already exist.
		/// </summary>
		public static SceneMap FindOrCreateSceneMap() {
			return FindSceneMap() ?? CreateSceneMap();
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Updates the <see cref="SceneMap" /> asset.
		/// </summary>
		public static SceneMap UpdateSceneMap() {
			
			EditorUtility.DisplayProgressBar(
				"UpdateSceneMap",
				"Finding scene map...", 
				0
			);
			
			SceneMap sceneMap;
			sceneMap = FindOrCreateSceneMap();
			
			EditorUtility.DisplayProgressBar(
				"UpdateSceneMap",
				"Finding scene guids...", 
				0
			);
			
			string[] sceneGuids;
			sceneGuids = (
				AssetDatabase
				.FindAssets("t:SceneAsset")
				.ToArray()
			);
			
			SerializedObject obj;
			obj = new SerializedObject(sceneMap);
			
			SerializedProperty values;
			values = obj.FindProperty("m_Values");
			values.arraySize = sceneGuids.Length;
			
			for (int index = 0; index < sceneGuids.Length; index++) {
			
				EditorUtility.DisplayProgressBar(
					"UpdateSceneMap",
					"Updating scene map...", 
					index / (float)(sceneGuids.Length - 1)
				);
				
				string assetPath;
				assetPath = AssetDatabase.GUIDToAssetPath(sceneGuids[index]);
				
				string assetBundleName;
				assetBundleName = AssetImporter.GetAtPath(assetPath).assetBundleName;
				
				string assetGuid;
				assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
				
				SerializedProperty value;
				value = values.GetArrayElementAtIndex(index);
				value.FindPropertyRelative("Guid").stringValue = assetGuid;
				value.FindPropertyRelative("AssetBundleName").stringValue = assetBundleName;
				value.FindPropertyRelative("AssetPath").stringValue = assetPath;
				
			}
			
			obj.ApplyModifiedPropertiesWithoutUndo();
			
			EditorUtility.ClearProgressBar();
			
			return sceneMap;
			
			
		}
		
		#endregion
		
		
	}
	
}