namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;
	using Juice.Depth;
	using Juice.Utils;

	public class BounceRotation : MonoBehaviour {


		#region Fields

		[SerializeField]
		private float ShakeRotation;

		[SerializeField]
		private float ShakeRotationCoolEase = 1;

		[SerializeField]
		private float ShakeFrequency;

		[SerializeField]
		private float ShakeFrequencyCoolEase = 1;

		[SerializeField]
		private bool RandomDirection = true;

		[System.NonSerialized]
		private float CurrentShakeRotation;

		[System.NonSerialized]
		private float CurrentShakeFrequency;

		[System.NonSerialized]
		private Transform m_Transform;

		[System.NonSerialized]
		private float CurrentTime;

		[System.NonSerialized]
		private float OriginalRotation;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		#endregion


		#region Methods

		void Start() {
			this.OriginalRotation = this.Transform.localEulerAngles.z;
		}

		public void SetOriginalRotation(float rotationVal) {
			this.OriginalRotation = rotationVal;
		}

		void Update() {
			this.CurrentShakeRotation -= this.CurrentShakeRotation * this.ShakeRotationCoolEase * Time.deltaTime;
			this.CurrentShakeFrequency -= this.CurrentShakeFrequency * this.ShakeFrequencyCoolEase * Time.deltaTime;
			this.CurrentTime += this.CurrentShakeFrequency * Time.deltaTime;
			this.Transform.localEulerAngles = new Vector3(0, 0, Mathf.Sin(this.CurrentTime) * this.CurrentShakeRotation + this.OriginalRotation);
		}

		public void Bounce(float scaler = 1) {
			this.CurrentShakeRotation = this.ShakeRotation * (this.RandomDirection ? (Random.Range(-1f, 1f) >= 0 ? 1 : -1) : 1) * scaler;
			this.CurrentShakeFrequency = this.ShakeFrequency;
		}

		#endregion


	}
}