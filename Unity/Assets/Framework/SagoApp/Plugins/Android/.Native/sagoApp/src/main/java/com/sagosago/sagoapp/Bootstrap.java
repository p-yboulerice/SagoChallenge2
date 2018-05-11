package com.sagosago.sagoapp;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import com.unity3d.player.UnityPlayerActivity;

public class Bootstrap extends Activity {

    private static final String LOG_TAG = "SAGO Bootstrap";
    
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		// setContentView(R.layout.activity_bootstrap);
	
		runUnity();
	}
	
    /*
     * Runs Unity activity
     */
    protected void runUnity() {
    	
	    	Log.d(LOG_TAG, "Running unity activity");
			Intent unityIntent = new Intent(this, UnityPlayerActivity.class );
			unityIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_RESET_TASK_IF_NEEDED);
			startActivity(unityIntent);
			Log.d(LOG_TAG, "Finishing Bootstrap activity");
			finish();
    }
		
}
