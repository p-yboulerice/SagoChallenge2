namespace SagoApp {

	using SagoLayout;
	using SagoTouch;
	using SagoUtils;
	using System.Collections;
	using UnityEngine;
	using Touch = SagoTouch.Touch;

	public class HomeButton : MonoBehaviour {


		//
		// States
		//
		public enum States {
			Showing,
			Hiding,
			Hidden,
			Armed,
			Fired
		}
		
		
		//
		// Inspector Properties
		//

		[Header("Visibility")]

		[SerializeField]
		public bool StartHidden;

		[Disable(typeof(HomeButton), "CheckStartHidden")]
		[SerializeField]
		public bool AutoShow;

		[Disable(typeof(HomeButton), "CheckAutoShow")]
		[SerializeField]
		public float AutoShowDelay;

		[Header("Armed Phase")]

		[SerializeField]
		public bool SkipArmedPhase;

		[Disable(typeof(HomeButton), "CheckSkipArmedPhase")]
		[SerializeField]
		public float SpazzGuardDuration;

		//
		// Constants
		//

		private const float ArmedDuration = 3f;


		//
		// Inspector Properties Helper
		//
		static public bool CheckStartHidden(Object button) {
			return !(button as HomeButton).StartHidden;
		}

		static public bool CheckAutoShow(Object button) {
			return !(button as HomeButton).AutoShow;
		}

		static protected bool CheckSkipArmedPhase(Object button) {
			return (button as HomeButton).SkipArmedPhase;
		}

		
		//
		// Properties
		//
		public System.Action Action {
			get;
			set;
		}
		
		private float ArmedTimestamp {
			get;
			set;
		}
		
		public Artboard Artboard {
			get {
				m_Artboard = m_Artboard ? m_Artboard : this.transform.Find("Artboard").GetComponent<Artboard>();
				return m_Artboard;
			}
		}
		
		public Transform Confirm {
			get {
				m_Confirm = m_Confirm ? m_Confirm : this.Artboard.Transform.Find("Meshes/Confirm");
				return m_Confirm;
			}
		}
		
		private TouchArea DismissArea {
			get {
				if (!m_DismissArea) {
					
					int priority;
					priority = this.TouchArea.Priority - 1;
					
					Camera camera;
					camera = this.Artboard.Camera;

					m_DismissArea = camera.gameObject.AddComponent<TouchArea>();
					m_DismissArea.TouchAnywhere = true;
					m_DismissArea.PassTouchesThrough = true;
					m_DismissArea.Priority = priority;

					TouchAreaObserver observer;
					observer = camera.gameObject.AddComponent<TouchAreaObserver>();
					observer.TouchDownDelegate = OnTouchDismissArea;

				}
				return m_DismissArea;
			}
		}
		
		public Transform Home {
			get {
				m_Home = m_Home ? m_Home : this.Artboard.Transform.Find("Meshes/Home");
				return m_Home;
			}
		}
		
		public SizeInPoints SizeInPoints {
			get {
				m_SizeInPoints = m_SizeInPoints ? m_SizeInPoints : GetComponentInChildren<SizeInPoints>();
				return m_SizeInPoints;
			}
		}
		
		public States State {
			get;
			private set;
		}
		
		public TouchArea TouchArea {
			get {
				m_TouchArea = m_TouchArea ? m_TouchArea : GetComponentInChildren<TouchArea>();
				return m_TouchArea;
			}
		}
		
		public TouchAreaObserver TouchAreaObserver {
			get {
				m_TouchAreaObserver = m_TouchAreaObserver ? m_TouchAreaObserver : GetComponentInChildren<TouchAreaObserver>();
				return m_TouchAreaObserver;
			}
		}

		public Transform Transform {
			get {
				m_Transform = m_Transform ?? GetComponent<Transform>();
				return m_Transform;
			}
		}

		
		//
		// Member Fields
		//
		private Artboard m_Artboard;
		private Transform m_Confirm;
		private TouchArea m_DismissArea;
		private TouchAreaObserver m_DismissAreaObserver;
		private Transform m_Home;
		private SizeInPoints m_SizeInPoints;
		private TouchArea m_TouchArea;
		private TouchAreaObserver m_TouchAreaObserver;
		private Transform m_Transform;

		
		//
		// MonoBehaviour
		//
		private void Reset() {
			this.StartHidden = false;
			this.SkipArmedPhase = false;
			this.SpazzGuardDuration = 0.2f;
		}

		private void Awake() {
			this.TouchAreaObserver.TouchUpDelegate = OnTouchUp;
			this.DismissArea.enabled = false;
			if (this.StartHidden) {
				HideImmediate();
			}

			#if VIDEO_BUILD
			foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true)) {
				renderer.enabled = false;
			}
			#endif

		}

		private void Start() {
			if (this.StartHidden && this.AutoShow) {
				ShowAfterDelay(this.AutoShowDelay);
			}
			AppleTVInputLoop();
		}

		private void Update() {
			RunArmedTimeout();
			RunHiding();
		}
		
		
		//
		// Public Methods
		//
		public void Show() {
			if (this.State == HomeButton.States.Hidden || this.State == HomeButton.States.Armed) {
				
				this.TouchArea.enabled = true;
				this.DismissArea.enabled = false;
				this.Confirm.gameObject.SetActive(false);
				this.Home.gameObject.SetActive(true);
				
				BounceScaleBehaviour bounce;
				bounce = this.Home.GetComponent<BounceScaleBehaviour>();
				
				float amp;
				amp = bounce.AmplitudeInPercent;
				
				bounce.AmplitudeInPercent = this.State == HomeButton.States.Armed ? amp : 100;
				bounce.Trigger();
				bounce.AmplitudeInPercent = amp;
				
				this.State = HomeButton.States.Showing;
				
			}
		}

		public void ShowAfterDelay(float delay) {
			StartCoroutine(RunShowAfterDelay(delay));
		}

		public void Hide() {
			if (this.State == HomeButton.States.Showing || this.State == HomeButton.States.Armed || this.State == HomeButton.States.Fired) {
				
				this.TouchArea.enabled = true;
				this.DismissArea.enabled = false;
				this.Confirm.gameObject.SetActive(false);
				this.Home.gameObject.SetActive(true);
				
				BounceScaleBehaviour bounce;
				bounce = this.Home.GetComponent<BounceScaleBehaviour>();
				bounce.enabled = false;
				
				this.State = HomeButton.States.Hiding;
				
			}
		}

		public void HideImmediate() {
			this.Confirm.gameObject.SetActive(false);
			this.Home.gameObject.SetActive(false);
			this.TouchArea.enabled = false;
			this.State = HomeButton.States.Hidden;
		}


		//
		// Show After Delay
		//
		private IEnumerator RunShowAfterDelay(float delay) {

			float timeElapsed;
			timeElapsed = 0;

			while (this.State == States.Hidden) {
				timeElapsed += Time.deltaTime;
				if (timeElapsed > delay) {
					Show();
				}
				yield return null;
			}

			yield break;

		}
		
		//
		// AppleTV
		//
		void AppleTVInputLoop() {
			#if UNITY_TVOS
				StartCoroutine("AppleTVInputLoopAsync");
			#endif
		}
		
		IEnumerator AppleTVInputLoopAsync() {
			
			#if UNITY_TVOS && !UNITY_EDITOR
				UnityEngine.Apple.TV.Remote.allowExitToHome = false;
			#endif
			
			// hide the home button on tvos
			Artboard.Camera.enabled = false;
			
			// skip the armed phase on tvos
			SkipArmedPhase = true;
			
			while (true) {
				
				bool isMenu = (
					Input.GetButtonDown("AppleTV.Remote.Menu") || 
					Input.GetButtonDown("AppleTV.Controller1.Menu") || 
					Input.GetButtonDown("AppleTV.Controller2.Menu") || 
					(Application.isEditor && Input.GetMouseButtonDown(1))
				);
				
				if (isMenu) {
					if (State == HomeButton.States.Showing && SkipArmedPhase) {
						FireFromShowing();
						yield break;
					} else if (State == HomeButton.States.Showing && !SkipArmedPhase) {
						Arm();
					} else if (State == HomeButton.States.Armed) {
						Fire();
						yield break;
					}
				}
				yield return null;
				
			}
			
		}
		
		//
		// Touch Handling
		//
		private void OnTouchUp(TouchArea touchArea, Touch touch) {

			if (touch.IsTap) {
				
				switch (this.State) {
				case HomeButton.States.Showing:
					if (this.SkipArmedPhase) this.FireFromShowing();
					else Arm();
					break;
				case HomeButton.States.Armed:
					Fire();
					break;
				}
				
			}
			
		}
		
		private void OnTouchDismissArea(TouchArea touchArea, Touch touch) {
			Disarm();
		}
		
		//
		// State Control
		//
		private void Arm() {
			if (this.State == HomeButton.States.Showing) {
				
				this.DismissArea.enabled = true;
				this.Confirm.gameObject.SetActive(true);
				this.Home.gameObject.SetActive(false);
				
				BounceScaleBehaviour bounce;
				bounce = this.Confirm.GetComponent<BounceScaleBehaviour>();
				bounce.Trigger();
				
				this.ArmedTimestamp = Time.time;
				this.State = HomeButton.States.Armed;
				
			}
		}
		
		private void Disarm() {
			if (this.State == HomeButton.States.Armed) {
				
				this.DismissArea.enabled = false;
				this.Confirm.gameObject.SetActive(false);
				this.Home.gameObject.SetActive(true);
				
				BounceScaleBehaviour bounce;
				bounce = this.Home.GetComponent<BounceScaleBehaviour>();
				bounce.Trigger();
				
				this.State = HomeButton.States.Showing;
				
			}
		}
		
		private void Fire() {
			if (this.State == HomeButton.States.Armed) {
				
				if (Time.time - this.ArmedTimestamp < this.SpazzGuardDuration) {
					Disarm();
					return;
				}
				
				this.TouchArea.enabled = false;
				this.DismissArea.enabled = false;
				
				BounceScaleBehaviour bounce;
				bounce = this.Confirm.GetComponent<BounceScaleBehaviour>();
				bounce.Trigger();
				
				this.State = HomeButton.States.Fired;
				
				if (this.Action == null) Debug.LogWarning("HomeButton fired with no Action assigned", this);
				else this.Action();
				
			}
		}
		
		private void FireFromShowing() {
			this.TouchArea.enabled = false;
			this.DismissArea.enabled = false;
			
			BounceScaleBehaviour bounce;
			bounce = this.Home.GetComponent<BounceScaleBehaviour>();
			bounce.Trigger();
			
			this.State = HomeButton.States.Fired;
			
			if (this.Action == null) Debug.LogWarning("HomeButton fired with no Action assigned", this);
			else this.Action();
		}

		private void RunArmedTimeout() {
			if (this.State == HomeButton.States.Armed) {
				if (Time.time - this.ArmedTimestamp > ArmedDuration) {
					Disarm();
				}
			}
		}
		
		private void RunHiding() {
			if (this.State == HomeButton.States.Hiding) {
				this.Home.localScale *= 0.6f;
			}
		}


	}

}
