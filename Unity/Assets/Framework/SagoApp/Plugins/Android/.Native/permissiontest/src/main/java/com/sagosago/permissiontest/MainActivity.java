package com.sagosago.permissiontest;

import android.app.Activity;
import android.content.Intent;
//import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;

import com.sagosago.sagopermission.*;

public class MainActivity extends Activity {

    public final static String PERMISSION_ACTIVITY = "com.sagosago.sagopermission.PermissionActivity";
    private String mAppName;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


        startActivity(preparePermissionIntent());
        finish();
    }

    private Intent preparePermissionIntent(){

         Intent permissionIntent = new Intent(this.getApplicationContext(), PermissionActivity.class);
         return permissionIntent;
    }

}
