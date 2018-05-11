namespace SagoLayout {

	using UnityEditor;
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

    [CustomEditor(typeof(Overlay))]
    public class OverlayEditor : Editor {
        
        protected Overlay Overlay {
            get { return this.target as Overlay; }
        }

		#region MonoBehaviours

        override public void OnInspectorGUI() {

            EditorGUI.BeginChangeCheck();

            SerializedProperty prop = serializedObject.FindProperty("m_ActiveOverlay");
			EditorGUILayout.PropertyField(prop);
			serializedObject.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck()) {
                this.Overlay.OnOverlayTypeChanged();
            }
        }

		#endregion
	}
}