namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using SagoTouch;
	using Touch = SagoTouch.Touch;
	using System.Collections.Generic;
	using Juice.Utils;

	public class SA_DragRotator : StateAction, ITouchForwarderController {


		#region Fields

		[SerializeField]
		private Rotator m_Rotator;

		[SerializeField]
		private Transform Anchor;

		[System.NonSerialized]
		private Camera m_Camera;

		#endregion


		#region Properties

		public Rotator Rotator {
			get { return m_Rotator = m_Rotator ?? this.GetComponentInParent<Rotator>(); }
		}

		public Touch Touch {
			get;
			private set;
		}

		private Camera Camera {
			get { return m_Camera = m_Camera ?? FindObjectOfType<GameCamera>().Camera; }
		}

		#endregion


		#region Methods

		private void StartDrag(Touch touch) {
			if (Touch != null) {
				return;
			}

			this.Touch = touch;

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
		}

		#endregion


		#region Methods

		public override void Run() {

			if (this.Touch != null) {
				Vector3 pos = this.Camera.ScreenToWorldPoint((Vector3)this.Touch.Position);

				Vector2 dif = (Vector2)(pos - this.Anchor.position);

				float angle = Vector2.Angle(Vector2.up, dif) * (dif.x > 0 ? -1 : 1);

				this.Rotator.GoToAngle = angle;
			}

		}

		#endregion


	}
}