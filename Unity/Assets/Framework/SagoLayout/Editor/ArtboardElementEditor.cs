namespace SagoLayoutEditor {
	
	using SagoLayout;
	using UnityEditor;
	using UnityEngine;
	
	[CustomEditor(typeof(ArtboardElement), true)]
	public class ArtboardElementEditor : LayoutComponentEditor {


		//
		// Serialized Properties
		//
		SerializedProperty Center;
		SerializedProperty Extents;
		SerializedProperty UseRendererBounds;


		//
		// Editor
		//
		override protected void OnEnable() {
			base.OnEnable();
			this.Center = serializedObject.FindProperty("Center");
			this.Extents = serializedObject.FindProperty("Extents");
			this.UseRendererBounds = serializedObject.FindProperty("UseRendererBounds");
		}
		
		override public void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.PropertyField(this.UseRendererBounds);
			if (this.UseRendererBounds.boolValue) {
				Bounds bounds;
				bounds = (this.target as ArtboardElement).LocalBounds;
				this.Center.vector2Value = bounds.center;
				this.Extents.vector2Value = bounds.extents;
			} else {
				EditorGUILayout.PropertyField(this.Center);
				EditorGUILayout.PropertyField(this.Extents);
			}
			serializedObject.ApplyModifiedProperties();
			base.OnInspectorGUI();
		}


	}

}
