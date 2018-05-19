namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using SagoAudio;

	public class SA_PlayAudio : StateAction {


		#region Fields

		[SerializeField]
		private AudioSetElementCollection m_Audio;

		[SerializeField]
		private bool m_PlayOnAudioManager;

		#endregion


		#region Properties

		public AudioSetElementCollection Audio {
			get { return m_Audio; }
			set { m_Audio = value; }
		}

		private bool PlayOnAudioManager {
			get { return m_PlayOnAudioManager; }
		}

		#endregion


		#region Methods

		public override void Run() {
			if (!this.Audio) {
				return;
			}

			if (this.PlayOnAudioManager) {
				AudioManager.Instance.Play(this.Audio.ElementToPlay.AudioClip);
			} else {
				this.Audio.Play();
			}
		}

		#endregion

	
	}

}