package com.sagosago.googleplaydownloader;

import com.google.android.vending.expansion.downloader.impl.DownloaderService;

public class GooglePlayDownloaderService extends DownloaderService {
    // stuff for LVL -- Retrieved from res/values/strings.xml
    public static String BASE64_PUBLIC_KEY = "";
    // used by the preference obfuscater
    static byte[] SALT = new byte[] { 87, 57, 99, 100, 114, 56, 83, 78, 97, 113, 0, 98, 98, 120, 84, 114, 83, 65 };

    /**
     * This public key comes from your Android Market publisher account, and it
     * used by the LVL to validate responses from Market on your behalf.
     */
    @Override
    public String getPublicKey() {
        return BASE64_PUBLIC_KEY;
    }

    /**
     * This is used by the preference obfuscater to make sure that your
     * obfuscated preferences are different than the ones used by other
     * applications.
     */
    @Override
    public byte[] getSALT() {
        return SALT;
    }

    /**
     * Fill this in with the class name for your alarm receiver. We do this
     * because receivers must be unique across all of Android (it's a good idea
     * to make sure that your receiver is in your unique package)
     */
    @Override
    public String getAlarmReceiverClassName() {
        return GooglePlayAlarmReceiver.class.getName();
    }

}
