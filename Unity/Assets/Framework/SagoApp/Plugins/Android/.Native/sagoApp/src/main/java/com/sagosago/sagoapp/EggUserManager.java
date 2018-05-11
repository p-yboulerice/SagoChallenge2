// For more info check https://drive.google.com/open?id=0BzqnviynDWkdaE8xWUU5Z1J3cmc
package com.sagosago.sagoapp;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.AlertDialog.Builder;
import android.content.ContentResolver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.DialogInterface.OnClickListener;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.database.Cursor;
import android.net.Uri;
import android.os.Handler;
import android.os.Process;
import android.util.Log;

/* This is the class used for SmartEducation platform's subscription check. */
public class EggUserManager {
    private static final EggUserManager instance = new EggUserManager();
    private static String mUdid = "";
    private static String mSeuid = "";
    private static boolean mChangeUdid = false;
    private static boolean mSubscriptionStatus = false;
    private static String TAG = "EggUserManager";
    static Handler mHandler;
    static Context mContext;
    private static boolean isAutoMessage = false;
    private static boolean isSubscriptionCheck = false;
    private static int mRepeatInterval = 100;
    private static final String UDID_KEY = "EGG_USERMANAGER_UDID";
    private static final String SEUID_KEY = "EGG_USERMANAGER_SEUID";

    public static EggUserManager getInstance() {
        return instance;
    }

    public void setContext(Context context) {
        mContext = context;
        mUdid = getStringForKey("EGG_USERMANAGER_UDID", "");
        mSeuid = getStringForKey("EGG_USERMANAGER_SEUID", "");
    }

    public static void setAutoMessage(boolean autoFlag) {
        isAutoMessage = autoFlag;
    }

    public static void setSubscriptionCheck(boolean checkFlag) {
        isSubscriptionCheck = checkFlag;
    }

    public void start(Activity activity) {
        String udid = "";
        Cursor cur = getCursor();
        if ((cur != null) &&
                (cur.moveToNext())) {
            if (cur.getColumnIndex("udid") == -1) {
                String temp = cur.getString(0);
                if (temp != null) {
                    udid = temp;
                }
            } else {
                String temp = cur.getString(cur.getColumnIndex("udid"));
                if (temp != null) {
                    udid = temp;
                }
                String subscription = cur.getString(cur.getColumnIndex("subscriptionStatus"));
                Log.d(TAG, "subscriptionStatus: " + subscription);
                if (subscription.equals("true")) {
                    mSubscriptionStatus = true;
                }
            }
        }
        Log.w(TAG, "prev udid = " + mUdid + ", current udid = " + udid);
        if ((udid == null) || (udid.length() <= 0)) {
            showAlert(activity);
            return;
        }
        if ((!mSubscriptionStatus) && (isSubscriptionCheck)) {
            showAlert(activity);
            return;
        }
        if (!udid.equals(mUdid)) {
            Log.w(TAG, "change udid");
            mUdid = udid;
            mChangeUdid = true;
            setStringForKey("EGG_USERMANAGER_UDID", mUdid);
        }
    }

    private Cursor getCursor() {
        Uri uri = Uri.parse("content://jp.smarteducation.egg.kdmode.provider/user");
        return mContext.getContentResolver().query(uri, null, null, null, null);
    }

    private static void showAlert(final Activity activity) {
        if (!isAutoMessage) {
            return;
        }
        Log.d(TAG, "showAlert");
        Log.d(TAG, activity != null ? "Activity is not null" : "Activity is null");

        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Log.d(TAG, "runOnUiThread.run()");
                AlertDialog.Builder alert = new AlertDialog.Builder(activity);
                alert.setTitle("");
                alert.setCancelable(false);
                alert.setMessage("契約情報を確認できませんでした。契約済みの場合には、通信状況を確認してアプリを停止・再起動してください。");
                alert.setPositiveButton("OK", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        Process.killProcess(Process.myPid());
                    }
                });
                Log.d(TAG, "runOnUiThread.run.alert.show()");
                alert.show();
            }
        });

    }

    private static final Runnable stopUnity = new Runnable() {
        public void run() {
        }
    };
    private static final String PREFS_NAME = "EggUserManager";

    public static String getSeuid() {
        return mSeuid;
    }

    public static String getUdid() {
        return mUdid;
    }

    public static void stopApplication() {
    }

    public static boolean getIsChangeUdid() {
        Log.d(TAG, "changed udid:" + mChangeUdid);
        return mChangeUdid;
    }

    public static boolean getSubscriptionStatus() {
        return mSubscriptionStatus;
    }

    public static void startCheck(boolean autoFlag, Activity activity) {
        Log.d(TAG, "startCheck");
        EggUserManager manager = getInstance();

        isAutoMessage = autoFlag;
        mContext = activity;
        manager.start(activity);
    }

    public static String getStringForKey(String key, String defaultValue) {
        if (mContext == null) {
            return defaultValue;
        }
        SharedPreferences settings = mContext.getSharedPreferences("EggUserManager", 0);
        return settings.getString(key, defaultValue);
    }

    public static void setStringForKey(String key, String value) {
        if (mContext == null) {
            return;
        }
        SharedPreferences settings = mContext.getSharedPreferences("EggUserManager", 0);
        SharedPreferences.Editor editor = settings.edit();
        editor.putString(key, value);
        editor.commit();
    }
}
