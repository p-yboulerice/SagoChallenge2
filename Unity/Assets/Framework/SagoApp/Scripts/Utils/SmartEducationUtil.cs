namespace SagoApp {
	
	using UnityEngine;
	
	public class SmartEducationUtil : MonoBehaviour {

		#region Static Methods
		public static void CheckSubscriptionStatus() {
			#if SAGO_SMART_EDUCATION && !UNITY_EDITOR

				var ajc = new AndroidJavaClass("com.sagosago.sagoapp.EggUserManager");
				AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				ajc.CallStatic("setSubscriptionCheck", true);	

				bool displayAlert = true;
				ajc.CallStatic("startCheck", displayAlert, activity);
				ajc.CallStatic<bool>("getSubscriptionStatus");
			#endif
		}
		
		#endregion
		
	}
	
}
