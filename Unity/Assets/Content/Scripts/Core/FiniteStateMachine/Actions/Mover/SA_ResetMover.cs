namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_ResetMover : StateAction {


		#region Fields

		[SerializeField]
		private Mover m_Mover;

		#endregion


		#region Properties

		private Mover Mover {
			get { return m_Mover = m_Mover ?? GetComponentInParent<Mover>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			if (this.Mover != null) {
				this.Mover.Velocity = Vector3.zero;
			}
		}

		#endregion


	}
}