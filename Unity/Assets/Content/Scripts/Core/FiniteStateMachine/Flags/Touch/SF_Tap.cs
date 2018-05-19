namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;
	using SagoTouch;
	using Touch = SagoTouch.Touch;

	public class SF_Tap : StateFlag, ITouchForwarderController {


		#region Fields

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
			get { 
				if (this.Touch != null) {
			
					if (this.Touch.Phase == SagoTouch.TouchPhase.Ended && this.Touch.Duration < 0.1f) {
						this.Touch = null;
						return true;
					}
	
				}
				return false;
			}
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
			
		}

		public void OnTouchEnded(Touch touch) {
	
		}

		public void OnTouchCancelled(Touch touch) {
			if (this.Touch == touch) {
				this.Touch = null;
			}
		}

		#endregion


	}
}