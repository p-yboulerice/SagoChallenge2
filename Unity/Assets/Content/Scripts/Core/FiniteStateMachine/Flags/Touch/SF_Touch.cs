namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;
	using SagoTouch;
	using Touch = SagoTouch.Touch;

	public class SF_Touch : StateFlag, ITouchForwarderController {


		#region Fields

		[SerializeField]
		private bool TrueWhenOver = true;

		[System.NonSerialized]
		private Touch m_Touch;

		[System.NonSerialized]
		private TouchForwarderController m_TouchForwarderController;

		#endregion


		#region Properties

		private TouchForwarderController TouchForwarderController {
			get { return m_TouchForwarderController = m_TouchForwarderController ?? this.GetComponentInParent<TouchForwarderController>(); }
		}

		public override bool IsActive {
			get { return this.Touch != null && this.TrueWhenOver && this.Touch.Duration > 0.0243f || this.Touch == null && !this.TrueWhenOver; }
		}

		public Touch Touch {
			get { return m_Touch; }
			set { m_Touch = value; }
		}

		#endregion


		#region ITouchForwarderController implementation

		public void OnTouchBegan(Touch touch) {
			this.Touch = touch;
		}

		public void OnTouchMoved(Touch touch) {
			this.Touch = touch;
		}

		public void OnTouchEnded(Touch touch) {
			if (this.Touch == touch) {
				this.Touch = null;
			}
		}

		public void OnTouchCancelled(Touch touch) {
			if (this.Touch == touch) {
				this.Touch = null;
			}
		}

		#endregion


	}
}