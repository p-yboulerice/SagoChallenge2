namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using SagoAudio;

	public class SA_PlayAudioManager : StateAction {

		#region Fields

		[SerializeField]
		private AudioClip m_Audio;
		
		[SerializeField]
		private AudioSource m_AudioSource;

		#endregion


		#region Properties

		private AudioClip Audio {
			get { return m_Audio; }
		}

		private AudioSource AudioSource {
			get { return m_AudioSource; }
		}

		#endregion


		#region Methods

		public override void Run() {

			if (this.AudioSource != null) {
				AudioManager.Instance.Play(this.AudioSource, AudioManager.Instance.Transform);
			} else {
				AudioManager.Instance.Play(this.Audio);
			}
		}

		#endregion

	
	}

}