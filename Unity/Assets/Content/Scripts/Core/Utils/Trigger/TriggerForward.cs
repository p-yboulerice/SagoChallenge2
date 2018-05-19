namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class TriggerForward : MonoBehaviour {

		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private List<ITriggerForwardListener> m_Listeners;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private List<ITriggerForwardListener> Listeners {
			get {
				if (m_Listeners == null) {
					m_Listeners = new List<ITriggerForwardListener>();
					GetComponentsInChildren<ITriggerForwardListener>(true, m_Listeners);
				}
				return m_Listeners;
			}
		}

		#endregion


		#region Methods

		void OnEnable() {
		}

		void OnDisable() {

			if (this.gameObject == null) {
				return;
			}

			foreach (ITriggerForwardListener listener in this.Listeners) {
				listener.Terminate();
			}
				
		}

		void OnTriggerEnter2D(Collider2D collider) {
			foreach (ITriggerForwardListener listener in this.Listeners) {
				listener.TriggerEnter2D(collider);
			}
		}

		void OnTriggerStay2D(Collider2D collider) {
			foreach (ITriggerForwardListener listener in this.Listeners) {
				listener.TriggerStay2D(collider);
			}
		}

		void OnTriggerExit2D(Collider2D collider) {
			foreach (ITriggerForwardListener listener in this.Listeners) {
				listener.TriggerExit2D(collider);
			}
		}

		#endregion


	}

}