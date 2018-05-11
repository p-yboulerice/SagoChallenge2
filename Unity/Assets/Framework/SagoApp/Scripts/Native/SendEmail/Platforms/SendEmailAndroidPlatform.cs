namespace SagoApp {
	
#if SAGO_GOOGLE_PLAY
	
	using UnityEngine;
	using SagoDebug;
	
	internal class SendEmailAndroidPlatform : ISendEmailPlatform {

		public void SendEmailWithBugReport(string recipientEmail, string title, string body, BugReport report) {
			
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
				using (AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
					using (AndroidJavaObject sendEmailNativeImplementation = new AndroidJavaObject("com.sagosago.sagoapp.SendEmailUtil")) {
						sendEmailNativeImplementation.CallStatic("_SendEmailWithBugReport", unityActivity, recipientEmail, title, body, report.ToJsonString());
					}
				}
			}
		}
	}
	
#endif
}