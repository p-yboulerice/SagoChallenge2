namespace SagoCore.AssetBundles {
	
	using System.Collections;
	using System.IO;
	using UnityEngine;
	
	/// <summary>
	/// The ServerAdaptorOptions struct contains the metadata required to load asset bundles from an asset bundle server.
	/// </summary>
	[System.Serializable]
	public struct ServerAdaptorOptions {
		
		
		#region Fields
		
		/// <summary>
		/// The build token.
		/// </summary>
		/// <remarks>
		/// When a project is built and deployed using an asset bundle server, the 
		/// asset bundles will have to be uploaded to an build-specific folder. I'm 
		/// not sure how that's going to be implemented, but we'll need to get the token
		/// that identifies the folder and include it in server adaptor options so the 
		/// app knows which folder to load the asset bundles from.
		/// </remarks>
		[SerializeField]
		public string Token;
		
		/// <summary>
		/// The server url.
		/// </summary>
		[SerializeField]
		public string Url;
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Gets the url for the specified asset bundle.
		/// </summary>
		/// <param name="assetBundleName">
		/// The name of the asset bundle.
		/// </param>
		public string GetAssetBundleUrl(string assetBundleName) {
			return string.Format("{0}/{1}/{2}.unity", Url, Token, assetBundleName);
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The ServerAdaptor class loads an asset bundle from an asset bundle server.
	/// </summary>
	public class ServerAdaptor : CustomYieldInstruction, IAssetBundleAdaptor {
		
		
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
		/// Creates a new <see cref="ServerAdaptor" /> instance.
		/// </summary>
		/// <param name="assetBundleName">
		/// The asset bundle name.
		/// </param>
		/// <param name="options">
		/// The options the adaptor will use to connect to the server.
		/// </param>
		public ServerAdaptor(string assetBundleName, ServerAdaptorOptions options) {
			if (AssetBundleLoader.Instance) {
				m_Coroutine = ServerAdaptorImpl(assetBundleName, options);
				AssetBundleLoader.Instance.StartCoroutine(m_Coroutine);
			}
		}
		
		/// <summary>
		/// Loads the asset bundle.
		/// </summary>
		private IEnumerator ServerAdaptorImpl(string assetBundleName, ServerAdaptorOptions options) {
			
			if (string.IsNullOrEmpty(assetBundleName)) {
				throw new System.ArgumentNullException("assetBundleName");
			}
			
			if (string.IsNullOrEmpty(options.Token)) {
				throw new System.ArgumentNullException("options.Token");
			}
			
			if (string.IsNullOrEmpty(options.Url)) {
				throw new System.ArgumentNullException("options.Url");
			}
			
			m_AssetBundle = null;
			m_AssetBundleName = assetBundleName;
			m_Error = null;
			m_KeepWaiting = true;
			m_WWW = WWW.LoadFromCacheOrDownload(options.GetAssetBundleUrl(assetBundleName), 0);
			
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