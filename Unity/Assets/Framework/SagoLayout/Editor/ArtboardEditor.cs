namespace SagoLayoutEditor {
	
	using SagoLayout;
	using UnityEditor;
    using UnityEngine;

	[CustomEditor(typeof(Artboard))]
    public class ArtboardEditor : LayoutComponentEditor {


		//
		// Menu Items
		//
		[MenuItem("GameObject/Create Other/Artboard", false, 3000)]
		public static Artboard CreateArtboard() {
			Transform transform;
			transform = Selection.activeGameObject ? Selection.activeGameObject.GetComponent<Transform>() : new GameObject("Artboard").GetComponent<Transform>();
			return transform.gameObject.AddComponent<Artboard>();
		}


		//
		// Serialized Properties
		//
		SerializedProperty BackgroundColor;
		SerializedProperty BackgroundOpaque;
		SerializedProperty CameraDepth;
		SerializedProperty Layer;
		SerializedProperty LockToLayer;
		SerializedProperty Size;
		
		
		//
		// Editor
		//
		override protected void OnEnable() {
			base.OnEnable();
			this.BackgroundColor = serializedObject.FindProperty("BackgroundColor");
			this.BackgroundOpaque = serializedObject.FindProperty("BackgroundOpaque");
			this.CameraDepth = serializedObject.FindProperty("CameraDepth");
			this.Layer = serializedObject.FindProperty("Layer");
			this.LockToLayer = serializedObject.FindProperty("LockToLayer");
			this.Size = serializedObject.FindProperty("Size");
		}
		
		override public void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.Size);
			EditorGUILayout.PropertyField(this.BackgroundOpaque, new GUIContent("Background"));
			if (this.BackgroundOpaque.boolValue) {
				EditorGUILayout.PropertyField(this.BackgroundColor);
			}
			EditorGUILayout.PropertyField(this.LockToLayer);
			if (this.LockToLayer.boolValue) {
				this.Layer.intValue = EditorGUILayout.LayerField("Camera Layer", this.Layer.intValue);
			}
			EditorGUILayout.PropertyField(this.CameraDepth);
			serializedObject.ApplyModifiedProperties();
			base.OnInspectorGUI();
			if (EditorGUI.EndChangeCheck()) Apply();
		}
	
	}
	
}
