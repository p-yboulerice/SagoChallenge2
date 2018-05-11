namespace SagoMesh {
    
    using SagoMesh;
    using SagoMesh.Internal;
    using UnityEngine;
    
    public class MeshAnimationSource : MonoBehaviour, IMeshAnimatorSource {
        
        // ================================================================= //
        // Fields
        // ================================================================= //
        
        [SerializeField]
        protected MeshAnimation m_Animation;
        
        
        // ================================================================= //
        // Properties
        // ================================================================= //
        
        public MeshAnimation Animation {
            get { return m_Animation; }
            set { 
                if (m_Animation != value) {
                    m_Animation = value;
                    AssetUtil.SetDirty(this);
                    this.Notify();
                }
            }
        }
        
        
        // ================================================================= //
        // Helper Methods
        // ================================================================= //
        
        protected void Notify() {
            
            MeshAnimator animator;
            animator = this.GetComponent<MeshAnimator>();
            
            if (animator) {
                animator.Pull();
            }
            
        }
        
        
    }
    
}
