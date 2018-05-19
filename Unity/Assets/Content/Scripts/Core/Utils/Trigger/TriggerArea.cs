namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class TriggerArea : MonoBehaviour {


		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private List<Collider2D> m_Colliders;

		public System.Action<Collider2D> Event_TriggerEnter;
		public System.Action<Collider2D> Event_TriggerExit;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		public List<Collider2D> Colliders {
			get { return m_Colliders = m_Colliders ?? new List<Collider2D>(); }
			private set { m_Colliders = value; }
		}

		public bool IsTriggering {
			get { return this.Colliders.Count > 0; }
		}

		#endregion


		#region Methods

		void OnDisable() {
			this.Colliders.Clear();
		}

		public void RemoveNullColliders(){
			List<Collider2D> colls = new List<Collider2D>();
			foreach (Collider2D c in this.Colliders) {
				if (c != null) {
					colls.Add(c);
				}
			}
			this.Colliders = colls;
		}

		void FixedUpdate() {
			this.Colliders.RemoveAll(item => item == null);
		}

		void OnTriggerEnter2D(Collider2D collider) {
			this.Colliders.Add(collider);
			if (this.Event_TriggerEnter != null) {
				this.Event_TriggerEnter(collider);
			}
		}

		void OnTriggerExit2D(Collider2D collider) {
			this.Colliders.Remove(collider);
			if (this.Event_TriggerExit != null) {
				this.Event_TriggerExit(collider);
			}
		}

		#endregion


	}
}