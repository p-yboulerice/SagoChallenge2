namespace SagoBiz {

	using UnityDebug = UnityEngine.Debug;
	/// <summary>
	/// A class for centerializing the debug messages in SagoBiz.
	/// </summary>
	public class DebugUtil {

		/// <summary>
		/// The prefix for the log, as oppose to SagoBizNative.
		/// </summary>
		public static readonly string prefix = "SagoBizUnity->"; 
			
		public static void Log(string message) {
			#if SAGO_BIZ_DEBUG
				UnityDebug.Log(message);
			#endif
		}

		public static void Log(string message, UnityEngine.Object context) {
			#if SAGO_BIZ_DEBUG
				UnityDebug.Log(message, context);
			#endif
		}

		public static void LogError(string message) {
			#if SAGO_BIZ_DEBUG
				UnityDebug.LogError(message);
			#endif
		}

		public static void LogError(string message, UnityEngine.Object context) {
			#if SAGO_BIZ_DEBUG
				UnityDebug.LogError(message, context);
			#endif
		}

		public static void LogWarning(string message) {
			#if SAGO_BIZ_DEBUG
				UnityDebug.LogWarning(message);
			#endif
		}

		public static void LogWarning(string message, UnityEngine.Object context) {
			#if SAGO_BIZ_DEBUG
				UnityDebug.LogWarning(message, context);
			#endif
		}

		public static void LogException(System.Exception exception) {
			#if SAGO_BIZ_DEBUG
				UnityDebug.LogException(exception);
			#endif
		}

		public static void LogException(System.Exception exception, UnityEngine.Object context) {
			#if SAGO_BIZ_DEBUG
				UnityDebug.LogException(exception, context);
			#endif
		}

	}

}