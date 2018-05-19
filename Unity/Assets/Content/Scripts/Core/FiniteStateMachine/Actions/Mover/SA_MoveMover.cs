namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_MoveMover : StateAction {


		#region Fields

		[SerializeField]
		private Mover m_Mover;

		[SerializeField]
		private Vector3 MoveAmount;

		#endregion


		#region Properties

		private Mover Mover {
			get { return m_Mover = m_Mover ?? this.GetComponentInParent<Mover>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.Mover.GoToPosition += this.MoveAmount;
		}

		#endregion


	}
}