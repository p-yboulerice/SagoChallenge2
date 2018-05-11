package com.sagosago.sagobiz;

import android.util.Log;

public class SagoBizDebug {
	
	private static boolean isDebug = false;
	public static final String debugPrefix = "SAGO->";
	
	// Called externally from Unity Engine
	public static void enableDebugging() {
		isDebug = true;
	}

	// Called externally from Unity Engine
	public static void disableDebugging() {
		isDebug = false;
	}
	
	public static boolean isDebuggingEnabled() {
		return isDebug;
	}
	
	public static void log(String tag, String message) {
		if (isDebug) {
			Log.d(debugPrefix + tag, message);
		}
	}

	public static void logWarning(String tag, String message){
		if(isDebug){
			Log.w(debugPrefix + tag, message);
		}
	}

	public static void logError(String tag, String message) {
		if (isDebug) {
			Log.e(debugPrefix + tag, message);
		}
	}
	
	public static void logErrorWithException(String tag, String message, Exception ex) {
		if (isDebug) {
			Log.e(debugPrefix + tag, message, ex);
		}
	}
}
