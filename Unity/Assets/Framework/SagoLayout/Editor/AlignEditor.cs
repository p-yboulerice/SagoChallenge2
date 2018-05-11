namespace SagoLayoutEditor {
	
	using SagoLayout;
	using UnityEditor;
	using UnityEngine;
	
	[CustomEditor(typeof(Align))]
	public class AlignEditor : ArtboardElementEditor {
		
		
		#region Fields
		
		SerializedProperty Mode;
		SerializedProperty Padding;
		SerializedProperty IgnoreSafeAreaInsets;
		
		#endregion
		
		
		#region Editor Methods
		
		override protected void OnEnable() {
			base.OnEnable();
			Mode = serializedObject.FindProperty("m_Mode");
			Padding = serializedObject.FindProperty("m_Padding");
			IgnoreSafeAreaInsets = serializedObject.FindProperty("m_IgnoreSafeAreaInsets");
		}
		
		override public void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(Mode);
			EditorGUILayout.PropertyField(Padding);
			EditorGUILayout.PropertyField(IgnoreSafeAreaInsets);
			serializedObject.ApplyModifiedProperties();
			base.OnInspectorGUI();
			if (EditorGUI.EndChangeCheck()) Apply();
		}
		
		#endregion
		
		
	}
	
}
