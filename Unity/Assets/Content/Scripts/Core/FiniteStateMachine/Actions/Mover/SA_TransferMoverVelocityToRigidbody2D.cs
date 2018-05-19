namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_TransferMoverVelocityToRigidbody2D : StateAction {


		#region Fields

		[SerializeField]
		private float VelocityTransferScaler = 1;

		[System.NonSerialized]
		private Rigidbody2D m_Rigidbody2D;

		[System.NonSerialized]
		private Mover m_Mover;

		#endregion


		#region Properties

		private Rigidbody2D Rigidbody2D {
			get { return m_Rigidbody2D = m_Rigidbody2D ?? this.GetComponentInParent<Rigidbody2D>(); }
		}

		private Mover Mover {
			get { return m_Mover = m_Mover ?? this.GetComponentInParent<Mover>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.Rigidbody2D.velocity = this.Mover.Velocity * this.VelocityTransferScaler;
		}

		#endregion


	}
}