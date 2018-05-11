namespace SagoCore.EventSystems {
	
	using UnityEngine;
	using UnityEngine.EventSystems;
	
	public static class EventSystemProvider {
		
		
		#region Static Fields
		
		private static EventSystem _EventSystem;
		
		#endregion
		
		
		#region Static Properties
		
		public static EventSystem EventSystem {
			get {
				
				#if UNITY_EDITOR
				if (UnityEditor.EditorApplication.isPlaying && UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) {
				#endif
				
					if (_EventSystem == null) {
						EventSystem prefab = Resources.Load<EventSystem>("EventSystem");
						if (prefab != null) {
							_EventSystem = Object.Instantiate(prefab) as EventSystem;
							Resources.UnloadAsset(prefab);
						}
					}
					
					if (_EventSystem == null) {
						GameObject gameObject = new GameObject("EventSystem");
						gameObject.AddComponent<EventSystem>();
						gameObject.AddComponent<StandaloneInputModule>();
						_EventSystem = gameObject.GetComponent<EventSystem>();
					}
				
					Object.DontDestroyOnLoad(_EventSystem);
					
				#if UNITY_EDITOR
				}
				#endif
				
				return _EventSystem;
				
			}
		}
		
		#endregion
		
		
		#region Static Methods
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeOnLoad() {
			if (EventSystem) {
				// call the accessor on load to create an event system
			}
		}
		
		#endregion
		
		
	}
	
}