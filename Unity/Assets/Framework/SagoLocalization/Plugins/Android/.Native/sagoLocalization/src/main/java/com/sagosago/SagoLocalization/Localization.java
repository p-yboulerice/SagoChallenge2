package com.sagosago.SagoLocalization;

import android.annotation.TargetApi;
import android.content.res.Configuration;
import android.os.Build;
import android.os.LocaleList;
import android.util.Log;

import java.util.HashMap;
import java.util.Locale;

import com.google.gson.Gson;
import com.unity3d.player.UnityPlayer;

/**
 * Created by minsoo on 2017-09-05.
 */

public class Localization {

    public static final String debugPrefix = "SagoLocalization->";

    public static final String concatenation = "-";

    public static  String GetScriptCode(Locale locale) {
        // On devices lower than Lollipop/API 21, getting script code is not supported.
        // So we need to just look at country code and add script code ourselves.
        // Even if API level supports getting script code but returns empty string, then do this.
        // So we are hard coding Taiwan and Hong Kong to Traditional Chinese and Macau to Simplified Chinese.
        String countryCode = locale.getCountry();
        String scriptCode = "";
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            scriptCode = locale.getScript();
        }
        if (scriptCode.isEmpty() && locale.getLanguage().equals(Locale.CHINESE.getLanguage())) {
            if (countryCode.equals("TW") || countryCode.equals("HK")) {
                return "Hant";
            } else if (countryCode.equals("CN")) {
                return "Hans";
            }
        }
        return scriptCode;
    }

    public static String LocaleToString(Locale locale) {
        String languageCode = locale.getLanguage();
        String countryCode = locale.getCountry();
        String scriptCode = GetScriptCode(locale);
        String languageTag = "";

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            languageTag = locale.toLanguageTag();
        }

        Log.d(debugPrefix, String.format("String[%s] Language[%s] Script[%s] Country[%s] LanguageTag[%s]", locale.toString(), languageCode, scriptCode, countryCode, languageTag));

        return languageCode + concatenation + (scriptCode.isEmpty() ? "" : scriptCode + concatenation) + countryCode;
    }

    @TargetApi(Build.VERSION_CODES.N)
    public static HashMap<String, Object> GetLocaleMapPostNougat() {
        HashMap<String, Object> localeMap = new HashMap<String, Object>();
        LocaleList localeList = UnityPlayer.currentActivity.getResources().getConfiguration().getLocales();
        String[] locales = new String[localeList.size()];
        for (int i = 0; i < localeList.size(); i++) {
            locales[i] = LocaleToString(localeList.get(i));
        }
        localeMap.put("localeIdentifier", LocaleToString(localeList.get(0)));
        localeMap.put("preferredLanguageIdentifiers", locales);
        return localeMap;
    }

    @SuppressWarnings("deprecation")
    public static HashMap<String, Object> GetLocaleMapPreNougat() {
        Configuration config = UnityPlayer.currentActivity.getResources().getConfiguration();
        HashMap<String, Object> localeMap = new HashMap<String, Object>();
        //Log.e(debugPrefix, "Language Identifier: " + LocaleToString(config.locale));
        localeMap.put("localeIdentifier", LocaleToString(config.locale));
        localeMap.put("preferredLanguageIdentifiers", new String[]{ LocaleToString(config.locale) });
        return  localeMap;
    }

    public static String _SagoLocalization_currentLocaleJson() {
        try {
            if (UnityPlayer.currentActivity != null) {
                HashMap<String, Object> localeMap = null;

                // If OS version is equal to or greater than Nougat/API 24.
                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N){
                    localeMap = GetLocaleMapPostNougat();
                } else {
                    localeMap = GetLocaleMapPreNougat();
                }

                Gson gson = new Gson();
                String json = gson.toJson(localeMap);
                //Log.d(debugPrefix, json);
                return json;
            }
        } catch (Exception e) {
            Log.e(debugPrefix, e.toString());
        }
        return null;
    }

}
