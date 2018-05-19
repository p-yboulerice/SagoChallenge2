namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class Collision2DController : MonoBehaviour {


		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private List<ICollision2DController> m_Listeners;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private List<ICollision2DController> Listeners {
			get { return m_Listeners = m_Listeners ?? new List<ICollision2DController>(this.GetComponentsInChildren<ICollision2DController>(true)); }
		}

		#endregion


		#region Methods

		public void RegisterListener(ICollision2DController listener) {
			if (!this.Listeners.Contains(listener)) {
				this.Listeners.Add(listener);	
			}
		}

		public void UnregisterListener(ICollision2DController listener) {
			if (this.Listeners.Contains(listener)) {
				this.Listeners.Remove(listener);
			}
		}

		void OnCollisionEnter2D(Collision2D collision) {
			foreach (ICollision2DController c in this.Listeners) {
				c.OnCollision2DEnter(collision);
			}
		}

		void OnCollisionStay2D(Collision2D collision) {
			foreach (ICollision2DController c in this.Listeners) {
				c.OnCollision2DStay(collision);
			}
		}	

		void OnCollisionExit2D(Collision2D collision) {
			foreach (ICollision2DController c in this.Listeners) {
				c.OnCollision2DExit(collision);
			}
		}	

		void OnDisable() {
			foreach (ICollision2DController c in this.Listeners) {
				c.Terminate();
			}
		}

		#endregion


	}
}