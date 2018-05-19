namespace Juice.FSM {
	
	using UnityEngine;
	using System.Collections;
	using SagoTouch;

	public class SA_ToggleTouchArea : StateAction {

		#region Fields

		[SerializeField]
		private TouchArea m_TouchArea;

		[SerializeField]
		private bool m_Enable = true;

		#endregion


		#region Properties

		private TouchArea TouchArea {
			get { return m_TouchArea; }
		}

		private bool Enable {
			get { return m_Enable; }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.TouchArea.enabled = this.Enable;
		}

		#endregion
	}
}