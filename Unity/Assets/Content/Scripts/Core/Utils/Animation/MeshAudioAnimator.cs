namespace Juice.Utils {

	using System.Collections.Generic;
	using UnityEngine;
	using SagoAudio;
	using SagoMesh;
	using SagoUtils;

	/// <summary>
	/// A MeshAnimatorObserver that plays AudioAnimations.
	/// </summary>
	public class MeshAudioAnimator : MonoBehaviour, IMeshAnimatorObserver {


		#region Serialized Properties

		[SerializeField]
		protected List<AudioAnimation> m_AudioAnimations;

		[SerializeField]
		protected bool m_StopAudioWithAnimation;

		#endregion


		#region Public Properties

		public AudioAnimation Animation {
			get { return m_Animation; }
			protected set { m_Animation = value; }
		}

		#endregion


		#region IMeshAnimatorObserver Methods

		public void OnMeshAnimatorPlay(MeshAnimator animator) {
			this.Animation = this.GetNextAudioAnimation();
			OnMeshAnimatorJump(animator);
		}

		public void OnMeshAnimatorJump(MeshAnimator animator) {
			if (animator.IsPlaying && this.Animation) {
				foreach (AudioAnimationElement element in this.Animation.ElementsAt(animator.CurrentIndex)) {
					PlayElement(element);
				}
			}
		}

		public void OnMeshAnimatorStop(MeshAnimator animator) {
			if (this.StopAudioWithAnimation) {
				foreach (AudioPlayer audioPlayer in GetComponentsInChildren<AudioPlayer>()) {
					audioPlayer.Source.Stop();
				}
			}
		}

		#endregion


		#region Internal Fields

		[System.NonSerialized]
		protected AudioAnimation m_Animation;

		[System.NonSerialized]
		protected List<AudioPlayer> m_AudioPlayers;

		[System.NonSerialized]
		protected RandomArrayIndex m_RandomIndex;

		#endregion


		#region Internal Properties

		public List<AudioAnimation> AudioAnimations {
			get { return m_AudioAnimations; }
			set { m_AudioAnimations = value; }
		}

		protected RandomArrayIndex RandomIndex {
			get {
				m_RandomIndex = m_RandomIndex ?? new RandomArrayIndex(this.AudioAnimations.Count);
				return m_RandomIndex;
			}
		}

		protected bool StopAudioWithAnimation {
			get { return m_StopAudioWithAnimation; }
		}

		#endregion


		#region Internal Methods

		protected void PlayElement(AudioAnimationElement element) {

			AudioPlayer audioPlayer;
			audioPlayer = AudioManager.Instance.Play(element.AudioClip, this.transform);

			if (audioPlayer) {
				audioPlayer.Source.loop = element.IsLoop;
				audioPlayer.Source.pitch = element.Pitch;
				audioPlayer.Source.volume = element.Volume;
			}

		}

		protected AudioAnimation GetNextAudioAnimation() {
			if (this.AudioAnimations.Count > 0) {
				return this.AudioAnimations[this.RandomIndex.Advance];
			} else {
				return null;
			}
		}

		#endregion


	}

}

