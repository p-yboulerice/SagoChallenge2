namespace SagoApp {
	
	using SagoDebug;
	
	/// <summary>
	/// Interface file for defining a send email implementation for a build target.
	/// </summary>
	internal interface ISendEmailPlatform {
		
		/// <summary>
		/// Spawns an email client with the supplied contents. Contents of the email are generated from the supplied parameters and bug report.
		/// The bug report data is placed into an attached text file. 
		/// </summary>
		/// <param name="recipientEmail">The recipient's email address.</param>
		/// <param name="title">Title of the email.</param>
		/// <param name="body">Body contents of the email.</param>
		/// <param name="report">BugReport Bug report string which will be inserted into an attached text file.</param>
		void SendEmailWithBugReport(string recipientEmail, string title, string body, BugReport report);
	}
}