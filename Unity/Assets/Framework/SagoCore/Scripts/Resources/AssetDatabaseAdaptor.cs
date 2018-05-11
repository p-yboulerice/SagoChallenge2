namespace SagoCore.Resources {
	
	using System.Collections;
	using UnityEngine;
	
	/// <summary>
	/// The AssetDatabaseAdaptor class loads an asset from the asset database (editor only).
	/// </summary>
	public sealed class AssetDatabaseAdaptor : CustomYieldInstruction, IResourceReferenceAdaptor {
		
		
		#region Fields
		
		/// <summary>
		/// The asset.
		/// </summary>
		private Object m_Asset;
		
		/// <summary>
		/// The asset path.
		/// </summary>
		private string m_AssetPath;
		
		/// <summary>
		/// The error that occured while loading the asset.
		/// </summary>
		private string m_Error;
		
		/// <summary>
		/// The flag that indicates whether the asset has finished loading or an error occurred.
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
		/// Creates a new AssetDatabaseAdaptor instance.
		/// </summary>
		/// <param name="resourceReference">
		/// The resource reference for the asset.
		/// </param>
		/// <param name="resourceType">
		/// The type of asset.
		/// </param>
		public AssetDatabaseAdaptor(IResourceReference resourceReference, System.Type resourceType) {
			if (ResourceReferenceLoader.Instance) {
				ResourceReferenceLoader.Instance.StartCoroutine(AssetDatabaseAdaptorImpl(resourceReference, resourceType));
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
		/// Whether to load the asset asynchronously or not. The <c>async</c> flag is 
		/// not used, the <c>AssetDatabaseAdaptor</c> always loads assets synchronously.
		/// </param>
		private IEnumerator AssetDatabaseAdaptorImpl(IResourceReference resourceReference, System.Type resourceType) {
			
			if (resourceReference == null) {
				throw new System.ArgumentNullException("resourceReference");
			} else if (string.IsNullOrEmpty(resourceReference.Guid)) {
				throw new System.ArgumentException("resourceReference.Guid cannot be null.", "resourceReference");
			}
			
			m_Asset = null;
			m_Error = null;
			m_KeepWaiting = true;
			
			#if UNITY_EDITOR
				
				Object asset;
				asset = UnityEditor.AssetDatabase.LoadAssetAtPath(resourceReference.AssetPath, resourceType);
				
				if (asset == null) {
					m_Error = string.Format(
						"Could not load asset from asset database: asset path = {0}", 
						resourceReference.AssetPath
					);
					m_KeepWaiting = false;
					yield break;
				}
				
				m_Asset = asset;
				m_KeepWaiting = false;
				yield break;
				
			#else
				
				m_Error = string.Format(
					"Could not load asset from asset database: asset path = {0}", 
					resourceReference.AssetPath
				);
				m_KeepWaiting = false;
				yield break;
				
			#endif
			
		}
		
		#endregion
		
		
	}
	
}