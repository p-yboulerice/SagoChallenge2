namespace SagoMesh {
    
    using UnityEngine;
    
    [System.Serializable]
    public class MeshAnimationLayer {
        
        // ================================================================= //
        // Member Variables
        // ================================================================= //
        
        [SerializeField]
        protected Mesh[] m_Meshes;
        
        
        // ================================================================= //
        // Public Properties
        // ================================================================= //
        
        public Mesh[] Meshes {
            get { return m_Meshes; }
            set { m_Meshes = value; }
        }
        
        
    }
    
}