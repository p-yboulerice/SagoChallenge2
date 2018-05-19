namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using SagoAudio;
	using SagoUtils;

	public class SA_PlayAudioSetManager : StateAction {

		#region Fields

		[SerializeField]
		private AudioClip[] m_Audio;

		[SerializeField]
		private bool PlayRandom = false;

		[System.NonSerialized]
		private RandomArrayIndex m_RandomIndex;

		#endregion


		#region Properties

		private AudioClip[] Audio {
			get { return m_Audio; }
		}

		private RandomArrayIndex RandomIndex {
			get { return m_RandomIndex = m_RandomIndex ?? new RandomArrayIndex(this.Audio.Length); }
		}

		private int CurrentIndex {
			get;
			set;
		}

		#endregion


		#region Methods

		public override void Run() {

			if (this.PlayRandom) {
				AudioManager.Instance.Play(this.Audio[this.RandomIndex.Advance], AudioManager.Instance.Transform);
			} else {
				AudioManager.Instance.Play(this.Audio[this.CurrentIndex]);
				this.CurrentIndex++;
				if (this.CurrentIndex >= this.Audio.Length) {
					this.CurrentIndex = 0;
				}
			}
		}

		#endregion

	
	}

}