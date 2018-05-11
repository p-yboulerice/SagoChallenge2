namespace SagoCore.Resources {
	
	using SagoCore.AssetBundles;
	using System.Collections;
	using UnityEngine;
	
	/// <summary>
	/// The ResourceReferenceLoaderRequest class provides a wrapper around a <see cref="IResourceReferenceAdaptor" /> object.
	/// </summary>
	public sealed class ResourceReferenceLoaderRequest : CustomYieldInstruction {
		
		
		#region Fields
		
		/// <summary>
		/// The adaptor.
		/// </summary>
		private IResourceReferenceAdaptor m_Adaptor;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets the asset.
		/// </summary>
		public Object asset {
			get { return m_Adaptor != null ? m_Adaptor.asset : null; }
		}
		
		/// <summary>
		/// Gets the error that occurred while loading the asset.
		/// </summary>
		public string error {
			get { return m_Adaptor != null ? m_Adaptor.error : null; }
		}
		
		/// <summary>
		/// <see cref="keepWaiting" />
		/// </summary>
		public bool isDone {
			get { return !keepWaiting; }
		}
		
		/// <summary>
		/// Gets the flag that indicates whether the asset has finished loading or an error has occurred.
		/// </summary>
		override public bool keepWaiting {
			get { return m_Adaptor != null ? m_Adaptor.keepWaiting : false; }
		}
		
		#endregion
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new ResourceReferenceLoaderRequest instance
		/// </summary>
		/// <param name="resourceReference">
		/// The resource reference for the asset.
		/// </param>
		/// <param name="resourceType">
		/// The type of asset.
		/// </param>
		/// <param name="async">
		/// Whether to load the asset asynchronously or not.
		/// </param>
		public ResourceReferenceLoaderRequest(IResourceReference resourceReference, System.Type resourceType, bool async) {
			try {
				if (resourceReference == null) {
					m_Adaptor = new ErrorAdaptor(string.Format("Invalid resource reference: {0}", resourceReference));
				} else if (string.IsNullOrEmpty(resourceReference.Guid)) {
					m_Adaptor = new ErrorAdaptor(string.Format("Invalid resource reference: guid = {0}", resourceReference.Guid));
				} else if (!string.IsNullOrEmpty(resourceReference.AssetBundleName)) {
					if (AssetBundleOptions.UseAssetDatabaseInEditor) {
						m_Adaptor = new AssetDatabaseAdaptor(resourceReference, resourceType);
					} else {
						m_Adaptor = new AssetBundleAdaptor(resourceReference, resourceType, async);
					}
				} else if (!string.IsNullOrEmpty(resourceReference.ResourcePath)) {
					m_Adaptor = new ResourcesAdaptor(resourceReference, resourceType, async);
				} else if (AssetBundleOptions.UseAssetDatabaseInEditor) {
					m_Adaptor = new AssetDatabaseAdaptor(resourceReference, resourceType);
				} else {
					m_Adaptor = new ErrorAdaptor("The referenced asset is not in an asset bundle or resources folder.");
				}
			} catch (System.Exception e) {
				m_Adaptor = new ErrorAdaptor(string.Format("Could not create adaptor: {0}", e.ToString()));
			}
		}
		
		#endregion
		
		
	}
	
}