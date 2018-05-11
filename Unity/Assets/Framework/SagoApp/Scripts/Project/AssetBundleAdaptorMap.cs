namespace SagoApp.Project {
	
	using SagoCore.AssetBundles;
	using SagoPlatform;
	using System.IO;
	using UnityEngine;
	
	/// <summary>
	/// Asset bundle adaptor types.
	/// </summary>
	[System.Serializable]
	public enum AssetBundleAdaptorType {
		
		/// <summary>
		/// Do not use an asset bundle adaptor.
		/// </summary>
		Unknown,
		
		/// <summary>
		/// Use a <see cref="StreamingAssetsAdaptor" />.
		/// </summary>
		StreamingAssets,
		
		/// <summary>
		/// Use a <see cref="OnDemandResourcesAdaptor" />.
		/// </summary>
		OnDemandResources,
		
		/// <summary>
		/// Use a <see cref="ServerAdaptor" />.
		/// </summary>
		AssetBundleServer
		
	}
	
	/// <summary>
	/// The AssetBundleAdaptorMap class stores build-specific asset bundle adaptor metadata.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Each project will have a singleton <see cref="AssetBundleAdaptorMap" /> asset, 
	/// which must be located in <c>Assets/Project/Resources/AssetBundleAdaptorMap.asset</c> 
	/// (so that it is accessible at runtime via <see cref="Resources.Load" />).
	/// </para>
	/// <para>
	/// The metadata in the <see cref="AssetBundleAdaptorMap" /> asset is populated 
	/// by resolving the <see cref="AssetBundleMap" /> using the current build 
	/// settings (platform, asset bundle deployment type, etc.) and is unique to 
	/// each build. The asset will be overwritten during every build and does not 
	/// get committed to the project repo.
	/// </para>
	/// </remarks>
	[System.Serializable]
	public class AssetBundleAdaptorMap : ScriptableObject {
		
		
		#region Static Properties
		
		/// <summary>
		/// Gets the singleton <see cref="AssetBundleAdaptorMap" /> instance.
		/// </summary>
		public static AssetBundleAdaptorMap Instance {
			get {
				AssetBundleAdaptorMap instance;
				instance = Resources.Load("AssetBundleAdaptorMap", typeof(AssetBundleAdaptorMap)) as AssetBundleAdaptorMap;
				return instance;
			}
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Assigns the <see cref="CreateAdaptor" /> method to <see cref="AssetBundleReference.CreateAdaptor" /> 
		/// so that the <see cref="AssetBundleLoader" /> can create adaptors for the asset bundles in the current project.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InjectDependencies() {
			Debug.Log("AssetBundleAdaptorMap-> InjectDependencies", DebugContext.SagoApp);
			AssetBundleReference.CreateAdaptor = (string assetBundleName) => {
				return AssetBundleAdaptorMap.Instance.CreateAdaptor(assetBundleName);
			};
		}
		
		#endregion
		
		
		#region Fields
		
		/// <summary>
		/// The array of values (adaptor types).
		/// </summary>
		[SerializeField]
		private AssetBundleAdaptorType[] m_AdaptorTypes;
		
		/// <summary>
		/// The array of keys (asset bundle names).
		[SerializeField]
		private string[] m_AssetBundleNames;
		
		/// <summary>
		/// The server adaptor options (for any asset bundle that use a <see cref="ServerAdaptor" />).
		/// </summary>
		[SerializeField]
		private ServerAdaptorOptions m_ServerAdaptorOptions;
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Creates an <see cref="IAssetBundleAdaptor" /> object for the specified asset bundle.
		/// </summary>
		/// <param name="assetBundleName">
		/// The name of the asset bundle.
		/// </param>
		public IAssetBundleAdaptor CreateAdaptor(string assetBundleName, bool downloadOnly = false) {

			AssetBundleAdaptorType adaptorType;
			adaptorType = GetAdaptorType(assetBundleName);
			
			Debug.LogFormat(DebugContext.SagoApp, "Creating an adaptor type [{0}] for asset bundle: {1}", adaptorType.ToString(), assetBundleName);
			switch (adaptorType) {
				case AssetBundleAdaptorType.StreamingAssets: {
					return new StreamingAssetsAdaptor(assetBundleName, new StreamingAssetsAdaptorOptions() {
						StreamingAssetsPath = (
							Application.isEditor ? 
							Path.Combine(Application.dataPath, "Project/AssetBundles/StreamingAssets") : 
							Application.streamingAssetsPath
						)
					}, 
					downloadOnly);
				}
				case AssetBundleAdaptorType.OnDemandResources: {
					#if UNITY_EDITOR
						// OnDemandResources don't work in the editor...
						return new StreamingAssetsAdaptor(assetBundleName, new StreamingAssetsAdaptorOptions() {
							StreamingAssetsPath = (
								Application.isEditor ? 
								Path.Combine(Application.dataPath, "Project/AssetBundles/OnDemandResources") : 
								Application.streamingAssetsPath
							)
						},
						downloadOnly);
					#else
						return new LowDiskSpaceAssetBundleAdaptor(new OnDemandResourcesAdaptor(assetBundleName, downloadOnly, (string name) => PlatformUtil.AppendVersion(name)));
					#endif
				}
				case AssetBundleAdaptorType.AssetBundleServer: {
					return new ServerAdaptor(assetBundleName, new ServerAdaptorOptions() {
						Token = m_ServerAdaptorOptions.Token,
						Url = m_ServerAdaptorOptions.Url
					});
				}
			}
			
			return null;
			
		}
		
		/// <summary>
		/// Gets the adaptor type for the specified asset bundle.
		/// </summary>
		/// <param name="assetBundleName">
		/// The name of the asset bundle.
		/// </param>
		public AssetBundleAdaptorType GetAdaptorType(string assetBundleName) {
			
			int index;
			index = System.Array.IndexOf(m_AssetBundleNames, assetBundleName);
			
			if (index != -1) {
				return m_AdaptorTypes[index];
			}
			
			return AssetBundleAdaptorType.Unknown;
			
		}
		
		/// <summary>
		/// Resets the asset bundle adaptor map.
		/// </summary>
		private void Reset() {
			m_AdaptorTypes = new AssetBundleAdaptorType[0];
			m_AssetBundleNames = new string[0];
			m_ServerAdaptorOptions = new ServerAdaptorOptions();
		}
		
		#endregion
		
		
	}

	public static class AssetBundleAdaptorTypeExtensions {

		public static bool IsLocal(this AssetBundleAdaptorType adaptorType) {
			switch (adaptorType) {
			case AssetBundleAdaptorType.StreamingAssets:
				return true;
			default:
				return false;
			}
		}

		public static bool IsRemote(this AssetBundleAdaptorType adaptorType) {
			switch (adaptorType) {
			case AssetBundleAdaptorType.AssetBundleServer:
			case AssetBundleAdaptorType.OnDemandResources:
				return true;
			default:
				return false;
			}
		}

	}
	
}