namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;

	public class SA_ToggleRigidbody2DIsKinematic : StateAction {


		#region Fields

		[SerializeField]
		private bool IsKinematic;

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
			if (this.Rigidbody2D!=null) {
				this.Rigidbody2D.isKinematic = this.IsKinematic;
				if (this.IsKinematic) {
					this.Rigidbody2D.velocity = Vector2.zero;
					this.Rigidbody2D.angularVelocity = 0f;
				}
			}
		}

		#endregion


	}
}