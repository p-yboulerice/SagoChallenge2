namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_ToggleMover : StateAction {

		#region Fields

		[SerializeField]
		private bool Toggle;

		[System.NonSerialized]
		private Mover m_Mover;

		#endregion


		#region Properties

		private Mover Mover {
			get { return m_Mover = m_Mover ?? GetComponentInParent<Mover>(); }	
		}

		#endregion


		#region Methods

		public override void Run() {
			this.Mover.enabled = this.Toggle;
		}

		#endregion


	}

}