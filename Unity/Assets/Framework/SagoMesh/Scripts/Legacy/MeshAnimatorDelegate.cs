namespace SagoEngine {
    
    using System;
    using UnityEngine;
    
    public interface IMeshAnimatorDelegate {
        
        void OnMeshAnimatorFrame(MeshAnimator animator);
        
        void OnMeshAnimatorPlay(MeshAnimator animator);
        
        void OnMeshAnimatorStop(MeshAnimator animator);
        
    }
    
    public class MeshAnimatorDelegate : MonoBehaviour, IMeshAnimatorDelegate {
        
        public virtual void OnMeshAnimatorFrame(MeshAnimator animator) {

			#if SAGO_MESH_USE_SAGO_AUDIO

			AudioClip[] audioClips = null;
			if (animator.Animation != null && animator.Animation.Audio != null) {
				audioClips = animator.Animation.Audio.GetAudioClips(animator.CurrentIndex);
			}
			if (audioClips != null) {
				foreach (AudioClip audioClip in audioClips) {
					SagoAudio.AudioManager.Instance.Play(audioClip, this.transform);
				}
			}

			#endif

        }
        
        public virtual void OnMeshAnimatorPlay(MeshAnimator animator) {
            
        }
        
        public virtual void OnMeshAnimatorStop(MeshAnimator animator) {
            
        }
        
    }
    
}
