namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using SagoTouch;
	using Touch = SagoTouch.Touch;

	public class TouchForwarderController : TouchForwarderListener {

		#region Fields

		[SerializeField]
		private bool SwallowTouch = true;

		[SerializeField]
		private bool RollOver = true;

		[SerializeField]
		private TouchShape m_TouchShape;

		[System.NonSerialized]
		private Touch m_Touch;


		[System.NonSerialized]
		private Camera m_Camera;

		[System.NonSerialized]
		private GameCamera m_GameCamera;

		[System.NonSerialized]
		private List<ITouchForwarderController> m_Listeners;

		#endregion


		#region Properties

		private List<ITouchForwarderController> Listeners {
			get {
				if (m_Listeners == null) {
					m_Listeners = new List<ITouchForwarderController>();
					this.GetComponentsInChildren<ITouchForwarderController>(true, m_Listeners);
					foreach (ITouchForwarderController t in m_Listeners.ToArray()) {
						MonoBehaviour m = (MonoBehaviour)t;
						if (m.GetComponentInParent<TouchForwarderController>() != this) {
							m_Listeners.Remove(t);
						}
					}
				}
				return m_Listeners;
			}
		}

		private GameCamera GameCamera {
			get { return m_GameCamera = m_GameCamera ?? FindObjectOfType<GameCamera>(); }
		}

		public Camera Camera {
			get { return m_Camera = m_Camera ?? this.GameCamera.Camera; }
		}

		private TouchShape TouchShape {
			get { return m_TouchShape = m_TouchShape ?? this.GetComponentInParent<TouchShape>(); }
			set { m_TouchShape = value; }
		}

		public Touch Touch {
			get { return m_Touch; }
			set { m_Touch = value; }
		}

		#endregion


		#region Methods

		override protected void OnEnable() {
			base.OnEnable();
		}

		override protected void OnDisable() {
			base.OnDisable();
		}

		private void Swallow(Touch touch) {
			this.TouchForwarderManager.SwallowTouch(touch, this);
		}

		public bool IsOverShape(Touch touch) {
			if (this.SwallowTouch && touch == this.Touch) {
				return true;
			}
			bool overShape = this.TouchShape.PositionOverShape(CameraUtils.TouchToWorldPoint(touch, this.Camera.transform, this.Camera));
			return overShape;
		}

		#endregion


		#region TouchForwarderListener   ITouchForwarderController

		override public void OnTouchBegan(Touch touch) {
			if (this.IsOverShape(touch)) {
				this.Touch = touch;
				this.Swallow(m_Touch);
			}
			if (this.Touch != null) {
				foreach (ITouchForwarderController t in this.Listeners) {
					t.OnTouchBegan(this.Touch);
				}
			}
		}

		override public void OnTouchMoved(Touch touch) {
			if (this.RollOver && this.Touch == null) {
				if (this.IsOverShape(touch)) {
					this.Touch = touch;
					this.Swallow(m_Touch);
				} else {
					if (this.Touch == touch) {
						this.Touch = null;
					}
				}
			}
			if (this.Touch != null) {
				foreach (ITouchForwarderController t in this.Listeners) {
					t.OnTouchMoved(touch);
				}
			}
		}

		override public void OnTouchEnded(Touch touch) {
			if (this.Touch != null && this.Touch == touch) {
				foreach (ITouchForwarderController t in this.Listeners) {
					t.OnTouchEnded(touch);
				}
				this.Touch = null;
			}
		}

		override public void OnTouchCancelled(Touch touch) {
			if (this.Touch != null && this.Touch == touch) {
				foreach (ITouchForwarderController t in this.Listeners) {
					t.OnTouchCancelled(touch);
				}
			}
		}

		#endregion


	}

}