namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_SetRotatorAngle : StateAction {


		#region Fields

		[SerializeField]
		private Rotator m_Rotator;

		[SerializeField]
		private float Angle = 0;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		private Rotator Rotator {
			get { return m_Rotator = m_Rotator ?? this.GetComponentInParent<Rotator>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			this.Rotator.GoToAngle = this.Angle;
		}

		#endregion


	}
}