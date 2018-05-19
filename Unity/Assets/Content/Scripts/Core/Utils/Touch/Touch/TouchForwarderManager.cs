namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;
	using SagoTouch;
	using Touch = SagoTouch.Touch;
	using System.Collections.Generic;

	public class TouchForwarderManager : MonoBehaviour, ISingleTouchObserver {

		#region Fields

		[System.NonSerialized]
		Dictionary<Touch, TouchForwarderListener> m_SwallowedTouches;

		[System.NonSerialized]
		private int m_TouchPriority = 0;

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private List<TouchForwarderListener> m_Listeners;

		[System.NonSerialized]
		private List<Touch> m_Touches;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private List<TouchForwarderListener> Listeners {
			get { return m_Listeners = m_Listeners ?? new List<TouchForwarderListener>(); }
		}

		private int TouchPriority {
			get { return m_TouchPriority; }
		}

		private List<Touch> Touches {
			get { return m_Touches = m_Touches ?? new List<Touch>(); }
		}

		private Dictionary<Touch, TouchForwarderListener> SwallowedTouches {
			get { return m_SwallowedTouches = m_SwallowedTouches ?? new Dictionary<Touch, TouchForwarderListener>(); }
		}

		#endregion


		#region Methods

		void Start() {
			TouchDispatcher.Instance.Add(this, this.TouchPriority, false);
		}

		public void RegisterListener(TouchForwarderListener listener) {
			if (!this.Listeners.Contains(listener)) {
				this.Listeners.Add(listener);
				this.Listeners.Sort((a, b) => a.Priority > b.Priority ? -1 : a.Priority < b.Priority ? 1 : 0);
			} else {
				Debug.LogFormat("This listener ( {0} ) has already been added.", listener.gameObject);	
			}
		}

		public void UnregisterListener(TouchForwarderListener listener) {
			if (this.Listeners.Contains(listener)) {
				this.Listeners.Remove(listener);
			} else {
				Debug.LogFormat("This listener ( {0} ) has already been removed or was never added.", listener.gameObject);	
			}
		}

		void Update() {
			foreach (Touch t in this.Touches.ToArray()) {
				if (t.Phase == SagoTouch.TouchPhase.Began) {
					this.OnTouchBeganInternal(t);
				} else if (t.Phase == SagoTouch.TouchPhase.Moved) {
					this.OnTouchMovedInternal(t);
				} else if (t.Phase == SagoTouch.TouchPhase.Ended) {
					this.OnTouchEndedInternal(t);
				} else if (t.Phase == SagoTouch.TouchPhase.Cancelled) {
					this.OnTouchCancelledInternal(t);
				}
			}
		}

		public void SwallowTouch(Touch touch, TouchForwarderListener controller) {
			if (!this.SwallowedTouches.ContainsKey(touch)) {
				this.SwallowedTouches.Add(touch, controller);
			}
		}

		void OnApplicationFocus(bool hasFocus) {
			this.ClearTouches();
		}

		void OnApplicationPause(bool pauseStatus) {
			this.ClearTouches();
		}

		private void ClearTouches() {
			foreach (TouchForwarderListener l in this.Listeners) {
				l.ClearTouches();
			}
		}

		#endregion

	
		#region ISingleTouchObserver implementation

		public bool OnTouchBegan(Touch touch) {
			this.Touches.Add(touch);
			return false;
		}

		public void OnTouchMoved(Touch touch) {

		}

		public void OnTouchEnded(Touch touch) {
			this.OnTouchEndedInternal(touch);
		}

		public void OnTouchCancelled(Touch touch) {
			this.OnTouchCancelledInternal(touch);
		}

		public void OnTouchBeganInternal(Touch touch) {
			foreach (TouchForwarderListener l in this.Listeners) {
				l.OnTouchBegan(touch);
				if (this.SwallowedTouches.ContainsKey(touch)) {
					break;
				}
			}
		}

		private void OnTouchMovedInternal(Touch touch) {
			foreach (TouchForwarderListener l in this.Listeners) {
				if (this.SwallowedTouches.ContainsKey(touch)) {
					if (l == this.SwallowedTouches[touch]) {
						l.OnTouchMoved(touch);
					}
				} else {
					l.OnTouchMoved(touch);
				}
			}
		}

		private void OnTouchEndedInternal(Touch touch) {
			foreach (TouchForwarderListener l in this.Listeners) {
				l.OnTouchEnded(touch);
			}
			this.Touches.Remove(touch);
			if (this.SwallowedTouches.ContainsKey(touch)) {
				this.SwallowedTouches.Remove(touch);
			}
		}

		private void OnTouchCancelledInternal(Touch touch) {
			foreach (TouchForwarderListener l in this.Listeners) {
				l.OnTouchCancelled(touch);
			}
			this.Touches.Remove(touch);
			if (this.SwallowedTouches.ContainsKey(touch)) {
				this.SwallowedTouches.Remove(touch);
			}
		}

		#endregion

	}

}