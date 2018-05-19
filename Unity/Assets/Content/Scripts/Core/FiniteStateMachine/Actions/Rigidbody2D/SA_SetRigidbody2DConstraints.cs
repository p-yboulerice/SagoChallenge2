namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;

	public class SA_SetRigidbody2DConstraints : StateAction {


		#region Fields

		[SerializeField]
		private RigidbodyConstraints2D RigidbodyConstraints2D;

		[System.NonSerialized]
		private Rigidbody2D m_Rigidbody2D;

		#endregion


		#region Properties

		private Rigidbody2D Rigidbody2D {
			get { return m_Rigidbody2D = m_Rigidbody2D ?? this.GetComponentInParent<Rigidbody2D>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.Rigidbody2D.constraints = this.RigidbodyConstraints2D;
		}

		#endregion


	}
}