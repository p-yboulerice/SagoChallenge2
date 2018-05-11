namespace SagoMesh {

	using UnityEngine;

	public class MeshAnimatorObserver : MonoBehaviour, IMeshAnimatorObserver {


		//
		// Delegate
		//
		public delegate void Delegate(MeshAnimator animator);


		//
		// Properties
		//
		public Delegate JumpDelegate {
			get;
			set;
		}

		public Delegate PlayDelegate {
			get;
			set;
		}

		public Delegate StopDelegate {
			get;
			set;
		}


		//
		// IMeshAnimatorObserver
		//
		public void OnMeshAnimatorJump(MeshAnimator animator) {
			if (this.enabled && this.JumpDelegate != null) {
				this.JumpDelegate(animator);
			}
		}
		
		public void OnMeshAnimatorPlay(MeshAnimator animator) {
			if (this.enabled && this.PlayDelegate != null) {
				this.PlayDelegate(animator);
			}
		}
		
		public void OnMeshAnimatorStop(MeshAnimator animator) {
			if (this.enabled && this.StopDelegate != null) {
				this.StopDelegate(animator);
			}
		}


	}

}
