namespace SagoEngine {
    
    using System;
    using UnityEngine;
    
    public class MeshAsset : ScriptableObject {
        
        // ================================================================= //
        // Static Methods
        // ================================================================= //
        
        public static implicit operator Mesh(MeshAsset asset) {
            if (asset && asset.Mesh) {
                return asset.Mesh;
            }
            return null;
        }
        
        // ================================================================= //
        // Variables
        // ================================================================= //
        
        [SerializeField]
        private Vector2 m_AnchorPoint;
        
        [SerializeField]
        private Vector2 m_ContentSize;
        
        [SerializeField]
        private Boolean m_IsOpaque;
        
        [SerializeField]
        private UnityEngine.Mesh m_Mesh;
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public Vector2 AnchorPoint {
            get { return m_AnchorPoint; }
            set { m_AnchorPoint = value; }
        }
        
        public Bounds Bounds {
            get { return this.Mesh != null ? this.Mesh.bounds : default(Bounds); }
        }
        
        public Vector2 ContentSize {
            get { return m_ContentSize; }
            set { m_ContentSize = value; }
        }
        
        public Bounds Crop {
            get {
                Bounds crop = new Bounds();
                crop.SetMinMax(
                    new Vector3(this.ContentSize.x * (0f - this.AnchorPoint.x), this.ContentSize.y * (0f - this.AnchorPoint.y), 0), 
                    new Vector3(this.ContentSize.x * (1f - this.AnchorPoint.x), this.ContentSize.y * (1f - this.AnchorPoint.y), 0)
                );
                return crop;
            }
        }
        
        public Boolean IsOpaque {
            get { return m_IsOpaque; }
            set { m_IsOpaque = value; }
        }
        
        public Mesh Mesh {
            get { return m_Mesh; } 
            set { m_Mesh = value; }
        }
        
    }
    
}