namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_RecursiveBounce : StateAction {


		#region Fields

		[SerializeField]
		private GameObject Target;

		[SerializeField]
		private float BounceForce = 1;

		[SerializeField]
		private float Delay = 0.07f;

		[Range(0f, 1f)]
		[SerializeField]
		private float Friction = 0.1f;

		#endregion


		#region Properties



		#endregion


		#region Methods

		public override void Run() {
			
			BounceScale[] bss = this.Target.GetComponentsInChildren<BounceScale>();

			for (int i = 0; i < bss.Length; i++) {
				float force = this.BounceForce * (1 - this.Friction * i);
				if (force < 0) {
					break;
				}
				bss[i].BounceWithDelay(force , this.Delay * i);
			}
		}

		#endregion


	}
}