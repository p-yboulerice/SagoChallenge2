namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using Juice.FSM;

	public class SA_SetMoverConfig : StateAction {


		#region Fields

		[Range(0f, 100f)]
		[SerializeField]
		private float Damper;

		[Range(0f, 5000)]
		[SerializeField]
		private float Acceleration;

		[System.NonSerialized]
		private Mover m_Mover;

		#endregion


		#region Properties
	
		private Mover Mover {
			get { return m_Mover = m_Mover ?? this.GetComponentInParent<Mover>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.Mover.Acceleration = this.Acceleration;
			this.Mover.Damper = this.Damper;
		}

		#endregion


	}
}