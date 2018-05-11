package com.sagosago.googleplaydownloader;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager.NameNotFoundException;

import com.google.android.vending.expansion.downloader.DownloaderClientMarshaller;

public class GooglePlayAlarmReceiver extends BroadcastReceiver {

    @Override
    public void onReceive(Context context, Intent intent) {
        try {
            DownloaderClientMarshaller.startDownloadServiceIfRequired(context, intent, GooglePlayDownloaderService.class);
        } catch (NameNotFoundException e) {
            GooglePlayDownloaderActivity.logError("GooglePlayAlarmReceiver", "NameNotFoundException", e);
        }       
    }

}

