namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;

	public class SA_SetRigidbody2DVelocity : StateAction {


		#region Fields

		[SerializeField]
		private Vector2 Velocity;

		[System.NonSerialized]
		private Rigidbody2D m_Rigidbody;

		#endregion


		#region Properties

		private Rigidbody2D Rigidbody {
			get { return m_Rigidbody = m_Rigidbody ?? this.GetComponentInParent<Rigidbody2D>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.Rigidbody.velocity = this.Velocity;
		}

		#endregion


	}
}