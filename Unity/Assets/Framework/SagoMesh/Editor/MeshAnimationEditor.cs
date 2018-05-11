namespace SagoMeshEditor {
    
    using SagoMesh;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MeshAnimation))]
    public class MeshAnimationEditor : Editor {
        
        // ================================================================= //
        // Static Methods
        // ================================================================= //
        
        public static void DrawPreviewNow(MeshAnimation animation, int width, int height, int frame) {
            try {
                
                // setup the context
                GL.PushMatrix();
                GL.LoadPixelMatrix(0, width, 0, height);
                GL.Clear(true, true, Color.clear);
                
                // set the active material
                Material material;
                material = DrawPreviewMaterial();
                
                if (material != null) {
                    material.SetPass(0);
                }
                
                // draw the frame
                if (animation != null && animation.Frames != null && animation.Frames.Length >= 0) {
                    
                    // calculate scale
                    Vector2 nativeSize = animation.ContentSize * animation.PixelsPerMeter;
                    float scale = Mathf.Min(width / nativeSize.x, height / nativeSize.y);
                    Vector2 scaledSize = nativeSize * scale;
                    
                    // calculate matrix
                    Matrix4x4 matrix = Matrix4x4.TRS(
                        new Vector3(scaledSize.x * animation.AnchorPoint.x, scaledSize.y * animation.AnchorPoint.y, 0f),
                        Quaternion.identity,
                        new Vector3(scale, scale, 1) * animation.PixelsPerMeter
                    );
                    
                    // calculate frame index
                    int frameIndex = 0;
                    
                    if (animation.Frames.Length == 0) {
                        // TODO: error, frames should not be empty...
                    } else if (animation.Frames.Length == 1) {
                        frameIndex = 0;
                    } else {
                        frameIndex = frame % animation.Frames.Length;
                        frameIndex += frameIndex < 0 ? animation.Frames.Length : 0;
                    }
                    
                    // loop through each layer in the frame
                    for (int layerIndex = animation.Layers.Length - 1; layerIndex >= 0; layerIndex--) {
                        
                        // draw the mesh
                        Mesh mesh;
                        mesh = animation.Frames[frameIndex].Meshes[layerIndex];
                        
                        if (mesh != null) {
                            Graphics.DrawMeshNow(mesh, matrix);
                        }
                        
                    }
                    
                }
                
            } catch (System.Exception e) {
                
                throw e;
                
            } finally {
                
                // clear the context
                GL.PopMatrix();
                
            }
        }
        
        public static Material DrawPreviewMaterial() {
            
            MeshAnimation animation = ScriptableObject.CreateInstance<MeshAnimation>();
            string animationPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(animation));
            DestroyImmediate(animation, false);
            
            string materialPath;
            materialPath = animationPath.Replace("Scripts/MeshAnimation.cs", "Materials/OpaqueMesh.mat");
            
            return AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
            
        }
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        protected SerializedProperty AnchorPointProperty {
            get; set;
        }
        
        protected SerializedProperty ContentSizeProperty {
            get; set;
        }
        
        protected SerializedProperty FramesProperty {
            get; set;
        }
        
        protected SerializedProperty FramePerSecondProperty {
            get; set;
        }
        
        protected SerializedProperty LayersProperty {
            get; set;
        }
        
        #if UNITY_PRO_LICENSE
        protected RenderTexture RenderTexture {
            get; set;
        }
        #endif
        
        // ================================================================= //
        // Methods
        // ================================================================= //
        
        override public string GetInfoString() {
            MeshAnimation animation = this.target as MeshAnimation;
            return string.Format("{0} Frame{1} - {2} FPS - {3:0.0} Seconds", 
                animation.Frames.Length,
                animation.Frames.Length == 0 ? "" : "s",
                animation.FramesPerSecond,
                animation.Duration
            );
        }
        
        override public bool HasPreviewGUI() {
            #if UNITY_PRO_LICENSE
                return true;
            #else
                return false;
            #endif
        }
        
        void OnEnable() {
            
            this.AnchorPointProperty = this.serializedObject.FindProperty("m_AnchorPoint");
            this.ContentSizeProperty = this.serializedObject.FindProperty("m_ContentSize");
            this.FramesProperty = this.serializedObject.FindProperty("m_Frames");
            this.FramePerSecondProperty = this.serializedObject.FindProperty("m_FramesPerSecond");
            this.LayersProperty = this.serializedObject.FindProperty("m_Layers");
            
            #if UNITY_PRO_LICENSE
                this.CreateRenderTexture();
                this.DrawRenderTexture(0);
            #endif
            
        }
        
        void OnDisable() {
            
            #if UNITY_PRO_LICENSE
                this.ReleaseRenderTexture();
            #endif
            
        }
        
        override public void OnInspectorGUI() {
            
            EditorGUI.BeginDisabledGroup(true);
            this.serializedObject.Update();
            
            EditorGUILayout.PropertyField(this.AnchorPointProperty, new GUIContent("Anchor Point"));
            EditorGUILayout.PropertyField(this.ContentSizeProperty, new GUIContent("Content Size"));
            EditorGUILayout.PropertyField(this.FramePerSecondProperty, new GUIContent("Frames Per Second"));
            EditorGUILayout.PropertyField(this.FramesProperty, new GUIContent("Frames"), true);
            EditorGUILayout.PropertyField(this.LayersProperty, new GUIContent("Layers"), true);
            
            this.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndDisabledGroup();
            
        }
        
        #if UNITY_PRO_LICENSE
        override public void OnPreviewGUI(Rect rect, GUIStyle style) {
            GUIStyle previewStyle;
            previewStyle = new GUIStyle(GUIStyle.none);
            previewStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(rect, this.RenderTexture, previewStyle);
        }
        #endif
        
        #if UNITY_PRO_LICENSE
        override public Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
            
            // get temporary render texture
            RenderTexture renderTexture = RenderTexture.GetTemporary(
                width,
                height,
                24,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.Default,
                1
            );
            
            // set the active render texture
            RenderTexture.active = renderTexture;
            
            // draw the preview
            Graphics.SetRenderTarget(renderTexture);
            MeshAnimationEditor.DrawPreviewNow(this.target as MeshAnimation, width, height, 0);
            
            // create the texture
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
            texture.Apply();
            
            // release the render texture
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
            
            // return the texture
            return texture;
            
        }
        #endif
        
        
        // ================================================================= //
        // Helper Methods
        // ================================================================= //
        
        #if UNITY_PRO_LICENSE
        void CreateRenderTexture() {
            
            // release
            this.ReleaseRenderTexture();
            
            // create
            this.RenderTexture = new RenderTexture(
                256,
                256,
                24,
                RenderTextureFormat.ARGB32
            );
            this.RenderTexture.antiAliasing = 8;
            this.RenderTexture.hideFlags = HideFlags.HideAndDontSave;
            this.RenderTexture.Create();
            
        }
        #endif
        
        #if UNITY_PRO_LICENSE
        void DrawRenderTexture(int index) {
            
            RenderTexture.active = this.RenderTexture;
            Graphics.SetRenderTarget(this.RenderTexture);
            MeshAnimationEditor.DrawPreviewNow(this.target as MeshAnimation, this.RenderTexture.width, this.RenderTexture.height, index);
            RenderTexture.active = null;
            
        }
        #endif
        
        #if UNITY_PRO_LICENSE
        void ReleaseRenderTexture() {
            if (this.RenderTexture != null) {
                this.RenderTexture.Release();
                DestroyImmediate(this.RenderTexture, true);
                this.RenderTexture = null;
            }
        }
        #endif
        
    }
    
}