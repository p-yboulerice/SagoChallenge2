namespace SagoCore.AssetBundles {
	
	using System.Collections;
	using UnityEngine;
	using UnityEngine.Networking;
	
	#if UNITY_IOS || UNITY_TVOS
	using UnityEngine.iOS;
	#endif
	
	/// <summary>
	/// The OnDemandResourcesAdaptor class loads an asset bundle using Apple's on demand resources.
	/// </summary>
	public class OnDemandResourcesAdaptor : CustomYieldInstruction, IAssetBundleAdaptor {
		
		
		#region Fields
		
		/// <summary>
		/// The asset bundle.
		/// </summary>
		private AssetBundle m_AssetBundle;
		
		/// <summary>
		/// The asset bundle name.
		/// </summary>
		private string m_AssetBundleName;
		
		/// <summary>
		/// The coroutine.
		/// </summary>
		private IEnumerator m_Coroutine;
		
		/// <summary>
		/// The error that occured while loading the asset bundle.
		/// </summary>
		private string m_Error;
		
		/// <summary>
		/// The flag that indicates whether the asset bundles has finished loading or an error has occurred.
		/// </summary>
		private bool m_KeepWaiting;
		
		/// <summary>
		/// The flag that indicates whether the adaptor should load the asset bundle into memory when the download is complete.
		/// </summary>
		private bool m_DownloadOnly;
		
		#if UNITY_IOS || UNITY_TVOS
		
		/// <summary>
		/// The on demand resources request.
		/// </summary>
		private OnDemandResourcesRequest m_OnDemandResourcesRequest;
		
		#endif
		
		/// <summary>
		/// The progress of the load operation.
		/// </summary>
		private float m_Progress;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// <see cref="IAssetBundleAdaptor.assetBundle" />
		/// </summary>
		public AssetBundle assetBundle {
			get { return m_AssetBundle; }
		}
		
		/// <summary>
		/// <see cref="IAssetBundleAdaptor.assetBundleName" />
		/// </summary>
		public string assetBundleName {
			get { return m_AssetBundleName; }
		}
		
		/// <summary>
		/// <see cref="IAssetBundleAdaptor.error" />
		/// </summary>
		public string error {
			get { return m_Error; }
		}
		
		/// <summary>
		/// <see cref="IAssetBundleAdaptor.isDone" />
		/// </summary>
		public bool isDone {
			get { return !keepWaiting; }
		}
		
		/// <summary>
		/// <see cref="IAssetBundleAdaptor.keepWaiting" />
		/// </summary>
		override public bool keepWaiting {
			get { return m_KeepWaiting; }
		}
		
		/// <summary>
		/// <see cref="IAssetBundleAdaptor.progress" />
		/// </summary>
		public float progress {
			get { return m_Progress; }
		}
		
		#endregion
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new <see cref="OnDemandResourcesAdaptor" /> instance.
		/// </summary>
		public OnDemandResourcesAdaptor(string assetBundleName, bool downloadOnly = false, System.Func<string, string> getAssetTagFunc = null) {
			m_DownloadOnly = downloadOnly;

			if (AssetBundleLoader.Instance) {
				m_Coroutine = OnDemandResourcesAdaptorImpl(assetBundleName, getAssetTagFunc);
				AssetBundleLoader.Instance.StartCoroutine(m_Coroutine);

				// We are now disabling this coroutine because China users have issues with 204 checks when they are on VPN.
				// Doing this check will prevents these China users from downloading any ODR contents.
				//AssetBundleLoader.Instance.StartCoroutine(Check204ReachabilityCoroutine(5.0f));

				// Starting additional coroutine that runs asynchronously to continuously check internet reachability.
				AssetBundleLoader.Instance.StartCoroutine(CheckInternetReachabilityCoroutine(5.0f));
			}
		}

		#endregion


		#region Coroutine
		
		/// <summary>
		/// Loads the asset bundle.
		/// </summary>
		private IEnumerator OnDemandResourcesAdaptorImpl(string assetBundleName, System.Func<string, string> getAssetTagFunc) {
			
			if (string.IsNullOrEmpty(assetBundleName)) {
				throw new System.ArgumentNullException("assetBundleName");
			}
			
			m_AssetBundle = null;
			m_AssetBundleName = assetBundleName;
			m_Error = null;
			m_KeepWaiting = true;
			
			#if UNITY_IOS || UNITY_TVOS
				
				string assetTag;
				if (getAssetTagFunc != null) {
					assetTag = getAssetTagFunc(assetBundleName);
				} else {
					assetTag = assetBundleName;
				}

				Debug.LogFormat("[OnDemandResourcesAdaptor] Requesting ODR Tag : {0}", assetTag);

				m_OnDemandResourcesRequest = OnDemandResources.PreloadAsync(new string[]{ assetTag });
				while (!m_OnDemandResourcesRequest.isDone) {
					m_Progress = m_OnDemandResourcesRequest.progress;
					if (!string.IsNullOrEmpty(m_Error)) {
						m_AssetBundle = null;
						m_KeepWaiting = false;
						m_OnDemandResourcesRequest.Dispose();
						m_OnDemandResourcesRequest = null;
						m_Progress = -1;
						yield break;
					}
					yield return null;
				}

				if (!string.IsNullOrEmpty(m_OnDemandResourcesRequest.error)) {
					m_AssetBundle = null;
					m_Error = m_OnDemandResourcesRequest.error;
					m_KeepWaiting = false;
					m_OnDemandResourcesRequest.Dispose();
					m_OnDemandResourcesRequest = null;
					m_Progress = -1;
					yield break;
				}
				
				// TODO:
				// The documentation isn't very detailed, but it seems like GetResourcePath 
				// should return the path to the asset bundle. As far as I can tell, it 
				// always returns null. In Unity's demo project, they just prefix the asset 
				// bundle name with "res://" (and don't use GetResourcePath).
				
				string assetBundlePath;
				// assetBundlePath = m_OnDemandResourcesRequest.GetResourcePath(assetBundleName);
				assetBundlePath = "res://" + assetBundleName;
				
				if (string.IsNullOrEmpty(assetBundlePath)) {
					m_AssetBundle = null;
					m_Error = string.Format("Could not get resource path for asset bundle: {0}", assetBundleName);
					m_KeepWaiting = false;
					m_OnDemandResourcesRequest.Dispose();
					m_OnDemandResourcesRequest = null;
					m_Progress = -1;
					yield break;
				}

				// Skip any loading from the adaptor.
				if (m_DownloadOnly) {
					Debug.LogFormat("[OnDemandResourcesAdaptor] Bundle loading skipped : {0}", assetBundleName);
					m_Error = null;
					m_KeepWaiting = false;
					m_Progress = 1;
					yield break;
				}
				
				Debug.LogFormat("[OnDemandResourcesAdaptor] Load asset bundle : {0}", assetBundleName);
				AssetBundleCreateRequest assetBundleCreateRequest;
				assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
				yield return assetBundleCreateRequest;
				
				if (assetBundleCreateRequest.assetBundle == null) {
					m_AssetBundle = null;
					m_Error = string.Format("Could not load asset bundle: {0}", assetBundleName);
					m_KeepWaiting = false;
					m_OnDemandResourcesRequest.Dispose();
					m_OnDemandResourcesRequest = null;
					m_Progress = -1;
					yield break;
				}
				
				m_AssetBundle = assetBundleCreateRequest.assetBundle;
				m_Error = null;
				m_KeepWaiting = false;
				m_Progress = 1;
				yield break;
				
			#else
				
				if (m_DownloadOnly) {
					// suppress unused variable warning on non-iOS platforms
				}
				
				m_Error = string.Format("OnDemandResources is not available on the current platform: {0}", Application.platform);
				m_KeepWaiting = false;
				m_Progress = -1;
				yield break;
				
			#endif
			
		}

		private IEnumerator Check204ReachabilityCoroutine(float interval) {
			#if UNITY_IOS || UNITY_TVOS

			while (m_OnDemandResourcesRequest != null && !m_OnDemandResourcesRequest.isDone) {
				yield return new WaitForSeconds(interval);
				using (UnityWebRequest www = UnityWebRequest.Get("https://www.google.com/generate_204")) {
					yield return www.SendWebRequest();
					// If 204 test case fails and the app seems to have no access to the internet
					if (www.responseCode != 204) {
						if (m_OnDemandResourcesRequest != null && !m_OnDemandResourcesRequest.isDone) {
							m_Error = AssetBundleAdaptorError.NoInternet;
						}
					}
				}
			}

			#else

			yield return null;

			#endif
		}

		private IEnumerator CheckInternetReachabilityCoroutine(float interval) {
			#if UNITY_IOS || UNITY_TVOS

			while (m_OnDemandResourcesRequest != null && !m_OnDemandResourcesRequest.isDone) {
				yield return new WaitForSeconds(interval);

				// If internet is not reachable
				if (Application.internetReachability == NetworkReachability.NotReachable && m_OnDemandResourcesRequest != null && !m_OnDemandResourcesRequest.isDone) {
					this.m_Error = AssetBundleAdaptorError.NoInternet;
				}
			}

			#else

			yield return null;

			#endif
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Disposes of the adaptor.
		/// </summary>
		public void Dispose() {
			
			Debug.Log("OnDemandResourcesAdaptor.Dispose()");
			
			#if UNITY_IOS || UNITY_TVOS
				
				if (m_OnDemandResourcesRequest != null) {
					m_OnDemandResourcesRequest.Dispose();
					m_OnDemandResourcesRequest = null;
				}

			#endif
			
			if (m_Coroutine != null) {
				if (AssetBundleLoader.Instance) {
					AssetBundleLoader.Instance.StopCoroutine(m_Coroutine);
				}
				m_Coroutine = null;
			}
				
			m_AssetBundle = null;
			m_AssetBundleName = null;
			m_Error = null;
			m_KeepWaiting = false;
			
		}
		
		#endregion
		
		
	}
	
}
