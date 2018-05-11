namespace SagoCore.Resources {
	
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEngine;
	
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	
	/// <summary>
	/// The ResourceMapMode enum defines the modes for the <see cref="ResourceMap" />.
	/// </summary>
	public enum ResourceMapMode {
		Editor,
		Player
	}
	
	/// <summary>
	/// The ResourceMapValue struct stores metadata about an asset.
	/// </summary>
	[System.Serializable]
	public struct ResourceMapValue {
		
		/// <summary>
		/// The guid.
		/// </summary>
		[SerializeField]
		public string Guid;
		
		/// <summary>
		/// The asset bundle name (<c>null</c> if the asset is not in an asset bundle).
		/// </summary>
		[SerializeField]
		public string AssetBundleName;
		
		/// <summary>
		/// The asset path.
		/// </summary>
		[SerializeField]
		public string AssetPath;
		
		/// <summary>
		/// The resource path (<c>null</c> if the asset is not in a <c>Resources</c> folder).
		/// </summary>
		[SerializeField]
		public string ResourcePath;
		
	}
	
	/// <summary>
	/// The ResourceMap class stores metadata about assets in <c>Resources</c> 
	/// folders and asset bundles.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="ResourceMap" /> class exists to keep asset metadata (which 
	/// is project-specific) in our project repos (and not in our submodule repos, 
	/// where it could cause conficts). <see cref="ResourceReference" /> objects 
	/// (which can live in submodule repos) only store a guid. The guid is used 
	/// to ask the <see cref="ResourceMap" /> (which always lives in the project 
	/// repo) for the metadata about the referenced asset.
	/// </para>
	/// <para>
	/// The <see cref="ResourceMap"> class lives in a <c>Resources</c> folder in 
	/// the project repo so that it always compiled into the application and 
	/// available at run time via <see cref="Resources.Load" />.
	/// </para>
	/// <para>
	/// The <see cref="ResourceMap" /> asset is updated automatically every time 
	/// the asset database changes.
	/// </para>
	/// </remarks>
	public class ResourceMap : ScriptableObject, ISerializationCallbackReceiver {
		
		
		#region Constants
		
		/// <summary>
		/// Defines keys used to get and set values in the <see cref="EditorPrefs" />.
		/// </summary>
		public static class EditorPrefsKey {
			
			public const string Mode = "SagoCore.Resources.ResourceMap.Mode";
			
		}
		
		/// <summary>
		/// Defines resource folder names. 
		/// </summary>
		public static class ResourceFolderName {
			
			public const string Custom = "_Resources_";
			
			public const string Unity = "Resources";
			
		}
		
		#endregion
		
		
		#region Static Properties
		
		/// <summary>
		/// Gets the singleton <see cref="ResourceMap" /> instance.
		/// </summary>
		public static ResourceMap Instance {
			get {
				
				ResourceMap instance;
				instance = Resources.Load("ResourceMap", typeof(ResourceMap)) as ResourceMap;
				
				if (instance == null) {
					throw new System.InvalidOperationException("Could not load resource map.");
				}
				
				return instance;
				
			}
		}
		
		/// <summary>
		/// Gets and sets the mode.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="ResourceMapMode.Editor" /> mode is used to bypass the 
		/// resource map asset in the editor. This mode allows the resource map
		/// class to work without constantly updating the asset during development.
		/// </para>
		/// </remarks>
		public static ResourceMapMode Mode {
			get {
				#if UNITY_EDITOR
					return (ResourceMapMode)EditorPrefs.GetInt(EditorPrefsKey.Mode, (int)ResourceMapMode.Editor);
				#else
					return ResourceMapMode.Player;
				#endif
			}
			set {
				#if UNITY_EDITOR
					EditorPrefs.SetInt(EditorPrefsKey.Mode, (int)value);
				#else
					throw new System.InvalidOperationException();
				#endif
			}
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Gets a flag that indicates whether the resource map contains information about the specified asset.
		/// </summary>
		/// <param name="assetGuid">
		/// The guid of the asset.
		/// </param>
		public static bool Contains(string assetGuid) {
			return (
				Mode == ResourceMapMode.Editor ? 
				ResourceMapEditorAdaptor.Contains(assetGuid) : 
				ResourceMapPlayerAdaptor.Contains(assetGuid)
			);
		}
		
		/// <summary>
		/// Gets the asset bundle name for the specified asset.
		/// </summary>
		/// <param name="assetGuid">
		/// The guid of the asset.
		/// </param>
		/// <returns>
		/// <c>null</c> if the asset is not in asset bundle.
		/// </returns>
		public static string GetAssetBundleName(string assetGuid) {
			return (
				Mode == ResourceMapMode.Editor ? 
				ResourceMapEditorAdaptor.GetAssetBundleName(assetGuid) : 
				ResourceMapPlayerAdaptor.GetAssetBundleName(assetGuid)
			);
		}
		
		/// <summary>
		/// Gets the asset path for the specified asset.
		/// </summary>
		/// <param name="assetGuid">
		/// The guid of the asset.
		/// </param>
		public static string GetAssetPath(string assetGuid) {
			return (
				Mode == ResourceMapMode.Editor ? 
				ResourceMapEditorAdaptor.GetAssetPath(assetGuid) : 
				ResourceMapPlayerAdaptor.GetAssetPath(assetGuid)
			);
		}
		
		/// <summary>
		/// Gets the resource path of the specified asset.
		/// </summary>
		/// <param name="assetGuid">
		/// The guid of the asset.
		/// </param>
		/// <returns>
		/// <c>null</c> if the asset is not in a <c>Resources</c> folder.
		/// </returns>
		public static string GetResourcePath(string assetGuid) {
			return (
				Mode == ResourceMapMode.Editor ? 
				ResourceMapEditorAdaptor.GetResourcePath(assetGuid) : 
				ResourceMapPlayerAdaptor.GetResourcePath(assetGuid)
			);
		}
		
		/// <summary>
		/// Gets a resource reference for the specified path.
		/// </summary>
		/// <param name="assetPath">
		/// The path of the asset.
		/// </param>
		/// <returns>
		/// <c>null</c> if the asset is not in a <c>Resources</c> folder.
		/// </returns>
		public static ResourceReference GetResourceReference(string assetPath) {
			return (
				Mode == ResourceMapMode.Editor ? 
				ResourceMapEditorAdaptor.GetResourceReference(assetPath) : 
				ResourceMapPlayerAdaptor.GetResourceReference(assetPath)
			);
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// Checks if the path is in any resource folder.
		/// </summary>
		/// <param name="path">
		/// The path to check.
		/// </param>
		public static bool IsResourcePath(string path) {
			return IsCustomResourcePath(path) || IsUnityResourcePath(path);
		}
		
		/// <summary>
		/// Checks if the path is in a custom resource folder.
		/// </summary>
		/// <param name="path">
		/// The path to check.
		/// </param>
		public static bool IsCustomResourcePath(string path) {
			return !string.IsNullOrEmpty(path) && path.Contains(string.Format("{0}{1}{0}", Path.DirectorySeparatorChar, ResourceFolderName.Custom));
		}
		
		/// <summary>
		/// Checks if the path is in a unity resource folder.
		/// </summary>
		/// <param name="path">
		/// The path to check.
		/// </param>
		public static bool IsUnityResourcePath(string path) {
			return !string.IsNullOrEmpty(path) && path.Contains(string.Format("{0}{1}{0}", Path.DirectorySeparatorChar, ResourceFolderName.Unity));
		}
		
		#endregion
		
		
		#region Fields
		
		/// <summary>
		/// The dictionary with the index of each guid and asset path in the <see cref="m_Values" /> array.
		/// </summary>
		[System.NonSerialized]
		private Dictionary<string,int> m_Indexes;
		
		/// <summary>
		/// The values (<see cref="ResourceMapValue" /> structs with metadata about the asset).
		/// </summary>
		[SerializeField]
		private ResourceMapValue[] m_Values;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets the array of values.
		/// </summary>
		public ResourceMapValue[] Values {
			get { return m_Values; }
		}
		
		#endregion
		
		
		#region ISerializationCallbackReceiver Methods
		
		public void OnBeforeSerialize() {
			
		}
		
		public void OnAfterDeserialize() {
			m_Indexes = new Dictionary<string,int>();
			for (int index = 0; index < m_Values.Length; index++) {
				m_Indexes.Add(m_Values[index].AssetPath, index);
				m_Indexes.Add(m_Values[index].Guid, index);
			}
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Checks if the resource map has a value for the specified asset path.
		/// </summary>
		public bool HasValueForAssetPath(string assetPath) {
			return !string.IsNullOrEmpty(ValueForAssetPath(assetPath).Guid);
		}
		
		/// <summary>
		/// Gets the value for the specified asset path.
		/// </summary>
		public ResourceMapValue ValueForAssetPath(string assetPath) {
			int index = 0;
			bool success = m_Indexes != null && m_Indexes.TryGetValue(assetPath, out index);
			return success ? m_Values[index] : default(ResourceMapValue);
		}
		
		/// <summary>
		/// Checks if the resource map has a value for the specified guid.
		/// </summary>
		public bool HasValueForGuid(string guid) {
			return !string.IsNullOrEmpty(ValueForGuid(guid).Guid);
		}
		
		/// <summary>
		/// Gets the value for the specified guid.
		/// </summary>
		public ResourceMapValue ValueForGuid(string guid) {
			int index = 0;
			bool success = m_Indexes != null && m_Indexes.TryGetValue(guid, out index);
			return success ? m_Values[index] : default(ResourceMapValue);
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Resets the map.
		/// </summary>
		private void Reset() {
			m_Values = new ResourceMapValue[0];
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The ResourceMapEditorAdaptor can be used in the editor to bypass 
	/// the resource map asset and get information directly from the 
	/// asset database. That way, code that depends on the resource map 
	/// will work in the editor, even if the asset has not been updated.
	/// </summary>
	public static class ResourceMapEditorAdaptor {
		
		
		#region Static Methods
		
		/// <summary>
		/// <see cref="ResourceMap.Contains" />
		/// </summary>
		public static bool Contains(string assetGuid) {
			#if UNITY_EDITOR
				return ResourceMap.IsResourcePath(AssetDatabase.GUIDToAssetPath(assetGuid));
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		/// <summary>
		/// <see cref="ResourceMap.GetAssetBundleName" />
		/// </summary>
		public static string GetAssetBundleName(string assetGuid) {
			#if UNITY_EDITOR
				if (Contains(assetGuid)) {
					
					string path;
					path = AssetDatabase.GUIDToAssetPath(assetGuid);
					
					AssetImporter importer;
					importer = AssetImporter.GetAtPath(path);
					
					if (importer != null && !string.IsNullOrEmpty(importer.assetBundleName)) {
						return importer.assetBundleName;
					}
					
				}
				return null;
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		/// <summary>
		/// <see cref="ResourceMap.GetAssetPath" />
		/// </summary>
		public static string GetAssetPath(string assetGuid) {
			#if UNITY_EDITOR
				return Contains(assetGuid) ? AssetDatabase.GUIDToAssetPath(assetGuid) : null;
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		/// <summary>
		/// <see cref="ResourceMap.GetResourcePath" />
		/// </summary>
		public static string GetResourcePath(string assetGuid) {
			#if UNITY_EDITOR
				
				string assetPath;
				assetPath = GetAssetPath(assetGuid);
				
				if (ResourceMap.IsUnityResourcePath(assetPath)) {
					
					List<string> components;
					components = new List<string>(assetPath.Split(Path.DirectorySeparatorChar));
					
					int offset;
					offset = components.LastIndexOf(ResourceMap.ResourceFolderName.Unity) + 1;
					
					int count;
					count = components.Count - offset;
					
					string resourcePath;
					resourcePath = string.Join(Path.DirectorySeparatorChar.ToString(), components.GetRange(offset, count).ToArray());
					resourcePath = Path.ChangeExtension(resourcePath, null);
					return resourcePath;
					
				}
				
				return null;
				
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		/// <summary>
		/// <see cref="ResourceMap.GetResourceReference" />
		/// </summary>
		public static ResourceReference GetResourceReference(string assetPath) {
			#if UNITY_EDITOR
				ResourceReference reference = ScriptableObject.CreateInstance<ResourceReference>();
				reference.Guid = AssetDatabase.AssetPathToGUID(assetPath);
				return reference;
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The ResourceMapPlayerAdaptor is used to get information from the resource map asset.
	/// </summary>
	public static class ResourceMapPlayerAdaptor {
		
		
		#region Static Methods
		
		/// <summary>
		/// <see cref="ResourceMap.Contains" />
		/// </summary>
		public static bool Contains(string assetGuid) {
			return ResourceMap.Instance.HasValueForGuid(assetGuid);
		}
		
		/// <summary>
		/// <see cref="ResourceMap.GetAssetBundleName" />
		/// </summary>
		public static string GetAssetBundleName(string assetGuid) {
			return ResourceMap.Instance.ValueForGuid(assetGuid).AssetBundleName;
		}
		
		/// <summary>
		/// <see cref="ResourceMap.GetAssetPath" />
		/// </summary>
		public static string GetAssetPath(string assetGuid) {
			return ResourceMap.Instance.ValueForGuid(assetGuid).AssetPath;
		}
		
		/// <summary>
		/// <see cref="ResourceMap.GetResourcePath" />
		/// </summary>
		public static string GetResourcePath(string assetGuid) {
			return  ResourceMap.Instance.ValueForGuid(assetGuid).ResourcePath;
		}
		
		/// <summary>
		/// <see cref="ResourceMap.GetResourceReference" />
		/// </summary>
		public static ResourceReference GetResourceReference(string assetPath) {
			ResourceReference reference = ScriptableObject.CreateInstance<ResourceReference>();
			reference.Guid = ResourceMap.Instance.ValueForAssetPath(assetPath).Guid;
			return reference;
		}
		
		#endregion
		
		
	}
	
	
}