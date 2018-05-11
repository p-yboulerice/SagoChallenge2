package com.sagosago.sagoapp;

import android.annotation.TargetApi;
import android.app.Activity;
import android.app.Application;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.Window;
import android.view.View.OnSystemUiVisibilityChangeListener;
import android.view.View.OnFocusChangeListener;

/**
 * A class that handles immersive mode feature that was introduced in Android 4.4+ 
 */
public class Immersive {

    private static final String LOG_TAG = "SAGO Immersive";
    private static boolean isFirstTime = true;
    
    /**
     * Enables or disables immersive mode. Only applies to Android 4.4+ (android-19)
     * @param unityActivity
     * @param enabled
     */
    @TargetApi(Build.VERSION_CODES.KITKAT)
	private static void setImmersiveMode(Activity unityActivity, boolean enabled) {
		if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.KITKAT && enabled) {
			
			int flags = View.SYSTEM_UI_FLAG_LAYOUT_STABLE 
						| View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
			            | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
			            | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
			            | View.SYSTEM_UI_FLAG_FULLSCREEN
						| View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY;
		
			unityActivity.getWindow().getDecorView().setSystemUiVisibility(flags);
		}
	}
	
	/**
	 * Activates immersive mode and registers all necessary activity life cycle methods to activate immersive mode upon callback.
	 * @param unityActivity
	 */
	public static void activateImmersiveMode(final Activity unityActivity) {
		if (isFirstTime) {
			isFirstTime = false;
			unityActivity.runOnUiThread(new Runnable(){
	
				@Override
				public void run() {
					
					View unityView = unityActivity.getWindow().getDecorView();
					
					setImmersiveMode(unityActivity, true);
					
					unityView.setOnSystemUiVisibilityChangeListener(new View.OnSystemUiVisibilityChangeListener() {
						
						@Override
						public void onSystemUiVisibilityChange(int visibility) {
							if ((visibility & View.SYSTEM_UI_FLAG_FULLSCREEN) == 0) {
								setImmersiveMode(unityActivity, true);
							}
						}
						
					});
					
					unityActivity.getApplication().registerActivityLifecycleCallbacks(new Application.ActivityLifecycleCallbacks() {
						
						@Override
						public void onActivityStopped(Activity activity) { }
						
						@Override
						public void onActivityStarted(Activity activity) { }
						
						@Override
						public void onActivitySaveInstanceState(Activity activity, Bundle outState) { }
						
						@Override
						public void onActivityResumed(Activity activity) {
							setImmersiveMode(unityActivity, true);
						}
						
						@Override
						public void onActivityPaused(Activity activity) { }
						
						@Override
						public void onActivityDestroyed(Activity activity) { }
						
						@Override
						public void onActivityCreated(Activity activity, Bundle savedInstanceState) { }
					});
					
				}
				
			});
		} else {
			unityActivity.runOnUiThread(new Runnable() {
				
				@Override
				public void run() {
					setImmersiveMode(unityActivity, true);		
				}
			});
			
		}
	}
}
