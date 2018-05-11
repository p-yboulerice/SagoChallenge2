namespace SagoCoreEditor.Resources {
	
	using SagoCore.Resources;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The ResourceMapEditor class is responsible for creating and updating the singleton <see cref="ResourceMap" /> asset.
	/// </summary>
	public class ResourceMapEditor : Editor {
		
		
		#region Context Menu
		
		[MenuItem("CONTEXT/ResourceMap/Update")]
		private static void UpdateResourceMapContextMenuItem(MenuCommand command) {
			UpdateResourceMap();
		}
		
		#endregion
		
		
		#region Constants
		
		public const string ResourceMapPath = "Assets/Project/Resources/ResourceMap.asset";
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Creates a new <see cref="ResourceMap" /> asset.
		/// </summary>
		public static ResourceMap CreateResourceMap() {
			Directory.CreateDirectory(Path.GetDirectoryName(ResourceMapPath));
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ResourceMap>(), ResourceMapPath);
			return FindResourceMap();
		}
		
		/// <summary>
		/// Creates the existing <see cref="ResourceMap" /> asset.
		/// </summary>
		public static ResourceMap FindResourceMap() {
			return AssetDatabase.LoadAssetAtPath(ResourceMapPath, typeof(ResourceMap)) as ResourceMap;
		}
		
		/// <summary>
		/// Finds the existing <see cref="ResourceMap" /> asset, or creates a 
		/// new <see cref="ResourceMap" /> asset if one doesn't already exist.
		/// </summary>
		public static ResourceMap FindOrCreateResourceMap() {
			return FindResourceMap() ?? CreateResourceMap();
		}
		
		/// <summary>
		/// Checks if <see cref="ResourceMap" /> should include the asset at the specified path.
		/// </summary>
		public static bool IncludeAssetAtPath(string path) {
			return (
				ResourceMap.IsResourcePath(path) && 
				!path.Contains("/Editor/") && 
				!Directory.Exists(path) && 
				!Regex.IsMatch(path, @"[a-z0-9]{32}\.asset$") && 
				!Regex.IsMatch(path, @".cs$")
			);
		}
		
		/// <summary>
		/// Updates the <see cref="ResourceMap" /> asset.
		/// </summary>
		public static ResourceMap UpdateResourceMap() {
			
			EditorUtility.DisplayProgressBar(
				"UpdateResourceMap", 
				"Finding resource map...", 
				0
			);
			
			ResourceMap resourceMap;
			resourceMap = FindOrCreateResourceMap();
			
			EditorUtility.DisplayProgressBar(
				"UpdateResourceMap", 
				"Finding resource guids...", 
				0
			);
			
			string[] guids;
			guids = (
				AssetDatabase
				.GetAllAssetPaths()
				.Where(path => IncludeAssetAtPath(path))
				.Select(path => AssetDatabase.AssetPathToGUID(path))
				.ToArray()
			);
			
			SerializedObject obj;
			obj = new SerializedObject(resourceMap);
			
			SerializedProperty values;
			values = obj.FindProperty("m_Values");
			values.arraySize = guids.Length;
			
			for (int index = 0; index < guids.Length; index++) {
				
				string guid;
				guid = guids[index];
				
				EditorUtility.DisplayProgressBar(
					"UpdateResourceMap", 
					"Updating resource map...",
					index / (float)(guids.Length - 1)
				);
				
				var value = values.GetArrayElementAtIndex(index);
				value.FindPropertyRelative("Guid").stringValue = guid;
				value.FindPropertyRelative("AssetBundleName").stringValue = ResourceMapEditorAdaptor.GetAssetBundleName(guid);
				value.FindPropertyRelative("AssetPath").stringValue = ResourceMapEditorAdaptor.GetAssetPath(guid);
				value.FindPropertyRelative("ResourcePath").stringValue = ResourceMapEditorAdaptor.GetResourcePath(guid);
				
			}
			
			obj.ApplyModifiedPropertiesWithoutUndo();
			
			EditorUtility.ClearProgressBar();
			return resourceMap;
			
		}
		
		#endregion
		
		
	}
	
}
