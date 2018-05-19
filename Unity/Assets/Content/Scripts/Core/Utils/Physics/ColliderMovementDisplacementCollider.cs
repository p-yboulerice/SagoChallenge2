namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;

	public class ColliderMovementDisplacementCollider : MonoBehaviour {

		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private TriggerController m_TriggerController;

		[System.NonSerialized]
		private Collider2D m_Collider;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private TriggerController TriggerController {
			get {
				return m_TriggerController = m_TriggerController ?? GetComponentInParent<TriggerController>(); 
			}
		}

		private Collider2D Collider {
			get { return m_Collider = m_Collider ?? GetComponent<Collider2D>(); }
		}

		#endregion


		#region Methods

		void OnTriggerEnter2D(Collider2D collider) {
			this.TriggerController.TriggerEnter2D(this.Collider, collider);
		}

		void OnTriggerStay2D (Collider2D collider){
			this.TriggerController.TriggerStay2D(this.Collider, collider);
		}

		void OnTriggerExit2D(Collider2D collider) {
			this.TriggerController.TriggerExit2D(this.Collider, collider);
		}

		#endregion

	
	}

}