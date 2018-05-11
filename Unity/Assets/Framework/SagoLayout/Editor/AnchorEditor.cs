namespace SagoLayoutEditor {
	
	using SagoLayout;
	using UnityEditor;
    using UnityEngine;

	[CustomEditor(typeof(Anchor))]
    public class AnchorEditor : ArtboardElementEditor {
		
		
		//
		// Serialized Properties
		//
		SerializedProperty Alignment;
		SerializedProperty IsDeviceSpecific;
		SerializedProperty MarginInPointsKindle;
		SerializedProperty MarginInPointsPad;
		SerializedProperty MarginInPointsPhone;


		//
		// Editor
		//
		override protected void OnEnable() {
			base.OnEnable();
			this.Alignment = serializedObject.FindProperty("Alignment");
			this.IsDeviceSpecific = serializedObject.FindProperty("IsDeviceSpecific");
			this.MarginInPointsKindle = serializedObject.FindProperty("MarginInPointsKindle");
			this.MarginInPointsPad = serializedObject.FindProperty("MarginInPointsPad");
			this.MarginInPointsPhone = serializedObject.FindProperty("MarginInPointsPhone");
		}
		
		override public void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.Alignment);
			EditorGUILayout.PropertyField(this.IsDeviceSpecific, new GUIContent("Device Specific Margins"));
			if (this.IsDeviceSpecific.boolValue) {
				EditorGUILayout.PropertyField(this.MarginInPointsPhone, new GUIContent("Margin in Points (iPhone)"));
				EditorGUILayout.PropertyField(this.MarginInPointsPad, new GUIContent("Margin in Points (iPad)"));
				EditorGUILayout.PropertyField(this.MarginInPointsKindle, new GUIContent("Margin in Points (Kindle)"));
			} else {
				EditorGUILayout.PropertyField(this.MarginInPointsPhone, new GUIContent("Margin In Points"));
			}
			serializedObject.ApplyModifiedProperties();
			base.OnInspectorGUI();
			if (EditorGUI.EndChangeCheck()) Apply();
		}


	}
	
}
