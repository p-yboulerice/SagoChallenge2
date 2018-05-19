namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_BounceSetGoToScale : StateAction {

		#region Fields

		[SerializeField]
		private Vector3 m_GoToScale = Vector3.one;

		[SerializeField]
		private bool ForceToScale = false;

		[SerializeField]
		private BounceScale m_Bounce;

		#endregion


		#region Properties

		private BounceScale Bounce {
			get { return m_Bounce; }
		}

		private Vector3 GoToScale {
			get { return m_GoToScale; }
		}

		#endregion


		#region Methods

		public override void Run() {
			if (!this.ForceToScale) {
				this.Bounce.GoToScale = this.GoToScale;
			} else {
				this.Bounce.ForceScale(this.GoToScale);
			}
		}

		#endregion


	}

}