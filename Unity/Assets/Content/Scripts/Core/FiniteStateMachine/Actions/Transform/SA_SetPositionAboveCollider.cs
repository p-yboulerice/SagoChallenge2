namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Juice.Utils;

	public class SA_SetPositionAboveCollider : StateAction {


		#region Fields

		[SerializeField]
		private Transform RelocateTransform;

		[SerializeField]
		private Collider2D Collider;

		#endregion

		#region Properties


		#endregion


		#region Methods

		public override void Run() {
			if (this.Collider != null) {
				this.RelocateTransform.position = new Vector3(this.RelocateTransform.position.x, this.Collider.bounds.max.y, this.RelocateTransform.position.z);
			}
		}

		#endregion


	}
}