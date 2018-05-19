namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using SagoTouch;

	public class SA_ToggleTouchAreaUseRenderBounds : StateAction {


		#region Fields

		[SerializeField]
		private bool ToggleOn;

		[System.NonSerialized]
		private TouchArea m_TouchArea;

		#endregion


		#region Properties

		private TouchArea TouchArea {
			get { return m_TouchArea = m_TouchArea ?? this.GetComponentInParent<TouchArea>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.TouchArea.UseRendererBounds = this.ToggleOn;
		}

		#endregion


	}
}