namespace SagoApp {
	
#if SAGO_IOS
	
	using SagoDebug;
	using System.Runtime.InteropServices;
	
	internal class SendEmailiOSPlatform : ISendEmailPlatform {
		
		[DllImport("__Internal")]
		private static extern void _SendEmailWithBugReport(string recipientEmail, string title, string body, string bugReport);
		
		public void SendEmailWithBugReport(string recipientEmail, string title, string body, BugReport report) {
			_SendEmailWithBugReport(recipientEmail, title, body, report.ToJsonString());
		}
	}
	
#endif
}