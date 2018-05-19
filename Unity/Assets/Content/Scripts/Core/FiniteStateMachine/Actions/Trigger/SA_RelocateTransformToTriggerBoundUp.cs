namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Juice.Utils;

	public class SA_RelocateTransformToTriggerBoundUp : StateAction, ITriggerController {


		#region Fields

		[SerializeField]
		private Transform RelocateTransform;

		#endregion

		#region Properties

		private Collider2D Collider {
			get;
			set;
		}


		#endregion


		#region Methods

		public override void Run() {
			if (this.Collider != null) {
				this.RelocateTransform.position = new Vector3(this.RelocateTransform.position.x, this.Collider.bounds.max.y, this.RelocateTransform.position.z);
			}
		}

		#endregion


		#region ITriggerController implementation

		public void TriggerEnter2D(Collider2D collider, Collider2D colliderAgainst) {
			this.Collider = colliderAgainst;
		}


		public void TriggerStay2D(Collider2D collider, Collider2D colliderAgainst) {
			this.Collider = colliderAgainst;
		}

		public void TriggerExit2D(Collider2D collider, Collider2D colliderAgainst) {
			if (this.Collider == colliderAgainst) {
				this.Collider = null;
			}
		}


		#endregion


	}
}