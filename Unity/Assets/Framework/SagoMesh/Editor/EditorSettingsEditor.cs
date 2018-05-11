namespace SagoMeshEditor {
    
    using UnityEngine;
    using UnityEditor;
    
    [CustomEditor(typeof(EditorSettings))]
    public class EditorSettingsEditor : Editor {
        
        // ================================================================= //
        // Fields
        // ================================================================= //
        
        protected SerializedProperty AutoImport;
        protected SerializedProperty DeleteIntermediateFiles;
        protected SerializedProperty LoadAssetBundles;
        protected SerializedProperty PixelsPerMeter;
        protected SerializedProperty SagoAudioMode;
        
        
        // ================================================================= //
        // Methods
        // ================================================================= //
        
        public void OnEnable() {
            this.AutoImport = this.serializedObject.FindProperty("m_AutoImport");
            this.DeleteIntermediateFiles = this.serializedObject.FindProperty("m_DeleteIntermediateFiles");
            this.PixelsPerMeter = this.serializedObject.FindProperty("m_PixelsPerMeter");
            this.SagoAudioMode = this.serializedObject.FindProperty("m_SagoAudioMode");
        }
        
        override public void OnInspectorGUI() {
            
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.AutoImport);
            EditorGUILayout.PropertyField(this.DeleteIntermediateFiles);
            EditorGUILayout.PropertyField(this.PixelsPerMeter);
            EditorGUILayout.PropertyField(this.SagoAudioMode);
            
            if (this.serializedObject.ApplyModifiedProperties()) {
                (this.target as EditorSettings).UpdateDefineSymbols();
            }
            
        }
        
        
    }
    
}