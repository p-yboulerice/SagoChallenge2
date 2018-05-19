namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_Rotate : StateAction {


		#region Fields

		[SerializeField]
		private TransformRotate TransformRotate;

		[SerializeField]
		private float RotationSpeed;

		#endregion


		#region Properties

		#endregion


		#region Methods

		public override void Run() {
			this.TransformRotate.GoToRotationSpeed = this.RotationSpeed;
		}

		#endregion


	}
}