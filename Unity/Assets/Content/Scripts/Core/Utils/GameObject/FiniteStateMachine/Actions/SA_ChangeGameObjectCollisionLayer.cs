namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;

	public class SA_ChangeGameObjectCollisionLayer : StateAction {


		#region Fields

		[SerializeField]
		private GameObject Target;

		[SerializeField]
		private LayerMask ToMask;

		#endregion


		#region Methods

		public override void Run() {
			this.Target.layer = this.ToMask.value;
		}

		#endregion


	}
}