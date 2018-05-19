namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class TransformShake : MonoBehaviour {


		#region Fields

		[SerializeField]
		public bool Shake;

		[SerializeField]
		public float Amplitude = 1;

		[SerializeField]
		public float Frequency = 0.143f;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		private float CurrentTime {
			get;
			set;
		}

		private Vector3 OriginalPosition {
			get;
			set;
		}

		#endregion


		#region Methods

		void Start() {
			this.OriginalPosition = this.Transform.localPosition;
		}

		void Update() {

			if (this.Shake) {
				this.CurrentTime += Time.deltaTime;

				if (this.CurrentTime >= this.Frequency) {
					this.CurrentTime = 0;
					this.Transform.localPosition = this.OriginalPosition + new Vector3(Random.Range(-this.Amplitude, this.Amplitude), Random.Range(-this.Amplitude, this.Amplitude), 0);
				}
			} else {
				this.Transform.localPosition = this.OriginalPosition;
			}

		}

		#endregion


	}
}