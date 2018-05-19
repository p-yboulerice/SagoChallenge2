namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;


	[RequireComponent(typeof(ColliderMovementDisplacement))]
	public class TriggerController : MonoBehaviour {

		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private List<ITriggerController> m_Listeners;

		[System.NonSerialized]
		private Dictionary<Collider2D, List<Collider2D>> m_CollidedAgainst;

		[System.NonSerialized]
		private List<Collider2D> m_FrameStayColliders;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private List<ITriggerController> Listeners {
			get {
				if (m_Listeners == null) {
					m_Listeners = new List<ITriggerController>();
					GetComponentsInChildren<ITriggerController>(true, m_Listeners);
				}
				return m_Listeners;
			}
		}

		private Dictionary<Collider2D, List<Collider2D>> CollidedAgainst {
			get { return m_CollidedAgainst = m_CollidedAgainst ?? new Dictionary<Collider2D, List<Collider2D>>(); }
			set { m_CollidedAgainst = value; }
		}

		private List<Collider2D> FrameStayColliders {
			get { return m_FrameStayColliders = m_FrameStayColliders ?? new List<Collider2D>(); }
			set { m_FrameStayColliders = value; }
		}

		#endregion


		#region Methods

		void OnEnable() {
			this.CollidedAgainst = new Dictionary<Collider2D, List<Collider2D>>();
		}

		void OnDisable() {

			if (this.gameObject == null) {
				return;
			}

			foreach (KeyValuePair<Collider2D, List<Collider2D>> kvp in this.CollidedAgainst) {
				foreach (ITriggerController t in this.Listeners) {
					t.TriggerExit2D(null, kvp.Key);
				}
			}

			this.CollidedAgainst = new Dictionary<Collider2D, List<Collider2D>>();

		}

		void FixedUpdate() {
			this.FrameStayColliders = null;
		}

		public void TriggerEnter2D(Collider2D collider, Collider2D colliderAgainst) {
			if (this.CollidedAgainst.ContainsKey(colliderAgainst)) {
				if (!this.CollidedAgainst[colliderAgainst].Contains(collider)) {
					this.CollidedAgainst[colliderAgainst].Add(collider);
				}
				return;
			} else {
				this.CollidedAgainst.Add(colliderAgainst, new List<Collider2D>());
			}
			this.CollidedAgainst[colliderAgainst].Add(collider);
			foreach (ITriggerController t in Listeners) {
				t.TriggerEnter2D(collider, colliderAgainst);
			}
		}

		public void TriggerStay2D(Collider2D collider, Collider2D colliderAgainst) {
			if (this.FrameStayColliders.Contains(colliderAgainst)) {
				return;
			}
			this.FrameStayColliders.Add(colliderAgainst);
			foreach (ITriggerController t in Listeners) {
				t.TriggerStay2D(collider, colliderAgainst);
			}
		}

		public void TriggerExit2D(Collider2D collider, Collider2D colliderAgainst) {
			if (this.CollidedAgainst.ContainsKey(colliderAgainst) && this.CollidedAgainst[colliderAgainst].Contains(collider)) {
				this.CollidedAgainst[colliderAgainst].Remove(collider);
				if (this.CollidedAgainst[colliderAgainst].Count > 0) {
					return;
				} else {
					this.CollidedAgainst.Remove(colliderAgainst);
				}
			}
			foreach (ITriggerController t in Listeners) {
				t.TriggerExit2D(collider, colliderAgainst);
			}
		}

		#endregion

	
	}

}