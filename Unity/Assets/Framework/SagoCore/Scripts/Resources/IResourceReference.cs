namespace SagoCore.Resources {
	
	public interface IResourceReference {
		
		
		#region Properties
		
		/// <summary>
		/// Gets the guid of the referenced asset.
		/// </summary>
		string Guid {
			get;
		}
		
		/// <summary>
		/// Gets the asset bundle name of the referenced asset.
		/// </summary>
		/// <returns>
		/// <c>null</c> if the referenced asset is not in an asset bundle.
		/// </returns>
		string AssetBundleName {
			get;
		}
		
		/// <summary>
		/// Gets the asset path of the referenced asset.
		/// </summary>
		string AssetPath {
			get;
		}
		
		/// <summary>
		/// Gets the resource path of the referenced asset.
		/// </summary>
		/// <returns>
		/// <c>null</c> if the referenced asset is not in a resources folder.
		/// </returns>
		string ResourcePath {
			get;
		}
		
		#endregion
		
		
	}
	
}