namespace SagoCore.AssetBundles {
	
	using UnityEngine;
	
	/// <summary>
	/// The AssetBundleLoader provides an methods for loading asset bundles.
	/// </summary>
	
	/// <remarks>
	/// IMPORTANT: The AssetBundleLoader won't be able to load asset bundles until you've 
	/// assigned a method to <see cref="AssetBundleReference.CreateAdaptor" />.
	/// </remarks>
	
	/// <example>
	/// The following code shows how to load an asset bundle using the <see cref="AssetBundleLoader" />.
	/// <code>
	/// private IEnumerator Example(string assetBundleName) {
	/// 	
	/// 	var request = AssetBundleLoader.LoadAsync(assetBundleName);
	/// 	yield return request;
	/// 	
	/// 	if (!string.IsNullOrEmpty(request.error)) {
	/// 		Debug.Log(request.error);
	/// 	} else {
	/// 		Debug.Log(request.asset);
	/// 	}
	/// 	
	/// 	request.Dispose();
	/// 	
	/// }
	/// </code>
	/// </example>
	
	public class AssetBundleLoader : MonoBehaviour {
		
		
		#region Singleton
		
		/// <summary>
		/// The singleton instance.
		/// </summary>
		private static AssetBundleLoader _Instance;
		
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		/// <remarks>
		/// The AssetBundleLoader class is a <see cref="MonoBehaviour" /> 
		/// singleton to allow <see cref="AssetBundleReference" /> and 
		/// <see cref="IAssetBundleAdaptor" /> objects to run coroutines.
		/// </remarks>
		public static AssetBundleLoader Instance {
			get {
				if (!_Instance && !_IsQuitting) {
					_Instance = new GameObject("AssetBundleLoader").AddComponent<AssetBundleLoader>();
					DontDestroyOnLoad(_Instance);
				}
				return _Instance;
			}
		}
		
		/// <summary>
		/// The flag that indicates whether the application is quitting.
		/// </summary>
		private static bool _IsQuitting;
		
		/// <summary>
		/// Sets the is quitting flag when the application quits.
		/// </summary>
		private void OnApplicationQuit() {
			_IsQuitting = true;
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Creates a new <see cref="AssetBundleLoaderRequest" /> object.
		/// </summary>
		/// <param name="assetBundleName">
		/// The name of the asset bundle to load.
		/// </param>
		/// <remarks>
		/// <para>
		/// This method will always return a new <c>AssetBundleLoaderRequest</c> 
		/// object so that it's always safe to yield on (even if you call the 
		/// method with multiple times with the same asset bundle name).
		/// </para>
		/// <para>
		/// IMPORTANT: You must call <see cref="Dispose" /> on the request when it's no 
		/// longer in use, so that it can release any unmanaged resources (like instances 
		/// of the WWW class). You should call <see cref="Dispose" /> as soon as possible 
		/// to minimize the application's the memory footprint.
		/// </para>
		/// </remarks>
		public static AssetBundleLoaderRequest LoadAsync(string assetBundleName) {
			return new AssetBundleLoaderRequest(assetBundleName);
		}
		
		#endregion
		
		
	}
	
}