namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_SetMoverPosition : StateAction {

		#region Fields

		[SerializeField]
		private Mover m_Mover;

		[SerializeField]
		private Vector3 m_Position = Vector3.zero;

		[SerializeField]
		private Transform m_TransformPosition;

		[SerializeField]
		private bool m_Instant = false;

		[SerializeField]
		private Space m_MoveSpace = Space.World;

		#endregion


		#region Properties

		public Mover Mover {
			get { return m_Mover; }
		}

		public Transform TransformPosition{
			get { return m_TransformPosition; }
			set { m_TransformPosition = value; }
		}

		public Vector3 Position {
			get { return m_TransformPosition != null ? (this.MoveSpace == Space.World ? m_TransformPosition.position : m_TransformPosition.localPosition) : m_Position; }
			set { m_Position = value; }
		}

		public bool Instant {
			get { return m_Instant; }
		}

		public Space MoveSpace {
			get { return m_MoveSpace; }
		}

		#endregion


		#region Methods

		public override void Run() {
			if (!this.Instant) {
				this.Mover.GoToPosition = this.Position;
			} else {
				this.Mover.ForcePosition(this.Position);
			}
		}

		#endregion


	}

}