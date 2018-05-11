namespace SagoMeshEditor.Internal {
    
    using SagoMesh;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;
    
    public class MeshUtil {
        
        // ================================================================= //
        // Menu Items
        // ================================================================= //
        
        [MenuItem("Sago/Mesh/Make Meshes Read Only", false, 50)]
        static public void SetReadOnly() {
            try {
                AssetDatabase.StartAssetEditing();
                SetIsReadable(false);
            } finally {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
        }
        
        [MenuItem("Sago/Mesh/Make Meshes Read Write", false, 50)]
        static public void SetReadWrite() {
            try {
                AssetDatabase.StartAssetEditing();
                SetIsReadable(true);
            } finally {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
        }
        
        
        // ================================================================= //
        // Static Methods
        // ================================================================= //
        
        
        static public void SetIsReadable(bool isReadable) {
            foreach (Object obj in Selection.objects) {
                SetIsReadable(obj, isReadable);
            }
        }
        
        static public void SetIsReadable(Object obj, bool isReadable) {
                
            string path;
            path = AssetDatabase.GetAssetPath(obj);
            
            if (!string.IsNullOrEmpty(path)) {
                foreach (Mesh mesh in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Mesh>()) {
                    SetIsReadable(mesh, isReadable);
                }
            }
            
        }
        
        static public void SetIsReadable(Mesh mesh, bool isReadable) {
            if (mesh != null) {
                
                // readwrite -> readonly
                if (mesh.isReadable && !isReadable) {
                    mesh.UploadMeshData(true);
                }
                
                // readonly -> readwrite
                if (!mesh.isReadable && isReadable) {
                    
                    // create a temporary mesh, which will be readwrite
                    Mesh temp = new Mesh();
                    
                    // copy the relevant data to the temporary mesh
                    temp.name = mesh.name;
                    temp.vertices = mesh.vertices;
                    temp.colors = mesh.colors;
                    temp.triangles = mesh.triangles;
                    temp.RecalculateBounds();
                    
                    // copy the temp mesh, including the readwrite flag, to 
                    // the mesh (copy instead of replace preserves the mesh's 
                    // guid and any connections to it)
                    EditorUtility.CopySerialized(temp, mesh);
                    EditorUtility.SetDirty(mesh);
                    
                    // destroy the temporary mesh
                    Object.DestroyImmediate(temp, false);
                    
                }
                
            }
        }
        
        
    }
    
}