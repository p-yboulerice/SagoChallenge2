namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;


	public enum PositionWaveDirection {
		Vectical,
		Horizontal
	}



	public class PositionWave : MonoBehaviour {


		#region Fields

		[SerializeField]
		private Transform RotateTransform;

		[SerializeField]
		private float MaxRotation;

		[SerializeField]
		private PositionWaveDirection Direction;

		[SerializeField]
		private float Min_Amplitude = 1;

		[SerializeField]
		private float Max_Amplitude = 1;
		 
		[SerializeField]
		private float Min_Frequency = 1;

		[SerializeField] 
		private float Max_Frequency = 1;

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

		private float Amplitude {
			get;
			set;
		}

		private float Frequency {
			get;
			set;
		}

		#endregion


		#region Methods

		void Start() {
			this.Amplitude = Random.Range(this.Min_Amplitude, this.Max_Amplitude);
			this.Frequency = Random.Range(this.Min_Frequency, this.Max_Frequency);
			this.CurrentTime = Random.Range(0, Mathf.PI * 2);
		}

		void Update() {

			this.CurrentTime += Time.deltaTime * this.Frequency;

			switch (this.Direction) {

			case PositionWaveDirection.Horizontal: 

				break;

			case PositionWaveDirection.Vectical: 
				float sine = Mathf.Sin(this.CurrentTime);
				this.Transform.localPosition = Vector3.up * sine * this.Amplitude;
				this.RotateTransform.eulerAngles = new Vector3(0, 0, 90 + (sine > 0 ? this.MaxRotation : -this.MaxRotation) * sine);
				
				break;
			}
		}

		#endregion


	}
}