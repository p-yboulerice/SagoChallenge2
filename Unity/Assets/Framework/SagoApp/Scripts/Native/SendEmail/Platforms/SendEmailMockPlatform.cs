namespace SagoApp {
	
	using SagoDebug;
	
	internal class SendEmailMockPlatform : ISendEmailPlatform {
		
		public void SendEmailWithBugReport(string recipientEmail, string title, string body, BugReport report) {
			UnityEngine.Debug.Log(string.Format("Sending email Mock Platform\nRecipientEmail: {0}\ntitle: {1}\nbody: {2}\nbugReport: {3}", recipientEmail, title, body, report.ToJsonString()));
		}
	}
}