namespace SagoBiz {
	
	using System.Collections.Generic;
	using UnityEngine;
	using Debug = SagoBiz.DebugUtil;

	/// <summary>
	/// The Facade class provides a simple interface for the application to 
	/// call event handlers on the <see cref="Controller" /> singleton.
	/// </summary>
	public class Facade {
		
		#region Static Fields
		
		#if !SAGO_DISABLE_SAGOBIZ
		private static object Lock = new object();
		#endif
		
		#endregion


		#region Static Properties

		public static System.Action<System.Action<object>> FetchAppDataDelegate {
			get {
				return Service.FetchAppDataDelegate;
			}

			set {
				Service.FetchAppDataDelegate = value;
			}
		}

		#endregion
		
		
		#region Static Methods
		
		/// <see cref="Controller.OnApplicationStart" />
		public static void OnApplicationStart() {
			#if !SAGO_DISABLE_SAGOBIZ
			if (Controller.Instance) {
				Controller.Instance.OnApplicationStart();
			}
			#endif
		}
		
		/// <see cref="Controller.OnSceneWillAppear" />
		public static void OnSceneWillAppear(bool animated) {
			#if !SAGO_DISABLE_SAGOBIZ
			if (Controller.Instance) {
				Controller.Instance.OnSceneWillAppear(animated);
			}
			#endif
		}
		
		/// <see cref="Controller.OnSceneDidAppear" />
		public static void OnSceneDidAppear(bool animated) {
			#if !SAGO_DISABLE_SAGOBIZ
			if (Controller.Instance) {
				Controller.Instance.OnSceneDidAppear(animated);
			}
			#endif
		}
		
		/// <see cref="Controller.OnSceneWillDisappear" />
		public static void OnSceneWillDisappear(bool animated) {
			#if !SAGO_DISABLE_SAGOBIZ
			if (Controller.Instance) {
				Controller.Instance.OnSceneWillDisappear(animated);
			}
			#endif
		}
		
		/// <see cref="Controller.OnSceneDidDisappear" />
		public static void OnSceneDidDisappear(bool animated) {
			#if !SAGO_DISABLE_SAGOBIZ
			if (Controller.Instance) {
				Controller.Instance.OnSceneDidDisappear(animated);
			}
			#endif
		}
		
		/// <see cref="Controller.OnWebViewWillAppear" />
		public static event System.Action OnWebViewWillAppear {
			add {
				#if !SAGO_DISABLE_SAGOBIZ
				if (Controller.Instance) {
					lock (Lock) {
						Controller.Instance.OnWebViewWillAppear += value;
					}
				}
				#endif
			}
			remove {
				#if !SAGO_DISABLE_SAGOBIZ
				if (Controller.Instance) {
					lock (Lock) {
						Controller.Instance.OnWebViewWillAppear -= value;
					}
				}
				#endif
			}
		}
		
		/// <see cref="Controller.OnWebViewDidDisappear" />
		public static event System.Action OnWebViewDidDisappear {
			add {
				#if !SAGO_DISABLE_SAGOBIZ
				if (Controller.Instance) {
					lock (Lock) {
						Controller.Instance.OnWebViewDidDisappear += value;
					}
				}
				#endif
			}
			remove {
				#if !SAGO_DISABLE_SAGOBIZ
				if (Controller.Instance) {
					lock (Lock) {
						Controller.Instance.OnWebViewDidDisappear -= value;
					}
				}
				#endif
			}
		}
		
		/// <see cref="Analytics.TrackEvent" />
		public static void TrackEvent(string eventName) {
			#if !SAGO_DISABLE_SAGOBIZ
			if (Controller.Instance && Controller.Instance.Analytics) {
				Controller.Instance.Analytics.TrackEvent(eventName);
			}
			#endif
		}
		
		/// <see cref="Analytics.TrackEvent" />
		public static void TrackEvent(string eventName, Dictionary<string,object> eventInfo) {
			#if !SAGO_DISABLE_SAGOBIZ
			if (Controller.Instance && Controller.Instance.Analytics) {
				Controller.Instance.Analytics.TrackEvent(eventName, eventInfo);
			}
			#endif
		}

		/// <see cref="Analytics.TimeEvent" />
		public static void TimeEvent(string eventName) {
			#if !SAGO_DISABLE_SAGOBIZ
			if (Controller.Instance && Controller.Instance.Analytics) {
				Controller.Instance.Analytics.TimeEvent(eventName);
			}
			#endif
		}

		/// <see cref="Analytics.SaveEventInfoToNative" />
		public static void SaveEventInfoToNative(string eventName, Dictionary<string, object> eventInfo) {
			#if !SAGO_DISABLE_SAGOBIZ
			if (Controller.Instance && Controller.Instance.Analytics) {
				Controller.Instance.Analytics.SaveEventInfoToNative(eventName, eventInfo);
			}
			#endif
		}

		public static void TrackEventWithAdjust(string eventName) {
			#if !SAGO_DISABLE_SAGOBIZ
			AdjustOptions adjOptions = SagoPlatform.PlatformUtil.GetSettings<AdjustOptions>();
			if (Controller.Instance && adjOptions) {
				string token = adjOptions.GetAdjustEventToken(eventName);
				TrackEventWithAdjustUsingToken(token);
			}
			#endif
		}

		/// <see cref="Analytics.TrackAdjustEvent" />
		public static void TrackEventWithAdjustUsingToken(string eventToken) {
			#if !SAGO_DISABLE_SAGOBIZ
			if (Controller.Instance && Controller.Instance.Analytics) {
				Controller.Instance.Analytics.TrackAdjustEvent(eventToken);
			}
			#endif
		}
		
		#endregion
		
		
	}
	
}