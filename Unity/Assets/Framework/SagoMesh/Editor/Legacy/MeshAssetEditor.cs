namespace SagoEngineEditor {
    
    using SagoEngine;
    using System;
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(MeshAsset))]
    public class MeshAssetEditor : Editor {
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public MeshAsset Component {
            get { return target as MeshAsset; }
        }
        
        // ================================================================= //
        // Methods
        // ================================================================= //
        
        override public void OnInspectorGUI() {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Mesh", this.Component.Mesh, typeof(UnityEngine.Mesh), false);
            EditorGUILayout.LabelField("Anchor Point", String.Format("{0:f3}, {1:f3}", this.Component.AnchorPoint.x, this.Component.AnchorPoint.y));
            EditorGUILayout.LabelField("Content Size", String.Format("{0:f1}, {1:f1}", this.Component.ContentSize.x, this.Component.ContentSize.y));
            EditorGUILayout.Toggle("Opaque", this.Component.IsOpaque);
            EditorGUI.EndDisabledGroup();
        }
        
        override public Boolean HasPreviewGUI() {
            return this.Component.Mesh != null;
        }
        
        override public void OnPreviewGUI(Rect rect, GUIStyle style) {
            if (this.Component.Mesh != null) {
                Editor meshEditor;
                meshEditor = Editor.CreateEditor(this.Component.Mesh);
                meshEditor.OnPreviewGUI(rect, style);
            }
        }
        
    }
    
}
