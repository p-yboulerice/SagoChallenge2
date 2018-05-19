namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_ToggleShake : StateAction {


		#region Fields

		[SerializeField]
		private TransformShake TransformShake;

		[SerializeField]
		private bool ToggleOn;

		#endregion


		#region Properties

		#endregion


		#region Methods

		public override void Run() {
			this.TransformShake.Shake = this.ToggleOn;
		}

		#endregion


	}
}