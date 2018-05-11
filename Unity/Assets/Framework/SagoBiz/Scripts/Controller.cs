namespace SagoBiz {
	
	using SagoPlatform;
	using SagoUtils;
	using UnityEngine;
	using System.Collections.Generic;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// ControllerState defines the possible states for the <see cref="Controller" /> component.
	/// </summary>
	public enum ControllerState {
		Unknown,
		Started,
		SceneWillAppear,
		SceneDidAppear,
		SceneWillDisappear,
		SceneDidDisappear
	}
	
	/// <summary>
	/// The Controller class a singleton loaded from a prefab in the project's 
	/// resources folder. The singleton provides methods for the app to call 
	/// when the it starts and when the user navigates to and from the title 
	/// scene. The singleton also provides access to it's child components. 
	/// </summary>
	public class Controller : MonoBehaviour {

		#region Static Fields
		
		/// <summary>
		/// The singleton <see cref="Controller" /> instance.
		/// </summary>
		protected static Controller s_Instance;

		#endregion
		
		
		#region Static Properties
		
		/// <summary>
		/// Gets the singleton <see cref="Controller" /> instance.
		/// </summary>
		public static Controller Instance {
			get {
				if (IsPlaying && !IsQuitting) {
					if (s_Instance == null) {
						
						GameObject prefab;
						prefab = Resources.Load(InstanceResourcePath, typeof(GameObject)) as GameObject;
						
						if (prefab == null) {
							Debug.LogError(string.Format(
								"Missing prefab: {0}", 
								InstanceResourcePath
							));
							return null;
						}
						
						GameObject gameObject;
						gameObject = Instantiate(prefab) as GameObject;
						
						if (gameObject == null) {
							Debug.LogError(string.Format(
								"Could not instantiate prefab: {0}", 
								InstanceResourcePath
							));
							return null;
						}
						
						s_Instance = gameObject.GetComponent<Controller>();
						if (s_Instance == null) {
							Debug.LogError(string.Format("Missing component: {0}", "Controller"));
							return null;
						}
						
						s_Instance.name = "SagoBiz";
						DontDestroyOnLoad(s_Instance);
						
					}
				}
				return s_Instance;
			}
		}
		
		public static string InstanceResourcePath {
			get { return "SagoBiz"; }
		}
		
		/// <summary>
		/// Gets the flag that indicates whether the application is playing or 
		/// not, used to avoid loading and instantiating the singleton while in 
		/// the editor.
		/// </summary>
		public static bool IsPlaying {
			get { return Application.isPlaying; }
		}
		
		/// <summary>
		/// Gets and sets the flag that indicates whether the application is 
		/// quitting or not, used to prevent loading and instantiating the 
		/// singleton while the app is quitting. 
		///
		/// When the application quits, objects are destroyed in an indeterminate 
		/// order. If the controller is destroyed and then another object tries 
		/// to access it, a new controller instance would be created but not 
		/// cleaned up (leaving a copy in the hierarchy in the editor).
		/// </summary>
		public static bool IsQuitting {
			get; protected set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether web view is loading or not.
		/// This is to prevent the promo / parent buttons being tapped multiple times which 
		/// could cause multiple webviews being loaded.
		/// </summary>
		/// <value><c>true</c> if is web view loading; otherwise, <c>false</c>.</value>
		public static bool IsWebViewLoading {
			get; protected set;
		}

		#endregion
		
		
		#region Fields
		
		/// <summary>
		/// Cached reference to the <see cref="Analytics" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Analytics m_Analytics;
		
		/// <summary>
		/// Cached reference to the <see cref="Native" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Native m_Native;
		
		/// <summary>
		/// Cached reference to the <see cref="Parents" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Parents m_Parents;
		
		/// <summary>
		/// Cached reference to the <see cref="Promo" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Promo m_Promo;
		
		/// <summary>
		/// Cached reference to the <see cref="Service" /> component.
		/// </summary>
		[System.NonSerialized]
		protected Service m_Service;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets a reference to the <see cref="Analytics" /> component.
		/// </summary>
		public Analytics Analytics {
			get {
				m_Analytics = m_Analytics ?? GetComponentInChildren<Analytics>();
				return m_Analytics;
			}
		}
		
		public AnalyticsOptions AnalyticsOptions {
			get { return PlatformUtil.GetSettings<AnalyticsOptions>(); }
		}
		
		/// <summary>
		/// Gets a reference to the <see cref="Native" /> component.
		/// </summary>
		public Native Native {
			get {
				m_Native = m_Native ?? GetComponentInChildren<Native>();
				return m_Native;
			}
		}
		
		/// <summary>
		/// Gets a reference to the <see cref="Parents" /> component.
		/// </summary>
		public Parents Parents {
			get {
				m_Parents = m_Parents ?? GetComponentInChildren<Parents>();
				return m_Parents;
			}
		}
		
		/// <summary>
		/// Indicates whether the user has enabled or disabled the parents button.
		/// </summary>
		public bool ParentsEnabled {
			get { return PlayerPrefs.GetInt("forParents", 1) > 0; }
		}
		
		public ParentsOptions ParentsOptions {
			get { return PlatformUtil.GetSettings<ParentsOptions>(); }
		}
		
		public Promo Promo {
			get {
				m_Promo = m_Promo ?? GetComponentInChildren<Promo>();
				return m_Promo;
			}
		}
		
		/// <summary>
		/// Indicates whether the user has enabled or disabled the promo button.
		/// </summary>
		public bool PromoEnabled {
			get { return PlayerPrefs.GetInt("sagoNews", 1) > 0; }
		}
		
		/// <summary>
		/// Gets a reference to the <see cref="Service" /> component.
		/// </summary>
		public Service Service {
			get {
				m_Service = m_Service ?? GetComponentInChildren<Service>();
				return m_Service;
			}
		}
		
		public ServiceOptions ServiceOptions {
			get { return PlatformUtil.GetSettings<ServiceOptions>(); }
		}
		
		/// <summary>
		/// Gets and sets the current state.
		/// </summary>
		public ControllerState State {
			get; protected set;
		}
		
		#endregion
		
		
		#region Events
		
		public event System.Action OnWebViewWillAppear;
		
		public event System.Action OnWebViewDidDisappear;
		
		#endregion
		
		
		#region MonoBehaviour Methods
		
		/// <see cref="MonoBehaviour.OnApplicationQuit" />
		public void OnApplicationQuit() {
			IsQuitting = true;
		}
		
		/// <see cref="MonoBehaviour.OnEnable" />
		public void OnEnable() {
			
		}
		
		/// <see cref="MonoBehaviour.OnDisable" />
		public void OnDisable() {

		}

		public void InvokeOnWebViewWillAppear(string data) {
			Debug.Log("Received a call for InvokeOnWebViewWillAppear. " + data);
			this.Native.isWebViewVisible = true;
			if (OnWebViewWillAppear != null) {
				OnWebViewWillAppear();
			}
		}

		public void InvokeOnWebViewDidDisappear(string data) {
			Debug.Log("Received a call for InvokeOnWebViewDidDisappear. " + data);
			if (ShouldReloadPromoAfterWebViewClose || data == "error" ) {
				ShouldReloadPromoAfterWebViewClose = false;
				ReloadPromoContent(true);
			}
			this.Native.isWebViewVisible = false;
			if (OnWebViewDidDisappear != null) {
				OnWebViewDidDisappear();
			}

			#if !UNITY_EDITOR && !UNITY_WSA
				IsWebViewLoading = false;
				SagoTouch.TouchDispatcher.Instance.enabled = true;
			#endif
		}
		#endregion
		
		
		#region Event Methods
		
		bool IsExpired {
			get {
				float then = Service.Timestamp;
				float now = Time.realtimeSinceStartup;
				float elapsed = now - then;
				float duration = 120; // Time in seconds
				return elapsed > duration;
			}
		}

		bool ShouldReloadPromoAfterWebViewClose { get; set; }

		void ReloadPromoContent(bool forceReload = false) {
			if (this.State == ControllerState.SceneWillAppear || this.State == ControllerState.SceneDidAppear) {
				if (IsExpired || forceReload) {
					this.Promo.Clear();
					this.Service.Clear();
					#if !UNITY_TVOS
						this.Service.Load();
					#endif
				} else {
					TurnPromoContentOn(true);
				}
			} else {
				TurnPromoContentOff(false);
			}
		}

		void OnApplicationPause(bool paused) {
			if (this.State == ControllerState.SceneWillAppear || this.State == ControllerState.SceneDidAppear) {
				if (paused) {
					Debug.Log("-> Paused unity application");

					if (!this.Native.isWebViewVisible) {
						Debug.Log("-> Turning promo content off");
						TurnPromoContentOff(false);
					}
				} else {
					Debug.Log("-> Unpaused unity application");
					if (this.Native.isWebViewVisible) {
						ShouldReloadPromoAfterWebViewClose = true;
					} else {
						ReloadPromoContent();
					}
				}
			}
		}

		/// <summary>
		/// Event handler called when the app starts.
		/// </summary>
		public void OnApplicationStart() {
			if (this.State == ControllerState.Unknown) {

				#if SAGO_BIZ_DEBUG
					Controller.Instance.Native.EnableDebugMode();
				#endif

				Controller.Instance.Native.InitializeAppleSearchAd();

				Controller.Instance.Native.InitializeWebView();
				
				// parents
				this.Parents.Clear();
				this.Parents.OnComplete += this.OnParentsComplete;
				this.Parents.OnError += this.OnParentsError;
				this.Parents.Content.Button.OnTap += this.OnParentsTap;
				#if !UNITY_TVOS
					if (this.ParentsOptions.IsEnabled) {
						this.Parents.Load();
					}
				#endif
				
				// promo
				this.Promo.Clear();
				this.Promo.OnComplete += this.OnPromoComplete;
				this.Promo.OnError += this.OnPromoError;
				this.Promo.Content.Button.OnTap += this.OnPromoTap;
				
				// service
				this.Service.Clear();
				this.Service.OnComplete += this.OnServiceComplete;
				this.Service.OnError += this.OnServiceError;
				#if !UNITY_TVOS
					this.Service.Load();
				#endif
				
				// state
				this.State = ControllerState.Started;
				
			}
		}


		/// <summary>
		/// Event handler called before the app navigates to the title scene.
		/// </summary>
		public void OnSceneWillAppear(bool animated) {
			this.OnApplicationStart();
			this.State = ControllerState.SceneWillAppear;
		}
		
		/// <summary>
		/// Event handler called after the app navigates to the title scene.
		/// </summary>
		public void OnSceneDidAppear(bool animated) {
			this.OnApplicationStart();
			this.State = ControllerState.SceneDidAppear;
			if (this.ParentsOptions.IsEnabled) {
				TurnParentsContentOn(true);
			}
			ReloadPromoContent();
		}
		
		/// <summary>
		/// Event handler called before the app navigates away from the title scene.
		/// </summary>
		public void OnSceneWillDisappear(bool animated) {
			this.State = ControllerState.SceneWillDisappear;
			this.TurnParentsContentOff(false);
			this.TurnPromoContentOff(false);
		}
		
		/// <summary>
		/// Event handler called after the app navigates away from the title scene.
		/// </summary>
		public void OnSceneDidDisappear(bool animated) {
			this.State = ControllerState.SceneDidDisappear;
			this.TurnParentsContentOff(false);
			this.TurnPromoContentOff(false);
		}
	
		#endregion
		
		
		#region Internal Methods
		
		/// <summary>
		/// Event handler called when the user taps the <see cref="Parents" /> button.
		/// </summary>
		void OnParentsTap(Button button) {

			this.Analytics.TrackEvent(MixpanelConstants.EventForParentsClicksHomeScreenIcon);
			if (!string.IsNullOrEmpty(this.ParentsOptions.AppUrl)) {
				if (!IsWebViewLoading) {
					#if !UNITY_EDITOR && !UNITY_WSA
						IsWebViewLoading = true;
						SagoTouch.TouchDispatcher.Instance.enabled = false;
					#endif
					this.Native.OpenWebView(this.ParentsOptions.AppUrl);
				}
			} else {
				Debug.LogError("For parents url is null");
			}
		}
		
		/// <summary>
		/// Event handler called when the <see cref="Parents" /> component finishes loading.
		/// </summary>
		void OnParentsComplete(Parents parents) {
			this.TurnParentsContentOn(true);
		}
		
		/// <summary>
		/// Event handler called when the <see cref="Parents" /> component has an error while loading.
		/// </summary>
		void OnParentsError(Parents parents) {
			Debug.Log(parents.Error);
			this.TurnParentsContentOff(false);				
		}
		
		/// <summary>
		/// Event handler called when the user taps the <see cref="Promo" /> button. 
		/// </summary>
		void OnPromoTap(Button button) {
			if (!IsWebViewLoading) {
				#if !UNITY_EDITOR
					IsWebViewLoading = true;
					SagoTouch.TouchDispatcher.Instance.enabled = false;
				#endif
				this.Analytics.TrackEvent(MixpanelConstants.EventXPromoClicksHomeScreenIcon);
				this.Native.OpenWebView(this.Service.PromoData.PageUrl);
			}
		}
		
		/// <summary>
		/// Event handler called when the <see cref="Promo" /> component finishes loading.
		/// </summary>
		void OnPromoComplete(Promo promo) {
			if (this.State != ControllerState.SceneWillDisappear && this.State != ControllerState.SceneDidDisappear) {
				this.TurnPromoContentOn(true);
			}
		}
		
		/// <summary>
		/// Event handler called when the <see cref="Promo" /> component has an error while loading.
		/// </summary>
		void OnPromoError(Promo promo) {
			Debug.Log(promo.Error);
			this.TurnPromoContentOff(false);
		}
		
		/// <summary>
		/// Event handler called when the <see cref="Service" /> component finishes loading.
		/// </summary>
		void OnServiceComplete(Service service) {
			// this.Parents.Load();
			this.Promo.Load();
		}
		
		/// <summary>
		/// Event handler called when the <see cref="Service" /> component has an error while loading.
		/// </summary>
		void OnServiceError(Service service) {
			Debug.Log(service.Error);
		}
		
		#endregion
				
		#region
		
		/// <summary>
		/// Turns the <see cref="Parents" /> content on.
		/// </summary>
		void TurnParentsContentOn(bool animated) {
			
			if (!this.ParentsEnabled) {
				this.Parents.TurnContentOff(false);
				return;
			}
			
			if (/*this.State == ControllerState.SceneWillAppear ||*/ this.State == ControllerState.SceneDidAppear) {
				if (this.Parents.State == ParentsState.Complete) {
					this.Parents.TurnContentOn(animated);
				}
			}
			
		}
		
		/// <summary>
		/// Turns the <see cref="Parents" /> content off.
		/// </summary>
		void TurnParentsContentOff(bool animated) {
			this.Parents.TurnContentOff(this.ParentsEnabled && animated);
		}
		
		/// <summary>
		/// Turns the <see cref="Promo" /> content on.
		/// </summary>
		void TurnPromoContentOn(bool animated) {
			
			if (!this.PromoEnabled) {
				this.Promo.TurnContentOff(false);
				return;
			}
			if (this.State == ControllerState.SceneDidAppear) {
				if (this.Promo.State == PromoState.Complete) {
					string tempJson = JsonConvert.SerializeObject(this.Service.PromoData, JsonConvert.Formatting.None, JsonConvert.Settings.CSharpToJson);
					var dictionary = JsonConvert.DeserializeObject<Dictionary<string,object>>(tempJson);
					this.Analytics.TrackEvent(MixpanelConstants.EventXPromoImpressionIconOnHomeScreen, dictionary);
					this.Promo.TurnContentOn(animated);
				}
			}
			
		}
		
		/// <summary>
		/// Turns the <see cref="Promo" /> content off.
		/// </summary>
		void TurnPromoContentOff(bool animated) {
			this.Promo.TurnContentOff(this.PromoEnabled && animated);
		}
		
		#endregion
		
		
	}
	
}