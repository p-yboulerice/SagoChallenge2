namespace SagoEngineEditor {
    
    using SagoEngine;
    using System;
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(MeshAnimationAsset))]
    public class MeshAnimationAssetEditor : Editor {
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        static bool AudioFoldout {
            get; set;
        }
        
        static bool MeshFoldout {
            get; set;
        }
        
        public MeshAnimationAsset Component {
            get { return target as MeshAnimationAsset; }
        }
        
        
        // ================================================================= //
        // Methods
        // ================================================================= //
        
        override public void OnInspectorGUI() {
            
            float duration;
            duration = this.Component.Duration;
            
            int fps;
            fps = (int)Math.Round(this.Component.Meshes.Length / this.Component.Duration);
            
            EditorGUILayout.LabelField("Anchor Point", String.Format("{0:f3}, {1:f3}", this.Component.AnchorPoint.x, this.Component.AnchorPoint.y));
            EditorGUILayout.LabelField("Content Size", String.Format("{0:f1}, {1:f1}", this.Component.ContentSize.x, this.Component.ContentSize.y));
            EditorGUILayout.LabelField("Duration", string.Format("{0:f1} seconds @ {1} fps", duration, fps));
            
            MeshFoldout = EditorGUILayout.Foldout(MeshFoldout, "Meshes");
            if (MeshFoldout) {
                EditorGUI.BeginDisabledGroup(true);
                    for (int index = 0; index < this.Component.Meshes.Length; index++) {
                        EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(Convert.ToString(index), EditorStyles.boldLabel, GUILayout.Width(30));
                            EditorGUILayout.ObjectField(this.Component.Meshes[index], typeof(MeshAsset), false);
                        EditorGUILayout.EndHorizontal();
                    }
                EditorGUI.EndDisabledGroup();
            }
            
            AudioFoldout = EditorGUILayout.Foldout(AudioFoldout, "Audio");
            if (AudioFoldout && this.Component.Audio != null) {
                
                int[] frames;
                frames = this.Component.Audio.GetFrames();
                
                EditorGUI.BeginDisabledGroup(true);
                    for (int index = 0; index < frames.Length; index++) {
                        EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(Convert.ToString(frames[index]), EditorStyles.boldLabel, GUILayout.Width(30));
                            EditorGUILayout.BeginVertical();
                                foreach (AudioClip audioClip in this.Component.Audio.GetAudioClips(frames[index])) {
                                    EditorGUILayout.ObjectField(audioClip, typeof(AudioClip), false);
                                }
                            EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                EditorGUI.EndDisabledGroup();
                
            }
            
        }
        
        override public Boolean HasPreviewGUI() {
            return this.Component.Meshes.Length > 0 && this.Component.Meshes[0] != null;
        }
        
        override public void OnPreviewGUI(Rect rect, GUIStyle style) {
            if (this.Component.Meshes.Length > 0 && this.Component.Meshes[0] != null) {
                Editor meshEditor;
                meshEditor = Editor.CreateEditor(this.Component.Meshes[0]);
                meshEditor.OnPreviewGUI(rect, style);
            }
        }
        
    }
    
}
