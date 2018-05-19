namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;

	public class SA_AlignTransformRelativeToRigidbody2DVelocity : StateAction {


		#region Fields

		[SerializeField]
		private Transform AlignTransform;

		[SerializeField]
		private float Ease = 3;

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
			this.AlignTransform.up = Vector3.Lerp(this.AlignTransform.up, -this.Rigidbody.velocity, Time.deltaTime * this.Ease);
		}

		#endregion


	}
}