namespace SagoApp {
	
	using UnityEngine;
	
	public class ImmersiveModeUtil : MonoBehaviour {

		#region Static Methods

		public static void RegisterWithSceneNavigator() {
			if (SagoNavigation.SceneNavigator.Instance) {
				SagoNavigation.SceneNavigator.Instance.OnWillNavigateToScene += (SagoNavigation.SceneController sceneController) => ActivateImmersiveMode();
			}
		}

		public static void ActivateImmersiveMode() {
			#if UNITY_ANDROID && !UNITY_EDITOR
				#if SAGO_GOOGLE_PLAY || SAGO_GOOGLE_PLAY_FREE|| SAGO_NABI 
					AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					AndroidJavaClass immersiveClass = new AndroidJavaClass("com.sagosago.sagoapp.Immersive");
					immersiveClass.CallStatic("activateImmersiveMode", activityClass.GetStatic<AndroidJavaObject>("currentActivity"));
				#endif
			#endif
		}
		
		#endregion
		
	}
	
}
