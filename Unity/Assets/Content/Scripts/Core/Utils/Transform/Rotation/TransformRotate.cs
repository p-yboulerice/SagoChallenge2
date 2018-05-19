namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class TransformRotate : MonoBehaviour {


		#region Fields

		[SerializeField]
		private float m_RotationSpeed;

		[SerializeField]
		private float AccelerationEase = 3;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private float RotationSpeed {
			get { return m_RotationSpeed; }
			set { m_RotationSpeed = value; }
		}

		public float GoToRotationSpeed {
			get;
			set;
		}

		#endregion


		#region Methods

		void Update() {
			this.RotationSpeed += (this.GoToRotationSpeed - this.RotationSpeed) * Time.deltaTime * this.AccelerationEase;
			this.Transform.Rotate(new Vector3(0, 0, this.RotationSpeed * Time.deltaTime));
		}

		#endregion


	}
}