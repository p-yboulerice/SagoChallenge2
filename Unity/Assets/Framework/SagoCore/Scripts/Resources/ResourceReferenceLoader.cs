namespace SagoCore.Resources {
	
	using UnityEngine;
	
	/// <summary>
	/// The ResourceReferenceLoader provides methods for loading assets using resource references.
	/// </summary>
	
	/// <remarks>
	/// IMPORTANT: The ResourceReferenceLoader won't be able to load assets from asset bundles 
	/// until you've assigned a method to <see cref="AssetBundleReference.CreateAdaptor" />.
	/// </remarks>
	
	/// <example>
	/// The following code shows how to load an asset asynchronously using the <see cref="ResourceReferenceLoader" />.
	/// <code>
	/// private IEnumerator Example(ResourceReference reference, System.Type type) {
	/// 	
	/// 	var request = ResourceReferenceLoader.LoadAsync(reference, type);
	/// 	yield return request;
	/// 	
	/// 	if (!string.IsNullOrEmpty(request.error)) {
	/// 		Debug.Log(request.error);
	/// 	} else {
	/// 		Debug.Log(request.asset);
	/// 	}
	/// 	
	/// }
	/// </code>
	/// </example>
	
	public class ResourceReferenceLoader : MonoBehaviour {
		
		
		#region Singleton
		
		/// <summary>
		/// The singleton instance.
		/// </summary>
		private static ResourceReferenceLoader _Instance;
		
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		/// <remarks>
		/// The ResourceReferenceLoader class is a <see cref="MonoBehaviour" /> 
		/// singleton to allow <see cref="IResourceReferenceAdaptor" /> objects 
		/// to run coroutines.
		/// </remarks>
		public static ResourceReferenceLoader Instance {
			get {
				if (!_Instance && !_IsQuitting) {
					_Instance = new GameObject("ResourceReferenceLoader").AddComponent<ResourceReferenceLoader>();
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
		/// Sets the <see cref="_IsQuitting" /> flag when the application quits.
		/// </summary>
		private void OnApplicationQuit() {
			_IsQuitting = true;
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Synchronously loads the referenced asset.
		/// </summary>
		/// <param name="resourceReference">
		/// The resource reference for the asset.
		/// </param>
		/// <typeparam name="T">
		/// The type of asset.
		/// </typeparam>
		public static ResourceReferenceLoaderRequest Load<T>(IResourceReference resourceReference) where T : Object {
			return Load(resourceReference, typeof(T));
		}
		
		/// <summary>
		/// Synchronously loads the referenced asset.
		/// </summary>
		/// <param name="resourceReference">
		/// The resource reference for the asset.
		/// </param>
		/// <param name="resourceType">
		/// The type of asset.
		/// </param>
		public static ResourceReferenceLoaderRequest Load(IResourceReference resourceReference, System.Type resourceType) {
			return new ResourceReferenceLoaderRequest(resourceReference, resourceType, false);
		}
		
		/// <summary>
		/// Asynchronously loads the referenced asset.
		/// </summary>
		/// <param name="resourceReference">
		/// The resource reference for the asset.
		/// </param>
		/// <typeparam name="T">
		/// The type of asset.
		/// </typeparam>
		public static ResourceReferenceLoaderRequest LoadAsync<T>(IResourceReference resourceReference) where T : Object {
			return LoadAsync(resourceReference, typeof(T));
		}
		
		/// <summary>
		/// Asynchronously loads the referenced asset.
		/// </summary>
		/// <param name="resourceReference">
		/// The resource reference for the asset.
		/// </param>
		/// <param name="resourceType">
		/// The type of asset.
		/// </param>
		public static ResourceReferenceLoaderRequest LoadAsync(IResourceReference resourceReference, System.Type resourceType) {
			return new ResourceReferenceLoaderRequest(resourceReference, resourceType, true);
		}
		
		#endregion
		
		
	}
	
}