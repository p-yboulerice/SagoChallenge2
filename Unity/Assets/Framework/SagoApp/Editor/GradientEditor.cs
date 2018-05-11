namespace SagoAppEditor {
    
    using UnityEditor;
    using UnityEngine;
	using UnityEngine.Rendering;
    using Gradient = SagoApp.Gradient;
    using GradientOrientation = SagoApp.GradientOrientation;
    
    [CustomEditor(typeof(Gradient))]
    public class GradientEditor : Editor {
        
        [MenuItem("GameObject/Create Other/Gradient")]
        static public void Create() {
            
            GameObject obj;
            obj = new GameObject();
            obj.name = "Gradient";
            obj.transform.parent = Selection.activeGameObject ? Selection.activeGameObject.transform : null;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            obj.AddComponent<Gradient>();
            
        }
        
        void OnEnable() {
            
            Gradient gradient;
            gradient = this.target as Gradient;
            
            MeshRenderer meshRenderer;
            meshRenderer = gradient.GetComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            
            if (meshRenderer.sharedMaterial == null) {
                foreach (string assetPath in UnityEditor.AssetDatabase.GetAllAssetPaths()) {
                    if (assetPath.EndsWith("Materials/Gradient.mat")) {
                        meshRenderer.sharedMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material)) as Material;
                        break;
                    }
                }
            }
            
        }
        
        override public void OnInspectorGUI() {
            
            Gradient gradient;
            gradient = this.target as Gradient;
            
            GradientOrientation orientationValue;
            orientationValue = (GradientOrientation)EditorGUILayout.EnumPopup("Orientation", gradient.Orientation);
            
            Color fromValue;
            fromValue = EditorGUILayout.ColorField("From", gradient.From);
            
            Color toValue;
            toValue = EditorGUILayout.ColorField("To", gradient.To);
            
            if (gradient.Orientation != orientationValue || gradient.From != fromValue || gradient.To != toValue) {
                gradient.Orientation = orientationValue;
                gradient.From = fromValue;
                gradient.To = toValue;
                EditorUtility.SetDirty(gradient);
            }
            
        }
        
    }
    
}
