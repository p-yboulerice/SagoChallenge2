namespace SagoMesh {
    
    using UnityEngine;
    
    [System.Serializable]
    public class MeshAnimationFrame {
        
        // ================================================================= //
        // Fields
        // ================================================================= //
        
        [SerializeField]
        protected AudioClip[] m_AudioClips;
        
        [SerializeField]
        protected Mesh[] m_Meshes;
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public AudioClip[] AudioClips {
            get { return m_AudioClips; }
            set { m_AudioClips = value; }
        }
        
        public Mesh[] Meshes {
            get { return m_Meshes; }
            set { m_Meshes = value; }
        }
        
        
    }
    
}