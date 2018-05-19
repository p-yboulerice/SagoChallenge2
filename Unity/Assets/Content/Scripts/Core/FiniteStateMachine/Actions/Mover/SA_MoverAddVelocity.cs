namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_MoverAddVelocity : StateAction {


		#region Fields

		[SerializeField]
		private Mover m_Mover;

		[SerializeField]
		private Vector3 Force;

		#endregion


		#region Properties

		private Mover Mover {
			get { return m_Mover = m_Mover ?? this.GetComponentInParent<Mover>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.Mover.Velocity += this.Force;
		}

		#endregion


	}
}