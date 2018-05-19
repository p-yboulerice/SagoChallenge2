namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_SetMoverSpace : StateAction {


		#region Fields

		[SerializeField]
		private Mover Mover;

		[SerializeField]
		private Space Space;

		#endregion


		#region Properties

		#endregion


		#region Methods

		public override void Run() {
			this.Mover.MoveSpace = this.Space;	
		}

		#endregion


	}
}