package com.sagosago.sagopermission;

/**
 * Created by aram on 16-09-13.
 */

import android.annotation.TargetApi;
import android.app.Activity;
import android.app.Dialog;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.content.pm.PermissionInfo;
import android.content.res.Configuration;
import android.graphics.Color;
import android.graphics.Paint;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.provider.Settings;
import android.support.annotation.NonNull;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.FragmentActivity;
import android.support.v4.content.ContextCompat;
import android.text.SpannableString;
import android.text.method.LinkMovementMethod;
import android.text.util.Linkify;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.Button;
import android.widget.TextView;

import com.sagosago.googleplaydownloader.GooglePlayDownloaderActivity;
import com.sagosago.sagoapp.Bootstrap;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

@TargetApi(Build.VERSION_CODES.M)
public class PermissionActivity extends FragmentActivity {
    private static final String TAG = PermissionActivity.class.getSimpleName();

    public static final String ARGS_PERMISSION = "permission";
    public static final String ARGS_PERMISSIONS = "permissions";
    public static final String ARGS_APPNAME = "appname";
    public static final String ARGS_TASK = "task";

    public static final int REQUEST_PERMISSIONS_CODE = 123;

    public enum Task {
        SHOW_APPSETTING, REQUEST_PERMISSION, UNKNOWN;

        public static Task from(String s){
            for(Task t : values()){
                if(t.toString().equals(s)){
                    return t;
                }
            }

            return UNKNOWN;
        }
    }

    private List<Permission> currentPermissions;
    private String task;
    private String mAppName;
    private Dialog dialog;
    private TextView titleTextView;
    private TextView contentTextView;
    private Button button;

    private boolean _comesFromSettings = false;
    private long lastActivityPause = -1L;
    private long ACTIVITY_PAUSE_THRESHOLD = 300L;
    private boolean _requestingPermissions = false;

    private int _orientation = -1;
    private boolean _orientationChanged;

    private Activity mContext;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        super.onCreate(savedInstanceState);

        // Init white background
        View view = new View(this);
        view.setLayoutParams(new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT));
        view.setBackgroundColor(Color.WHITE);
        setContentView(view);


        final PackageManager pm = this.getApplicationContext().getPackageManager();
        try {
            mAppName = (String) pm.getApplicationLabel(this.getApplicationInfo());
        } catch (Exception e) { Log.e(TAG, e.toString()); }
        mAppName = mAppName == null ? "Sago Sago" : mAppName;

        mContext = this;

        ApplicationInfo ai = null;
        try {
            ai = pm.getApplicationInfo(getApplicationContext().getPackageName(), 0);
        } catch (Exception e) { Log.e(TAG, e.toString()); }

        if (ai != null) {
            Log.d(TAG, "TargetSDKVersion " + ai.targetSdkVersion + " running on " + Build.VERSION.SDK_INT);
        }

        Log.d(TAG, "onCreate(" + (savedInstanceState == null ? "null" : "bundle") + ")  -> comes from settings: " + _comesFromSettings);
        if(savedInstanceState == null){
            checkPermissions();
        } else {
            // This might occur when a Permission is being revoked.
            checkPermissions();
        }
    }

    @Override
    protected void onResume() {

        Log.d(TAG, "onResume() -> comes from settings: " + _comesFromSettings);
        super.onResume();
        if(dialog != null){
            Log.d(TAG, "Killing Dialog.");
            dialog.dismiss();
            dialog = null;
        }

        checkPermissions();
    }
    @Override
    protected void onStop() {
        Log.d(TAG, "onStop() -> comes from settings: " + _comesFromSettings);
        super.onStop();
    }
    @Override
    protected void onStart() {
        Log.d(TAG, "onStart() -> comes from settings: " + _comesFromSettings);
        super.onStart();
    }

    @Override
    protected void onDestroy(){
        super.onDestroy();
        Log.d(TAG, "onDestroy()");

        if(dialog != null){
            Log.d(TAG, "Killing Dialog.");
            dialog.dismiss();
            dialog = null;
        }

        currentPermissions = null;
        task = null;
        mAppName = null;
        titleTextView = null;
        contentTextView = null;
        button = null;
    }

    /**
     * In singleTop mode, you have to handle an incoming Intent in both onCreate() and onNewIntent()
     * to make it works for all the cases.
     */
    @Override
    protected void onNewIntent(Intent intent) {
        Log.d(TAG, "onNewIntent(" + (intent == null ? "null": "intent") + ")");
        checkPermissions();
    }

    protected void StartNextActivity()
    {
        try {
            int expansionIdentifier = getResources().getIdentifier("google_play_use_expansion_file", "string", getPackageName());

            if (expansionIdentifier == 0) {
                // The google_play_use_expansion_file is undefined therefore navigate to BootstrapActivity.
                StartBootstrapActivity();
            } else {
                String useExpansionFileValue = getResources().getText(getResources().getIdentifier("google_play_use_expansion_file", "string", getPackageName())).toString().toLowerCase();
                boolean useExpansionFile = useExpansionFileValue.equals("true");

                if (useExpansionFile) {
                    // Navigate to GooglePlayDownloaderActivity
                    StartGooglePlayDownloaderActivity();
                } else {
                    // Navigate to BootstrapActivity
                    StartBootstrapActivity();
                }
            }
        } catch (Exception ex) {
            StartBootstrapActivity();
        }
        finish();
    }

    protected void StartGooglePlayDownloaderActivity() {
        Log.d(TAG, "Starting GooglePlay Downloader Activity");
        Intent bootstrapIntent = new Intent(this, GooglePlayDownloaderActivity.class );
        startActivity(bootstrapIntent);
    }

    protected void StartBootstrapActivity() {
        Log.d(TAG, "Starting Bootstrap Activity");
        Intent bootstrapIntent = new Intent(this, Bootstrap.class );
        startActivity(bootstrapIntent);
    }

    private void storePermissionStatus(@NonNull Permission permission, @NonNull Boolean status) {
        mContext.getSharedPreferences(Permission.class.getSimpleName(), Context.MODE_PRIVATE).edit().putBoolean(permission.getAndroidName(), status).commit();
    }

    private Permission permissionForName(@NonNull String name){
        for(final Permission p : currentPermissions){
            if(name.equals(p.getAndroidName())){
                return p;
            }
        }

        return null;
    }

    /**
     * Goes through permissions declared in the manifest and checks if user has allowed them
     * @param context the context
     * @return a list of permissions. Returns an empty list if no permissions are found
     */
    protected static List<String> listPermissionsNotGranted(final Activity context) {
        final List<String> permissionsNotYetGranted = new ArrayList<>();

        try {
            final List<String> manifestPermissions = Arrays.asList(context.getPackageManager().getPackageInfo(context.getPackageName(), PackageManager.GET_PERMISSIONS).requestedPermissions);
            // If App TargetSDK >= M or OS Runtime SDK >= M,
            // handle requesting for permissions otherwise, let the system deal with the permissions
            if (context.getApplicationInfo().targetSdkVersion >= Build.VERSION_CODES.M &&
                    Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                for(String s : manifestPermissions){
                    if(ContextCompat.checkSelfPermission(context, s) != PackageManager.PERMISSION_GRANTED){
                        permissionsNotYetGranted.add(s);
                    }
                }
            }
        } catch (Exception e){
            Log.d(TAG, "Exception happening at permissions check.", e);
        }

        return permissionsNotYetGranted;
    }

    private void checkPermissions(){
        Log.d(TAG, "checkPermissions()");
        final List<String> perms = listPermissionsNotGranted((Activity) mContext);
        if (perms.size() == 0) {
            StartNextActivity();
            return;
        }

        currentPermissions = new ArrayList<>();

        PackageManager pm = this.getApplicationContext().getPackageManager();
        PermissionInfo permissionInfo = null;

        for(String s : perms){
            try {
                permissionInfo = pm.getPermissionInfo(s, 0);
                Log.d(TAG, s + " permission has protection level: " + protectionLevelToString(permissionInfo.protectionLevel));
                if ((permissionInfo.protectionLevel & PermissionInfo.PROTECTION_MASK_BASE) == PermissionInfo.PROTECTION_DANGEROUS) {
                    Log.d(TAG, "Requesting for permission: " + s);
                    currentPermissions.add(Permission.from(mContext, s, false));
                } else {
                    Log.d(TAG, "Permission " + s + " has normal protection level so not going to request for permission.");
                }
            } catch (Exception e) { Log.e(TAG, e.toString()); }
        }

        handlePermissions();
    }

    public static String protectionLevelToString(int protectionLevel) {
        switch (protectionLevel & PermissionInfo.PROTECTION_MASK_BASE) {
            case PermissionInfo.PROTECTION_NORMAL:
                return "normal";
            case PermissionInfo.PROTECTION_DANGEROUS:
                return "dangerous";
            case PermissionInfo.PROTECTION_SIGNATURE:
                return "signature";
            default:
                return "undefined";
        }
    }

    private void handlePermissions() {
        Log.d(TAG, "handlePermissions()");
        if (currentPermissions == null || currentPermissions.size() == 0) {
            return;
        }

        if (canAskForPermissions()) {
            showSagoBocaPermissionsDialog();
        } else {
            showSagoBocaSettingsLinkDialog();
        }
    }

    private Boolean canAskForPermissions() {
        for(Permission p : currentPermissions){
            if(!mContext.getSharedPreferences(Permission.class.getSimpleName(), Context.MODE_PRIVATE).getBoolean(p.getAndroidName(), true)){
                return false;
            }
        }
        return true;
    }


    /**
     * Shows the dialog explaining for the user why we need to have this permission.
     * This is used when we're no longer allowed to ask permission from the user.
     *
     * The button will link to the app setting
     */
    private void showSagoBocaSettingsLinkDialog() {
        Log.d(TAG, "Time to show Sago App Settings Permission Dialog for: " + currentPermissions.size() + " elements");

        initDialog();

        final String title = ResourceUtil.getResourceString(this, ResourceUtil.permissions_multiple_title);
        final String content = ResourceUtil.getResourceStringFormatted(this, ResourceUtil.permissions_multiple_app_settings, new Object[]{mAppName});

        titleTextView.setText(title);
        contentTextView.setText(content);
        button.setText(ResourceUtil.getResourceString(this, ResourceUtil.permission_string_app_settings));
        button.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                dialog.hide();
                final Intent appSettingsIntent = new Intent(Settings.ACTION_APPLICATION_DETAILS_SETTINGS, Uri.parse("package:" + PermissionActivity.this.getPackageName()));
                appSettingsIntent.addCategory(Intent.CATEGORY_DEFAULT);
                appSettingsIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
                startActivity(appSettingsIntent);
                setComesFromSettings(true);
            }
        });

        button.setVisibility(View.INVISIBLE);
        button.setClickable(false);

        Log.d(TAG, "Showing dialog.");

        dialog.show();
    }

    private void initDialog(){

        if (dialog != null && _orientationChanged) {
            dialog.dismiss();
            dialog = null;
        }
        if (dialog == null) {
            dialog = new Dialog(this);
            dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
            dialog.setContentView(ResourceUtil.getResourceIdentifier(this, ResourceUtil.permissions_dialog, ResourceUtil.DefType.layout));

            dialog.setCanceledOnTouchOutside(false);
            dialog.setCancelable(false);

            titleTextView = (TextView) dialog.findViewById(ResourceUtil.getResourceIdentifier(this, ResourceUtil.permissions_dialog_title, ResourceUtil.DefType.id));
            contentTextView = (TextView) dialog.findViewById(ResourceUtil.getResourceIdentifier(this, ResourceUtil.permissions_dialog_text, ResourceUtil.DefType.id));
            button = (Button) dialog.findViewById(ResourceUtil.getResourceIdentifier(this, ResourceUtil.permissions_dialog_button, ResourceUtil.DefType.id));

            titleTextView.getPaint().setFlags(titleTextView.getPaint().getFlags() | Paint.SUBPIXEL_TEXT_FLAG | Paint.LINEAR_TEXT_FLAG);
            titleTextView.getPaint().setAntiAlias(true);

            contentTextView.getPaint().setFlags(titleTextView.getPaint().getFlags() | Paint.SUBPIXEL_TEXT_FLAG | Paint.LINEAR_TEXT_FLAG);
            contentTextView.getPaint().setAntiAlias(true);

            button.getPaint().setFlags(titleTextView.getPaint().getFlags() | Paint.SUBPIXEL_TEXT_FLAG | Paint.LINEAR_TEXT_FLAG);
            button.getPaint().setAntiAlias(true);
        }
    }

    /**
     * Shows the dialog explaining for the user why we need to have this permission
     */
    private void showSagoBocaPermissionsDialog() {
        Log.d(TAG, "Time to show Sago Permission Dialog: " + currentPermissions.size() + " permissions");

        initDialog();

        final String title = ResourceUtil.getResourceString(this, ResourceUtil.permissions_multiple_title);
        final String content = ResourceUtil.getResourceStringFormatted(this, ResourceUtil.permissions_multiple_permission_request, new Object[]{mAppName});

        final SpannableString s = new SpannableString(content);
        Linkify.addLinks(s, Linkify.WEB_URLS);

        Log.d(TAG, "showSagoBocaPermissionsDialog() -> title: " + title + ", content: " + content + ". mAppName set to: " + mAppName);

        titleTextView.setText(title);
        contentTextView.setText(content);
        contentTextView.setMovementMethod(LinkMovementMethod.getInstance());

        View buttonView = dialog.findViewById(ResourceUtil.getResourceIdentifier(this, ResourceUtil.permissions_dialog_button, ResourceUtil.DefType.id));
        buttonView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                dialog.hide();
                requestOsPermission();
            }
        });

        buttonView.setVisibility(View.VISIBLE);
        buttonView.setClickable(true);

        Log.d(TAG, "Calling dialog.show()");
        dialog.show();
    }

    private void setComesFromSettings(boolean b){
        _comesFromSettings = b;
        lastActivityPause = System.currentTimeMillis();
    }

    private boolean comesFromSettings(){
        return _comesFromSettings && ((System.currentTimeMillis() - lastActivityPause) > ACTIVITY_PAUSE_THRESHOLD);
    }

    private void requestOsPermission() {
        _requestingPermissions = true;
        String[] permissionsArray = getPermissionsAsArray();
        if (permissionsArray != null && permissionsArray.length != 0) {
            ActivityCompat.requestPermissions(this, permissionsArray, REQUEST_PERMISSIONS_CODE);
        }
    }

    private String[] getPermissionsAsArray(){
        final String[] arr = new String[currentPermissions.size()];
        for(int i = 0; i < currentPermissions.size(); i++){
            arr[i] = currentPermissions.get(i).getAndroidName();
        }
        return arr;
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        Log.d(TAG, "onConfigurationChanged(" + newConfig.toString() + ")");

        super.onConfigurationChanged(newConfig);

        if (_orientation != newConfig.orientation) {
            _orientation = newConfig.orientation;
            _orientationChanged = true;
        } else {
            _orientationChanged = false;
        }
        if (!_requestingPermissions && _orientationChanged) {
            handlePermissions();
        }
    }
    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {

        _requestingPermissions = false;
        for(int i = 0; i < permissions.length; i++){
            final Permission p = permissionForName(permissions[i]);
            if(p == null){
                Log.w(TAG, "Failed to find permission \"" + permissions[i] + "\"in RequestPermissionResultEvent.");
                continue;
            }
            if (requestCode == PermissionActivity.REQUEST_PERMISSIONS_CODE) {
                if (grantResults[i] == PackageManager.PERMISSION_GRANTED) {
                    Log.d(TAG, "Permission has been granted for: " + p.toString());
                } else {
                    if (ActivityCompat.shouldShowRequestPermissionRationale((Activity) mContext, p.getAndroidName())) {
                        // User has denied permission but did NOT check the DONT ASK AGAIN checkbox
                        Log.d(TAG, "Showing request permission rationale for " + p.getAndroidName() + ": True");
                    } else {
                        Log.d(TAG, "Showing request permission rationale for " + p.getAndroidName() + ": false");
                        // User has checked DONT ASK AGAIN checkbox
                        storePermissionStatus(p, false);
                    }

                }
            }
        }

        checkPermissions();
    }



}

