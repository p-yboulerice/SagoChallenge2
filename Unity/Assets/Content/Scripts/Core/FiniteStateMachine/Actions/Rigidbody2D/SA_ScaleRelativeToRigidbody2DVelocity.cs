namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_ScaleRelativeToRigidbody2DVelocity : StateAction {


		#region Fields

		[SerializeField]
		private BounceScale BounceScale;

		[SerializeField]
		private float MaximunVelocity = 1;

		[SerializeField]
		private Vector3 Scale_Min;

		[SerializeField]
		private Vector3 Scale_Max;

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
			float percent = this.Rigidbody2D.velocity.magnitude / this.MaximunVelocity;

			if (percent > 1) {
				percent = 1;
			}

			this.BounceScale.GoToScale = Vector3.Lerp(this.Scale_Min, this.Scale_Max, percent);

		}

		#endregion


	}
}