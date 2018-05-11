namespace SagoLayoutEditor {
	
	using SagoLayout;
	using UnityEditor;
	using UnityEngine;
	
	[CustomEditor(typeof(SizeInPoints))]
	public class SizeInPointsEditor : ArtboardElementEditor {


		//
		// Serialized Properties
		//
		SerializedProperty HeightInPointsKindle;
		SerializedProperty HeightInPointsPad;
		SerializedProperty HeightInPointsPhone;
		SerializedProperty IsDeviceSpecific;


		//
		// Editor
		//
		override protected void OnEnable() {
			base.OnEnable();
			this.HeightInPointsKindle = serializedObject.FindProperty("HeightInPointsKindle");
			this.HeightInPointsPad = serializedObject.FindProperty("HeightInPointsPad");
			this.HeightInPointsPhone = serializedObject.FindProperty("HeightInPointsPhone");
			this.IsDeviceSpecific = serializedObject.FindProperty("IsDeviceSpecific");
		}
		
		override public void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.IsDeviceSpecific, new GUIContent("Device Specific Sizes"));
			if (this.IsDeviceSpecific.boolValue) {
				EditorGUILayout.PropertyField(this.HeightInPointsPhone, new GUIContent("Height in Points (iPhone)"));
				EditorGUILayout.PropertyField(this.HeightInPointsPad, new GUIContent("Height in Points (iPad)"));
				EditorGUILayout.PropertyField(this.HeightInPointsKindle, new GUIContent("Height in Points (Kindle)"));
			} else {
				EditorGUILayout.PropertyField(this.HeightInPointsPhone, new GUIContent("Height In Points"));
			}
			serializedObject.ApplyModifiedProperties();
			base.OnInspectorGUI();
			if (EditorGUI.EndChangeCheck()) Apply();
		}


	}

}
