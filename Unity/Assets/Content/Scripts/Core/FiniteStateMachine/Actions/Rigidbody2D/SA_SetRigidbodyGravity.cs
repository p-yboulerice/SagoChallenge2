namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;

	public class SA_SetRigidbodyGravity : StateAction {


		#region Fields

		[SerializeField]
		private float Min_GravityScale = 1; 

		[SerializeField]
		private float Max_GravityScale = 1;

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
			this.Rigidbody.gravityScale = Random.Range(this.Min_GravityScale, this.Max_GravityScale);
		}

		#endregion


	}
}