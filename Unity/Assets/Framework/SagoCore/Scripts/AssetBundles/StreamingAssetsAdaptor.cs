namespace SagoCore.AssetBundles {
	
	using System.Collections;
	using System.IO;
	using UnityEngine;
	
	/// <summary>
	/// The StreamingAssetsAdaptorOptions struct contains the metadata required to load asset bundles from the streaming assets folder.
	/// </summary>
	[System.Serializable]
	public struct StreamingAssetsAdaptorOptions {
		
		
		#region Fields
		
		/// <summary>
		/// The path to the streaming assets folder.
		/// </summary>
		[SerializeField]
		public string StreamingAssetsPath;
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Gets the path to the asset bundle.
		/// </summary>
		/// <param name="assetBundleName">
		/// The name of the asset bundle.
		/// </param>
		public string GetAssetBundlePath(string assetBundleName) {
			return Path.Combine(StreamingAssetsPath, assetBundleName);
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The StreamingAssetsAdaptor class loads an asset bundle from the streaming assets folder.
	/// </summary>
	public class StreamingAssetsAdaptor : CustomYieldInstruction, IAssetBundleAdaptor {
		
		
		#region Fields
		
		/// <summary>
		/// The asset bundle.
		/// </summary>
		private AssetBundle m_AssetBundle;
		
		/// <summary>
		/// The asset bundle create request;
		/// </summary>
		private AssetBundleCreateRequest m_AssetBundleCreateRequest;
		
		/// <summary>
		/// The asset bundle name.
		/// </summary>
		private string m_AssetBundleName;
		
		/// <summary>
		/// The coroutine.
		/// </summary>
		private IEnumerator m_Coroutine;
		
		/// <summary>
		/// The error that occurred while loading the asset bundle.
		/// </summary>
		private string m_Error;
		
		/// <summary>
		/// The flag that indicates whether the asset bundle has finished loading or an error has occurred.
		/// </summary>
		private bool m_KeepWaiting;
		
		/// <summary>
		/// The progress of the load operation.
		/// </summary>
		private float m_Progress;
		
		/// <summary>
		/// The www object.
		/// </summary>
		private WWW m_WWW;

		private bool m_DownloadOnly;
		
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
		/// Creates a new <see cref="StreamingAssetsAdaptor" /> instance.
		/// </summary>
		public StreamingAssetsAdaptor(string assetBundleName, StreamingAssetsAdaptorOptions options, bool downloadOnly = false) {
			m_DownloadOnly = downloadOnly;

			if (AssetBundleLoader.Instance) {
				m_Coroutine = StreamingAssetsAdaptorImpl(assetBundleName, options);
				AssetBundleLoader.Instance.StartCoroutine(m_Coroutine);
			}
		}
		
		/// <summary>
		/// Loads the asset bundle.
		/// </summary>
		private IEnumerator StreamingAssetsAdaptorImpl(string assetBundleName, StreamingAssetsAdaptorOptions options) {
			
			if (string.IsNullOrEmpty(assetBundleName)) {
				throw new System.ArgumentNullException("assetBundleName");
			}
			if (string.IsNullOrEmpty(options.StreamingAssetsPath)) {
				throw new System.ArgumentNullException("options.StreamingAssetsPath");
			}
			
			m_AssetBundle = null;
			m_AssetBundleName = assetBundleName;
			m_Error = null;
			m_KeepWaiting = true;

			// Skip any loading from the adaptor.
			if (m_DownloadOnly) {
				Debug.LogFormat("[StreamingAssetsAdaptor] Bundle loading skipped : {0}", assetBundleName);
				m_KeepWaiting = false;
				m_Progress = 1;
				yield break;
			}

			// use AssetBundle.LoadFromFileAsync instead of WWW on iOS and tvOS

#if UNITY_IOS || UNITY_TVOS

				Debug.LogFormat("[StreamingAssetsAdaptor] Load asset bundle : {0}", assetBundleName);
				m_AssetBundleCreateRequest = AssetBundle.LoadFromFileAsync(options.GetAssetBundlePath(assetBundleName));
				while (!m_AssetBundleCreateRequest.isDone) {
					m_Progress = m_AssetBundleCreateRequest.progress;
					yield return null;
				}
				
				if (m_AssetBundleCreateRequest.assetBundle == null) {
					m_AssetBundle = null;
					m_AssetBundleCreateRequest = null;
					m_Error = string.Format("Could not load asset bundle from file: {0}", options.GetAssetBundlePath(assetBundleName));
					m_KeepWaiting = false;
					m_Progress = -1;
					yield break;
				}

				m_AssetBundle = m_AssetBundleCreateRequest.assetBundle;
				m_AssetBundleCreateRequest = null;
				m_Error = null;
				m_KeepWaiting = false;
				m_Progress = 1;
				yield break;
				
			#else
				
				#if UNITY_EDITOR
					// Getting streaming assets path when playing from the editor but have set "Platform" to Android derivative.
					m_WWW = new WWW("file://" + options.GetAssetBundlePath(assetBundleName));
				#else
					// Getting path to Android streaming assets when on an Android device.
					m_WWW = new WWW(options.GetAssetBundlePath(assetBundleName));
				#endif
				Debug.LogFormat("StreamingAssetsAdaptor-> StreamingAssetsAdaptorImpl: Getting asset bundle path from {0}", m_WWW.url);
				while (!m_WWW.isDone) {
					m_Progress = m_WWW.progress;
					yield return null;
				}
				
				if (!string.IsNullOrEmpty(m_WWW.error)) {
					m_AssetBundle = null;
					m_Error = m_WWW.error;
					m_KeepWaiting = false;
					m_Progress = -1;
					m_WWW.Dispose();
					m_WWW = null;
					yield break;
				}
				
				m_AssetBundle = m_WWW.assetBundle;
				m_Error = null;
				m_KeepWaiting = false;
				m_Progress = 1;
				m_WWW.Dispose();
				m_WWW = null;
				yield break;
				
			#endif
			
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Disposes of the adaptor.
		/// </summary>
		public void Dispose() {
			
			if (m_Coroutine != null) {
				if (AssetBundleLoader.Instance) {
					AssetBundleLoader.Instance.StopCoroutine(m_Coroutine);
				}
				m_Coroutine = null;
			}
			
			if (m_AssetBundleCreateRequest != null) {
				m_AssetBundleCreateRequest = null;
			}
			
			if (m_WWW != null) {
				m_WWW.Dispose();
				m_WWW = null;
			}
			
			m_AssetBundle = null;
			m_AssetBundleName = null;
			m_Error = null;
			m_KeepWaiting = false;
			
		}
		
		#endregion
		
		
	}
	
}