namespace SagoMeshEditor {
    
    using SagoMesh;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(MeshAnimationCollection))]
    public class MeshAnimationCollectionEditor : Editor {
        
        
        // ================================================================= //
        // Static Methods
        // ================================================================= //
        
        [MenuItem("Assets/Create/Mesh Animation Collection", false, 2000)]
        public static void Create() {
            
            // find animations
            HashSet<MeshAnimation> animations = new HashSet<MeshAnimation>();
            foreach (Object obj in Selection.objects) {
                if (obj is MeshAnimation) {
                    animations.Add(obj as MeshAnimation);
                }
            }
            foreach (GameObject obj in Selection.gameObjects) {
                if (obj.GetComponent<MeshAnimator>()) {
                    animations.Add(obj.GetComponent<MeshAnimator>().Animation);
                }
                if (obj.GetComponent<MeshAnimatorMultiplexer>()) {
                    foreach (MeshAnimator animator in obj.GetComponent<MeshAnimatorMultiplexer>().Animators) {
                        animations.Add(animator.Animation);
                    }
                }
            }
            
            // create collection
            MeshAnimationCollection collection;
            collection = ScriptableObject.CreateInstance<MeshAnimationCollection>();
            collection.Animations = animations.ToArray();
            AssetDatabase.CreateAsset(collection, AssetDatabase.GenerateUniqueAssetPath("Assets/MeshAnimationCollection.asset"));
            AssetDatabase.SaveAssets();
            
        }
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        protected MeshAnimationCollection Collection {
            get { return this.target as MeshAnimationCollection; }
        }
        
        
        // ================================================================= //
        // Editor Methods
        // ================================================================= //
        
        public void OnEnable() {
            this.Collection.Normalize();
        }
        
        public void OnDisable() {
            this.Collection.Normalize();
        }
        
        override public void OnInspectorGUI() {
            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Animations"), new GUIContent("Animations"), true);
            this.serializedObject.ApplyModifiedProperties();
        }
        
        
    }
    
}
