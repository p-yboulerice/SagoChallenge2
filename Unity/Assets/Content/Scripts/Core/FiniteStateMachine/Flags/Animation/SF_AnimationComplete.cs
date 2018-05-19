namespace Juice.FSM {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Juice.FSM;
	using SagoMesh;
	using SagoUtils;

	public class SF_AnimationComplete : StateFlag {


		#region Fields

		[Disable(typeof(SF_AnimationComplete), "DisableMux")]
		[SerializeField]
		protected MeshAnimatorMultiplexer m_Mux;

		[Disable(typeof(SF_AnimationComplete), "DisableMeshAnimator")]
		[SerializeField]
		protected MeshAnimator m_MeshAnimator;

		#endregion


		#region Properties

		public override bool IsActive {
			get {
				return this.Animator && this.Animator.IsComplete;
			}
		}

		protected MeshAnimator Animator {
			get {
				if (m_Mux && m_Mux.Animator) {
					return m_Mux.Animator;
				} else {
					return m_MeshAnimator;
				}
			}
		}

		#endregion


		#region Methods

		private static bool DisableMux(Object obj) {
			var o = (obj as SF_AnimationComplete);
			return o.m_MeshAnimator && !o.m_Mux;
		}

		private static bool DisableMeshAnimator(Object obj) {
			var o = (obj as SF_AnimationComplete);
			return o.m_Mux && !o.m_MeshAnimator;
		}

		#endregion


	}

}
