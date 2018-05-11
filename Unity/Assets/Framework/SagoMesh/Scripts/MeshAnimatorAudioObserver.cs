namespace SagoMesh.Audio {
    
    using SagoMesh;
    using System.Reflection;
    using UnityEngine;
    
    public class MeshAnimatorAudioObserver : MonoBehaviour, IMeshAnimatorObserver {
        
        // ================================================================= //
        // IMeshAnimatorObserver Methods
        // ================================================================= //
        
        public void OnMeshAnimatorJump(MeshAnimator animator) {
            #if SAGO_MESH_USE_SAGO_AUDIO
            if (animator.Animation && animator.IsPlaying) {
                foreach (AudioClip audioClip in animator.Animation.Frames[animator.CurrentIndex].AudioClips) {
                    SagoAudio.AudioManager.Instance.Play(audioClip, this.GetComponent<Transform>());
                }
            }
            #endif
        }
        
        public void OnMeshAnimatorPlay(MeshAnimator animator) {
            OnMeshAnimatorJump(animator);
        }
        
        public void OnMeshAnimatorStop(MeshAnimator animator) {
            
        }
        
    }
    
}