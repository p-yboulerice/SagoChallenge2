namespace SagoDebug {

	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Analytics;

	public static class CrashLogger {

		#if SAGO_DEBUG

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize() {

			UnityEngine.Debug.Log("SagoDebug.CrashLogger: hooking up custom exception handler");

			Application.logMessageReceived += HandleLogMessage;
			Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);

			var lastReport = CrashReport.lastReport;
			if (lastReport != null) {
				UnityEngine.Debug.LogFormat("SagoDebug.CrashLogger: lastReport: {0}\n{1}", lastReport.time, lastReport.text);

				Analytics.CustomEvent("CrashReport.lastReport", new Dictionary<string, object> {
					{ "time", lastReport.time },
					{ "text", lastReport.text }
				});

				lastReport.Remove();
			}

			CrashTest.Create();
		}

		private static void HandleLogMessage(string message, string stackTrace, LogType type) {
			if (type == LogType.Exception) {
				Analytics.CustomEvent("Exception", new Dictionary<string, object> {
					{ "type", type.ToString() },
					{ "message", message },
					{ "stackTrace", stackTrace }
				});
			}
		}

		#endif

	}
}