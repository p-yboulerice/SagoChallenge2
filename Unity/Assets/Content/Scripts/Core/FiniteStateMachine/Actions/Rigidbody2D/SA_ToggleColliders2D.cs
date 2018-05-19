namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using Juice.FSM;

	public class SA_ToggleColliders2D : StateAction {


		#region Fields

		[SerializeField]
		private GameObject RootObject;

		[SerializeField]
		private bool On;

		[SerializeField]
		private bool All = true;

		#endregion


		#region Methods

		public override void Run() {

			if (this.All) {
				Collider2D[] colliders = this.RootObject.GetComponentsInChildren<Collider2D>(true);

				foreach (Collider2D c in colliders) {
					c.enabled = this.On;
				}
			} else {
				this.RootObject.GetComponent<Collider2D>().enabled = this.On;
			}
		}

		#endregion


	}
}