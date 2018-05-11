namespace SagoApp.Analytics {
	
	using SagoNavigation;
	using SagoCore.Resources;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	
	public class AnalyticsController : MonoBehaviour {

		#region Static Fields

		public const string APP_STARTED = "App Started";
		public const string APP_EXIT = "App Exit";

		/// <summary>
		/// The player prefs key that stores the time the app was installed and launched for the first time.
		/// </summary>
		private static string PlayerPrefsAppInstallTimeKey = "appInstallTime";

		/// <summary>
		/// The player prefs' key that stores the number of launches.
		/// </summary>
		private static string PlayerPrefsNumberOfLaunchesKey = "numberOfLaunches";

		/// <summary>
		/// The mix panel property number of launches key.
		/// </summary>
		private static string MixPanelPropertyNumberOfLaunchesCountKey = "count";

		#endregion


		#region Static Properties

		/// <summary>
		/// Gets the time when the app was installed and launched for the first time.
		/// </summary>
		/// <value>The app install time.</value>
		public static int AppInstallTime {
			get {
				int installTime = PlayerPrefs.GetInt(PlayerPrefsAppInstallTimeKey, 0);
				if (installTime == 0) {
					installTime = (int)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
					PlayerPrefs.SetInt(PlayerPrefsAppInstallTimeKey, installTime);
					return installTime;
				}
				return installTime;
			}
		}

		/// <summary>
		/// Gets the app launch counter from the PlayerPrefs.
		/// </summary>
		/// <value>The app launch counter.</value>
		public static int AppLaunchCounter {
			get {
				return PlayerPrefs.GetInt(PlayerPrefsNumberOfLaunchesKey, 0);
			}
			private set {
				PlayerPrefs.SetInt(PlayerPrefsNumberOfLaunchesKey, value);
				PlayerPrefs.Save();
			}
		}

		#endregion


		#region Static Methods

		static public void Init() {
			CreateInstance();
			TrackEventAppStarted();
			TimeEventAppExit();
		}
		
		static public void ClearPlayerPrefs() {
			PlayerPrefs.DeleteKey(PlayerPrefsNumberOfLaunchesKey);
		}
		
		static private void TrackEventAppStarted() {
			
			if (AppInstallTime == 0) {
				// Accessing AppInstallTime to save to PlayerPrefs.
			}

			AppLaunchCounter++;

			SagoBiz.Facade.TrackEvent(APP_STARTED, new Dictionary<string, object> {
				{ MixPanelPropertyNumberOfLaunchesCountKey, AppLaunchCounter }
			});

		}

		static private void TimeEventAppExit() {
			SagoBiz.Facade.TimeEvent(APP_EXIT);
		}

		static private void CollectEventInfoFromAnalyticsObservers(string eventName, Dictionary<string, object> eventInfo) {

			List<IAnalyticsObserver> observers;
			observers = new List<IAnalyticsObserver>();

			foreach (IAnalyticsObserver o in Instance.GetComponentsInChildren(typeof(IAnalyticsObserver))) {
				observers.Add(o);
			}

			if (SceneNavigator.Instance && SceneNavigator.Instance.CurrentSceneController) {
				foreach (IAnalyticsObserver o in SceneNavigator.Instance.CurrentSceneController.GetComponentsInChildren(typeof(IAnalyticsObserver))) {
					observers.Add(o);
				}
			}

			foreach (IAnalyticsObserver observer in observers.OrderBy(o => o.Priority)) {
				observer.OnTrackEvent(eventName, eventInfo);
			}
		}

		static public void TrackEvent(string eventName, Dictionary<string, object> eventInfo) {
			
			CollectEventInfoFromAnalyticsObservers(eventName, eventInfo);
			SagoBiz.Facade.TrackEvent(eventName, eventInfo);
			PrintEvent(eventName, eventInfo);
			
		}

		#endregion


		#region Singleton Implementation

		static private AnalyticsController Instance {
			get {
				CreateInstance();
				return s_Instance;
			}
		}

		static private void CreateInstance() {
			if (!s_DidQuit && !s_Instance) {
				
				// guid: SagoApp/Resources/SagoApp/AnalyticsController.prefab
				
				ResourceReference reference;
				reference = ScriptableObject.CreateInstance<ResourceReference>();
				reference.Guid = "fde7cd87248144d369abeb5aef694ecc";

				ResourceReferenceLoaderRequest request;
				request = ResourceReferenceLoader.Load(reference, typeof(GameObject));
				
				if (!string.IsNullOrEmpty(request.error)) {
					Debug.LogError(request.error, DebugContext.SagoApp);
					return;
				}
				
				GameObject gameObject;
				gameObject = Instantiate(request.asset) as GameObject;
				gameObject.name = request.asset.name;
				DontDestroyOnLoad(gameObject);
				
				s_Instance = gameObject.GetComponent<AnalyticsController>();
				
			}
		}

		static private bool s_DidQuit;
		static private AnalyticsController s_Instance;

		#endregion


		#region MonoBehaviour

		private void OnApplicationPause(bool isPaused) {
			if (isPaused) {
				#if SAGO_IOS
					// The APP_EXIT event is logged natively using application lifecycle callbacks
					// to guarantee we only trigger the event when the customer has fully exited the
					// app. Unity's OnApplicationPause isn't enough because we can't tell the difference
					// between a full app exit or the case where the app is paused and then resumed
					// (eg: notifications).
					//
					// Since we store event info for APP_EXIT in Unity we collect it now and save
					// it to a native data structure. This allows the native logic to read the event data
					// if the user does end up exiting.
					Dictionary<string, object> appExitEventInfo = new Dictionary<string, object>();
					CollectEventInfoFromAnalyticsObservers(APP_EXIT, appExitEventInfo);
					SagoBiz.Facade.SaveEventInfoToNative(APP_EXIT, appExitEventInfo);
				#else
					TrackEvent(APP_EXIT, new Dictionary<string, object>());
				#endif
			} else {
				#if !SAGO_IOS
					TimeEventAppExit();
				#endif
			}
		}

		private void OnApplicationQuit() {
			s_DidQuit = true;
		}

		#endregion


		#region Helper

		static private void PrintEvent(string eventName, Dictionary<string, object> eventInfo) {
			Debug.Log(string.Format("Analytics - eventName: {0}", eventName), DebugContext.SagoApp);
			foreach (string key in eventInfo.Keys) {
				Debug.Log(string.Format("{0} : {1}", key, eventInfo[key].ToString()), DebugContext.SagoApp);
			}
		}

		#endregion


	}
		
}