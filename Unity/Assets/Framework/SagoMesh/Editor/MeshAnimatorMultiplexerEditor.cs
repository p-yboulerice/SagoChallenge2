namespace SagoMeshEditor {
    
    using SagoMesh;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(MeshAnimatorMultiplexer))]
    public class MeshAnimatorMultiplexerEditor : Editor {
        
        // ================================================================= //
        // Menu Methods
        // ================================================================= //
        
        [MenuItem("GameObject/Create Other/Mesh Animator Multiplexer", false, 2001)]
        public static MeshAnimatorMultiplexer CreateMeshAnimatorMultiplexer() {
            
            List<MeshAnimation> animations;
            animations = new List<object>(Selection.objects).OfType<MeshAnimation>().OrderBy(a => a.name.ToLower()).ToList();
            
            if (animations.Count < 2) {
                Debug.LogError("Cannot create MeshAnimatorMultiplexer. Please select two or more mesh animations from the project window.");
                return null;
            }
            
            MeshAnimatorMultiplexer mux;
            mux = new GameObject().AddComponent<MeshAnimatorMultiplexer>();
            mux.name = "MeshAnimatorMultiplexer";
            
            Transform muxTransform;
            muxTransform = mux.GetComponent<Transform>();
            
            string prefix;
            prefix = ToPascalCase(FindPrefix(animations.Select(a => ToUnderscoreCase(a.name)).ToArray()));
            
            foreach (MeshAnimation animation in animations) {
                
                string name;
                name = ToPascalCase(animation.name);
                name = name.Substring(prefix.Length, name.Length - prefix.Length);
                
                MeshAnimator animator;
                animator = new GameObject().AddComponent<MeshAnimator>();
                animator.name = name;
                
                MeshAnimationSource animatorSource;
                animatorSource = animator.gameObject.AddComponent<MeshAnimationSource>();
                animatorSource.Animation = animation;
                
                Transform animatorTransform;
                animatorTransform = animator.GetComponent<Transform>();
                animatorTransform.parent = muxTransform;
                animatorTransform.localPosition = Vector3.zero;
                animatorTransform.localRotation = Quaternion.identity;
                animatorTransform.localScale = Vector3.one;
                
            }
            
            return mux;
            
        }
        
        /// <see>
        /// http://blogs.microsoft.co.il/yuvmaz/2013/05/10/longest-common-prefix-with-c-and-linq/
        /// </see>
        static string FindPrefix(string[] values) {
            
            if (values == null || values.Length == 0) {
                return string.Empty;
            }
            
            const char SEPARATOR = '_';
            
            List<string> parts;
            parts = new List<string>(values[0].Split(SEPARATOR));
            
            for (int index = 1; index < values.Length; index++) {
                List<string> first = parts;
                List<string> second = new List<string>(values[index].Split(SEPARATOR));
                int maxLength = Mathf.Min(first.Count, second.Count);
                List<string> tempParts = new List<string>(maxLength);
                for (int part = 0; part < maxLength; part++)
                {
                    if (first[part] == second[part])
                        tempParts.Add(first[part]);
                }
                parts = tempParts;
            }
            
            return string.Join(SEPARATOR.ToString(), parts.ToArray());
            
        }
        
        static string ToPascalCase(string value) {
            string result = ToUnderscoreCase(value);
            result = Regex.Replace(result, "_", " ", RegexOptions.Compiled);
            result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(result).Replace(" ", string.Empty);
            return result;
        }
        
        static string ToUnderscoreCase(string value) {
            string result = value;
            result = Regex.Replace(result, "(.)([A-Z][a-z]+)", "$1_$2", RegexOptions.Compiled);
            result = Regex.Replace(result, "([a-z0-9])([A-Z])", "$1_$2", RegexOptions.Compiled);
            return result.ToLower();
        }
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        protected MeshAnimatorMultiplexer Mux {
            get { return this.target as MeshAnimatorMultiplexer; }
        }
        
        
        // ================================================================= //
        // Editor Methods
        // ================================================================= //
        
        public void OnEnable() {
            this.Mux.Invalidate();
        }
        
        override public void OnInspectorGUI() {
            
            EditorGUI.BeginChangeCheck();
            
            MeshAnimator animator;
            animator = this.AnimatorField(this.Mux.Animator);
            
            MeshAnimatorMultiplexerEditMode editMode;
            editMode = this.EditModeField(this.Mux.EditMode);
            
            LayerMask layerMask;
            layerMask = this.LayerMaskField(this.Mux.LayerMask);
            
            if (EditorGUI.EndChangeCheck()) {
                this.Mux.Invalidate();
                this.Mux.EditMode = editMode;
                this.Mux.LayerMask = layerMask;
                this.Mux.Solo(animator);
            }
            
        }
        
        protected MeshAnimator AnimatorField(MeshAnimator value) {
            
            MeshAnimator[] animators; 
            animators = new MeshAnimator[this.Mux.Animators.Length + 1];
            animators[0] = null;
            this.Mux.Animators.CopyTo(animators, 1);
            
            string[] keys;
            keys = new string[this.Mux.Keys.Length + 1];
            keys[0] = "None";
            this.Mux.Keys.CopyTo(keys, 1);
            
            int index;
            index = Mathf.Max(System.Array.IndexOf(animators, value), 0);
            
            MeshAnimator animator;
            animator = animators[EditorGUILayout.Popup("Animator", index, keys)];
            return animator;
            
        }
        
        protected MeshAnimatorMultiplexerEditMode EditModeField(MeshAnimatorMultiplexerEditMode value) {
            
            if (Application.isPlaying) {
                return value;
            }
            
            MeshAnimatorMultiplexerEditMode editMode;
            editMode = (MeshAnimatorMultiplexerEditMode)EditorGUILayout.EnumPopup("Editor Mask", value);
            return editMode;
            
        }
        
        protected LayerMask LayerMaskField(LayerMask value) {
            
            // There are a fixed number of layers in Unity and some of the 
            // layers can be empty. To avoid showing all the empty layers in
            // the popup, we have to do a bunch of bit twiddling...
            // @see: http://answers.unity3d.com/questions/60959/mask-field-in-the-editor.html
            
            // find non-empty layers
            List<int> layerMasks = new List<int>();
            List<string> layerNames = new List<string>();
            for (int index = 0; index < 32; index++) {
                string name = LayerMask.LayerToName(index);
                if (!string.IsNullOrEmpty(name)) {
                    layerMasks.Add(1 << index);
                    layerNames.Add(name);
                }
            }
            
            // encode a temporary mask
            int tempMask = 0;
            for (int index = 0; index < layerMasks.Count; index++) {
                int currentMask = layerMasks[index];
                if (currentMask != 0) {
                    if ((value & currentMask) == currentMask) {
                        tempMask |= (1 << index);
                    }
                } else if (value == 0) {
                    tempMask |= (1 << index);
                }
            }
            
            // display the temporary mask
            int resultMask;
            resultMask = EditorGUILayout.MaskField("Layer Mask", tempMask, layerNames.ToArray());
            
            // decode the result mask
            int layerMask = value;
            for (int index = 0; index < layerMasks.Count; index++) {
                int currentMask = layerMasks[index];
                if (((tempMask ^ resultMask) & (1 << index)) != 0) {
                    if ((resultMask & (1 << index)) != 0) {
                        if (currentMask == 0) {
                            layerMask = 0;
                            break;
                        } else {
                            layerMask |= currentMask;
                        }
                    } else {
                        layerMask &= ~currentMask;
                    }
                }
            }
            
            // return the layer mask
            return layerMask;
            
        }
        
        
    }
    
}