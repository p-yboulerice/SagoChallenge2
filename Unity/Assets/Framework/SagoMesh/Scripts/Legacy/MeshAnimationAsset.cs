namespace SagoEngine {
    
    using UnityEngine;
    
    public class MeshAnimationAsset : ScriptableObject {
        
        // ================================================================= //
        // Variables
        // ================================================================= //
        
        [SerializeField]
        private Vector2 m_AnchorPoint;
        
        [SerializeField]
        private MeshAudioAsset m_Audio;
        
        [SerializeField]
        private Bounds m_Bounds;
        
        [SerializeField]
        private Vector2 m_ContentSize;
        
        [SerializeField]
        private Bounds m_Crop;
        
        [SerializeField]
        private float m_Duration;
        
        [SerializeField]
        private MeshAsset[] m_Meshes;
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public Vector2 AnchorPoint {
            get { return m_AnchorPoint; }
            set { m_AnchorPoint = value; }
        }
        
        public MeshAudioAsset Audio {
            get { return m_Audio; }
            set { m_Audio = value; }
        }
        
        public Bounds Bounds {
            get { return m_Bounds; }
        }
        
        public Vector2 ContentSize {
            get { return m_ContentSize; }
            set { m_ContentSize = value; }
        }
        
        public Bounds Crop {
            get { return m_Crop; }
        }
        
        public float Duration {
            get { return m_Duration; }
            set { m_Duration = value; }
        }
        
        public float Interval {
            get { return this.Meshes != null ? this.Duration / this.Meshes.Length : 0; }
        }
        
        public MeshAsset[] Meshes {
            get { return m_Meshes; }
            set { m_Meshes = value; }
        }
        
        public string Path {
            get; set;
        }
        
        
        // ================================================================= //
        // Methods
        // ================================================================= //
        
        public void RecalculateBounds() {
            Bounds bounds = new Bounds();
            if (this.Meshes != null) {
                for (int index = 0; index < this.Meshes.Length; index++) {
                    if (this.Meshes[index] != null) {
                        bounds.Encapsulate(this.Meshes[index].Bounds);
                    }
                }
            }
            this.m_Bounds = bounds;
        }
        
        public void RecalculateCrop() {
            Bounds crop = new Bounds();
            if (this.Meshes != null) {
                for (int index = 0; index < this.Meshes.Length; index++) {
                    if (this.Meshes[index] != null) {
                        crop.Encapsulate(this.Meshes[index].Crop);
                    }
                }
            }
            this.m_Crop = crop;
        }
        
    }
    
}