namespace SagoCore.Resources {
	
	using System.Collections;
	using UnityEngine;
	
	/// <summary>
	/// The ResourcesAdaptor class loads an asset from a <c>Resources</c> folder.
	/// </summary>
	public sealed class ResourcesAdaptor : CustomYieldInstruction, IResourceReferenceAdaptor {
		
		
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
		/// Creates a new ResourcesAdaptor instance.
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
		public ResourcesAdaptor(IResourceReference resourceReference, System.Type resourceType, bool async) {
			if (ResourceReferenceLoader.Instance) {
				ResourceReferenceLoader.Instance.StartCoroutine(ResourcesAdaptorImpl(resourceReference, resourceType, async));
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
		/// Whether to load the asset asynchronously or not.
		/// </param>
		private IEnumerator ResourcesAdaptorImpl(IResourceReference resourceReference, System.Type resourceType, bool async) {
			
			if (resourceReference == null) {
				throw new System.ArgumentNullException("resourceReference");
			} else if (string.IsNullOrEmpty(resourceReference.ResourcePath)) {
				throw new System.ArgumentException("resourceReference.ResourcePath cannot be null.", "resourceReference");
			}
			
			m_Asset = null;
			m_Error = null;
			m_KeepWaiting = true;
			
			if (async) {
				
				ResourceRequest request;
				request = Resources.LoadAsync(resourceReference.ResourcePath, resourceType);
				yield return request;
				
				if (request.asset == null) {
					m_Error = string.Format(
						"Could not load asset from resources: resource path = {0}, resource type = {1}", 
						resourceReference.ResourcePath, 
						resourceType
					);
					m_KeepWaiting = false;
					yield break;
				}
				
				m_Asset = request.asset;
				m_KeepWaiting = false;
				yield break;
				
			} else {
				
				Object asset;
				asset = Resources.Load(resourceReference.ResourcePath, resourceType);
				
				if (asset == null) {
					m_Error = string.Format(
						"Could not load asset from resources: resource path = {0}, resource type = {1}", 
						resourceReference.ResourcePath, 
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