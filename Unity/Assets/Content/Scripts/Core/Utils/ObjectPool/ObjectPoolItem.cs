namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class ObjectPoolItem : MonoBehaviour {
		

		#region Fields

		[SerializeField]
		private float m_ProbabilityWeight = 64;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		public float PriorityWeight {
			get{ return m_ProbabilityWeight; }
		}

		#endregion

	}
}