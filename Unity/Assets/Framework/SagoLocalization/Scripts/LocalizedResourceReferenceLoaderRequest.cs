namespace SagoLocalization {
	
	using SagoCore.Resources;
	using System.Collections;
	using UnityEngine;
	
	/// <summary>
	/// The LocalizedResourceReferenceLoaderRequest class provides a wrapper around a <see cref="ResourceReferenceLoaderRequest" /> object.
	/// </summary>
	public sealed class LocalizedResourceReferenceLoaderRequest<T> : CustomYieldInstruction where T : Object {
		
		
		#region Fields
		
		/// <summary>
		/// The error.
		/// </summary>
		private string m_Error;
		
		/// <summary>
		/// The request.
		/// </summary>
		private ResourceReferenceLoaderRequest m_Request;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets the asset.
		/// </summary>
		public T asset {
			get { return m_Request != null ? m_Request.asset as T : null; }
		}
		
		/// <summary>
		/// Gets the error that occurred while loading the asset.
		/// </summary>
		public string error {
			get { return m_Request != null && !string.IsNullOrEmpty(m_Request.error) ? m_Request.error : m_Error; }
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
			get { return m_Request != null ? m_Request.keepWaiting : false; }
		}
		
		#endregion
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new LocalizedResourceReferenceLoaderRequest instance
		/// </summary>
		/// <param name="resourceReference">
		/// The resource reference for the asset.
		/// </param>
		/// <param name="async">
		/// Whether to load the asset asynchronously or not.
		/// </param>
		public LocalizedResourceReferenceLoaderRequest(ResourceReference resourceReference, bool async) {
			try {
				m_Request = new ResourceReferenceLoaderRequest(resourceReference, typeof(T), async);
			} catch (System.Exception exception) {
				m_Error = exception.ToString();
				m_Request = null;
			}
		}
		
		#endregion
		
		
	}
	
}