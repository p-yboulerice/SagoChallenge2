namespace SagoLocalizationEditor {
	
	using SagoCore.Resources;
	using SagoLocalization;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;
	
	/// <summary>
	/// The LocalizedResourceReferenceEditor class provides a custom inspector for <see cref="LocalizedResourceReference" /> assets.
	/// </summary>
	[CustomEditor(typeof(LocalizedResourceReferenceBase), true)]
	public class LocalizedResourceReferenceEditor : Editor {
		
		
		#region Types
		
		private static class Styles {
			
			
			#region Static Fields
			
			private static GUIStyle _headerStyle;
			
			private static GUIStyle _identifierStyle;
			
			private static GUIStyle _identifierErrorStyle;
			
			#endregion
			
			
			#region Static Properties
			
			public static GUIStyle headerStyle {
				get {
					if (_headerStyle == null) {
						_headerStyle = new GUIStyle(EditorStyles.boldLabel);
						_headerStyle.margin.left = 0;
						_headerStyle.margin.right = 0;
					}
					return _headerStyle;
				}
			}
			
			public static GUIStyle identifierStyle {
				get {
					if (_identifierStyle == null) {
						_identifierStyle = new GUIStyle(EditorStyles.textField);
					}
					return _identifierStyle;
				}
			}
			
			public static GUIStyle identifierErrorStyle {
				get {
					if (_identifierErrorStyle == null) {
						_identifierErrorStyle = new GUIStyle(identifierStyle);
						_identifierErrorStyle.active.textColor = Color.red;
						_identifierErrorStyle.focused.textColor = Color.red;
						_identifierErrorStyle.hover.textColor = Color.red;
						_identifierErrorStyle.normal.textColor = Color.red;
					}
					return _identifierErrorStyle;
				}
			}
			
			#endregion
			
			
		}
		
		#endregion
		
		
		#region Static Methods
		
		private static string AssetToGuid(Object asset) {
			return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
		}
		
		private static Object GuidToAsset(string guid) {
			return AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
		}
		
		#endregion
		
		
		#region Fields
		
		private SerializedProperty m_Guids;
		
		private SerializedProperty m_Identifiers;
		
		private LocalizedResourceReferenceBase m_Reference;
		
		#endregion
		
		
		#region Methods
		
		
		private void OnEnable() {
			m_Guids = serializedObject.FindProperty("m_Guids");
			m_Identifiers = serializedObject.FindProperty("m_Identifiers");
			m_Reference = target as LocalizedResourceReferenceBase;
		}
		
		override public void OnInspectorGUI() {
			
			serializedObject.Update();
			
			int count;
			count = Mathf.Min(m_Guids.arraySize, m_Identifiers.arraySize);
			
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label("Identifier", Styles.headerStyle);
				GUILayout.Label("Resource", Styles.headerStyle);
				GUILayout.Space(20);
			}
			EditorGUILayout.EndHorizontal();
			
			for (int index = 0; index < count; index++) {
				EditorGUILayout.BeginHorizontal();
				{
					SerializedProperty identifier;
					identifier = m_Identifiers.GetArrayElementAtIndex(index);
					identifier.stringValue = EditorGUILayout.TextField(
						identifier.stringValue, 
						Locale.IsValid(identifier.stringValue) ? Styles.identifierStyle : Styles.identifierErrorStyle
					);
					
					SerializedProperty guid;
					guid = m_Guids.GetArrayElementAtIndex(index);
					guid.stringValue = AssetToGuid(EditorGUILayout.ObjectField(
						GuidToAsset(guid.stringValue), 
						m_Reference.GetResourceType(), 
						false
					));
					
					if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20))) {
						guid.stringValue = null;
						identifier.stringValue = null;
						m_Guids.DeleteArrayElementAtIndex(index);
						m_Identifiers.DeleteArrayElementAtIndex(index);
						count = Mathf.Max(count - 1, 0);
						index = Mathf.Max(index - 1, 0);
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20))) {
					m_Guids.InsertArrayElementAtIndex(count);
					m_Guids.GetArrayElementAtIndex(count).stringValue = null;
					m_Identifiers.InsertArrayElementAtIndex(count);
					m_Identifiers.GetArrayElementAtIndex(count).stringValue = null;
				}
			}
			EditorGUILayout.EndHorizontal();
			
			serializedObject.ApplyModifiedProperties();
			
		}
		
		#endregion
		
		
	}
	
}
