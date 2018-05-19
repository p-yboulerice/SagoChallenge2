namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_SetRotatorSpringGoToRotation : StateAction {


		#region Fields

		[SerializeField]
		private RotatorSpring Rotator;

		[SerializeField]
		private float GoToRotation;

		#endregion


		#region Properties

		#endregion


		#region Methods

		public override void Run() {
			this.Rotator.GoToRotation = this.GoToRotation;	
		}

		#endregion


	}
}