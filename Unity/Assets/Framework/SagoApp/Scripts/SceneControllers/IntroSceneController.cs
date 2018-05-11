namespace SagoApp {
	
	using SagoAudio;
	using SagoLayout;
	using SagoMesh;
	using SagoNavigation;
	using SagoPlatform;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.Serialization;
	using Touch = SagoTouch.Touch;
	
	public class IntroSceneController : BaseSceneController {
		
		
		#region Fields

		[Header("Audio")]
		[FormerlySerializedAs("BrandAudio")]
		[SerializeField]
		private AudioClip m_BrandAudio;
		
		[SerializeField]
		private int m_BrandAudioFrame;

		[Header("Animator")]
		[FormerlySerializedAs("IntroAnimator")]
		[SerializeField]
		private MeshAnimator m_IntroAnimator;
		
		#endregion
		
		
		#region Properties
		
		public AudioClip BrandAudio {
			get { return m_BrandAudio; }
			set { m_BrandAudio = value; }
		}
		
		public int BrandAudioFrame {
			get { return m_BrandAudioFrame; }
			set { m_BrandAudioFrame = Mathf.Clamp(value, 0, this.IntroAnimator ? this.IntroAnimator.LastIndex : 0); }
		}
		
		public AudioPlayer BrandAudioPlayer {
			get;
			private set;
		}
		
		public MeshAnimator IntroAnimator {
			get { return m_IntroAnimator; }
			set { m_IntroAnimator = value; }
		}
		
		#endregion
		
		
		#region MonoBehaviour Methods
		
		override protected void Reset() {
			base.Reset();
			this.IntroAnimator = GetComponentInChildren<MeshAnimator>();
			this.NextSceneName = "Title";
		}
		
		override protected void Start() {
			base.Start();
			InitIntroAnimator();
			AppleTVInputLoop();
			SmartEducationSubscriptionCheck();

		}
		
		#endregion
		
		
		#if SAGO_IOS_DEMO || SAGO_TVOS_DEMO
		
		void OnGUI() {
			if (SceneNavigator.Instance && SceneNavigator.Instance.IsReady) {
				
				GUIStyle style;
				style = new GUIStyle(GUI.skin.label);
				style.alignment = TextAnchor.MiddleCenter;
				style.fontSize = 18;
				style.normal.textColor = new Color(0f,0f,0f,0.5f);
				
				Rect rect;
				rect = new Rect(0, 0, Screen.width, Screen.height);
				
				ProductInfo info;
				info = PlatformUtil.GetSettings<ProductInfo>();
				
				string name;
				name = info ? info.DisplayName : "this app";
				
				string text;
				text = string.Format("This version of {0} has been created specifically for demonstration purposes.", name);
				
				GUILayout.BeginArea(rect);
					GUILayout.BeginVertical();
						GUILayout.FlexibleSpace();
						GUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
							GUILayout.Label(text, style, GUILayout.Width(Screen.width), GUILayout.Height(rect.height * 0.2f));
							GUILayout.FlexibleSpace();
						GUILayout.EndHorizontal();
					GUILayout.EndVertical();
				GUILayout.EndArea();
				
			}
		}
		
		#endif

		#region Smart Education Methods

		void SmartEducationSubscriptionCheck() {
			#if SAGO_SMART_EDUCATION && !UNITY_EDITOR
				SmartEducationUtil.CheckSubscriptionStatus();
			#endif
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
					if (NavigateToNextScene()) {
						FadeMasterVolumeOut();
						DeregisterFromTouchDispatcher();
						yield break;
					}
				} else if (isMenu) {
					// allowExitToHome is true, clicking the menu button will pause our app 
					// and take the user to the home screen. the coroutine will keep running 
					// when the user resumes our app
					Debug.Log("AppleTV: Exit To Home");
					yield return null;
				} else {
					yield return null;
				}
				
			}
			
		}
		
		#endregion
		
		
		#region ISingleTouchObserver Methods
		
		override public bool OnTouchBegan(Touch touch) {
			if (NavigateToNextScene()) {
				DeregisterFromTouchDispatcher();
				FadeMasterVolumeOut();
			}
			return false;
		}
		
		#endregion
		
		
		#region Helper Methods
		
		protected void InitIntroAnimator() {
			if (this.IntroAnimator) {
				
				MeshAnimatorObserver.Delegate playDelegate;
				playDelegate = (MeshAnimator animator) => {
					if (this.BrandAudio && animator.CurrentIndex == this.BrandAudioFrame) {
						PlayBrandAudio();
					}
				};
				
				MeshAnimatorObserver.Delegate jumpDelegate;
				jumpDelegate = (MeshAnimator animator) => {
					if (animator.CurrentIndex == this.BrandAudioFrame) {
						PlayBrandAudio();
					}
				};
				
				MeshAnimatorObserver.Delegate stopDelegate;
				stopDelegate = (MeshAnimator animator) => {
					StartCoroutine(NavigateToNextSceneAsync());
				};
				
				MeshAnimatorObserver observer;
				observer = this.IntroAnimator.gameObject.AddComponent<MeshAnimatorObserver>();
				observer.PlayDelegate = playDelegate;
				observer.JumpDelegate = jumpDelegate;
				observer.StopDelegate = stopDelegate;
				
				this.IntroAnimator.Play();
				
			}
		}

		protected IEnumerator NavigateToNextSceneAsync() {

			while (!NavigateToNextScene()) {
				yield return null;
			}
		}
		
		protected void PlayBrandAudio() {
			if (!this.BrandAudioPlayer && AudioManager.Instance) {
				
				Transform transform;
				transform = new GameObject("BrandAudio").transform;
				transform.parent = this.Transform;
				
				this.BrandAudioPlayer = AudioManager.Instance.Play(this.BrandAudio, transform);
				
			}
		}
		
		#endregion
		
		
	}
	
}
