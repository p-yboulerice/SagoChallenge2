namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using SagoUtils;

	public class SA_RemoveFromDepthGroup : StateAction {


		#region Fields

		[System.NonSerialized]
		private DepthGroupElement m_DepthGroupElement;

		#endregion


		#region Properties

		private DepthGroupElement DepthGroupElement {
			get { return m_DepthGroupElement = m_DepthGroupElement ?? GetComponentInParent<DepthGroupElement>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			if (this.DepthGroupElement.DepthGroup != null) {
				this.DepthGroupElement.DepthGroup.RemoveElement(this.DepthGroupElement);
			}
		}

		#endregion


	}
}