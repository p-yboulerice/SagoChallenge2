namespace Juice.FSM {

	using UnityEngine;
	using System.Collections;
	using SagoAudio;

	public class SA_StopAudio : StateAction {


		#region Fields

		[SerializeField]
		private AudioSetElementCollection m_Audio;

		#endregion


		#region Properties

		public AudioSetElementCollection Audio {
			get { return m_Audio; }
			set { m_Audio = value; }
		}

		#endregion


		#region Methods

		public override void Run() {
			if (!this.Audio) {
				return;
			}

			this.Audio.Stop();
		}

		#endregion

	
	}

}