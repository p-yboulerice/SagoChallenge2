package com.sagosago.sagobiz;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.ActivityInfo;
import android.content.pm.ResolveInfo;
import android.net.Uri;
import android.util.Base64;
import android.webkit.JavascriptInterface;

import org.json.*;

import com.sagosago.sagomixpanel.Mixpanel;

public class WebViewJavascriptInterface implements Serializable {
	
	public enum Platform {
		Unknown,
		GooglePlay,
		Kindle,
		KindleFreeTime,
		Nabi,
		AmazonTeal
	}
	
	public enum ActionType {
	    actionTypeClose,
	    actionTypeEvent,
	    actionTypeLink,
	    actionTypeStore,
	    actionTypeVideo,
	    actionTypePrefs,
	    actionTypeOther,
	    actionTypeInvalid
	}
	
	private static final long serialVersionUID = 465489766;
	
	private static final String LOG_TAG = "WebView JavaScript Interface";
	
	private static final String itunesUrl = "itunes_url";
	private static final String googleplayUrl = "googleplay_url";
	private static final String kindleUrl = "kindle_url";
	private static final String webUrl = "web_url";
	
	private static final String sagoBizWebClientPreferencesKey = "sagoBizWebClientPreferences";
	
	/**
	 * This is the name that will be available from the javascript code. 
	 */
	protected static final String interfaceName = "SagoBiz";
	protected WebView webView;
	
	public WebViewJavascriptInterface(WebView webView) {
		this.webView = webView;
	}
	
	public WebViewJavascriptInterface() {
	}
	
	
	public String getInterfaceName() {
		return interfaceName;
	}
	
	public String getWebClientPreferences() {
		SharedPreferences packagePrefs = webView.activity.getSharedPreferences( webView.activity.getPackageName(), Context.MODE_PRIVATE);
    	Map<String, ?> allValues = packagePrefs.getAll();
    	
    	for (Map.Entry<String, ?> entry : allValues.entrySet())
    	{
    	    System.out.println(entry.getKey() + "/" + entry.getValue());
    	}
    	
    	String result = packagePrefs.getString(sagoBizWebClientPreferencesKey, "");
    	if (!result.equalsIgnoreCase("")) {
    		String ret = new String( Base64.decode(result, Base64.DEFAULT));
    		return ret;
    	} else {
    		return result;
    	}
	}
	
    public void saveWebClientPreferences(JSONObject data) {
    	SagoBizDebug.log(LOG_TAG, "Checking for web client preferences...");
    	if (!data.isNull("save_to_preferences")) {
    		try {
    			// Traditionally unitysendmessage() were used to store player preferences from native side:
				//	SagoBizHelper.Log(LOG_TAG, "The server preferences were received properly.");
				//	com.unity3d.player.UnityPlayer.UnitySendMessage("SagoBizNativeToUnityBridge", "SaveServerPreferences", webPreferences.toString());
				//	SagoBizHelper.Log(LOG_TAG, "Server preferences were successfully sent to Unity");
    			
    			// This introduced synchronization problems as it seems Unity may be using the apply() method
    			// of PrefsEditor and that is not atomic. The following implementation is using PrefEditor.commit()
    			// which is atomic:
    			
    			String prefs = getWebClientPreferences();
    			ArrayList<JSONObject> jsonCollection = new ArrayList<JSONObject>();
    			
    			if (!prefs.isEmpty()) {
	    				SagoBizDebug.log(LOG_TAG, "Adding web client prefs from disk to collection");
	    				jsonCollection.add(new JSONObject(prefs));	
	    		} else {
	    				SagoBizDebug.log(LOG_TAG, "Web client prefs does not exist on the device");
	    		}
				
    			JSONObject webPreferences = data.getJSONObject("save_to_preferences");
				SagoBizDebug.log(LOG_TAG, "Adding the save_to_preferences value to the collection");
				jsonCollection.add(webPreferences);
				JSONObject merged = new JSONObject();
				SagoBizDebug.log(LOG_TAG, "Merging the current and the new values.");
				
				for (JSONObject obj : jsonCollection) {
				    Iterator it = obj.keys();
				    while (it.hasNext()) {
				        String key = (String)it.next();
				        merged.put(key, obj.get(key));
				    }
				}
				
				String jsonText = merged.toString();
				SagoBizDebug.log(LOG_TAG, "The json string:\n" + jsonText);
				byte[] base64data = Base64.encode( jsonText.getBytes(), Base64.DEFAULT);
				String base64string = new String(base64data);
				SagoBizDebug.log(LOG_TAG, "Storing to player preferences");
				SharedPreferences packagePrefs = webView.activity.getSharedPreferences( webView.activity.getPackageName(), Context.MODE_PRIVATE);
				SharedPreferences.Editor prefsEditor = packagePrefs.edit();
				prefsEditor.putString(sagoBizWebClientPreferencesKey, base64string);
				SagoBizDebug.log(LOG_TAG, "Committing the changes.");
				prefsEditor.commit();
					
			} catch (JSONException e) {
				SagoBizDebug.logErrorWithException(LOG_TAG, "Invalid JSON format for server preferences. Expected type: json dictionary", e);
			}
    	}
    }
    
    public void closeWebView(JSONObject data) {
    	SagoBizDebug.logError(LOG_TAG, "closeWebView");
    	
    	eventAction(data);
    	
    	if (webView != null) {
    		webView.webViewDialogFragment.dismiss();
    	}
    	
    	SagoBizDebug.log(LOG_TAG, "Sending a message to Unity as SagoBiz.NativeWebViewDidDisappear()");
    	com.unity3d.player.UnityPlayer.UnitySendMessage("SagoBiz", "InvokeOnWebViewDidDisappear", "");
    }
    
    public void urlExternalAction(JSONObject data) {
    	eventAction(data);
    	
    	String url = "";
    	try {
			url = data.getString("web_url");
			SagoBizDebug.log(LOG_TAG, "Retrieved URL from JSON: " + url);
		}
    	catch (JSONException e) {
    		SagoBizDebug.logErrorWithException(LOG_TAG, "JSONException", e);
    	}

		// If url value is a package name
    	if (url.toLowerCase().startsWith("com.")) {
    		Intent appIntent = webView.activity.getPackageManager().getLaunchIntentForPackage(url);
    		if (appIntent != null) {
    			webView.activity.startActivity(appIntent);
    		} else {
    			SagoBizDebug.logError(LOG_TAG, "The package " + url + " cannot be launched.");
    		}
    	} else if (!url.isEmpty()) {
			SagoBizDebug.log(LOG_TAG, "Attempting to launch package: " + url);
			Intent browserIntent = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
			webView.activity.startActivity(browserIntent);
    	} else {
			SagoBizDebug.logError(LOG_TAG, "The package " + url + " cannot be launched.");
		}
    }
    
    public void eventAction(JSONObject data) {
    	if (!data.isNull("analytics")) {
    		try {
				JSONObject analytics = data.getJSONObject("analytics");
				String eventName = analytics.getString("event_name");
				if (!eventName.isEmpty() && eventName != null) {
					if (analytics.isNull("event_properties")) {
						Mixpanel.trackEvent(analytics.getString("event_name"));
					} else {
						JSONObject eventProperties = analytics.getJSONObject("event_properties");
						Mixpanel.trackEventWithPropertiesJSONObject(eventName, eventProperties);
					}
				} else {
					SagoBizDebug.logError(LOG_TAG, "Error in analytics. Json could not be parsed.");
				}
			} catch (JSONException e) {
				SagoBizDebug.logErrorWithException(LOG_TAG, "JSONException", e);
			}
    	}
    	
    	saveWebClientPreferences(data);
    }
    
    public void videoAction(JSONObject data) {
    	eventAction(data);
    }
    
    public void openStoreAction(JSONObject data)  {

    	eventAction(data);
    	
    	String expectedStorePackageName = DeviceInfo.getAppStoreName(webView.activity);
    	SagoBizDebug.log(LOG_TAG, "Expected store: " + expectedStorePackageName);
    	String storeUrl = ""; 
    	
    	try {
			storeUrl = data.getString("store_url");
			SagoBizDebug.log(LOG_TAG, "Store url: " + storeUrl);
		} catch (JSONException e) {
			SagoBizDebug.logErrorWithException(LOG_TAG, "JSONException", e);
			return;
		}

		try {
			String webUrl = data.getString("web_url");
			// If JSON data received does not have a value for web_url key.
			if (webUrl.equals("")) {
				// We replace web_url value with store_url value
				data.put("web_url", storeUrl);
				SagoBizDebug.log(LOG_TAG, "No value set for web_url in JSON data, replacing it with store_url.");
			}
		} catch (JSONException e) {
			SagoBizDebug.logErrorWithException(LOG_TAG, "JSONException", e);
			return;
		}

    	Intent marketIntent = new Intent(Intent.ACTION_VIEW, Uri.parse(storeUrl));
        marketIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_RESET_TASK_IF_NEEDED);
    	boolean marketFound = false;

	    // find all applications able to handle our marketIntent
	    final List<ResolveInfo> otherApps = webView.activity.getPackageManager().queryIntentActivities(marketIntent, 0);
	    for (ResolveInfo otherApp: otherApps) {
	    	SagoBizDebug.log(LOG_TAG, "Found applications: " + otherApp.activityInfo.applicationInfo.packageName);

			// If we've not installed the app from an app store or from a distribution service
			// eg. Side load with adb or install via hockeyapp
			// What if we have multiple markets installed on one device?
			// Do we just use the first one that we can find?
	        if (expectedStorePackageName == null || expectedStorePackageName.endsWith("android.packageinstaller")) {
				SagoBizDebug.log(LOG_TAG, "No expected store. Using: " + otherApp.activityInfo.applicationInfo.packageName);
	        	ActivityInfo otherAppActivity = otherApp.activityInfo;
	            ComponentName componentName = new ComponentName(
	                    otherAppActivity.applicationInfo.packageName,
	                    otherAppActivity.name
	            );
	            marketIntent.setComponent(componentName);
	        	marketFound = true;
	        	break;
	        }
	    	// look for the expected application
	        if (otherApp.activityInfo.applicationInfo.packageName.startsWith(expectedStorePackageName)) {
	        	SagoBizDebug.log(LOG_TAG, "Using: " + otherApp.activityInfo.applicationInfo.packageName);
	        	ActivityInfo otherAppActivity = otherApp.activityInfo;
	            ComponentName componentName = new ComponentName(
	                    otherAppActivity.applicationInfo.packageName,
	                    otherAppActivity.name
	            );
	            marketIntent.setComponent(componentName);
	            marketFound = true;
	            break;
	        }
	    }
	    
	    if (marketFound) {
	        webView.activity.startActivity(marketIntent);
	    } else {
	    	// if the expected market not present on device, open web browser
	        urlExternalAction(data);
	    }
    }
    
    @JavascriptInterface
    public String isNativeAvailable() {
    	return "true";
    }
    
    @JavascriptInterface
    public void homeButtonIsAvailable(String data) {
    	SagoBizDebug.log(LOG_TAG, "homeButtonIsAvailable was called.");
    	WebViewDialogFragment.ShouldCloseWebViewOnPause = false;
    }
    
    @JavascriptInterface
    public String getOSVersion() {
    	SagoBizDebug.log(LOG_TAG, "getOSVersion was called.");
    	return Integer.toString(DeviceInfo.getOSVersion());
    }
    
    @JavascriptInterface
    public String getPreferences() {
    	return getWebClientPreferences();
    }
    
    public Platform determinePlatform(JSONObject data) {
		
    	if (!data.isNull(googleplayUrl)) {
			return Platform.GooglePlay;
		} else if (!data.isNull(kindleUrl)) {
			return Platform.Kindle;
		}
		
    	return Platform.Unknown;
    }
    
    public String getStorePackageName(Platform platform) {
    	switch(platform) {
	    	case GooglePlay: {
	    		return "com.android.vending";
	    	}
	    	case Kindle:
	    	case KindleFreeTime:
	    	case AmazonTeal: {
	    		return "com.amazon.venezia";
	    	}
	    	default: {
	    		return "";
	    	}
    	}
    }
    
    @JavascriptInterface
    public void handleAction(String jsonData) {
    	SagoBizDebug.log(LOG_TAG, "handleAction json data: " + jsonData);
    	try {
    		JSONObject action = new JSONObject(jsonData);
    		
    		String actionType = action.getString("action");
    		
      		 if (actionType.equalsIgnoreCase("closeAction")) {
                     
                     SagoBizDebug.log(LOG_TAG, "closeAction was called: " + jsonData);
                     closeWebView(action);
                     
                 } else if (actionType.equalsIgnoreCase("linkAction")) {
                     
                	 SagoBizDebug.log(LOG_TAG, "linkAction was called: " + jsonData);
                     urlExternalAction(action);
                     
                 } else if (actionType.equalsIgnoreCase("eventAction")) {
                     
                	 SagoBizDebug.log(LOG_TAG, "eventAction was called: " + jsonData);
                	 eventAction(action);
                     
                 } else if (actionType.equalsIgnoreCase("videoAction")) {
                     
                	 SagoBizDebug.log(LOG_TAG, "videoAction was called: " + jsonData);
                	 videoAction(action);
                     
                 } 
                 else if (actionType.equalsIgnoreCase("storeAction")) {
                	                      
                    openStoreAction(action);
                     
                 } else {
                     SagoBizDebug.log(LOG_TAG, "Invalid action"); 
                 }
    		
    		
		} catch (JSONException e) {
			SagoBizDebug.logErrorWithException(LOG_TAG, "JSONException", e);
		}
    }
    
    public  ActionType getActionType(JSONObject jsonData) {
        
    	String actionType = "";
		try {
			actionType = jsonData.getString("action");
		} catch (JSONException e) {
			SagoBizDebug.logErrorWithException(LOG_TAG, "JSONException", e);
		}
    	
        if (!actionType.equalsIgnoreCase("")) {
      
            if (actionType.equalsIgnoreCase("closeAction")) {
                
                return ActionType.actionTypeClose;
                
            } else if (actionType.equalsIgnoreCase("linkAction")) {
                
                return ActionType.actionTypeLink;
                
            } else if (actionType.equalsIgnoreCase("eventAction")) {
                
                return ActionType.actionTypeEvent;
                
            } else if (actionType.equalsIgnoreCase("storeAction")) {
                
                return ActionType.actionTypeStore;
                
            } else if (actionType.equalsIgnoreCase("videoAction")) {
                
                return ActionType.actionTypeVideo;
                
			} else if (actionType.equalsIgnoreCase("prefsAction")) {
    
				return ActionType.actionTypePrefs;

            } else {
                return ActionType.actionTypeOther;
            }
            
        } else {
            return ActionType.actionTypeInvalid;
        }
    }
     
}
