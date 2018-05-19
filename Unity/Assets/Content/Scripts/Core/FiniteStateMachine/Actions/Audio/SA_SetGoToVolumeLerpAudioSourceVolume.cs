namespace Juice.FSM {
	using UnityEngine;
	using System.Collections;
	using Juice.Utils;

	public class SA_SetGoToVolumeLerpAudioSourceVolume : StateAction {


		#region Fields

		[SerializeField]
		private LerpAudioSourceVolume LerpAudioSourceVolume;

		[SerializeField]
		private float GoToVolume;

		#endregion


		#region Properties

		#endregion


		#region Methods

		public override void Run() {
			this.LerpAudioSourceVolume.GoToVolume = this.GoToVolume;
		}

		#endregion


	}
}