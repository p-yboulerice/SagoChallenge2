namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using Juice.FSM;

	public class SA_RemoveRigidbody2D : StateAction {


		#region Fields

		[SerializeField]
		private Rigidbody2D Rigidbody2D;

		#endregion


		#region Properties

		#endregion


		#region Methods

		public override void Run() {
			if (this.Rigidbody2D != null) {
				Destroy(this.Rigidbody2D);
			}
		}

		#endregion


	}
}