namespace Juice.Utils {
	using UnityEngine;
	using System.Collections;

	public class LerpAudioSourceVolume : MonoBehaviour {


		#region Fields

		[SerializeField]
		public float GoToVolume = 1;

		[SerializeField]
		private float Ease = 3;

		[SerializeField]
		private AudioSource AudioSource;

		[System.NonSerialized]
		private Transform m_Transform;

		#endregion


		#region Properties

		public Transform Transform {
			get { return m_Transform = m_Transform ?? GetComponent<Transform>(); }
		}

		#endregion


		#region Methods

		void Update() {
			this.AudioSource.volume += (this.GoToVolume - this.AudioSource.volume) * Time.deltaTime * this.Ease;
		}

		#endregion


	}
}