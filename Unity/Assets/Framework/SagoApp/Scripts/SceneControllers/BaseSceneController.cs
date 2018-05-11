namespace SagoApp {

	using SagoAudio;
	using SagoNavigation;
	using SagoTouch;
	using SagoUtils;
	using SagoCore.Scenes;
	using UnityEngine;

	using Touch = SagoTouch.Touch;

	public class BaseSceneController : SceneController, ISingleTouchObserver {


		#region Serialized Fields

		[Header("Frame Rate")]
		[SerializeField]
		public int TargetFrameRate;

		[Header("Touch")]
		[SerializeField]
		private bool m_ReceiveTouches;

		[Disable(typeof(BaseSceneController), "ReceiveTouchesIsDisabled", 0, true)]
		[SerializeField]
		private bool m_PassTouchesThrough;

		[Disable(typeof(BaseSceneController), "ReceiveTouchesIsDisabled", 0, true)]
		[SerializeField]
		private int m_TouchPriority;

		[Header("Navigation")]
		[SerializeField]
		public SceneReference NextScene;
		
		[SerializeField]
		public string NextSceneName; // TODO: remove this later

		[SerializeField]
		public bool ShouldPreloadNextScene;

		[SerializeField]
		private GameObject m_CustomLoader;

		[SerializeField]
		private GameObject m_CustomTransition;

		#endregion


		#region Disable Attribute Function

		static private bool ReceiveTouchesIsDisabled(Object obj) {
			return !(obj as BaseSceneController).ReceiveTouches;
		}

		#endregion


		#region Properties

		public GameObject CustomLoader {
			get { return m_CustomLoader; }
			set { m_CustomLoader = value && value.GetComponent<LoadingIndicator>() ? value : null; }
		}

		public GameObject CustomTransition {
			get { return m_CustomTransition; }
			set { m_CustomTransition = value && value.GetComponent<SceneTransition>() ? value : null; }
		}

		public bool ReceiveTouches {
			get { return m_ReceiveTouches; }
			set {
				if (m_ReceiveTouches != value) {
					m_ReceiveTouches = value;
					SyncWithTouchDispatcher();
				}
			}
		}

		public bool PassTouchesThrough {
			get { return m_PassTouchesThrough; }
			set {
				if (m_PassTouchesThrough != value) {
					m_PassTouchesThrough = value;
					SyncWithTouchDispatcher();
				}
			}
		}

		public int TouchPriority {
			get { return m_TouchPriority; }
			set {
				if (m_TouchPriority != value) {
					m_TouchPriority = value;
					SyncWithTouchDispatcher();
				}
			}
		}

		#endregion


		#region Methods

		override public void OnWillBeDestroyed() {
			DeregisterFromTouchDispatcher();
			base.OnWillBeDestroyed();
		}

		#endregion


		#region MonoBehaviour

		override protected void Awake() {
			base.Awake();
			Application.targetFrameRate = this.TargetFrameRate;
			InitBackButtonHandler();
		}

		virtual protected void OnDisable() {
			if (this.ReceiveTouches) {
				DeregisterFromTouchDispatcher();
			}
		}

		virtual protected void OnEnable() {
			if (this.ReceiveTouches) {
				RegisterWithTouchDispatcher();
			}
		}

		virtual protected void Reset() {
			this.ReceiveTouches = true;
			this.TargetFrameRate = 60;
		}

		virtual protected void Start() {
			PreloadNextScene();
		}

		#endregion


		#region ISingleTouchObserver

		virtual public bool OnTouchBegan(Touch touch) {
			return false;
		}
		
		virtual public void OnTouchMoved(Touch touch) {
		}
		
		virtual public void OnTouchEnded(Touch touch) {
		}
		
		virtual public void OnTouchCancelled(Touch touch) {
		}

		#endregion


		#region Touch

		protected void SyncWithTouchDispatcher() {
			if (this.ReceiveTouches) RegisterWithTouchDispatcher();
			else DeregisterFromTouchDispatcher();
		}

		protected void DeregisterFromTouchDispatcher() {
			if (TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Remove(this);
			}
		}

		protected void RegisterWithTouchDispatcher() {
			if (TouchDispatcher.Instance) {
				TouchDispatcher.Instance.Add(this, this.TouchPriority, !this.PassTouchesThrough);
			}
		}

		#endregion


		#region Scene Navigation

		protected bool NavigateToNextScene() {
			return NavigateToScene(this.NextScene);
		}

		protected bool NavigateToScene(SceneReference sceneReference) {

			if (SceneNavigator.Instance && SceneNavigator.Instance.IsReady) {

				SceneTransition transitionOut;
				transitionOut = this.CustomTransition ? ((GameObject)Instantiate(this.CustomTransition)).GetComponent<SceneTransition>() : null; //FadeTransition.Create(); // TODO:

				SceneTransition transitionIn;
				transitionIn = this.CustomTransition ? ((GameObject)Instantiate(this.CustomTransition)).GetComponent<SceneTransition>() : null; //FadeTransition.Create(); // TODO:

				LoadingIndicator loader;
				loader = this.CustomLoader ? ((GameObject)Instantiate(this.CustomLoader)).GetComponent<LoadingIndicator>() : null;

				NavigateToScene(sceneReference, transitionOut, transitionIn, loader, true);

				return true;

			}

			return false;

		}
		
		protected bool NavigateToScene(SceneReference sceneReference, SceneTransition transitionOut, SceneTransition transitionIn, LoadingIndicator progressIndicator, bool unloadCurrentScene) {
			if (SceneNavigator.Instance && SceneNavigator.Instance.IsReady) {
				SceneNavigator.Instance.NavigateToScene(
					sceneReference, 
					transitionOut, 
					transitionIn, 
					progressIndicator, 
					unloadCurrentScene,
					true
				);
				return true;
			}
			return false;
		}

		protected void PreloadNextScene() {
			if (this.ShouldPreloadNextScene && SceneNavigator.Instance) {
				SceneNavigator.Instance.LoadScene(this.NextScene);
			}
		}

		#endregion


		#region Audio

		protected void FadeMasterVolumeIn() {
			FadeMasterVolumeIn(1.0f);
		}

		protected void FadeMasterVolumeIn(float duration) {
			if (AudioManager.Instance) {
				AudioManager.Instance.FadeVolume(0, 1, duration, null);
			}
		}

		protected void FadeMasterVolumeOut() {
			FadeMasterVolumeOut(0.5f);
		}

		protected void FadeMasterVolumeOut(float duration) {
			if (AudioManager.Instance) {
				AudioManager.Instance.FadeVolume(-1, 0, duration, null);
			}
		}

		#endregion


		#region BackButtonController 

		protected void InitBackButtonHandler(){
			if (BackButtonController.Instance) {
				BackButtonController.Instance.ShouldEnable = true;
			}
		}

		#endregion


	}
	
}
