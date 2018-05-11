namespace SagoApp {
	
	using SagoApp.Content;
	using SagoApp.Project;
	using SagoAudio;
	using SagoTouch;
	using SagoLayout;
	using SagoNavigation;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.Serialization;
	using Touch = SagoTouch.Touch;

	public class TitleSceneController : BaseSceneController {
		
		
		#region Fields

		[Header("Music")]
		[FormerlySerializedAs("Music")]
		[SerializeField]
		private AudioClip m_Music;
		
		[System.NonSerialized]
		private VersionDisplayUtil m_VersionDisplay;
		
		[System.NonSerialized]
		private TitleSceneTouchAreaObserver m_StartButton;
		
		[SerializeField]
		private ContentInfo m_ContentInfo;

		#endregion


		#region Properties
		
		public ContentInfo ContentInfo {
			get { return m_ContentInfo; }
			set { m_ContentInfo = value; }
		}
		
		public AudioClip Music {
			get { return m_Music; }
			set { m_Music = value; }
		}
		
		protected AudioPlayer MusicPlayer {
			get;
			set;
		}
		
		protected TitleSceneTouchAreaObserver StartButton {
			get {
				if (m_StartButton == null) {
					foreach (TitleSceneTouchAreaObserver button in GetComponentsInChildren<TitleSceneTouchAreaObserver>()) {
						if (button.ButtonType == TitleSceneTouchAreaObserver.ButtonTypes.Start) {
							m_StartButton = button;
							break;
						}
					}
				}
				return m_StartButton;
			}
		}
		
		#endregion
		
		
		#region MonoBehaviour Methods
		
		override protected void Start() {
			base.Start();
			m_VersionDisplay = GetComponent<VersionDisplayUtil>();
			if (m_VersionDisplay == null) {
				m_VersionDisplay = gameObject.AddComponent<VersionDisplayUtil>();
			}
			AppleTVInputLoop();

			QuitButton.Init(this);
		}

		#endregion
		
		
		#region SceneController Methods
		
		override public void OnSceneWillTransitionOut(SceneController sceneController, SceneTransition transition) {
			
			base.OnSceneWillTransitionOut(sceneController, transition);
			
			// don't call OnSceneWillDisappear, it's called when the start button is 
			// touched so that the parents and promo content disappear immediately, 
			// instead of waiting for the music to fade out first...
			// SagoBiz.Facade.OnSceneWillDisappear(transition != null);
			
		}

		override public void OnSceneDidTransitionOut(SceneController sceneController, SceneTransition transition) {
			base.OnSceneDidTransitionOut(sceneController, transition);
			SagoBiz.Facade.OnWebViewWillAppear -= OnWebViewWillAppear;
			SagoBiz.Facade.OnWebViewDidDisappear -= OnWebViewDidDisappear;
			SagoBiz.Facade.OnSceneDidDisappear(transition != null);
		}
		
		override public void OnSceneWillTransitionIn(SceneController sceneController, SceneTransition transition) {
			base.OnSceneWillTransitionIn(sceneController, transition);
			SagoBiz.Facade.OnSceneWillAppear(transition != null);
			SagoBiz.Facade.OnWebViewWillAppear += OnWebViewWillAppear;
			SagoBiz.Facade.OnWebViewDidDisappear += OnWebViewDidDisappear;

			// Should be able to just call StartMusic() here but for unknown reasons
			// music doesn't actually play on some devices (iPadMini, iPad4 , iPhone5)
			// Jira ROB-28
			StartCoroutine(StartMusicNextFrame());

		}

		override public void OnSceneDidTransitionIn(SceneController sceneController, SceneTransition transition) {
			base.OnSceneDidTransitionIn(sceneController, transition);
			SagoBiz.Facade.OnSceneDidAppear(transition != null);
		}
		
		void OnWebViewWillAppear() {
			StopMusic();
		}
		
		void OnWebViewDidDisappear() {
			StartMusic();
		}
		
		#endregion
		
		
		#region AppleTV Methods
		
		void AppleTVInputLoop() {
			#if UNITY_TVOS
				StartCoroutine("AppleTVInputLoopAsync");
			#endif
		}
		
		IEnumerator AppleTVInputLoopAsync() {
			
			#if UNITY_TVOS && !UNITY_EDITOR
				UnityEngine.Apple.TV.Remote.allowExitToHome = true;
			#endif
			
			while (true) {
				
				bool isClick = (
					Input.GetButtonDown("AppleTV.Remote.Click") || 
					Input.GetButtonDown("AppleTV.Controller1.Click") || 
					Input.GetButtonDown("AppleTV.Controller2.Click") || 
					(Application.isEditor && Input.GetMouseButtonDown(0))
				);
				
				bool isMenu = (
					Input.GetButtonDown("AppleTV.Remote.Menu") || 
					Input.GetButtonDown("AppleTV.Controller1.Menu") || 
					Input.GetButtonDown("AppleTV.Controller2.Menu") || 
					(Application.isEditor && Input.GetMouseButtonDown(1))
				);
				
				bool isPlay = (
					Input.GetButtonDown("AppleTV.Remote.Play") || 
					Input.GetButtonDown("AppleTV.Controller1.Play") ||
					Input.GetButtonDown("AppleTV.Controller2.Play") || 
					(Application.isEditor && Input.GetMouseButtonDown(0))
				);
				
				if (isClick || isPlay) {
					OnTouchStartButton(StartButton.GetComponent<TouchArea>(), null);
					yield break;
				} else if (isMenu) {
					// allowExitToHome is true, clicking the menu button will pause our app 
					// and take the user to the home screen. the coroutine will keep running 
					// when the user resumes our app
					yield return null;
				} else {
					yield return null;
				}
				
			}
			
		}
		
		#endregion
		
		
		#region Touch Methods
		
		virtual public void OnTouchStartButton(TouchArea touchArea, Touch touch) {
			if (SceneNavigator.Instance && SceneNavigator.Instance.IsReady) {
				SagoBiz.Facade.OnSceneWillDisappear(true);
				DisableAllTouchAreas();
				BounceStartButton(touchArea.gameObject);
				StopMusicAndNavigateToNextScene();
			}
		}
		
		#endregion
		
		
		#region Button Methods
		
		protected void BounceStartButton(GameObject startButton) {
			PulseScaleBehaviour pulse;
			pulse = startButton.GetComponent<PulseScaleBehaviour>();
			if (pulse) {
				pulse.enabled = false;
			}
			Bounce(startButton);
		}

		protected void Bounce(GameObject target) {
			BounceScaleBehaviour bounce;
			bounce = target.GetComponent<BounceScaleBehaviour>();
			if (bounce) {
				bounce.enabled = true;
				bounce.Trigger();
			}
		}

		protected void DisableAllTouchAreas() {
			foreach (TouchArea touchArea in GetComponentsInChildren<TouchArea>()) {
				touchArea.enabled = false;
			}
		}

		protected void EnableAllTouchAreas() {
			foreach (TouchArea touchArea in GetComponentsInChildren<TouchArea>()) {
				touchArea.enabled = true;
			}
		}
		
		#endregion
		
		
		#region Music Methods

		protected IEnumerator StartMusicNextFrame() {
			yield return null;
			StartMusic();
		}

		virtual protected void StartMusic() {
			if (!Music) {
				return;
			}

			if (!MusicPlayer && AudioManager.Instance) {

				Transform transform;
				transform = new GameObject("TitleMusic").transform;
				transform.parent = Transform;

				MusicPlayer = AudioManager.Instance.Play(Music, transform);
				MusicPlayer.DontPool = true;
				MusicPlayer.Source.loop = true;

			}
			if (MusicPlayer) {
				MusicPlayer.Play();
			}
			FadeMasterVolumeIn();
		}
		
		virtual protected void StopMusic() {
			if (AudioManager.Instance) {
				if (MusicPlayer) {
					MusicPlayer.Stop();
					AudioManager.Instance.Volume = 0;
					MusicPlayer.Source.time = 0;
				}
			}
		}
		
		virtual protected void StopMusicAndNavigateToNextScene() {
			if (AudioManager.Instance) {
				AudioManager.Instance.FadeVolume(-1, 0, 0.5f, (AudioPlayer player) => {
					if (MusicPlayer) {
						MusicPlayer.Stop();
					}
					if (AudioManager.Instance) {
						AudioManager.Instance.Volume = 1;
					}
					if (ProjectNavigator.Instance && this.ContentInfo) {
						ProjectNavigator.Instance.NavigateToContent(this.ContentInfo);
					} else {
						NavigateToNextScene();
					}
				});
			}
		}
		
		#endregion
		
		
	}
	
}
