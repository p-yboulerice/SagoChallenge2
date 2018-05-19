namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_SetMoverPositionRelativeToBezier : StateAction {


		#region Fields

		[Range(0f, 1f)]
		[SerializeField]
		public float AtPercent;

		[SerializeField]
		public Vector3 Offset;

		[SerializeField]
		private Mover m_Mover;

		[SerializeField]
		private KnobAndTubeBezierRenderer Bezier;


		#endregion


		#region Properties

		private Mover Mover {
			get { return m_Mover = m_Mover ?? this.GetComponentInParent<Mover>(); }
		}

		#endregion


		#region Methods

		public override void Run() {
			Vector3 pos = Bezier.PointAt(this.AtPercent);
			pos.z = this.Mover.MoveTransform.position.z;
			this.Mover.GoToPosition = pos + this.Offset;
		}


		#endregion


	}
}