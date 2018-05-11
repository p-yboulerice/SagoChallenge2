namespace SagoCore.AssetBundles {
	
	using UnityEngine;
	
	/// <summary>
	/// The IAssetBundleAdaptor interface defines the functionality required by an asset bundle adaptor.
	/// </summary>
	
	/// <example>
	/// <para>
	/// IMPORTANT: You must call <see cref="Dispose" /> on the adaptor when it's no 
	/// longer in use, so that it can release any unmanaged resources (like instances 
	/// of the WWW class). You should call <see cref="Dispose" /> as soon as possible 
	/// to minimize the application's the memory footprint.
	/// </para>
	/// <code>
	/// private IEnumerator Start() {
	/// 	
	/// 	IAssetBundleAdaptor adaptor;
	/// 	adaptor = new StreaminAssetAdaptor("my_asset_bundle");
	/// 	yield return adaptor;
	/// 	
	/// 	Debug.Log(adaptor.assetBundle);
	/// 	
	/// 	adaptor.Dispose();
	/// 	
	/// }
	/// </code>
	/// </example>
	
	public static class AssetBundleAdaptorError {
		public const string NoInternet = "SagoCore.AssetBundles.AssetBundleAdaptorError.NoInternet";
	}
	
	public interface IAssetBundleAdaptor : System.IDisposable {
		
		
		#region Properties
		
		/// <summary>
		/// Gets the asset bundle.
		/// </summary>
		AssetBundle assetBundle {
			get;
		}
		
		/// <summary>
		/// Gets the asset bundle name.
		/// </summary>
		string assetBundleName {
			get;
		}
		
		/// <summary>
		/// Gets the error that occurred while loading the asset bundle.
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
		/// Gets the flag indicating whether the asset bundle has finished loading or an error has occurred.
		/// </summary>
		bool keepWaiting {
			get;
		}
		
		/// <summary>
		/// Gets the progress of the load operation.
		/// </summary>
		float progress {
			get;
		}
		
		#endregion
		
		
	}
	
}