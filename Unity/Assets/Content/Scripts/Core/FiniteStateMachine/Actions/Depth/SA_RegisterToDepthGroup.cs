namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using SagoUtils;

	public class SA_RegisterToDepthGroup : StateAction {


		#region Fields

		[SerializeField]
		private DepthGroup m_TargetDepthGroup;

		[System.NonSerialized]
		private DepthGroupElement m_DepthGroupElement;

		#endregion


		#region Properties

		private DepthGroupElement DepthGroupElement {
			get { return m_DepthGroupElement = m_DepthGroupElement ?? GetComponentInParent<DepthGroupElement>(); }
		}

		public DepthGroup TargetDepthGroup {
			get { return m_TargetDepthGroup; }
			set { m_TargetDepthGroup = value;}
		}

		#endregion


		#region Methods

		public override void Run() {
			this.TargetDepthGroup.AddElement(this.DepthGroupElement);
		}

		#endregion


	}
}