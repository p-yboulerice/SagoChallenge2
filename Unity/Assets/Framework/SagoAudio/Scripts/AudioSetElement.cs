namespace SagoAudio {

	#if UNITY_EDITOR
	using UnityEditorInternal;
	#endif

	using System.Collections.Generic;
	using UnityEngine;


	#region Enums

	public enum AudioSetElementType {
		AudioClip,
		AudioSource
	}

	public enum AudioSetElementPlaybackMode {
		PlayOnce,
		Loop
	}

	#endregion


	[ExecuteInEditMode]
	public class AudioSetElement : MonoBehaviour {


		#region Serialized Properties

		[SerializeField]
		public AudioClip AudioClip;

		[SerializeField]
		public AudioSource AudioSource;

		[SerializeField]
		public bool Loop;

		[Range(1, 32)]
		[SerializeField]
		public int MaxChannels;

		[SerializeField]
		public AudioSetElementType Type;

		[SerializeField]
		public bool UseSourceClip;

		[SerializeField]
		public bool UseSourceLoop;

		[SerializeField]
		public bool UseSourceVolume;

		[Range(0, 1)]
		[SerializeField]
		public float Volume;

		#endregion


		#region Properties

		public bool IsPlaying {
			get {
				foreach (AudioPlayer player in this.AudioPlayers) {
					if (player.IsPlaying) return true;
				}
				return false;
			}
		}

		public bool IsSourceType {
			get { return (this.Type == AudioSetElementType.AudioSource); }
		}

		public AudioClip PlaybackClip {
			get {
				if (this.IsSourceType && this.UseSourceClip) {
					return (this.AudioSource == null) ? null : this.AudioSource.clip;
				}
				return this.AudioClip;
			}
			set { this.AudioClip = value; }
		}

		public AudioSetElementPlaybackMode PlaybackMode {
			get {
				if (this.IsSourceType && this.UseSourceLoop) {
					if (this.AudioSource == null) return AudioSetElementPlaybackMode.PlayOnce;
					return (this.AudioSource.loop) ? AudioSetElementPlaybackMode.Loop : AudioSetElementPlaybackMode.PlayOnce;
				}
				return this.Loop ? AudioSetElementPlaybackMode.Loop : AudioSetElementPlaybackMode.PlayOnce;
			}
			set { this.Loop = (value == AudioSetElementPlaybackMode.Loop); }
		}

		public float PlaybackVolume {
			get {
				if (this.IsSourceType && this.UseSourceVolume) {
					return (this.AudioSource == null) ? 1 : this.AudioSource.volume;
				}
				return this.Volume;
			}
			set { this.Volume = value; }
		}

		public Transform Transform {
			get {
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}

		#endregion


		#region Methods

		public AudioPlayer FadeIn(float duration) {

			AudioPlayer player;
			player = Play();

			if (player) {
				player.FadeVolume(0, this.Volume, duration, null);
			}

			return player;

		}

		public void FadeOut(float duration) {

			foreach (AudioPlayer player in this.AudioPlayers) {
				if (player.IsPlaying) {
					player.FadeVolume(player.Volume, 0, duration, OnFadeOutComplete);
				}
			}

		}

		public AudioPlayer Play() {
			
			if (!AudioManager.Instance) return null;
			if (!this.PlaybackClip) return null;

			AudioPlayer player;
			player = GetExistingAudioPlayerForPlayback();

			if (player) {

				ReplaceSourceAudioClip(player.Source, this.PlaybackClip);
				ReplaceSourceLoop(player.Source, this.PlaybackMode);
				ReplaceSourceVolume(player.Source, this.PlaybackVolume);

				player.Play();
			
			} else {

				AudioSource source;
				source = GetAudioSourceForPlayback();

				ReplaceSourceAudioClip(source, this.PlaybackClip);
				ReplaceSourceLoop(source, this.PlaybackMode);
				ReplaceSourceVolume(source, this.PlaybackVolume);

				player = AudioManager.Instance.Play(source, this.Transform);

			}

			player.DontPool = true;

			this.AudioPlayers.Add(player);

			return player;

		}

		public void Stop() {
			foreach (AudioPlayer player in this.AudioPlayers) {
				player.Stop();
			}
		}

		#endregion


		#region Private Properties

		private List<AudioPlayer> AudioPlayers {
			get {
				m_AudioPlayers = m_AudioPlayers ?? new List<AudioPlayer>();
				return m_AudioPlayers;
			}
		}

		#endregion


		#region Fields

		private List<AudioPlayer> m_AudioPlayers;
		private Transform m_Transform;

		#endregion


		#region MonoBehaviour

		private void Awake() {

			AudioSetElement[] elements;
			elements = GetComponents<AudioSetElement>();

			AudioSetElementCollection collection;
			collection = GetComponent<AudioSetElementCollection>();

			bool needCollection;
			needCollection = !collection && elements.Length > 1;

			if (needCollection) {

				collection = this.gameObject.AddComponent<AudioSetElementCollection>();

				#if UNITY_EDITOR
				int i = 0;
				while (i++ < elements.Length) {
					ComponentUtility.MoveComponentUp(collection);
				}
				#endif

			}

		}

		private void Reset() {
			this.MaxChannels = 1;
			this.UseSourceVolume = false;
			this.Volume = 1;
		}

		#endregion


		#region Event Handling

		private void OnFadeOutComplete(AudioPlayer player) {
			player.Stop();
		}

		#endregion


		#region Playback Functions

		private AudioPlayer GetExistingAudioPlayerForPlayback() {

			AudioPlayer result;
			result = null;

			int maxPlayers;
			maxPlayers = Mathf.Max(1, this.MaxChannels);

			if (this.AudioPlayers.Count >= maxPlayers) {
				
				result = this.AudioPlayers[0];
				this.AudioPlayers.Remove(result);
			
			} else {

				result = GetStoppedAudioPlayer();
			
			}

			return result;

		}

		private AudioPlayer GetStoppedAudioPlayer() {
			foreach (AudioPlayer player in this.AudioPlayers) {
				if (!player.IsPlaying) return player;
			}
			return null;
		}

		private AudioSource GetAudioSourceForPlayback() {

			AudioSource source;
			source = this.AudioSource ? Instantiate(this.AudioSource) : CreateAudioSourceForPlayback();

			return source;

		}

		private AudioSource CreateAudioSourceForPlayback() {

			AudioSource source;
			source = new GameObject(this.name).AddComponent<AudioSource>();
			source.clip = this.AudioClip;
			source.loop = this.Loop;
			source.volume = this.Volume;
			source.playOnAwake = false;

			return source;

		}

		private void ReplaceSourceAudioClip(AudioSource source, AudioClip clip) {
			source.clip = clip;
		}

		private void ReplaceSourceLoop(AudioSource source, AudioSetElementPlaybackMode playbackMode) {
			source.loop = (playbackMode == AudioSetElementPlaybackMode.Loop);
		}

		private void ReplaceSourceVolume(AudioSource source, float value) {
			source.volume = value;
		}

		#endregion


	}

}
