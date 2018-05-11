namespace SagoCore.Submodules {
	
	using System.IO;
	using System.Linq;
	using UnityEngine;
	
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	
	/// <summary>
	/// The SubmoduleMapElement struct contains metadata about one submodule.
	/// </summary>
	[System.Serializable]
	public struct SubmoduleMapElement {
		
		
		#region Fields
		
		/// <summary>
		/// The name of the submodule.
		/// </summary>
		[SerializeField]
		public string SubmoduleName;
		
		/// <summary>
		/// The path of the submodule.
		/// </summary>
		[SerializeField]
		public string SubmodulePath;
		
		/// <summary>
		/// The type of the submodule.
		/// </summary>
		[SerializeField]
		public string SubmoduleType;
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The SubmoduleMapMode enum defines the modes for the <see cref="SubmoduleMap" />.
	/// </summary>
	[System.Serializable]
	public enum SubmoduleMapMode {
		Editor,
		Player
	}
	
	/// <summary>
	/// The SubmoduleMap stores metadata about all of the submodules in a project.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="SubmoduleMap" /> asset lives in the project repo because the 
	/// metadata it contains is project-specific (i.e. submodule paths may be different 
	/// from project to project).
	/// </para>
	/// <para>
	/// The <see cref="SubmoduleMap" /> asset lives in <c>Assets/Project/Resources</c> 
	/// to make sure it is always compiled into the app and available at runtime via 
	/// <see cref="Resources.Load" /> where it can be used to build relative paths.
	/// </para>
	/// </remarks>
	[System.Serializable]
	public class SubmoduleMap : ScriptableObject {
		
		
		#region Constants
		
		/// <summary>
		/// Defines keys used to get and set values in the <see cref="EditorPrefs" />.
		/// </summary>
		public static class EditorPrefsKey {
			
			public const string Mode = "SagoCore.Submodules.SubmoduleMap.Mode";
			
		}
		
		#endregion
		
		
		#region Static Properties
		
		/// <summary>
		/// Gets the <see cref="SubmoduleMap" /> instance.
		/// </summary>
		public static SubmoduleMap Instance {
			get {
				SubmoduleMap instance = Resources.Load<SubmoduleMap>("SubmoduleMap");
				if (instance == null) {
					throw new System.InvalidOperationException("SubmoduleMap not found.");
				}
				return instance;
			}
		}
		
		/// <summary>
		/// Gets and sets the mode.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="SubmoduleMapMode.Editor" /> mode is used to bypass the 
		/// submodule map asset in the editor. This mode allows the submodule map
		/// class to work without constantly updating the asset during development.
		/// </para>
		/// </remarks>
		public static SubmoduleMapMode Mode {
			get {
				#if UNITY_EDITOR
					return (SubmoduleMapMode)EditorPrefs.GetInt(EditorPrefsKey.Mode, (int)SubmoduleMapMode.Editor);
				#else
					return SubmoduleMapMode.Player;
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
		
		/// <summary>
		/// Gets the array of all submodule types in the project.
		/// </summary>
		public static System.Type[] SubmoduleTypes {
			get {
				return (
					Mode == SubmoduleMapMode.Editor ? 
					SubmoduleMapEditorAdaptor.SubmoduleTypes : 
					SubmoduleMapPlayerAdaptor.SubmoduleTypes
				);
			}
		}
		
		#endregion
		
		
		#region static Methods
		
		/// <summary>
		/// Gets the flag that indicates whether the submodule map contains data for the specified type.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="SubmoduleInfo" /> type for the submodule.
		/// </typeparam>
		public static bool Contains<T>() where T : SubmoduleInfo {
			return Contains(typeof(T));
		}
		
		/// <summary>
		/// Gets the flag that indicates whether the submodule map contains data for the specified type.
		/// </summary>
		/// <param name="type">
		/// The <see cref="SubmoduleInfo" /> type for the submodule.
		/// </param>
		public static bool Contains(System.Type type) {
			return (
				Mode == SubmoduleMapMode.Editor ? 
				SubmoduleMapEditorAdaptor.Contains(type) : 
				SubmoduleMapPlayerAdaptor.Contains(type)
			);
		}
		
		/// <summary>
		/// Gets the absolute path of the specified submodule.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="SubmoduleInfo" /> type for the submodule.
		/// </typeparam>
		public static string GetAbsoluteSubmodulePath<T>() where T : SubmoduleInfo {
			return GetAbsoluteSubmodulePath(typeof(T));
		}
		
		/// <summary>
		/// Gets the absolute path of the specified submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="SubmoduleInfo" /> type for the submodule.
		/// </param>
		public static string GetAbsoluteSubmodulePath(System.Type type) {
			return (
				Mode == SubmoduleMapMode.Editor ? 
				SubmoduleMapEditorAdaptor.GetAbsoluteSubmodulePath(type) : 
				SubmoduleMapPlayerAdaptor.GetAbsoluteSubmodulePath(type)
			);
		}
		
		/// <summary>
		/// Gets the name of the specified submodule.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="SubmoduleInfo" /> type for the submodule.
		/// </typeparam>
		public static string GetSubmoduleName<T>() where T : SubmoduleInfo {
			return GetSubmoduleName(typeof(T));
		}
		
		/// <summary>
		/// Gets the name of the specified submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="SubmoduleInfo" /> type for the submodule.
		/// </param>
		public static string GetSubmoduleName(System.Type type) {
			return (
				Mode == SubmoduleMapMode.Editor ? 
				SubmoduleMapEditorAdaptor.GetSubmoduleName(type) : 
				SubmoduleMapPlayerAdaptor.GetSubmoduleName(type)
			);
		}
		
		/// <summary>
		/// Gets the path of the specified submodule.
		/// </summary>
		/// <typeparam name="T">
		/// The <see cref="SubmoduleInfo" /> type for the submodule.
		/// </typeparam>
		public static string GetSubmodulePath<T>() where T : SubmoduleInfo {
			return GetSubmodulePath(typeof(T));
		}
		
		/// <summary>
		/// Gets the path of the specified submodule.
		/// </summary>
		/// <param name="type">
		/// The <see cref="SubmoduleInfo" /> type for the submodule.
		/// </param>
		public static string GetSubmodulePath(System.Type type) {
			return (
				Mode == SubmoduleMapMode.Editor ? 
				SubmoduleMapEditorAdaptor.GetSubmodulePath(type) : 
				SubmoduleMapPlayerAdaptor.GetSubmodulePath(type)
			);
		}
		
		/// <summary>
		/// Gets the <see cref="SubmoduleInfo" /> type for the submodule that contains the specified path.
		/// </summary>
		/// <param name="path">
		/// The path.
		/// </param>
		public static System.Type GetSubmoduleType(string path) {
			return (
				Mode == SubmoduleMapMode.Editor ? 
				SubmoduleMapEditorAdaptor.GetSubmoduleType(path) : 
				SubmoduleMapPlayerAdaptor.GetSubmoduleType(path)
			);
		}
		
		#endregion
		
		
		#region Fields
		
		/// <summary>
		/// The array of elements.
		/// </summary>
		[SerializeField]
		private SubmoduleMapElement[] m_Elements;
		
		#endregion
		
		
		#region Properties
		
		/// <summary>
		/// Gets the array of elements.
		/// </summary>
		public SubmoduleMapElement[] Elements {
			get { return m_Elements; }
		}
		
		#endregion
		
		
		#region Methods
		
		/// <summary>
		/// Resets the submodule map.
		/// </summary>
		private void Reset() {
			m_Elements = new SubmoduleMapElement[0];
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The SubmoduleMapEditorAdaptor can be used in the editor to bypass 
	/// the submodule map asset and get information directly from the 
	/// asset database. That way, code that depends on the submodule map 
	/// will work in the editor, even if the asset has not been updated.
	/// </summary>
	public static class SubmoduleMapEditorAdaptor {
		
		
		#region Static Properties
		
		/// <summary>
		/// <see cref="SubmoduleMap.SubmoduleTypes" />
		/// </summary>
		public static System.Type[] SubmoduleTypes {
			get {
				#if UNITY_EDITOR
					return (
						System
						.AppDomain
						.CurrentDomain
						.GetAssemblies()
						.SelectMany(assembly => assembly.GetTypes())
						.Where(type => type.IsSubclassOf(typeof(SubmoduleInfo)) && !type.IsAbstract)
						.ToArray()
					);
				#else
					throw new System.InvalidOperationException();
				#endif
			}
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// <see cref="SubmoduleMap.Contains" />
		/// </summary>
		public static bool Contains<T>() {
			return Contains(typeof(T));
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.Contains" />
		/// </summary>
		public static bool Contains(System.Type type) {
			#if UNITY_EDITOR
				return System.Array.IndexOf(SubmoduleTypes, type) != -1;
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetAbsoluteSubmodulePath" />
		/// </summary>
		public static string GetAbsoluteSubmodulePath<T>() {
			return GetAbsoluteSubmodulePath(typeof(T));
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetAbsoluteSubmodulePath" />
		/// </summary>
		public static string GetAbsoluteSubmodulePath(System.Type type) {
			#if UNITY_EDITOR
				return Path.Combine(Path.GetDirectoryName(Application.dataPath), GetSubmodulePath(type));
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetSubmoduleName" />
		/// </summary>
		public static string GetSubmoduleName<T>() {
			return GetSubmoduleName(typeof(T));
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetSubmoduleName" />
		/// </summary>
		public static string GetSubmoduleName(System.Type type) {
			#if UNITY_EDITOR
				return type.Namespace;
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetSubmodulePath" />
		/// </summary>
		public static string GetSubmodulePath<T>() {
			return GetSubmodulePath(typeof(T));
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetSubmodulePath" />
		/// </summary>
		public static string GetSubmodulePath(System.Type type) {
			#if UNITY_EDITOR
				ScriptableObject instance = ScriptableObject.CreateInstance(type);
				MonoScript asset = MonoScript.FromScriptableObject(instance);
				string assetPath = AssetDatabase.GetAssetPath(asset);
				return Path.GetDirectoryName(Path.GetDirectoryName(assetPath));
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetSubmoduleType" />
		/// </summary>
		public static System.Type GetSubmoduleType(string path) {
			#if UNITY_EDITOR
				return (
					SubmoduleTypes
					.Where(type => path.StartsWith(GetSubmodulePath(type)))
					.FirstOrDefault()
				);
			#else
				throw new System.InvalidOperationException();
			#endif
		}
		
		#endregion
		
		
	}
	
	/// <summary>
	/// The SubmoduleMapPlayerAdaptor is used to get information from the submodule map asset.
	/// </summary>
	public static class SubmoduleMapPlayerAdaptor {
		
		
		#region Static Properties
		
		/// <summary>
		/// <see cref="SubmoduleMap.SubmoduleTypes" />
		/// </summary>
		public static System.Type[] SubmoduleTypes {
			get { return SubmoduleMap.Instance.Elements.Select(e => System.Type.GetType(e.SubmoduleType)).ToArray(); }
		}
		
		#endregion
		
		
		#region Static Methods
		
		/// <summary>
		/// <see cref="SubmoduleMap.Contains" />
		/// </summary>
		public static bool Contains<T>() {
			return Contains(typeof(T));
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.Contains" />
		/// </summary>
		public static bool Contains(System.Type type) {
			return SubmoduleMap.Instance.Elements.Where(e => e.SubmoduleType.Equals(type.FullName)).Count() != 0;
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetAbsoluteSubmodulePath" />
		/// </summary>
		public static string GetAbsoluteSubmodulePath<T>() {
			return GetAbsoluteSubmodulePath(typeof(T));
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetAbsoluteSubmodulePath" />
		/// </summary>
		public static string GetAbsoluteSubmodulePath(System.Type type) {
			return Path.Combine(Path.GetDirectoryName(Application.dataPath), GetSubmodulePath(type));
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetSubmoduleName" />
		/// </summary>
		public static string GetSubmoduleName<T>() {
			return GetSubmoduleName(typeof(T));
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetSubmoduleName" />
		/// </summary>
		public static string GetSubmoduleName(System.Type type) {
			return (
				SubmoduleMap
				.Instance
				.Elements
				.Where(e => e.SubmoduleType.Equals(type.FullName))
				.FirstOrDefault()
				.SubmoduleName
			);
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetSubmodulePath" />
		/// </summary>
		public static string GetSubmodulePath<T>() {
			return GetSubmodulePath(typeof(T));

		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetSubmodulePath" />
		/// </summary>
		public static string GetSubmodulePath(System.Type type) {
			return (
				SubmoduleMap
				.Instance
				.Elements
				.Where(e => e.SubmoduleType.Equals(type.FullName))
				.FirstOrDefault()
				.SubmodulePath
			);
		}
		
		/// <summary>
		/// <see cref="SubmoduleMap.GetSubmoduleType" />
		/// </summary>
		public static System.Type GetSubmoduleType(string path) {
			
			SubmoduleMapElement element = (
				SubmoduleMap
				.Instance
				.Elements
				.Where(e => path.StartsWith(e.SubmodulePath))
				.FirstOrDefault()
			);
			
			if (!string.IsNullOrEmpty(element.SubmoduleType)) {
				return System.Type.GetType(element.SubmoduleType);
			}
			
			return null;
			
		}
		
		#endregion
		
		
	}
	
}