namespace SagoCore.Resources {
	
	using UnityEngine;
	
	/// <summary>
	/// The IResourceReferenceAdaptor interface defines the functionality required by an resource reference adaptor.
	/// </summary>
	public interface IResourceReferenceAdaptor {
		
		
		#region Properties
		
		/// <summary>
		/// Gets the asset.
		/// </summary>
		Object asset {
			get;
		}
		
		/// <summary>
		/// Gets the error that occurred while loading the asset.
		/// </summary>
		string error {
			get;
		}
		
		/// <summary>
		/// <see cref="keepWaiting" />
		/// </summary>
		bool isDone {
			get;
		}
		
		/// <summary>
		/// The flag that indicates whether the asset has finished loading or an error has occurred.
		/// </summary>
		bool keepWaiting {
			get;
		}
		
		#endregion
		
		
	}
	
}