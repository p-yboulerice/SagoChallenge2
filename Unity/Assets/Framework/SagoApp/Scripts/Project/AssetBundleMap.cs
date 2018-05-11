namespace SagoApp.Project {
	
	using SagoPlatform;
	using SagoCore.AssetBundles;
	using UnityEngine;
	
	/// <summary>
	/// The AssetBundleDeploymentInfo struct contains metadata about where an asset bundle is deployed.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Each project will have a singleton <see cref="AssetBundleMap" /> asset, 
	/// which must be located in <c>Assets/Project/Resources/AssetBundleMap.asset</c> 
	/// (so that it is accessible at runtime via <see cref="Resources.Load" />).
	/// </para>
	/// </remarks>
	[System.Serializable]
	public struct AssetBundleDeploymentInfo {
		
		
		#region Fields
		
		/// <summary>
		/// The name of the asset bundle.
		/// </summary>
		[SerializeField]
		public string AssetBundleName;
		
		/// <summary>
		/// The deployment type.
		/// </summary>
		[SerializeField]
		public AssetBundleDeploymentType DeploymentType;
		
		/// <summary>
		/// The platform.
		/// </summary>
		[SerializeField]
		public Platform Platform;
		
		#endregion
		
		
	}
	
	/// <summary>
	/// Asset bundle deployment types.
	/// </summary>
	[System.Serializable]
	public enum AssetBundleDeploymentType {
		
		/// <summary>
		/// The asset bundle is not deployed.
		/// </summary>
		Unknown,
			
		/// <summary>
		/// The asset bundle is deployed using the streaming assets folder.
		/// </summary>
		Local,
			
		/// <summary>
		/// The asset bundle is deployed using a server or on demand resources.
		/// </summary>
		Remote
		
	}
	
	/// <summary>
	/// The AssetBundleServerInfo struct contains metadata about an asset bundle server.
	/// </summary>
	[System.Serializable]
	public struct AssetBundleServerInfo {
		
		
		#region Fields
		
		/// <summary>
		/// The build platform.
		/// </summary>
		[SerializeField]
		public Platform Platform;
		
		/// <summary>
		/// The server type.
		/// </summary>
		[SerializeField]
		public AssetBundleServerType ServerType;
		
		/// <summary>
		/// The server url.
		/// </summary>
		[SerializeField]
		public string Url;
		
		#endregion
		
		
	}
	
	/// <summary>
	/// Asset bundle server types.
	/// </summary>
	[System.Serializable]
	public enum AssetBundleServerType {
		
		/// <summary>
		/// Do not use an asset bundle server.
		/// </summary>
		Unknown,
		
		/// <summary>
		/// Use the development asset bundle server.
		/// </summary>
		Development,
		
		/// <summary>
		/// Use the staging asset bundle server.
		/// </summary>
		Staging,
		
		/// <summary>
		/// Use the production asset bundle server.
		/// </summary>
		Production
		
	}
	
	/// <summary>
	/// The AssetBundleMap class stores metadata about how to build and load the asset bundles for a project.
	/// </summary>
	
	/// <remarks>
	/// <para>
	/// The <see cref="AssetBundleMap" /> class exists to keep asset bundle metadata (which is 
	/// project-specific) in our project repos (and not in the <see cref="SagoUtils" /> submodule).
	/// </para>
	/// <para>
	/// The <see cref="AssetBundleMap"> asset lives in a <c>Resources</c> folder in the project repo so that 
	/// it always compiled into the application and available at run time via <see cref="Resources.Load" />.
	/// </para>
	/// <para>
	/// <see cref="SagoApp" /> assigns <see cref="AssetBundleMap.CreateAdaptor" /> to <see cref="AssetBundleReference.CreateAdaptor" /> 
	/// at runtime to let the <see cref="AssetBundleLoader" /> know where to find asset bundles for the current project.
	/// </para>
	/// </remarks>
	
	[System.Serializable]
	public class AssetBundleMap : ScriptableObject {
		
		
		#region Static Properties
		
		/// <summary>
		/// The singleton <see cref="AssetBundleMap" /> instance.
		/// </summary>
		public static AssetBundleMap Instance {
			get {
				AssetBundleMap instance;
				instance = Resources.Load("AssetBundleMap", typeof(AssetBundleMap)) as AssetBundleMap;
				return instance;
			}
		}
		
		#endregion
		
		
		#region Fields
		
		/// <summary>
		/// The array of <see cref="DeploymentInfo" /> structs.
		/// </summary>
		[SerializeField]
		private AssetBundleDeploymentInfo[] m_DeploymentInfo;
		
		/// <summary>
		/// The array of <see cref="ServerInfo" /> structs.
		/// </summary>
		[SerializeField]
		private AssetBundleServerInfo[] m_ServerInfo;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets the array of <see cref="DeploymentInfo" /> structs.
		/// </summary>
		public AssetBundleDeploymentInfo[] DeploymentInfo {
			get { return m_DeploymentInfo; }
		}
		
		/// <summary>
		/// Gets the array of <see cref="ServerInfo" /> structs.
		/// </summary>
		public AssetBundleServerInfo[] ServerInfo {
			get { return m_ServerInfo; }
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Creates an <see cref="IAssetBundleAdaptor" /> object for the specified asset bundle.
		/// </summary>
		/// <param name="assetBundleName">
		/// The name of the asset bundle.
		/// </summary>
		public IAssetBundleAdaptor CreateAdaptor(string assetBundleName) {
			return AssetBundleAdaptorMap.Instance.CreateAdaptor(assetBundleName);
		}
		
		/// <summary>
		/// Resets the asset bundle map.
		/// <summary>
		private void Reset() {
			m_DeploymentInfo = new AssetBundleDeploymentInfo[0];
			m_ServerInfo = new AssetBundleServerInfo[0];
		}
		
		#endregion
		
		
	}
	
}