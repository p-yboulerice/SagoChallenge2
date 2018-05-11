namespace SagoCore.Resources {
	
	using System.Collections;
	using UnityEngine;
		
	/// <summary>
	/// The ErrorAdaptor class.
	/// </summary>
	public sealed class ErrorAdaptor : CustomYieldInstruction, IResourceReferenceAdaptor {
		
		
		#region Fields
		
		/// <summary>
		/// The exception that occurred while loading the asset.
		/// </summary>
		private string m_Error;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// <see cref="IResourceReferenceAdaptor.asset" />
		/// </summary>
		public Object asset {
			get { return null; }
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
			get { return false; }
		}
		
		#endregion
		
		
		#region Constructor
		
		/// <summary>
		/// Creates a new ErrorAdaptor instance.
		/// </summary>
		public ErrorAdaptor(string error) {
			m_Error = error;
		}
		
		#endregion
		
		
	}
	
}