namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using SagoTouch;
	using Touch = SagoTouch.Touch;
	using System.Collections.Generic;
	using Juice.Utils;

	public class SA_DragMover : StateAction, ITouchForwarderController {

		#region Fields

		[SerializeField]
		private Mover m_Mover;

		[SerializeField]
		private bool m_LockXAxis = false;

		[SerializeField]
		private bool m_LockYAxis = false;

		[SerializeField]
		private bool m_KeepInitialTouchOffset = true;

		[SerializeField]
		private Vector3 Offset;

		[System.NonSerialized]
		private Camera m_Camera;

		#endregion


		#region Properties

		public Mover Mover {
			get { return m_Mover = m_Mover ?? this.GetComponentInParent<Mover>(); }
			private set { m_Mover = value; }
		}

		public Touch Touch {
			get;
			private set;
		}

		public Vector3 OriginalTouchPosition {
			get;
			private set;
		}

		public Vector3 TouchGoToPosition {
			get;
			private set;
		}

		public bool UseOffset {
			get { return m_KeepInitialTouchOffset; }
		}

		private Vector3 TouchOffset {
			get;
			set;
		}

		private bool LockXAxis {
			get { return m_LockXAxis; }
		}

		private bool LockYAxis {
			get { return m_LockYAxis; }
		}

		private Camera Camera {
			get { return m_Camera = m_Camera ?? FindObjectOfType<GameCamera>().Camera; }
		}

		#endregion


		#region Methods

		void Reset() {
			this.Mover = this.Mover;
		}

		private void StartDrag(Touch touch) {
			if (Touch != null) {
				return;
			}

			this.Touch = touch;

			Vector3 dif = this.Mover.MoveTransform.position - this.Camera.ScreenToWorldPoint((Vector3)this.Touch.Position);
			dif.z = 0;

			if (this.UseOffset) {
				this.TouchOffset = dif;
			} else {
				this.TouchOffset = Vector3.zero;
			}
		}

		private void EndDrag(Touch touch) {
			if (this.Touch == null || this.Touch != touch) {
				return;
			}
			this.Touch = null;
		}


		#endregion


		#region ITouchForwarderController implementation

		void ITouchForwarderController.OnTouchCancelled(Touch touch) {
			this.EndDrag(touch);
		}

		void ITouchForwarderController.OnTouchBegan(Touch touch) {
			this.StartDrag(touch);
		}

		void ITouchForwarderController.OnTouchEnded(Touch touch) {
			this.EndDrag(touch);
		}

		void ITouchForwarderController.OnTouchMoved(Touch touch) {
			if (this.Touch == null) {
				this.StartDrag(touch);
			}
		}

		#endregion


		#region Methods

		public override void Run() {
			
			if (this.Touch != null) {
				
				Vector3 pos = this.Camera.ScreenToWorldPoint(this.Touch.Position);
				pos.z = this.Mover.Transform.position.z;

				this.Mover.GoToPosition = ApplyAxisLocks(pos + this.TouchOffset + this.Offset);

				this.TouchGoToPosition = this.Mover.GoToPosition;

			}

		}

		private Vector3 ApplyAxisLocks(Vector3 pos) {

			if (this.LockXAxis) {
				pos.x = this.Mover.Transform.position.x;
			}

			if (this.LockYAxis) {
				pos.y = this.Mover.Transform.position.y;
			}

			if (this.Mover.MoveSpace == Space.Self) {
				pos = this.Mover.Transform.parent.InverseTransformPoint(pos);
			}

			return pos;
		}

		#endregion


	}

}