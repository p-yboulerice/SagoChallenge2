namespace Juice {
	using UnityEngine;
	using System.Collections;

	public class ForceRotation : MonoBehaviour {


		#region Fields

		[SerializeField]
		private float Rotation;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? this.GetComponent<Transform>(); }
		}

		#endregion


		#region Methods

		void LateUpdate() {
			this.Transform.eulerAngles = new Vector3(0, 0, this.Rotation);	 
		}

		#endregion


	}
}