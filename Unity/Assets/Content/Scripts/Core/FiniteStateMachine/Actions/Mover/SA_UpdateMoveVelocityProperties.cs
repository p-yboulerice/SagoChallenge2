namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_UpdateMoveVelocityProperties : StateAction {

		#region Fields

		[SerializeField]
		private Mover m_Mover;

		[SerializeField]
		private float m_Damper = 14.3f;

		[SerializeField]
		private float m_Acceleration = 800f;

		#endregion


		#region Properties

		private Mover Mover {
			get { return m_Mover = m_Mover ?? GetComponentInParent<Mover>(); }
		}

		private float Damper {
			get { return m_Damper; }
		}

		private float Acceleration {
			get { return m_Acceleration; }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.Mover.Acceleration = this.Acceleration;
			this.Mover.Damper = this.Damper;
		}

		#endregion


	}
}