namespace SagoApp {
	
	using SagoApp.Project;
	using SagoCore.Scenes;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	
	public class BackButtonController : MonoBehaviour {
		
		//
		// Singleton
		//
		public static BackButtonController Instance {
			get {
				if (s_Instance == null && s_IsQuitting == false) {
					s_Instance =  new GameObject().AddComponent<BackButtonController>();
					s_Instance.name = "BackButtonController";
					DontDestroyOnLoad(s_Instance);
				}
				return s_Instance;
			}
		}

		public bool ShouldEnable {
			get { return m_ShouldEnable; }
			set { m_ShouldEnable = value; }
		}

		//
		// Member Fields
		//
		private bool m_ShouldEnable;


		//
		// Static Variables
		//
		private static BackButtonController s_Instance;
		private static bool s_IsQuitting;
		
		void OnApplicationQuit() {
			s_IsQuitting = true;
		}

		void Update () {
			#if !UNITY_IOS && !UNITY_EDITOR
			if (ShouldEnable && ProjectNavigator.Instance && ProjectNavigator.Instance.IsReady) {
					
					// android and windows back button functionality
					// takes the user back to the home scene or quits 
					// the app if already at the home scene
					
					if (Input.GetKeyDown(KeyCode.Escape)) {
						
						SceneReference sceneReference;
						sceneReference = SagoNavigation.SceneNavigator.Instance.CurrentScene;
						
						Scene scene;
						scene = SceneManager.GetSceneByPath(sceneReference.AssetPath);

						if (scene.IsValid() && scene.buildIndex != -1) {
							Application.Quit();
						} else {
							ProjectNavigator.Instance.NavigateToProject();
						}
						
					}
				}
			#endif
		}
	}
}
