namespace TestEditor.UI {
	
	using SagoUI;
	using UnityEngine;
	using UnityEditor;
	
	[CustomEditor(typeof(MeshGraphic))]
	public class MeshGraphicEditor : Editor {
		
		
		#region Fields
		
		private SerializedProperty m_AlignMode;
		
		private SerializedProperty m_Color;
		
		private SerializedProperty m_ColorMode;
		
		private SerializedProperty m_Material;
		
		private SerializedProperty m_Mesh;
		
		private SerializedProperty m_OnCullStateChanged;
		
		private SerializedProperty m_PixelsPerUnit;
		
		private SerializedProperty m_ScaleMode;
		
		private SerializedProperty m_RaycastTarget;
		
		#endregion
		
		
		#region Methods
		
		public void OnEnable() {
			m_AlignMode = serializedObject.FindProperty("m_AlignMode");
			m_Color = serializedObject.FindProperty("m_Color");
			m_ColorMode = serializedObject.FindProperty("m_ColorMode");
			m_Material = serializedObject.FindProperty("m_Material");
			m_Mesh = serializedObject.FindProperty("m_Mesh");
			m_OnCullStateChanged = serializedObject.FindProperty("m_OnCullStateChanged");
			m_PixelsPerUnit = serializedObject.FindProperty("m_PixelsPerUnit");
			m_ScaleMode = serializedObject.FindProperty("m_ScaleMode");
			m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget");
		}
		
		override public void OnInspectorGUI() {
			
			GUIStyle headerStyle;
			headerStyle = new GUIStyle(EditorStyles.boldLabel);
			headerStyle.normal.textColor = Color.white;
			
			serializedObject.Update();
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Mesh", headerStyle);
			EditorGUILayout.PropertyField(m_Mesh);
			EditorGUILayout.PropertyField(m_PixelsPerUnit);
			
			Mesh mesh;
			mesh = m_Mesh.objectReferenceValue as Mesh;
			
			if (mesh && !mesh.isReadable) {
				EditorGUILayout.HelpBox("The mesh is not readable and cannot be displayed. Please make it readable or choose another mesh.", MessageType.Warning, false);
			}
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Layout", headerStyle);
			EditorGUILayout.PropertyField(m_AlignMode, new GUIContent("Align"));
			EditorGUILayout.PropertyField(m_ScaleMode, new GUIContent("Scale"));
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Render", headerStyle);
			EditorGUILayout.PropertyField(m_ColorMode, new GUIContent("Mode"));
			if ((MeshGraphicColorMode)m_ColorMode.intValue == MeshGraphicColorMode.Color) {
				EditorGUILayout.PropertyField(m_Color);
			}
			EditorGUILayout.PropertyField(m_Material);
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Other", headerStyle);
			EditorGUILayout.PropertyField(m_RaycastTarget);
			EditorGUILayout.PropertyField(m_OnCullStateChanged);
			
			serializedObject.ApplyModifiedProperties();
			
		}
		
		#endregion
		
	}
	
}