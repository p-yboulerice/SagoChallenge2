namespace SagoMeshEditor {
    
    using SagoMesh;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MeshAnimator))]
    public class MeshAnimatorEditor : Editor {
        
        // ================================================================= //
        // Menu Methods
        // ================================================================= //
        
        [MenuItem("GameObject/Create Other/Mesh Animator", false, 2000)]
        public static MeshAnimator CreateMeshAnimator() {
            
            Transform parentTransform;
            parentTransform = null;
            
            if (Selection.activeGameObject) {
                parentTransform = Selection.activeGameObject.GetComponent<Transform>();
            }
            
            GameObject gameObject;
            gameObject = new GameObject();
            gameObject.name = "MeshAnimator";
            
            MeshAnimator animator;
            animator = gameObject.AddComponent<MeshAnimator>();
            
            MeshAnimationSource source;
            source = gameObject.AddComponent<MeshAnimationSource>();
            
            foreach (Object obj in Selection.objects) {
                if (obj is MeshAnimation) {
                    source.Animation = obj as MeshAnimation;
                    break;
                }
            }
            
            Transform transform;
            transform = animator.GetComponent<Transform>();
            transform.parent = parentTransform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            
            return animator;
            
        }
        
        // ================================================================= //
        // Static Methods
        // ================================================================= //
        
        protected static int FrameField(MeshAnimator animator, GUIContent label) {
            int index = EditorGUILayout.IntSlider(label, animator.CurrentIndex, 0, animator.LastIndex);
            if (index != animator.CurrentIndex) {
                animator.Jump(index);
            }
            return animator.CurrentIndex;
        }
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        protected SerializedProperty AutoPlayProperty {
            get; set;
        }
        
        protected SerializedProperty DirectionProperty {
            get; set;
        }
        
        protected SerializedProperty FramerateProperty {
            get; set;
        }
        
        protected SerializedProperty IsLoopProperty {
            get; set;
        }
        
        protected SerializedProperty LockedProperty {
            get; set;
        }
        
        
        // ================================================================= //
        // Methods
        // ================================================================= //
        
        protected void OnEnable() {
            this.AutoPlayProperty = this.serializedObject.FindProperty("m_AutoPlay");
            this.DirectionProperty = this.serializedObject.FindProperty("m_Direction");
            this.FramerateProperty = this.serializedObject.FindProperty("m_Framerate");
            this.IsLoopProperty = this.serializedObject.FindProperty("m_IsLoop");
            this.LockedProperty = this.serializedObject.FindProperty("m_Locked");
        }
        
        override public void OnInspectorGUI() {
            
            this.serializedObject.Update();
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            
            if (this.serializedObject.isEditingMultipleObjects) {
                
                EditorGUILayout.PropertyField(this.DirectionProperty, new GUIContent("Direction"));
                EditorGUILayout.PropertyField(this.AutoPlayProperty, new GUIContent("AutoPlay"));
                EditorGUILayout.PropertyField(this.LockedProperty, new GUIContent("Lock"));
                EditorGUILayout.PropertyField(this.IsLoopProperty, new GUIContent("Loop"));
                EditorGUILayout.PropertyField(this.FramerateProperty, new GUIContent("Framerate"));
                
            } else {
                
                MeshAnimator animator = this.serializedObject.targetObject as MeshAnimator;
                EditorGUILayout.PropertyField(this.DirectionProperty, new GUIContent("Direction"));
                FrameField(animator, new GUIContent("Frame"));
                EditorGUILayout.PropertyField(this.AutoPlayProperty, new GUIContent("AutoPlay"));
                EditorGUILayout.PropertyField(this.LockedProperty, new GUIContent("Lock"));
                EditorGUILayout.PropertyField(this.IsLoopProperty, new GUIContent("Loop"));
                EditorGUILayout.PropertyField(this.FramerateProperty, new GUIContent("Framerate"));
                
            }
            
            EditorGUI.EndDisabledGroup();
            this.serializedObject.ApplyModifiedProperties();
            
            // make sure the animators are up to date
            foreach (Object target in this.serializedObject.targetObjects) {
                MeshAnimator animator = null;
                animator = target as MeshAnimator;
                animator.Pull();
            }
            
        }
        
        
    }
    
}