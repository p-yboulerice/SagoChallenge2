namespace SagoAudio {
    
	using SagoEasing;
	using System.Collections;
	using UnityEngine;
    
    public enum AudioManagerState {
        Unknown,
        Quit
    }
    
    public class AudioManager : MonoBehaviour {
		
		/// <summary>
		/// Automatically creates the <see cref="AudioManager" /> instance so that there is always an <see cref="AudioListener" />.
		/// </summary>
		[RuntimeInitializeOnLoadMethod]
		private static void InitializeOnLoad() {
			if (AudioManager.Instance) {
				
			}
		}
        

		#region Static Properties
        
        public static AudioManager Instance {
            get {
                if (AudioManager.State == AudioManagerState.Quit) {
                    return null;
                }
                if (s_Instance == null) {
                    s_Instance = GameObject.FindObjectOfType(typeof(AudioManager)) as AudioManager;
                }
                if (s_Instance == null) {
                    s_Instance = new GameObject().AddComponent<AudioManager>();
                    s_Instance.name = "AudioManager";
                    DontDestroyOnLoad(s_Instance.gameObject);
                }
                return s_Instance;
            }
        }
        
        private static AudioManagerState State {
            get; set;
        }
        
		#endregion


		#region Static Fields

		private static AudioManager s_Instance;

		#endregion

        
		#region Properties

		private Transform AudioPlayerPool {
			get {
				if (!m_AudioPlayerPool) {
					m_AudioPlayerPool = new GameObject("AudioPlayerPool").GetComponent<Transform>();
					m_AudioPlayerPool.parent = this.Transform;
					m_AudioPlayerPool.localPosition = Vector3.zero;
					m_AudioPlayerPool.localRotation = Quaternion.identity;
					m_AudioPlayerPool.localScale = Vector3.one;
					m_AudioPlayerPool.gameObject.SetActive(false);
				}
				return m_AudioPlayerPool;
			}
		}

		public float Volume {
			get { return AudioListener.volume; }
			set { AudioListener.volume = value; }
		}

		public Transform Transform {
			get {
				m_Transform = m_Transform ? m_Transform : this.GetComponent<Transform>();
				return m_Transform;
			}
		}

		#endregion


		#region Methods

		public void FadeVolume(float from, float to, float duration, AudioPlayer.Delegate completionDelegate) {

			string coroutine;
			coroutine = "RunFadeVolume";

			VolumeFadeParameters parameters;
			parameters = new VolumeFadeParameters() {
				Completion = completionDelegate,
				Duration = duration,
				From = (from >= 0) ? from : this.Volume,
				To = to
			};

			StopCoroutine(coroutine);
			StartCoroutine(coroutine, parameters);

		}

		public AudioPlayer Play(AudioClip audioClip) {
			return Play(audioClip, null);
		}

		public AudioPlayer Play(AudioClip audioClip, Transform parent) {

			if (!audioClip) return null;

			AudioPlayer player;
			player = FindAudioPlayer() ?? CreateAudioPlayer();
			ParentAudioPlayer(player, parent);
			player.Source.clip = audioClip;
			player.Source.time = 0f;
			player.Play();

			return player;

		}

		public AudioPlayer Play(AudioSource source, Transform parent) {

			if (!source || !source.clip) return null;

			AudioPlayer player;
			player = source.GetComponent<AudioPlayer>() ?? source.gameObject.AddComponent<AudioPlayer>();
			ParentAudioPlayer(player, parent);
			player.DontPool = true;
			player.Play();

			return player;

		}

		public void PoolAudioPlayer(AudioPlayer player) {
			player.ResetAudioSource();
			ParentAudioPlayer(player, this.AudioPlayerPool);
		}

		#endregion


		#region Member Fields

		private Transform m_AudioPlayerPool;
		private Transform m_Transform;

		#endregion


		#region MonoBehaviour
        
        protected void Awake() {
            if (this != AudioManager.Instance) {
                DestroyObject(gameObject);
                return;
            }
            if (this.GetComponent<AudioListener>() == null) {
                this.gameObject.AddComponent<AudioListener>();
			}
        }
        
        protected void OnApplicationQuit() {
            AudioManager.State = AudioManagerState.Quit;
        }
        
		#endregion

        
		#region Protected Methods

        protected AudioPlayer CreateAudioPlayer() {
            
            AudioPlayer player;
            player = new GameObject("AudioPlayer").AddComponent<AudioPlayer>();
           	
			ParentAudioPlayer(player, this.AudioPlayerPool);
            
            return player;
            
        }
        
        protected AudioPlayer FindAudioPlayer() {
			return (this.AudioPlayerPool.childCount > 0) ? this.AudioPlayerPool.GetChild(0).GetComponent<AudioPlayer>() : null;
        }
        
		protected void ParentAudioPlayer(AudioPlayer player, Transform parent) {
			player.Transform.parent = parent ?? this.Transform;
			player.Transform.localPosition = Vector3.zero;
			player.Transform.localRotation = Quaternion.identity;
			player.Transform.localScale = Vector3.one;
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
				parameters.Completion(null);
			}
			
		}

		#endregion

        
    }
    
}