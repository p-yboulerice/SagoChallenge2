namespace SagoApp {

	using SagoDebug;
	
	/// <summary>
	/// Static class which handles email interactions on the current build target.
	/// </summary>
	public static class SendEmailUtil {
		
		private static ISendEmailPlatform Platform;
		
		static SendEmailUtil() {
			
#if SAGO_IOS && !UNITY_EDITOR
			Platform = new SendEmailiOSPlatform();
#elif SAGO_GOOGLE_PLAY && !UNITY_EDITOR
			Platform = new SendEmailAndroidPlatform();
#else
			Platform = new SendEmailMockPlatform();
#endif
		}
		
		/// <summary>
		/// Spawns an email client with the supplied contents. Contents of the email are generated from the supplied parameters and bug report.
		/// The bug report data is placed into an attached text file. 
		/// </summary>
		/// <param name="recipientEmail">The recipient's email address.</param>
		/// <param name="title">Title of the email.</param>
		/// <param name="body">Body contents of the email.</param>
		/// <param name="report">BugReport Bug report string which will be inserted into an attached text file.</param>
		public static void SendEmailWithBugReport(string recipientEmail, string title, string body, BugReport report) {
			Platform.SendEmailWithBugReport(recipientEmail, title, body, report);
		}
	}
}