namespace SagoCoreEditor.Submodules {
	
	using SagoCore.Submodules;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	[CustomEditor(typeof(SubmoduleMap))]
	public class SubmoduleMapEditor : Editor {
		
		
		#region Constants
		
		/// <summary>
		/// The path to the <see cref="SubmoduleMap" /> asset.
		/// </summary>
		private const string SubmoduleMapPath = "Assets/Project/Resources/SubmoduleMap.asset";
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Updates the <see cref="SubmoduleMap" /> asset.
		/// </summary>
		[MenuItem("CONTEXT/SubmoduleMap/Update")]
		private static void UpdateSubmoduleMapContextMenuItem(MenuCommand command) {
			UpdateSubmoduleMap();
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Creates a new <see cref="SubmoduleMap" /> asset.
		/// </summary>
		public static SubmoduleMap CreateSubmoduleMap() {
			Directory.CreateDirectory(Path.GetDirectoryName(SubmoduleMapPath));
			AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SubmoduleMap>(), SubmoduleMapPath);
			return FindSubmoduleMap();
		}
		
		/// <summary>
		/// Finds the <see cref="SubmoduleMap" /> asset.
		/// </summary>
		public static SubmoduleMap FindSubmoduleMap() {
			return AssetDatabase.LoadAssetAtPath(SubmoduleMapPath, typeof(SubmoduleMap)) as SubmoduleMap;
		}
		
		/// <summary>
		/// Finds the <see cref="SubmoduleMap" /> asset or creates a new <see cref="SubmoduleMap" /> asset.
		/// </summary>
		public static SubmoduleMap FindOrCreateSubmoduleMap() {
			return FindSubmoduleMap() ?? CreateSubmoduleMap();
		}
		
		/// <summary>
		/// Updates the <see cref="SubmoduleMap" /> asset.
		/// </summary>
		public static SubmoduleMap UpdateSubmoduleMap() {
			
			EditorUtility.DisplayProgressBar(
				"UpdateSubmoduleMap",
				"Finding submodule map...", 
				0
			);
			
			SubmoduleMap map;
			map = FindOrCreateSubmoduleMap();
			
			SerializedObject obj;
			obj = new SerializedObject(map);
			
			EditorUtility.DisplayProgressBar(
				"UpdateSubmoduleMap",
				"Finding submodule types...", 
				0
			);
			
			System.Type[] types;
			types = SubmoduleMapEditorAdaptor.SubmoduleTypes;
			
			SerializedProperty elements;
			elements = obj.FindProperty("m_Elements");
			elements.arraySize = types.Length;
			
			for (int index = 0; index < types.Length; index++) {
			
				EditorUtility.DisplayProgressBar(
					"UpdateSubmoduleMap",
					"Updating submodule map...", 
					index / (float)(types.Length - 1)
				);
				
				System.Type type;
				type = types[index];
				
				SerializedProperty element;
				element = elements.GetArrayElementAtIndex(index);
				element.FindPropertyRelative("SubmoduleName").stringValue = SubmoduleMapEditorAdaptor.GetSubmoduleName(type);
				element.FindPropertyRelative("SubmodulePath").stringValue = SubmoduleMapEditorAdaptor.GetSubmodulePath(type);
				element.FindPropertyRelative("SubmoduleType").stringValue = type.FullName;
				
			}
			
			obj.ApplyModifiedPropertiesWithoutUndo();
			
			EditorUtility.ClearProgressBar();
			
			return FindSubmoduleMap();
			
		}
		
		#endregion
		
		
	}
	
}