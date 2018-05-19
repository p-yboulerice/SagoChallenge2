namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;
	using SagoTouch;
	using Touch = SagoTouch.Touch;

	public abstract class TouchForwarderListener : MonoBehaviour {

		#region Fields

		[SerializeField]
		private int m_Priority;

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private TouchForwarderManager m_TouchForwarderManager;


		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		public int Priority {
			get { return m_Priority; }
		}

		protected TouchForwarderManager TouchForwarderManager {
			get { return m_TouchForwarderManager = m_TouchForwarderManager ?? this.GetComponentInParent<TouchForwarderManager>(); }
		}

		#endregion


		#region Methods

		protected virtual void OnEnable() {
			this.RegisterToTouchManager();
		}

		protected virtual void OnDisable() {
			this.UnregisterFromTouchManager();
		}

		protected void RegisterToTouchManager() {
			this.TouchForwarderManager.RegisterListener(this);
		}

		protected void UnregisterFromTouchManager() {
			this.TouchForwarderManager.UnregisterListener(this);
		}

		virtual public void OnTouchBegan(Touch touch) {
		}

		virtual public void OnTouchMoved(Touch touch) {
		}

		virtual public void OnTouchEnded(Touch touch) {
		}

		virtual public void OnTouchCancelled(Touch touch) {
		}

		virtual public void ClearTouches() {
			
		}

		#endregion

	
	}

}