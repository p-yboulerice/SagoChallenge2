namespace SagoCore.Scenes {
	
	using SagoCore.AssetBundles;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using UnityEngine;
	
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	
	/// <summary>
	/// The SceneMapMode enum defines the modes for the <see cref="SceneMap" />.
	/// </summary>
	public enum SceneMapMode {
		Editor,
		Player
	}
	
	/// <summary>
	/// The SceneMapValue struct contains metadata about a scene.
	/// </summary>
	[System.Serializable]
	public struct SceneMapValue {
		
		/// <summary>
		/// The guid for the scene.
		/// </summary>
		[SerializeField]
		public string Guid;
		
		/// <summary>
		/// The asset bundle name for the scene (<c>null</c> if the scene is not in an asset bundle).
		/// </summary>
		[SerializeField]
		public string AssetBundleName;
		
		/// <summary>
		/// The asset path for the scene.
		/// </summary>
		[SerializeField]
		public string AssetPath;
		
	}
	
	/// <summary>
	/// The SceneMap class stores metadata about scenes.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="SceneMap" /> class exists to keep scene metadata (which 
	/// is project-specific) in our project repos (and not in our submodule repos, 
	/// where it could cause conficts). <see cref="SceneReference" /> objects 
	/// (which can live in submodule repos) only store a guid. The guid is used 
	/// to ask the <see cref="SceneMap" /> (which always lives in the project 
	/// repo) for the metadata about the referenced scene.
	/// </para>
	/// <para>
	/// The <see cref="SceneMap"> class lives in a <c>Resources</c> folder in 
	/// the project repo so that it always compiled into the application and 
	/// available at run time via <see cref="Resources.Load" />.
	/// </para>
	/// <para>
	/// The <see cref="SceneMap" /> asset is updated automatically every time 
	/// the asset database changes.
	/// </para>
	/// </remarks>
	public class SceneMap : ScriptableObject, ISerializationCallbackReceiver {
		
		
		#region Constants
		
		/// <summary>
		/// Defines keys used to get and set values in the <see cref="EditorPrefs" />.
		/// </summary>
		public static class EditorPrefsKey {
			
			public const string Mode = "SagoCore.Scenes.SceneMap.Mode";
			
		}
		
		#endregion
		
		
		#region Static Properties
		
		/// <summary>
		/// Gets the singleton <see cref="SceneMap" /> instance.
		/// </summary>
		public static SceneMap Instance {
			get {
				
				SceneMap instance;
				instance = Resources.Load("SceneMap", typeof(SceneMap)) as SceneMap;
				
				if (instance == null) {
					throw new System.InvalidOperationException("Could not load scene map.");
				}
				
				return instance;
				
			}
		}
		
		/// <summary>
		/// Gets and sets the mode.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="SceneMapMode.Editor" /> mode is used to bypass the 
		/// scene map asset in the editor. This mode allows the scene map
		/// class to work without constantly updating the asset during development.
		/// </para>
		/// </remarks>
		public static SceneMapMode Mode {
			get {
				#if UNITY_EDITOR
					return (SceneMapMode)EditorPrefs.GetInt(EditorPrefsKey.Mode, (int)SceneMapMode.Editor);
				#else
					return SceneMapMode.Player;
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
		/// Converts an asset path to a scene path.
		/// </summary>
		public static string AssetPathToScenePath(string assetPath) {
			if (!string.IsNullOrEmpty(assetPath)) {
				string[] components = (
					Path.ChangeExtension(assetPath, null)
					.Split(Path.DirectorySeparatorChar)
					.Skip(1)
					.TakeWhile(component => true)
					.ToArray()
				);
				return string.Join(Path.DirectorySeparatorChar.ToString(), components);
			}
			return null;
		}
		
		/// <summary>
		/// Gets a flag that indicates whether the scene map contains information about the specified scene.
		/// </summary>
		/// <param name="guid">
		/// The guid of the scene.
		/// </param>
		public static bool Contains(string guid) {
			return (
				Mode == SceneMapMode.Editor ? 
				SceneMapEditorAdaptor.Contains(guid) :
				SceneMapPlayerAdaptor.Contains(guid)
			);
		}
		
		/// <summary>
		/// Gets the asset bundle name for the specified scene.
		/// </summary>
		/// <param name="guid">
		/// The guid of the scene.
		/// </param>
		/// <returns>
		/// <c>null</c> if the scene is not in an asset bundle.
		/// </returns>
		public static string GetAssetBundleName(string guid) {
			return (
				Mode == SceneMapMode.Editor ? 
				SceneMapEditorAdaptor.GetAssetBundleName(guid) :
				SceneMapPlayerAdaptor.GetAssetBundleName(guid)
			);
		}
		
		/// <summary>
		/// Gets the asset path for the specified scene.
		/// </summary>
		/// <param name="guid">
		/// The guid of the scene.
		/// </param>
		public static string GetAssetPath(string guid) {
			return (
				Mode == SceneMapMode.Editor ? 
				SceneMapEditorAdaptor.GetAssetPath(guid) :
				SceneMapPlayerAdaptor.GetAssetPath(guid)
			);
		}
		
		/// <summary>
		/// Gets the scene path for the specified scene.
		/// </summary>
		/// <param name="guid">
		/// The guid of the scene.
		/// </param>
		/// <remarks>
		/// The scene path is different depending on where the scene is being loaded 
		/// from. When the scene is being loaded from an asset bundle <see cref="SceneManager.LoadScene" />
		/// only works if you pass the name of the scene. When the scene is being loaded 
		/// normally, <see cref="SceneManager.LoadScene" /> works if you pass the name 
		/// or path of the scene (as of Unity 5.3.4f1).
		/// </remarks>
		public static string GetScenePath(string guid) {
			return (
				Mode == SceneMapMode.Editor ? 
				SceneMapEditorAdaptor.GetScenePath(guid) :
				SceneMapPlayerAdaptor.GetScenePath(guid)
			);
		}
		
		/// <summary>
		/// Gets a scene reference for the specified asset path;
		/// </summary>
		/// <param name="assetPath">
		/// The asset path of the scene.
		/// </param>
		public static SceneReference GetSceneReference(string assetPath) {
			return (
				Mode == SceneMapMode.Editor ? 
				SceneMapEditorAdaptor.GetSceneReference(assetPath) :
				SceneMapPlayerAdaptor.GetSceneReference(assetPath)
			);
		}
		
		#endregion
		
		
		#region Fields
		
		/// <summary>
		/// The dictionary with the index of each guid and asset path in the <see cref="m_Values" /> array.
		/// </summary>
		[System.NonSerialized]
		private Dictionary<string,int> m_Indexes;
		
		/// <summary>
		/// The values (<see cref="SceneMapValue" /> structs with metadata about scenes).
		/// </summary>
		[SerializeField]
		private SceneMapValue[] m_Values;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets the array of values.
		/// </summary>
		public SceneMapValue[] Values {
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
		/// Checks if the scene map has a value for the specified asset path.
		/// </summary>
		public bool HasValueForAssetPath(string assetPath) {
			return !string.IsNullOrEmpty(ValueForAssetPath(assetPath).Guid);
		}
		
		/// <summary>
		/// Gets the value for the specified asset path.
		/// </summary>
		public SceneMapValue ValueForAssetPath(string assetPath) {
			int index = 0;
			bool success = m_Indexes != null && m_Indexes.TryGetValue(assetPath, out index);
			return success ? m_Values[index] : default(SceneMapValue);
		}
		
		/// <summary>
		/// Checks if the scene map has a value for the specified guid.
		/// </summary>
		public bool HasValueForGuid(string guid) {
			return !string.IsNullOrEmpty(ValueForGuid(guid).Guid);
		}
		
		/// <summary>
		/// Gets the value for the specified guid.
		/// </summary>
		public SceneMapValue ValueForGuid(string guid) {
			int index = 0;
			bool success = m_Indexes != null && m_Indexes.TryGetValue(guid, out index);
			return success ? m_Values[index] : default(SceneMapValue);
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Resets the scene map.
		/// </summary>
		private void Reset() {
			m_Values = new SceneMapValue[0];
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The SceneMapEditorAdaptor can be used in the editor to bypass 
	/// the scene map asset and get information directly from the 
	/// asset database. That way, code that depends on the scene map 
	/// will work in the editor, even if the asset has not been updated.
	/// </summary>
	public static class SceneMapEditorAdaptor {
		
		
		#region Static Methods
		
		/// <summary>
		/// <see cref="SubmoduleMap.Contains" />
		/// </param>
		public static bool Contains(string guid) {
			#if UNITY_EDITOR
				string path = AssetDatabase.GUIDToAssetPath(guid);
				return !string.IsNullOrEmpty(path) && path.EndsWith(".unity");
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetAssetBundleName" />
		/// </summary>
		public static string GetAssetBundleName(string guid) {
			#if UNITY_EDITOR
				if (Contains(guid)) {
					
					string path;
					path = AssetDatabase.GUIDToAssetPath(guid);
					
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
		/// <see cref="SubmoduleMap.GetAssetPath" />
		/// </summary>
		public static string GetAssetPath(string guid) {
			#if UNITY_EDITOR
				return Contains(guid) ? AssetDatabase.GUIDToAssetPath(guid) : null;
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetScenePath" />
		/// </summary>
		public static string GetScenePath(string guid) {
			#if UNITY_EDITOR
				return Contains(guid) ? SceneMap.AssetPathToScenePath(GetAssetPath(guid)) : null;
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetSceneReference" />
		/// </summary>
		public static SceneReference GetSceneReference(string scenePath) {
			#if UNITY_EDITOR
				SceneReference reference = new SceneReference();
				reference.Guid = AssetDatabase.AssetPathToGUID(scenePath);
				return reference;
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The SceneMapPlayerAdaptor is used to get information from the scene map asset.
	/// </summary>
	public static class SceneMapPlayerAdaptor {
		
		
		#region Static Methods
		
		/// <summary>
		/// <see cref="SceneMap.Contains" />
		/// </summary>
		public static bool Contains(string guid) {
			return SceneMap.Instance.HasValueForGuid(guid);
		}
		
		/// <summary>
		/// <see cref="SceneMap.GetAssetBundleName" />
		/// </summary>
		public static string GetAssetBundleName(string guid) {
			return SceneMap.Instance.ValueForGuid(guid).AssetBundleName;
		}
		
		/// <summary>
		/// <see cref="SceneMap.GetAssetPath" />
		/// </summary>
		public static string GetAssetPath(string guid) {
			return SceneMap.Instance.ValueForGuid(guid).AssetPath;
		}
		
		/// <summary>
		/// <see cref="SceneMap.GetScenePath" />
		/// </summary>
		public static string GetScenePath(string guid) {
			return SceneMap.AssetPathToScenePath(GetAssetPath(guid));
		}
		
		/// <summary>
		/// <see cref="SceneMap.GetSceneReference" />
		/// </summary>
		public static SceneReference GetSceneReference(string assetPath) {
			SceneReference reference = new SceneReference();
			reference.Guid = SceneMap.Instance.ValueForAssetPath(assetPath).Guid;
			return reference;
		}
		
		#endregion
		
		
	}
	
}