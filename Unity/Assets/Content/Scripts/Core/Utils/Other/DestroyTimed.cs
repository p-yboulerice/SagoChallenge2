namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class DestroyTimed : MonoBehaviour {


		#region Fields

		[SerializeField]
		private float Time = 2;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		#endregion


		#region Methods

		void Start() {
			Destroy(this.gameObject, this.Time);
		}

		#endregion


	}
}