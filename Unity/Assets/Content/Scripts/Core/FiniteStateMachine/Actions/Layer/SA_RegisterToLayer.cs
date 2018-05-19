namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Depth;

	public class SA_RegisterToLayer : StateAction {


		#region Fields

		[SerializeField]
		private Layer m_TargetLayer;

		[System.NonSerialized]
		private Layer m_Layer;

		#endregion


		#region Properties

		public Layer TargetLayer {
			get { return m_TargetLayer; }
			set { m_TargetLayer = value; }
		}

		private Layer Layer {
			get { return m_Layer = m_Layer ?? this.GetComponentInParent<Layer>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.TargetLayer.Add(this.Layer);
		}

		#endregion


	}
}