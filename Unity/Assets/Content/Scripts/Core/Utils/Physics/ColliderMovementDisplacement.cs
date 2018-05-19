namespace Juice.Utils {

	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class ColliderMovementDisplacement : MonoBehaviour {

		#region Fields

		[SerializeField]
		private int m_Iterations = 4;

		[SerializeField]
		private GameObject m_ColliderPrefab;

		[System.NonSerialized]
		private Collider2D m_Collider;

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private List<Transform> m_Colliders;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}


		private List<Transform> Colliders {
			get {
				if (m_Colliders == null) {
					m_Colliders = new List<Transform>();
					for (int i = 0; i < this.Iterations; i++) {
						GameObject go = GameObject.Instantiate(this.ColliderPrefab);
						go.transform.parent = this.Transform;
						if (go.GetComponent<ColliderMovementDisplacementCollider>() == null) {
							go.AddComponent<ColliderMovementDisplacementCollider>();
						}
						go.SetActive(true);
						m_Colliders.Add(go.transform);

					}
				}

				return m_Colliders;
			}
		}

		private int Iterations {
			get { return m_Iterations; }
		}

		private GameObject ColliderPrefab {
			get { return m_ColliderPrefab; }
		}

		private Vector3 PreviousPos {
			get;
			set;
		}

		#endregion


		#region Methods

		void OnEnable() {
			this.PreviousPos = this.Transform.position;
			
		}

		void LateUpdate() {
			Vector3 difVect = this.PreviousPos - this.Transform.position;

			for (int i = 0; i < this.Iterations; i++) {
				this.Colliders[i].position = this.Transform.position + difVect * (1f / (float)this.Colliders.Count) * i;
			}

			this.PreviousPos = this.Transform.position;
		}

		#endregion

	
	}

}