namespace SagoPlatform {
	
	using System.Collections.Generic;
	using UnityEngine;
	
	public class PlatformSettingsMultiplexer : MonoBehaviour {
		
		
		#region Singleton
		
		/// <summary>
		/// The singleton instance of the platform settings prefab.
		/// </summary>
		private static PlatformSettingsMultiplexer _Instance;
		
		/// <summary>
		/// The flag that indicates whether the application is quitting or not, 
		/// used to prevent loading and instantiating the singleton while the 
		/// app is quitting. 
		///
		/// When the application quits, objects are destroyed in an indeterminate 
		/// order. If the controller is destroyed and then another object tries 
		/// to access it, a new controller instance would be created but not 
		/// cleaned up (leaving a copy in the hierarchy in the editor).
		/// </summary>
		private static bool _IsQuitting;
		
		/// <summary>
		/// Gets the singleton instance of the platform settings prefab.
		/// </summary>
		public static PlatformSettingsMultiplexer Instance {
			get {
				if (_Instance == null && _IsQuitting == false) {
					_Instance = Resources.Load(
						"PlatformSettingsMultiplexer", 
						typeof(PlatformSettingsMultiplexer)
					) as PlatformSettingsMultiplexer;
					if (Application.isPlaying) {
						DontDestroyOnLoad(_Instance);
					}
				}
				return _Instance;
			}
		}
		
		/// <summary>
		/// Sets the <see cref="_IsQuitting" /> flag when the application quits.
		/// </summary>
		private void OnApplicationQuit() {
			_IsQuitting = true;
		}
		
		#endregion
		
		
		#region Fields
		
		[SerializeField]
		protected List<PlatformSettingsPrefab> m_Prefabs;
		
		#endregion
		
		
		#region Public Methods
		
		public PlatformSettingsPrefab GetPrefab() {
			return this.GetPrefab(PlatformUtil.ActivePlatform);
		}
		
		public PlatformSettingsPrefab GetPrefab(Platform platform) {
			this.Normalize();
			return m_Prefabs[(int)platform];
		}
		
		public void SetPrefab(Platform platform, PlatformSettingsPrefab prefab) {
			this.Normalize();
			m_Prefabs[(int)platform] = prefab;
			#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(this);
			#endif
		}
		
		public T GetPrefabComponent<T>() where T : Component {
			return GetPrefabComponent<T>(PlatformUtil.ActivePlatform);
		}
		
		public T GetPrefabComponent<T>(Platform platform) where T : Component {
			
			PlatformSettingsPrefab prefab;
			prefab = GetPrefab(platform);
			
			T component;
			component = prefab ? prefab.GetComponent<T>() : null;
			
			return component;
			
		}
		
		#endregion
		
		
		#region Helper Methods
		
		void Normalize() {
			
			Platform[] platforms;
			platforms = PlatformUtil.AllPlatforms;
			
			if (m_Prefabs == null) {
				m_Prefabs = new List<PlatformSettingsPrefab>();
				#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(this);
				#endif
			}
			
			while (m_Prefabs.Count < platforms.Length) {
				m_Prefabs.Add(null);
				#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(this);
				#endif
			}
			
			while (m_Prefabs.Count > platforms.Length) {
				m_Prefabs.RemoveAt(m_Prefabs.Count - 1);
				#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(this);
				#endif
			}
			
		}
		
		#endregion
		
		
	}
	
}