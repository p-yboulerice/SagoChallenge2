namespace SagoMesh {
    
    using UnityEngine;
    
    public interface IMeshAnimatorObserver {
        
        void OnMeshAnimatorJump(MeshAnimator animator);
        
        void OnMeshAnimatorPlay(MeshAnimator animator);
        
        void OnMeshAnimatorStop(MeshAnimator animator);
        
    }
    
}