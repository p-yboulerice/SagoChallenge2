namespace SagoCore.AssetBundles {
	
	using UnityEngine;
	
	/// <summary>
	/// The AssetBundleLoaderRequest class provides a wrapper around a 
	/// <see cref="AssetBundleReference" /> object to allow multiple 
	/// coroutines to wait while an asset bundle is loading.
	/// </summary>
	
	/// <example>
	/// <para>
	/// IMPORTANT: You must call <see cref="Dispose" /> on the request when it's no 
	/// longer in use, so that it can release any unmanaged resources (like instances 
	/// of the WWW class). You should call <see cref="Dispose" /> as soon as possible 
	/// to minimize the application's the memory footprint.
	/// </para>
	/// <code>
	/// private IEnumerator Example() {
	/// 	var request = new AssetBundleLoaderRequest("my_asset_bundle");
	/// 	yield return request;
	/// 	Debug.Log(request.assetBundle);
	/// 	request.Dispose();
	/// }
	/// </code>
	/// </example>
	
	/// <example>
	/// <para>
	/// The following code yields on the same <see cref="AssetBundleReference" /> 
	/// object twice and will throw errors.
	/// </para>
	/// <code>
	/// private IEnumerator Example() {
	/// 	yield return AssetBundleReference.FindOrCreateReference("my_asset_bundle");
	/// 	yield return AssetBundleReference.FindOrCreateReference("my_asset_bundle");
	/// }
	/// </code>
	/// <para>
	/// The following code yields on two different <see cref="AssetBundleLoaderRequest" /> 
	/// objects (which use the same <see cref="AssetBundleReference" /> object) and will 
	/// not throw any errors.
	/// </para>
	/// <code>
	/// private IEnumerator Example() {
	/// 	yield return new AssetBundleLoaderRequest("my_asset_bundle");
	/// 	yield return new AssetBundleLoaderRequest("my_asset_bundle");
	/// }
	/// </code>
	/// </example>
	
	public class AssetBundleLoaderRequest : CustomYieldInstruction, System.IDisposable {
		
		
		#region Fields
		
		/// <summary>
		/// The asset bundle reference.
		/// </summary>
		private AssetBundleReference m_AssetBundleReference;
		
		#endregion
		
		
		#region Properties
		
		/// <example>
		/// <see cref="AssetBundleReference.assetBundle" />
		/// </example>
		public AssetBundle assetBundle {
			get { return m_AssetBundleReference != null ? m_AssetBundleReference.assetBundle : null; }
		}
		
		/// <example>
		/// <see cref="AssetBundleReference.error" />
		/// </example>
		public string error {
			get { return m_AssetBundleReference != null ? m_AssetBundleReference.error : null; }
		}
		
		/// <example>
		/// <see cref="AssetBundleReference.isDone" />
		/// </example>
		public bool isDone {
			get { return !keepWaiting; }
		}
		
		/// <example>
		/// <see cref="AssetBundleReference.keepWaiting" />
		/// </example>
		override public bool keepWaiting {
			get { return m_AssetBundleReference != null ? m_AssetBundleReference.keepWaiting : false; }
		}
		
		/// <example>
		/// <see cref="AssetBundleReference.progress" />
		/// </example>
		public float progress {
			get { return m_AssetBundleReference != null ? m_AssetBundleReference.progress : 0; }
		}
		
		#endregion
		
		
		#region Constructor
		
		/// <example>
		/// Creates a new AssetBundleLoaderRequest instance.
		/// </example>
		/// <param name="assetBundleName">
		/// The name of the asset bundle to load.
		/// </param>
		public AssetBundleLoaderRequest(string assetBundleName) {
			
			AssetBundleReference assetBundleReference;
			assetBundleReference = AssetBundleReference.FindOrCreateReference(assetBundleName);
			
			if (assetBundleReference != null) {
				m_AssetBundleReference = assetBundleReference;
				m_AssetBundleReference.Retain();
			}
			
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Disposes of the request.
		/// </summary>
		public void Dispose() {
			if (m_AssetBundleReference != null) {
				m_AssetBundleReference.Release();
				m_AssetBundleReference = null;
			}
		}
		
		#endregion
		
		
	}
	
}