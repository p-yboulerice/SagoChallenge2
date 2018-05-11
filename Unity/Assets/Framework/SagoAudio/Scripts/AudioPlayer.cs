namespace SagoAudio {
    
	using SagoEasing;
    using System.Collections;
    using UnityEngine;
    
    public class AudioPlayer : MonoBehaviour {


		#region Delegate

		public delegate void Delegate(AudioPlayer player);

		#endregion


		#region Properties

		public bool DontPool {
			get;
			set;
		}

		public bool IsPlaying {
			get { return this.Source.isPlaying; }
		}

		public Delegate PlaybackCompleteDelegate {
			get;
			set;
		}

		public AudioSource Source {
			get {
				m_Source = m_Source ?? GetComponent<AudioSource>();
				if (!m_Source) {
					m_Source = this.gameObject.AddComponent<AudioSource>();
					m_Source.playOnAwake = false;
				}
				return m_Source;
			}
		}

		public Transform Transform {
			get {
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}

		public float Volume {
			get { return this.Source.volume; }
			set { this.Source.volume = value; }
		}

		#endregion


		#region Methods

		public void FadeVolume(float from, float to, float duration, Delegate completionDelegate) {

			if (!this.gameObject.activeInHierarchy) return;

			VolumeFadeParameters parameters;
			parameters = new VolumeFadeParameters() {
				Completion = completionDelegate,
				Duration = duration,
				From = (from >= 0) ? from : this.Volume,
				To = to
			};

			StopCoroutine(FADE_VOLUME_COROUTINE);
			StartCoroutine(FADE_VOLUME_COROUTINE, parameters);

		}

		public void Play() {
			StopCoroutine(FADE_VOLUME_COROUTINE);
			this.Source.Play();
			this.PlaybackCompleteEnabled = true;
		}

		public void Stop() {
			if (this.IsPlaying) {
				this.Source.Stop();
				this.PlaybackCompleteEnabled = false;
			}
		}

		public void ResetAudioSource() {
			Stop();
			this.Source.clip = null;
			this.Source.playOnAwake = false;
			this.Source.loop = false;
			this.Source.panStereo = 0;
			this.Source.pitch = 1;
			this.Source.spatialBlend = 0;
			this.Source.volume = 1;
		}

		#endregion


		#region Private Constants

		private const string FADE_VOLUME_COROUTINE = "RunFadeVolume";

		#endregion


		#region Private Properties

		private bool ApplicationIsPaused {
			get;
			set;
		}

		private bool PlaybackCompleteEnabled {
			get;
			set;
		}

		#endregion


		#region Fields

		private AudioSource m_Source;
		private Transform m_Transform;

		#endregion


		#region MonoBehaviour

		public void OnApplicationPause(bool status) {
			this.ApplicationIsPaused = status;
		}

        public void Update() {
			if (!this.ApplicationIsPaused) {
				if (this.PlaybackCompleteEnabled && !this.IsPlaying) {
					this.PlaybackCompleteEnabled = false;
					OnPlaybackComplete();
				}
			}
        }
        
		#endregion


		#region Fade

		private IEnumerator RunFadeVolume(VolumeFadeParameters parameters) {

			float t;
			t = 0;

			float frameRate;
			frameRate = (Application.targetFrameRate > 0) ? Application.targetFrameRate : 60;

			float step;
			step = 1 / (frameRate * parameters.Duration);

			this.Volume = parameters.From;

			while (t < 1) {
				yield return null;
				t = Mathf.Clamp(t + step, 0, 1);
				this.Volume = Quadratic.EaseInOut(parameters.From, parameters.To, t);
			}

			if (parameters.Completion != null) {
				parameters.Completion(this);
			}

		}

		#endregion


		#region Functions

		virtual protected void OnPlaybackComplete() {
			if (this.PlaybackCompleteDelegate != null) {
				this.PlaybackCompleteDelegate(this);
			}
			Cleanup();
		}

		private void Cleanup() {

			StopAllCoroutines();

			if (!this.DontPool) {
				if (AudioManager.Instance) AudioManager.Instance.PoolAudioPlayer(this);
				else Destroy(this.gameObject);
			}

		}

		#endregion


    }

    
}