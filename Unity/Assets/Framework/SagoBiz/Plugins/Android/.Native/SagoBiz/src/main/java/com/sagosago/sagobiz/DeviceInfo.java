package com.sagosago.sagobiz;

import java.util.List;
import java.util.Locale;
import java.util.TimeZone;
import java.util.Vector;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.Manifest;
import android.annotation.TargetApi;
import android.app.Activity;
import android.app.ActivityManager;
import android.app.ActivityManager.MemoryInfo;
import android.content.Context;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Build;
import android.provider.Settings;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.WindowManager;

public class DeviceInfo {
	
	public static final String LOG_TAG = "DeviceInfo";

	// Gets the manufacturer of the product/hardware.
	public static String getManufacturer() {
		return Build.MANUFACTURER;
	}

	// Gets the chipset board of the product/hardware.
	public static String getBoard() { return Build.BOARD; }

	// Gets the hardware platform of the product/hardware.
	public static String getHardware() { return Build.HARDWARE; }

	// The name of the overall product.
	public static String getProduct() {
		return Build.PRODUCT;
	}
	
	// The end-user-visible name for the end product. 
	public static String getModel() {
		return Build.MODEL;
	}
	
	// Gets the brand of the device.
	public static String getBrand() {
		return Build.BRAND;
	}
	
	// Gets the operating system version
	// Example: 15, 19
	public static int getOSVersion() {
		return Build.VERSION.SDK_INT;
	}
	
	// Gets the device architecture. ex. x86, ArmV7 etc.
	public static String getArchitecture() {
		return System.getProperty("os.arch");
	}
	// Gets the screen DPI density
	public static int getScreenDensityDPI(Activity activity) {
		Context context = activity.getApplicationContext(); 
		Display display = ((WindowManager) context.getSystemService(Context.WINDOW_SERVICE)).getDefaultDisplay();
		DisplayMetrics displayMetrics = new DisplayMetrics();
		display.getMetrics(displayMetrics);
		return displayMetrics.densityDpi;
	}

	// TODO: Evaluate if this is the universal standard among Android, iOS and Windows Phone devices
	// Gets the device's language in 2 character format.
	// Example: en, de, fr
	public static String getLanguage() {
		return Locale.getDefault().getLanguage();
	}
	
	// TODO: Evaluate if this is the universal standard among Android, iOS and Windows Phone devices
	// Gets the device region in 2 character format
	// Example: GB, CA, US
	public static String getCountry() {
		return Locale.getDefault().getCountry(); // http://en.wikipedia.org/wiki/ISO_3166-1
	}
	
	// TODO: Check if Daylight Saving time is correctly applied.
	public static String getTimeZone() {
		return TimeZone.getDefault().getDisplayName(false, TimeZone.SHORT); 
	}

	// Gets the unique device android id. Note that the value is consistent across the apps on the device, but factory reset will re-generate a new value.
	public static String getAndroidID(Activity activity) {
		return Settings.Secure.getString(activity.getContentResolver(), Settings.Secure.ANDROID_ID);
	}
	
	// Gets the package name
	// Example: com.SagoSago.SagoBizTest
	public static String getAppBundleId(Activity activity) {
		return activity.getPackageName();
	}
	
	// Gets the end-user-visible name for the app.
	// Example: Toolbox, Ocean Swimmer
	public static String getAppDisplayName(Activity activity) {
	    PackageManager lPackageManager = activity.getPackageManager();
	    ApplicationInfo applicationInfo = null;
	    try {
	        applicationInfo = lPackageManager.getApplicationInfo(activity.getApplicationInfo().packageName, 0);
	    } catch (final NameNotFoundException e) {
	    }
	    return (String) (applicationInfo != null ? lPackageManager.getApplicationLabel(applicationInfo) : null);
	}
	
	// Gets the version code of the app
	// Example: 5
	public static int getAppVersionCode(Activity activity) {
		PackageInfo pInfo;
		try {
			pInfo = activity.getPackageManager().getPackageInfo(activity.getPackageName(), 0);
		} catch (PackageManager.NameNotFoundException e) {
			return 0;
		}
		
		return pInfo.versionCode;
	}
	
	// Gets the version name of the app
	public static String getAppVersionName(Activity activity) {
		
		PackageInfo pInfo;
		
		try {
			pInfo = activity.getPackageManager().getPackageInfo(activity.getPackageName(), 0);
		} catch (PackageManager.NameNotFoundException e) {
			return "0";
		}
		
		return pInfo.versionName;
	}
	
	// Gets if there is a WiFi connectivity. Note that this does not guarantee Internet access.
    public static boolean isWifiConnected(Activity activity) {
        
    	boolean ret = false;
    	
        if (PackageManager.PERMISSION_GRANTED == activity.checkCallingOrSelfPermission(Manifest.permission.ACCESS_NETWORK_STATE)) {
            ConnectivityManager connManager = (ConnectivityManager) activity.getSystemService(Context.CONNECTIVITY_SERVICE);
            NetworkInfo wifiInfo = connManager.getNetworkInfo(ConnectivityManager.TYPE_WIFI);
            ret = wifiInfo.isConnected();
        }

        return ret;
    }
    
    // Gets if the device has camera feature.
    public static boolean hasCamera(Activity activity) {
    	
    	boolean ret = false;
    	
    	PackageManager pm = activity.getPackageManager();
		if (pm.hasSystemFeature(PackageManager.FEATURE_CAMERA_ANY)) {
			ret = true;
		}
		
		return ret;
    }

    // Gets if the device has front camera
    public static boolean hasFrontCamera(Activity activity) {
    	
    	boolean ret = false;
    	
    	PackageManager pm = activity.getPackageManager();
		if (pm.hasSystemFeature(PackageManager.FEATURE_CAMERA_FRONT)) {
			ret = true;
		}
		
		return ret;
    }
    
    // Gets if the device has a back camera
    public static boolean hasRearCamera(Activity activity) {
    	
    	boolean ret = false;
    	
    	PackageManager pm = activity.getPackageManager();
		if (pm.hasSystemFeature(PackageManager.FEATURE_CAMERA)) {
			ret = true;
		}
		
		return ret;
    }
    
    @TargetApi(Build.VERSION_CODES.JELLY_BEAN)
    public static long totalMemory(Activity activity) {
    	if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN) {
	    	MemoryInfo mi = new MemoryInfo();
	    	ActivityManager activityManager = (ActivityManager) activity.getSystemService(Context.ACTIVITY_SERVICE);
	    	activityManager.getMemoryInfo(mi);
	    	long totalMegs = mi.totalMem / (1024 * 1024);
	
	    	return totalMegs;
    	} else {
    		return -1;
    	}
    }
    
    // Gets the available memory. Note that kernel occupies a small portion of this memory, so it is not absolute.
    public static long availableMemory(Activity activity) {
    	
    	MemoryInfo mi = new MemoryInfo();
    	ActivityManager activityManager = (ActivityManager) activity.getSystemService(Context.ACTIVITY_SERVICE);
    	activityManager.getMemoryInfo(mi);
    	long availableMegs = mi.availMem / (1024 * 1024);

    	return availableMegs;
    }

    // True if the device is low on memory, False otherwise.
    public static boolean isLowMemory(Activity activity) {
    	
    	MemoryInfo mi = new MemoryInfo();
    	ActivityManager activityManager = (ActivityManager) activity.getSystemService(Context.ACTIVITY_SERVICE);
    	activityManager.getMemoryInfo(mi);
    	
    	return mi.lowMemory;
    }

    // Gets the real physical screen width of the device. Refer to getRealMetrics() on Android
    @TargetApi(Build.VERSION_CODES.JELLY_BEAN_MR1)
    public static int screenRealPhysicalWidth(Activity activity) {
    	
    	DisplayMetrics realMetrics = new DisplayMetrics();
    	
    	activity.getWindowManager().getDefaultDisplay().getRealMetrics(realMetrics);
    	return realMetrics.widthPixels;
    }
    
    // Gets the real physical screen height of the device. Refer to getRealMetrics() on Android
    @TargetApi(Build.VERSION_CODES.JELLY_BEAN_MR1)
    public static int screenRealPhysicalHeight(Activity activity) {
    	
    	DisplayMetrics realMetrics = new DisplayMetrics();
    	
    	activity.getWindowManager().getDefaultDisplay().getRealMetrics(realMetrics);
    	return realMetrics.heightPixels;
    }
    
    // Gets the physical screen width of the device. Refer to getMetrics() on Android
    public static int screenPhysicalWidth(Activity activity) {
    	
    	DisplayMetrics metrics=new DisplayMetrics();
    	
    	activity.getWindowManager().getDefaultDisplay().getMetrics(metrics);
    	return metrics.widthPixels;
    }
    
    // Gets the physical screen height of the device. Refer to getMetrics() on Android
    public static int screenPhysicalHeight(Activity activity) {
    	
    	DisplayMetrics metrics=new DisplayMetrics();
    	
    	activity.getWindowManager().getDefaultDisplay().getMetrics(metrics);
    	return metrics.heightPixels;
    }
    
    // Gets the name of the store where this app was installed from.
    // Example: com.amazon.venezia , com.android.vending
    public static String getAppStoreName(Activity activity) {
    	
    	PackageManager pm = activity.getPackageManager();
    	String storeName = pm.getInstallerPackageName(activity.getPackageName());
    
    	return storeName;
    }
    
    // Returns a comma separated list of apps starting with the package name from parameter 'appBasePackageName'
	public static String getInstalledAppsWithPackageNamePrefix(Context context, String appBasePackageName) {
		
		PackageManager pm = context.getPackageManager();
        
        String packages = null;
        List<ApplicationInfo> packageList = pm.getInstalledApplications(PackageManager.GET_META_DATA);
        
        for (ApplicationInfo packageInfo : packageList) {
            if (packageInfo.packageName.startsWith(appBasePackageName)) {
                if (packages == null) {
                    packages = "";
                } else {
                    packages += ",";
                }
                packages += packageInfo.packageName;
            }
        }
        
        return packages;
	} 
	
	// Returns the list of string where each element contains the package name and its availability (ex. result[0] = "com.sagosago.Toolbox.googleplay,true")
	public static String getInstalledAppsWithPackageNames(Context context, String packageNamesJSONArray) {
		
		PackageManager pm = context.getPackageManager();
		
		
        List<ApplicationInfo> packageList = pm.getInstalledApplications(PackageManager.GET_META_DATA);
        List<String> installedPackageNames = new Vector<String>();
        
        for (ApplicationInfo packageInfo : packageList) {
        	installedPackageNames.add(packageInfo.packageName);
        }
        
        JSONArray result = new JSONArray();
		
        
        try {
        	SagoBizDebug.log(LOG_TAG, "Deserializing package names..");
			JSONArray packageNames = new JSONArray(packageNamesJSONArray);
        
			SagoBizDebug.log(LOG_TAG, "Iterating through package names..");
	        for(int i=0; i< packageNames.length(); i++) {
	        	SagoBizDebug.log(LOG_TAG, "Deserializing json object at index " + i);
	        	JSONObject currentIndex = packageNames.getJSONObject(i);
	        	SagoBizDebug.log(LOG_TAG, "Getting the bundle_id key.");
	        	if (currentIndex.isNull("bundle_id"))
	        		SagoBizDebug.logError(LOG_TAG, "bundle_id key is not found.");
	        	if (currentIndex.isNull("company_name"))
	        		SagoBizDebug.logError(LOG_TAG, "company_name key is not found.");
	        	if (currentIndex.isNull("name"))
	        		SagoBizDebug.logError(LOG_TAG, "name key is not found.");
	        	String packageName = currentIndex.getString("bundle_id");
	        	String companyName = currentIndex.getString("company_name");
	        	String name = currentIndex.getString("name");
	        	
	        	if (installedPackageNames.contains(packageName)) {
	        		SagoBizDebug.log(LOG_TAG, "Creating json object for packagename: " + packageName);
	        		JSONObject temp = new JSONObject();
	        		temp.put("bundle_id", packageName);
	        		temp.put("installed", "true");
	        		temp.put("company_name", companyName);
	        		temp.put("name", name);
	        		SagoBizDebug.log(LOG_TAG, "Adding json object to JSONArray");
	        		result.put(temp);
	        	} else {
	        		SagoBizDebug.log(LOG_TAG, "Creating json object for packagename: " + packageName);
	        		JSONObject temp = new JSONObject();
	        		temp.put("bundle_id", packageName);
	        		temp.put("installed", "false");
	        		temp.put("company_name", companyName);
	        		temp.put("name", name);
	        		SagoBizDebug.log(LOG_TAG, "Adding json object to JSONArray");
	        		result.put(temp);
	        	}
	        }
	        
		} catch (JSONException e) {
			SagoBizDebug.logErrorWithException(LOG_TAG, "JSONException", e);
		}

        return result.toString();
	} 
}