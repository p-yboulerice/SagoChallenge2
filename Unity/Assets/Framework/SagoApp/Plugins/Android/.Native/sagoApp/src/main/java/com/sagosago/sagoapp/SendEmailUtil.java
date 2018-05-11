package com.sagosago.sagoapp;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Build;
import android.os.Environment;
import android.support.v4.content.FileProvider;
import android.util.Log;

import java.io.File;
import java.io.FileOutputStream;

public class SendEmailUtil {

    private static final String LOG_TAG = "SendEmailUtil";

    /**
     * Spawns an email media chooser with the supplied contents. Contents of the email are generated from the supplied parameters and bug report.
     * The bug report data is placed into an attached text file.
     * @param mainActivity The main Unity activity.
     * @param recipientEmail The recipient's email address.
     * @param title Title of the email.
     * @param body Body contents of the email.
     * @param bugReport Bug report string which will be inserted into an attached text file.
     */
    public static void _SendEmailWithBugReport(Activity mainActivity, String recipientEmail, String title, String body, String bugReport) {

        try {
            //create a temporary text file and inserts the bug report contents into it.
            File outputDir = Environment.getExternalStorageDirectory();
            File deviceInfoFile = File.createTempFile("device_info", ".txt", outputDir);
            FileOutputStream stream = new FileOutputStream(deviceInfoFile);
            stream.write(bugReport.getBytes());
            stream.close();

            //fallback for < API version 24 devices.
            Uri deviceInfoFilePath = Uri.fromFile(deviceInfoFile);

            if(Build.VERSION.SDK_INT >= 24) {
                //new way to get a uri for a file as the previous will not work on newer devices.
                deviceInfoFilePath = FileProvider.getUriForFile(mainActivity, "com.sagosago.sagoapp.provider", deviceInfoFile);
            }

            Log.d(LOG_TAG, "Creating temp device information file at: " + Uri.fromFile(deviceInfoFile).toString());

            //create a ACTION_SEND intent and populating email fields.
            Intent emailIntent = new Intent(Intent.ACTION_SEND);
            emailIntent.setType("text/plain");
            emailIntent.putExtra(Intent.EXTRA_EMAIL, new String[]{ recipientEmail });
            emailIntent.putExtra(Intent.EXTRA_STREAM, deviceInfoFilePath);
            emailIntent.putExtra(Intent.EXTRA_SUBJECT, title);
            emailIntent.putExtra(Intent.EXTRA_TEXT, body);
            mainActivity.startActivity(Intent.createChooser(emailIntent , "Send Bug Report"));
        }
        catch(Exception exception) {
            Log.d(LOG_TAG, "Error Sending Mail: " + exception.getMessage() + "\n" + exception.getStackTrace());
        }
    }
}
