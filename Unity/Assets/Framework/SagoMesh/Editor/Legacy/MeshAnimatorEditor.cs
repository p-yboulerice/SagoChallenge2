namespace SagoEngineEditor {
    
	using SagoCore.Submodules;
    using SagoEngine;
    using System.Collections.Generic;
	using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(MeshAnimator))]
    public class MeshAnimatorEditor : Editor {
        
        // ================================================================= //
        // Static Methods
        // ================================================================= //
        
        [MenuItem("Sago/Mesh/Legacy/Analyze Scene", false, 2100)]
        public static void AnalyzeScene() {
            
            
            // animators
            List<MeshAnimator> animators;
            animators = new List<MeshAnimator>();
            foreach (GameObject obj in GameObject.FindObjectsOfType(typeof(GameObject))) {
                if (obj.transform.parent == null) {
                    animators.AddRange(obj.GetComponentsInChildren<MeshAnimator>(true));
                }
            }
            
            List<MeshAnimator> serializedAnimators;
            serializedAnimators = animators.Where( a => a.IsAsync == false ).ToList();
            
            List<MeshAnimator> asyncAnimators;
            asyncAnimators = animators.Where ( a => a.IsAsync == true ).ToList();
            
            List<MeshAnimator> invalidAnimators;
            invalidAnimators = animators.Where( a => a.IsAsync == true && a.AnimationAsset != null ).ToList();
            
            
            // animations
            List<MeshAnimationAsset> serializedAnimations;
            serializedAnimations = new List<MeshAnimationAsset>();
            
            List<MeshAsset> serializedMeshes;
            serializedMeshes = new List<MeshAsset>();
            
            foreach (MeshAnimator animator in serializedAnimators) {
                if (animator.AnimationAsset) {
                    serializedAnimations.Add(animator.AnimationAsset);
                    serializedMeshes.AddRange(animator.AnimationAsset.Meshes);
                }
            }
            
            serializedAnimations = serializedAnimations.Distinct().ToList();
            serializedMeshes = serializedMeshes.Distinct().ToList();
            
            
            // meshes
            List<MeshAnimationAsset> asyncAnimations;
            asyncAnimations = new List<MeshAnimationAsset>();
            
            List<MeshAsset> asyncMeshes;
            asyncMeshes = new List<MeshAsset>();
            
            foreach (MeshAnimator animator in asyncAnimators) {
                if (!string.IsNullOrEmpty(animator.AnimationAssetGuid)) {
                    MeshAnimationAsset asset;
                    if (asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(animator.AnimationAssetGuid), typeof(MeshAnimationAsset)) as MeshAnimationAsset) {
                        asyncAnimations.Add(asset);
                        asyncMeshes.AddRange(asset.Meshes);
                    }
                }
            }
            
            asyncAnimations = asyncAnimations.Distinct().ToList();
            asyncMeshes = asyncMeshes.Distinct().ToList();
            
            
            // duplicates
            List<MeshAnimationAsset> duplicateAnimations;
            duplicateAnimations = serializedAnimations.Intersect(asyncAnimations).ToList();
            
            // duplicateAnimations.Sort( (a,b) => { return a.name.CompareTo(b.name); });
            // foreach (MeshAnimationAsset animation in duplicateAnimations) {
            //     Debug.Log(animation.name);
            // }
            
            List<MeshAsset> duplicateMeshes;
            duplicateMeshes = serializedMeshes.Intersect(asyncMeshes).ToList();
            
            
            // log
            Debug.Log(string.Format("{0}\n{1}\n{2}\n\n{3}\n\n{4}\n",
                string.Format(
                    "MeshAnimator: total: {0}, serialized: {1}, async: {2}, invalid: {3}", 
                    animators.Count, 
                    serializedAnimators.Count, 
                    asyncAnimators.Count, 
                    invalidAnimators.Count),
                string.Format(
                    "MeshAnimationAsset: total: {0}, serialized: {1}, async {2}, duplicate: {3}",
                    serializedAnimations.Union(asyncAnimations).Distinct().ToList().Count, 
                    serializedAnimations.Count, 
                    asyncAnimations.Count,
                    duplicateAnimations.Count),
                string.Format(
                    "MeshAsset: total: {0}, serialized: {1}, async {2}, duplicate: {3}",
                    serializedMeshes.Union(asyncMeshes).Distinct().ToList().Count, 
                    serializedMeshes.Count, 
                    asyncMeshes.Count,
                    duplicateMeshes.Count),
                string.Join("\n", duplicateAnimations.Select( m => m.ToString()).ToArray()),
                string.Join("\n", duplicateMeshes.Select( m => m.ToString()).ToArray())
            ));
            
        }
        
        [MenuItem("Sago/Mesh/Legacy/Clear Mesh Filters", false, 2100)]
        public static void ClearMeshFilters() {
            foreach (string assetPath in AssetDatabase.GetAllAssetPaths()) {
                if (assetPath.EndsWith(".prefab")) {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                    foreach (MeshAnimator animator in prefab.GetComponentsInChildren<MeshAnimator>(true)) {
                        animator.GetComponent<MeshFilter>().sharedMesh = null;
                    }
                }
            }
                    
            AssetDatabase.SaveAssets();
            foreach (GameObject obj in GameObject.FindObjectsOfType(typeof(GameObject))) {
                if (obj.transform.parent == null) {
                    foreach (MeshAnimator animator in obj.GetComponentsInChildren<MeshAnimator>(true)) {
                        animator.GetComponent<MeshFilter>().sharedMesh = null;
                    }
                }
            }
        }
        
        [MenuItem("Sago/Mesh/Legacy/Set Async", false, 2200)]
        public static void SetAsAsync() {
            foreach (GameObject obj in Selection.gameObjects) {
                foreach (MeshAnimator animator in obj.GetComponentsInChildren<MeshAnimator>(true)) {
                    if (animator.IsAsync == false) {
                        animator.IsAsync = true;
                        EditorUtility.SetDirty(animator);
                    }
                    if (animator.AnimationAsset != null) {
                        animator.AnimationAssetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(animator.AnimationAsset));
                        animator.AnimationAsset = null;
                        EditorUtility.SetDirty(animator);
                    }
                }
            }
        }
        
        [MenuItem("Sago/Mesh/Legacy/Set Not Async", false, 2200)]
        public static void SetAsSerialized() {
            foreach (GameObject obj in Selection.gameObjects) {
                foreach (MeshAnimator animator in obj.GetComponentsInChildren<MeshAnimator>(true)) {
                    if (animator.IsAsync == true) {
                        animator.IsAsync = false;
                        EditorUtility.SetDirty(animator);
                    }
                    if (animator.AnimationAsset == null) {
                        if (animator.AnimationAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(animator.AnimationAssetGuid), typeof(MeshAnimationAsset)) as MeshAnimationAsset) {
                            EditorUtility.SetDirty(animator);
                        }
                    }
                }
            }
        }
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public MeshAnimator Component {
            get { return target as MeshAnimator; }
        }
        
        // ================================================================= //
        // Methods
        // ================================================================= //
        
        private MeshAnimationAsset GetAnimation() {
            
            if (Application.isPlaying) {
                return this.Component.Animation;
            }
            
            MeshAnimationAsset animation;
            animation = null;
            
            if (animation == null) {
                animation = this.Component.AnimationAsset;
            }
            
            if (animation == null && !Application.isPlaying) {
                animation = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(this.Component.AnimationAssetGuid), typeof(MeshAnimationAsset)) as MeshAnimationAsset;
            }
            
            return animation;
            
        }
        
        override public void OnInspectorGUI() {
            
            // EditorGUI.BeginDisabledGroup(true);
            //     EditorGUILayout.ObjectField("Animation", this.Component.Animation, typeof(MeshAnimationAsset), false);
            //     EditorGUILayout.ObjectField("Animation Asset", this.Component.AnimationAsset, typeof(MeshAnimationAsset), false);
            //     EditorGUILayout.TextField("Animation Asset Path", this.Component.AnimationAssetPath);
            //     EditorGUILayout.TextField("Animation Asset Guid", this.Component.AnimationAssetGuid);
            // EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
                
                // animation
                MeshAnimationAsset oldAnimationAsset = this.GetAnimation();
                MeshAnimationAsset newAnimationAsset = EditorGUILayout.ObjectField("Animation", oldAnimationAsset, typeof(MeshAnimationAsset), false) as MeshAnimationAsset;
                
                // current frame
                int oldCurrentFrame = this.Component.CurrentIndex;
                int newCurrentFrame = 0;
                if (newAnimationAsset && newAnimationAsset.Meshes.Length > 1) {
                    newCurrentFrame = EditorGUILayout.IntSlider("Current Frame", oldCurrentFrame, 0, newAnimationAsset.Meshes.Length - 1);
                }
                
                // default frame
                int oldDefaultFrame = this.Component.DefaultFrame;
                int newDefaultFrame = 0;
                if (newAnimationAsset && newAnimationAsset.Meshes.Length > 1) {
                    newDefaultFrame = EditorGUILayout.IntSlider("Default Frame", oldDefaultFrame, 0, newAnimationAsset.Meshes.Length - 1);
                }
                
                // autoplay
                bool oldAutoPlay = this.Component.AutoPlay;
                bool newAutoPlay = EditorGUILayout.Toggle("AutoPlay", oldAutoPlay);
                
                // async
                bool oldAsync = this.Component.IsAsync;
                bool newAsync = EditorGUILayout.Toggle("Async", oldAsync);
                
                // loop
                int oldLoop = this.Component.Loop;
                int newLoop = 0;
                if (newAnimationAsset && newAnimationAsset.Meshes.Length > 1) {
                    newLoop = EditorGUILayout.Toggle("Loop", oldLoop < 0) ? -1 : 0;
                }
                
            EditorGUI.EndDisabledGroup();
            
            if (Application.isPlaying) {
                return;
            }
            
            if (oldAnimationAsset != newAnimationAsset || oldAsync != newAsync) {
                newCurrentFrame = 0;
                newDefaultFrame = 0;
                this.Component.Animation = newAnimationAsset;
                this.Component.AnimationAsset = newAsync ? null : newAnimationAsset;
				this.Component.AnimationAssetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newAnimationAsset));
                this.Component.IsAsync = newAsync;
                this.Component.Jump(newCurrentFrame);
                EditorUtility.SetDirty(this.Component);
            }
            
            if (oldAutoPlay != newAutoPlay) {
                this.Component.AutoPlay = newAutoPlay;
                EditorUtility.SetDirty(this.Component);
            }
            
            if (oldCurrentFrame != newCurrentFrame) {
                this.Component.Jump(newCurrentFrame);
            }
            
            if (oldDefaultFrame != newDefaultFrame) {
                this.Component.DefaultFrame = newDefaultFrame;
                this.Component.Jump(newDefaultFrame);
                EditorUtility.SetDirty(this.Component);
            }
            
            if (oldLoop != newLoop) {
                this.Component.Loop = newLoop;
                EditorUtility.SetDirty(this.Component);
            }
            
        }
        
    }
    
}
