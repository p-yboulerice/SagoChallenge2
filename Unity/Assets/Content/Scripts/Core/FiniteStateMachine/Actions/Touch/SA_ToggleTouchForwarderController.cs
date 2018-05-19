namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_ToggleTouchForwarderController : StateAction {


		#region Fields

		[SerializeField]
		private TouchForwarderController m_TouchForwarderController;

		[SerializeField]
		private bool Toggle;

		#endregion


		#region Properties

		private TouchForwarderController TouchForwarderController {
			get { return m_TouchForwarderController = m_TouchForwarderController ?? this.GetComponentInParent<TouchForwarderController>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.TouchForwarderController.enabled = this.Toggle;
		}

		#endregion


	}
}