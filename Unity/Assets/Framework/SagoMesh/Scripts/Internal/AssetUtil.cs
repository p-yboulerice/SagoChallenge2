namespace SagoMesh.Internal {
    
    using UnityEngine;
    
    public class AssetUtil {
        
        public static string AssetToGUID(Object obj) {
            #if UNITY_EDITOR
                return UnityEditor.AssetDatabase.AssetPathToGUID(AssetToPath(obj));
            #else
                return null;
            #endif
        }
        
        public static string AssetToPath(Object obj) {
            #if UNITY_EDITOR
                return UnityEditor.AssetDatabase.GetAssetPath(obj);
            #else
                return null;
            #endif
        }
        
        public static T GUIDToAsset<T>(string guid) where T : Object {
            #if UNITY_EDITOR
                return UnityEditor.AssetDatabase.LoadAssetAtPath(GUIDToPath(guid), typeof(T)) as T;
            #else
                return null;
            #endif
        }
        
        public static string GUIDToPath(string guid) {
            #if UNITY_EDITOR
                return UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            #else
                return null;
            #endif
        }
        
        public static T PathToAsset<T>(string path) where T : Object {
            #if UNITY_EDITOR
                return UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
            #else
                return null;
            #endif
        }
        
        public static string PathToGUID(string path) {
            #if UNITY_EDITOR
                return UnityEditor.AssetDatabase.AssetPathToGUID(path);
            #else
                return null;
            #endif
        }
        
        public static void SetDirty(Object obj) {
            #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(obj);
            #endif
        }
        
    }
    
}
