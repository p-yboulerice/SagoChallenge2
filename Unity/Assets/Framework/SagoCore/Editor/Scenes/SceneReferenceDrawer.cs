namespace SagoCoreEditor.Scenes {
	
	using SagoCore.Scenes;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The SceneReferenceDrawer class provides a custom inspector for <see cref="SceneReference" /> properties.
	/// </summary>
	[CustomPropertyDrawer(typeof(SceneReference), true)]
	public class SceneReferenceDrawer : PropertyDrawer {
		
		
		#region Methods
		
		/// <summmary>
		/// <see cref="PropertyDrawer.OnGUI" />
		/// </summmary>
		override public void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
			
			EditorGUI.BeginProperty(rect, label, property);
			EditorGUI.BeginChangeCheck();
			
			SerializedProperty guid;
			guid = property.FindPropertyRelative("m_Guid");
			
			string path;
			path = AssetDatabase.GUIDToAssetPath(guid.stringValue);
			
			SceneAsset scene;
			scene = AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset)) as SceneAsset;
			scene = EditorGUI.ObjectField(rect, label, scene, typeof(SceneAsset), false) as SceneAsset;
			
			if (EditorGUI.EndChangeCheck()) {
				guid.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(scene));
			}
			
			EditorGUI.EndProperty();
			
		}
		
		#endregion
		
		
	}
	
}
