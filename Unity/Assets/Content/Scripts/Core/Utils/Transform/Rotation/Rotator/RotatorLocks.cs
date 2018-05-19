namespace Juice {
	using UnityEngine;
	using System.Collections;

	public class RotatorLocks : MonoBehaviour {


		#region Fields

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		#endregion


		#region Methods
		#endregion


	}
}