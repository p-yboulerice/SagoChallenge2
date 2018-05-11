namespace SagoCore.Resources {
	
	using SagoCore.AssetBundles;
	using System.Collections;
	using UnityEngine;
	
	/// <summary>
	/// The AssetBundleAdaptor class loads an asset from an asset bundle.
	/// </summary>
	public sealed class AssetBundleAdaptor : CustomYieldInstruction, IResourceReferenceAdaptor {
		
		
		#region Fields
		
		/// <summary>
		/// The asset.
		/// </summary>
		private Object m_Asset;
		
		/// <summary>
		/// The error that occurred while loading the asset.
		/// </summary>
		private string m_Error;
		
		/// <summary>
		/// The flag that indicates whether the asset has finished loading or an error has occurred.
		/// </summary>
		private bool m_KeepWaiting;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// <see cref="IResourceReferenceAdaptor.asset" />
		/// </summary>
		public Object asset {
			get { return m_Asset; }
		}
		
		/// <summary>
		/// <see cref="IResourceReferenceAdaptor.error" />
		/// </summary>
		public string error {
			get { return m_Error; }
		}
		
		/// <summary>
		/// <see cref="IResourceReferenceAdaptor.isDone" />
		/// </summary>
		public bool isDone {
			get { return !keepWaiting; }
		}
		
		/// <summary>
		/// <see cref="IResourceReferenceAdaptor.keepWaiting" />
		/// </summary>
		override public bool keepWaiting {
			get { return m_KeepWaiting; }
		}
		
		#endregion
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new AssetBundleAdaptor instance.
		/// </summary>
		/// <param name="resourceReference">
		/// The resource reference for the asset.
		/// </param>
		/// <param name="resourceType">
		/// The type of asset.
		/// </param>
		/// <param name="async">
		/// Whether to load the asset asynchronously or not. If <c>true</c>, the asset bundle must already be loaded.
		/// </param>
		public AssetBundleAdaptor(IResourceReference resourceReference, System.Type resourceType, bool async) {
			if (ResourceReferenceLoader.Instance) {
				ResourceReferenceLoader.Instance.StartCoroutine(AssetBundleAdaptorImpl(resourceReference, resourceType, async));
			}
		}
		
		/// <summary>
		/// Loads the asset.
		/// </summary>
		/// <param name="resourceReference">
		/// The resource reference for the asset.
		/// </param>
		/// <param name="resourceType">
		/// The type of asset.
		/// </param>
		/// <param name="async">
		/// Whether to load the asset asynchronously or not. If <c>true</c>, the asset bundle must already be loaded.
		/// </param>
		private IEnumerator AssetBundleAdaptorImpl(IResourceReference resourceReference, System.Type resourceType, bool async) {
			
			if (resourceReference == null) {
				throw new System.ArgumentNullException("resourceReference");
			} else if (string.IsNullOrEmpty(resourceReference.AssetBundleName)) {
				throw new System.ArgumentException("resourceReference.AssetBundleName cannot be null.", "resourceReference");
			}
			
			m_Asset = null;
			m_Error = null;
			m_KeepWaiting = true;
			
			if (async) {
				
				AssetBundleLoaderRequest assetBundleLoaderRequest;
				assetBundleLoaderRequest = AssetBundleLoader.LoadAsync(resourceReference.AssetBundleName);
				yield return assetBundleLoaderRequest;
				
				if (!string.IsNullOrEmpty(assetBundleLoaderRequest.error)) {
					m_Error = assetBundleLoaderRequest.error;
					m_KeepWaiting = false;
					assetBundleLoaderRequest.Dispose();
					yield break;
				}
				
				AssetBundleRequest assetBundleRequest;
				assetBundleRequest = assetBundleLoaderRequest.assetBundle.LoadAssetAsync(resourceReference.AssetPath, resourceType);
				yield return assetBundleRequest;
				
				if (assetBundleRequest.asset == null) {
					m_Error = string.Format(
						"Could not load asset from asset bundle: asset bundle name = {0}, asset path = {1}, asset type = {2}", 
						resourceReference.AssetBundleName, 
						resourceReference.AssetPath, 
						resourceType
					);
					m_KeepWaiting = false;
					assetBundleLoaderRequest.Dispose();
					yield break;
				}
				
				m_Asset = assetBundleRequest.asset;
				m_KeepWaiting = false;
				assetBundleLoaderRequest.Dispose();
				yield break;
				
			} else {
				
				AssetBundleReference assetBundleReference;
				assetBundleReference = AssetBundleReference.FindReference(resourceReference.AssetBundleName);
				
				if (assetBundleReference == null) {
					m_Error = string.Format(
						"Could not load asset from asset bundle: asset bundle not loaded: {0}", 
						resourceReference.AssetBundleName
					);
					m_KeepWaiting = false;
					yield break;
				}
				
				Object asset;
				asset = assetBundleReference.assetBundle.LoadAsset(resourceReference.AssetPath, resourceType);
				
				if (asset == null) {
					m_Error = string.Format(
						"Could not load asset from asset bundle: asset bundle name = {0}, asset path = {1}, asset type = {2}", 
						resourceReference.AssetBundleName, 
						resourceReference.AssetPath, 
						resourceType
					);
					m_KeepWaiting = false;
					yield break;
				}
				
				m_Asset = asset;
				m_KeepWaiting = false;
				yield break;
				
			}
			
		}
		
		#endregion
		
		
	}
	
}