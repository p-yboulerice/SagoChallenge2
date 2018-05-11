namespace SagoLayoutEditor {
	
	using SagoLayout;
	using UnityEditor;
    using UnityEngine;
	
	[CustomEditor(typeof(LayoutComponent), true)]
    public class LayoutComponentEditor : Editor {
		

		//
		// Menu Items
		//
		[MenuItem("Sago/Layout/Apply All", false, 500)]
		public static void ApplyAll() {
			foreach (LayoutComponent layout in FindObjectsOfType<LayoutComponent>()) {
				layout.Apply();
			}
		}


		//
		// Serialized Properties
		//
		SerializedProperty ApplyOnAwake {
			get;
			set;
		}
		
		SerializedProperty ApplyOnStart {
			get;
			set;
		}
		
		SerializedProperty ApplyOnUpdate {
			get;
			set;
		}
		
		SerializedProperty ShowOptionsInEditor {
			get;
			set;
		}


		//
		// Editor
		//
		virtual protected void OnEnable() {
			this.ApplyOnAwake = serializedObject.FindProperty("ApplyOnAwake");
			this.ApplyOnStart = serializedObject.FindProperty("ApplyOnStart");
			this.ApplyOnUpdate = serializedObject.FindProperty("ApplyOnUpdate");
			this.ShowOptionsInEditor = serializedObject.FindProperty("ShowOptionsInEditor");
		}
		
		override public void OnInspectorGUI() {
			serializedObject.Update();
			this.ShowOptionsInEditor.boolValue = EditorGUILayout.Foldout(this.ShowOptionsInEditor.boolValue, "Script Execution");
			if (this.ShowOptionsInEditor.boolValue) {
				EditorGUILayout.PropertyField(this.ApplyOnAwake);
				EditorGUILayout.PropertyField(this.ApplyOnStart);
				EditorGUILayout.PropertyField(this.ApplyOnUpdate);
			}
			serializedObject.ApplyModifiedProperties();
		}

		protected void Apply() {
			(this.target as LayoutComponent).Apply();
		}

		
	}
	
}
