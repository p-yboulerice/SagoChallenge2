namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_RemoveMoverRadialBounds : StateAction {


		#region Fields

		[SerializeField]
		private MoverRadialBounds MoverRadialBounds;

		#endregion


		#region Properties

		#endregion


		#region Methods

		public override void Run() {
			Destroy(this.MoverRadialBounds);
		}

		#endregion


	}
}