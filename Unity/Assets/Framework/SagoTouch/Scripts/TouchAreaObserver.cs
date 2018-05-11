namespace SagoTouch {
	
	using UnityEngine;
	
	public class TouchAreaObserver : MonoBehaviour, ITouchAreaObserver {
		
		
		#region Types
		
		public delegate void Delegate(TouchArea touchArea, Touch touch);
		
		#endregion
		
		
		#region Properties
		
		public Delegate TouchCancelledDelegate {
			get;
			set;
		}
		
		public Delegate TouchDownDelegate {
			get;
			set;
		}
		
		public Delegate TouchEnterDelegate {
			get;
			set;
		}
		
		public Delegate TouchExitDelegate {
			get;
			set;
		}
		
		public Delegate TouchUpDelegate {
			get;
			set;
		}
		
		#endregion
		
		
		#region ITouchAreaObserver Methods
		
		public void OnTouchCancelled(TouchArea touchArea, Touch touch) {
			if (this.enabled && this.TouchCancelledDelegate != null) {
				this.TouchCancelledDelegate(touchArea, touch);
			}
		}
		
		public void OnTouchDown(TouchArea touchArea, Touch touch) {
			if (this.enabled && this.TouchDownDelegate != null) {
				this.TouchDownDelegate(touchArea, touch);
			}
		}
		
		public void OnTouchEnter(TouchArea touchArea, Touch touch) {
			if (this.enabled && this.TouchEnterDelegate != null) {
				this.TouchEnterDelegate(touchArea, touch);
			}
		}
		
		public void OnTouchExit(TouchArea touchArea, Touch touch) {
			if (this.enabled && this.TouchExitDelegate != null) {
				this.TouchExitDelegate(touchArea, touch);
			}
		}
		
		public void OnTouchUp(TouchArea touchArea, Touch touch) {
			if (this.enabled && this.TouchUpDelegate != null) {
				this.TouchUpDelegate(touchArea, touch);
			}
		}
		
		#endregion
		
		
	}
	
}
