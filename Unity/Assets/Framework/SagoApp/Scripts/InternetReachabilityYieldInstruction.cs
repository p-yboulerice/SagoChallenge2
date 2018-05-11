namespace SagoApp.Project {
	
	using UnityEngine;
	using UnityEngine.Networking;
	using System.Collections;
	
	/// <summary>
	/// Custom yield instruction class that determines the state of internet reachability for users.
	/// </summary>
	public class InternetReachabilityYieldInstruction : CustomYieldInstruction, System.IDisposable {

		#region Fields

		private ProjectNavigatorError m_Error;

		private bool m_IsInternetReachable;

		private bool m_IsDone;

		private MonoBehaviour m_MonoBehaviour;

		private IEnumerator m_Coroutine;

		#endregion


		#region Properties

		public ProjectNavigatorError Error {
			get { return m_Error; }
		}

		public bool IsInternetReachable {
			get { return m_IsInternetReachable; }
		}

		public bool IsDone {
			get { return m_IsDone; }
		}

		public override bool keepWaiting
		{
			get {
				return !m_IsDone;
			}
		}

		#endregion


		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="SagoApp.Project.InternetReachabilityYieldInstruction"/> class.
		/// </summary>
		/// <param name="monoBehaviour">Mono behaviour object that will start an internal coroutine for testing internet reachability.</param>
		public InternetReachabilityYieldInstruction(MonoBehaviour monoBehaviour) {
			m_MonoBehaviour = null;
			m_Coroutine = null;
			m_IsDone = false;
			if (monoBehaviour != null) {
				m_MonoBehaviour = monoBehaviour;
				m_Coroutine = ReachabilityTestCoroutine();
				m_MonoBehaviour.StartCoroutine(m_Coroutine);
			} else {
				m_IsDone = true;
				Debug.LogError("InternetReachabilityYieldInstruction-> MonoBehaviour reference passed is null.", DebugContext.SagoApp);
			}
		}

		#endregion


		#region Public Methods

		public void Dispose() {
			if (m_MonoBehaviour != null && m_Coroutine != null) {
				m_Error = ProjectNavigatorError.OdrLoadingCancelled;
				m_MonoBehaviour.StopCoroutine(m_Coroutine);
				Complete();
			}
		}

		#endregion


		#region Private Methods

		private void Complete() {
			this.m_IsDone = true;
			this.m_MonoBehaviour = null;
			this.m_Coroutine = null;
		}

		#endregion


		#region Coroutines

		private IEnumerator ReachabilityTestCoroutine() {
			// User data describing whether the app can use carrier data to download asset bundles or not
			// TO-DO: Use proper key for download setting or call boolean property getter in the case we are using a wrapper for this.
			bool downloadViaWifiOnly = ProjectNavigator.Instance ? ProjectNavigator.Instance.DownloadViaWiFiOnly : true;

			// List of all permutations
			// Wifi & 204 => ODR Downloadable
			// Wifi & !204 => No Internet
			// Carrier Data & 204 & Use Data => ODR Downloadable
			// Carrier Data & 204 & Don't Use Data => No Wifi
			// Carrier Data & !204 & Use Data => No Internet
			// Carrier Data & !204 & Don't Use Data => No Wifi
			// Not Reachable & 204 => No Internet
			// Not Reachable & !204 => No Internet

			// If internet is not reachable
			if (Application.internetReachability == NetworkReachability.NotReachable) {
				Debug.Log("ProjectNavigator-> Internet not reachable", DebugContext.SagoApp);
				this.m_Error = ProjectNavigatorError.OdrErrorNoInternet;
				m_IsInternetReachable = false;
				Complete();
				yield break;
			// If user has access to Wifi or carrier data (ex. 3G, 4G, or LTE)
			} else {
				// If user only has access to carrier Data and user said not to use carrier data so bypassing 204 test
				if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork && downloadViaWifiOnly) {
					Debug.Log("ProjectNavigator-> There is no access to Wifi", DebugContext.SagoApp);
					this.m_Error = ProjectNavigatorError.OdrErrorNoWiFi;
					m_IsInternetReachable = false;
					Complete();
					yield break;
				}

				// We've decided to remove 204 check since any VPNs are blocked in China and
				// this was blocking us from release World in China.

//				// Using web request to see if we actually have access to internet even though Wifi or Carrier Data is available
//				// Ex. Like in the case where a user is at an airport and have Wifi turned on but haven't got access to the internet
//				using (UnityWebRequest www = UnityWebRequest.Get("https://www.google.com/generate_204")) {
//					yield return www.Send();
//					// If 204 test case fails and the app seems to have no access to the internet
//					if (www.responseCode != 204) {
//						Debug.Log("ProjectNavigator-> 204 Test Failed. Internet not reachable.", DebugContext.SagoApp);
//						this.m_Error = ProjectNavigatorError.OdrErrorNoInternet;
//						Complete();
//						m_IsInternetReachable = false;
//						yield break;
//					}
//				}
			}

			m_IsInternetReachable = true;
			Complete();
		}

		#endregion
		
	}
	
}