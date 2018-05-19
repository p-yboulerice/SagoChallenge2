namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using SagoMesh;

	public class SA_PlayMeshAnimator : StateAction {


		#region Fields

		[SerializeField]
		private MeshAnimator MeshAnimator;

		#endregion


		#region Properties

		#endregion


		#region Methods

		public override void Run() {
			if (this.MeshAnimator != null) {
				this.MeshAnimator.Play();
			}
		}

		#endregion


	}
}