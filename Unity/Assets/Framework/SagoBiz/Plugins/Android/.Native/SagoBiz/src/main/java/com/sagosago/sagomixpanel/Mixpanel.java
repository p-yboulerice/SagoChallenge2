package com.sagosago.sagomixpanel;

import org.json.JSONException;
import org.json.JSONObject;

import android.app.Activity;

import com.mixpanel.android.mpmetrics.MixpanelAPI;
import com.sagosago.sagobiz.SagoBizDebug;

public class Mixpanel {

	public static final String LOG_TAG = "Mixpanel";
	
	protected static MixpanelAPI mixpanel;
	protected static boolean initialized;
	
	protected static boolean isInitialized() {
		if (!initialized) {
			SagoBizDebug.log(LOG_TAG, "Mixpanel is not initialized.");
			return false;
		} else {
			return true;
		}
	}
	
	public static void initializeMixPanelWithToken(Activity activity, String token) {
		if (!initialized || mixpanel == null) {
			SagoBizDebug.log(LOG_TAG, "Initializing mixpanel native..");
			mixpanel = MixpanelAPI.getInstance(activity, token);
			initialized = true;
		}
	}
	
	public static void setDistinctID(String distinctID) {
		if (isInitialized()) {
			SagoBizDebug.log(LOG_TAG, "Setting mixpanel distinct id..");
			mixpanel.identify(distinctID);
		}
	}
	
	public static String getDistinctID() {
		if (isInitialized()) {
			SagoBizDebug.log(LOG_TAG, "Getting mixpanel distinct id..");
			return mixpanel.getDistinctId();
		} else {
			return "";
		}
	}
	
	public static void registerSuperProperties(String propertiesJson) {
		if (isInitialized()) {
			try {
				if (propertiesJson != null && !propertiesJson.equalsIgnoreCase("")) {
					JSONObject properties = new JSONObject(propertiesJson);
					SagoBizDebug.log(LOG_TAG, "Mixpanel super properties were received.\nSetting mixpanel super properties..");
					mixpanel.registerSuperProperties(properties);
				}
			} catch (JSONException e) {
				SagoBizDebug.logErrorWithException(LOG_TAG, "JSONException", e);
			}
		}
	}
	
	public static void trackEvent(String eventName) {
		if (isInitialized()) {
			//  @"Mixpanel - Track Event - Event='%@' Properties='%@",
            // event,
            // properties
			SagoBizDebug.log(LOG_TAG, String.format("Track Event - Event='%s' ", eventName));
			mixpanel.track(eventName);
		}
	}
	
	public static void trackEventWithProperties(String eventName, String propertiesJson) {
		if (isInitialized()) {
			try {
				SagoBizDebug.log(LOG_TAG, "properties json is " + propertiesJson);
				if (propertiesJson != null && !propertiesJson.equalsIgnoreCase("")) {
					JSONObject properties = new JSONObject(propertiesJson);
					SagoBizDebug.log(LOG_TAG, String.format("Track Event - Event='%s' Properties='%s", eventName, properties));
					mixpanel.track(eventName, properties);
				} else {
					mixpanel.track(eventName);
				}
			} catch (JSONException e) {
				SagoBizDebug.logErrorWithException(LOG_TAG, "JSONException", e);
			}
		}
	}

	public static void timeEvent(String eventName) {
		if (isInitialized()) {
			SagoBizDebug.log(LOG_TAG, String.format("Time Event - Event='%s'", eventName));
			mixpanel.timeEvent(eventName);
		}
	}

	public static void trackEventWithPropertiesJSONObject(String eventName, JSONObject properties) {
		if (isInitialized()) {
			SagoBizDebug.log(LOG_TAG, "properties json is " + properties);
			if (properties != null ) {
				SagoBizDebug.log(LOG_TAG, String.format("Track Event - Event='%s' Properties='%s", eventName, properties));
				mixpanel.track(eventName, properties);
			} else {
				mixpanel.track(eventName);
			}
		}
	}
}
