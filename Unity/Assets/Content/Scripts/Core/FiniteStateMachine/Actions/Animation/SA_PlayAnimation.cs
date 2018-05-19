namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using SagoMesh;

	public class SA_PlayAnimation : StateAction {

		#region Fields

		[SerializeField]
		private MeshAnimatorMultiplexer m_Multiplexer;

		[SerializeField]
		private MeshAnimator m_Animator;

		#endregion


		#region Properties

		private MeshAnimatorMultiplexer Multiplexer {
			get { return m_Multiplexer; }
		}

		private MeshAnimator Animator {
			get { return m_Animator; }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.Multiplexer.Play(this.Animator, 0);
		}

		#endregion

	
	}

}