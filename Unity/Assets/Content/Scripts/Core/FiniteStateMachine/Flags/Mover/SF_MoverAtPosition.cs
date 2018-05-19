namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SF_MoverAtPosition : StateFlag {


		#region Fields

		[System.NonSerialized]
		private Mover m_Mover;

		#endregion


		#region Properties

		private Mover Mover {
			get { return m_Mover = m_Mover ?? this.GetComponentInParent<Mover>(); }
		}

		public override bool IsActive {
			get {
				return this.Mover.AtPosition;
			}
		}

		#endregion


	}
}