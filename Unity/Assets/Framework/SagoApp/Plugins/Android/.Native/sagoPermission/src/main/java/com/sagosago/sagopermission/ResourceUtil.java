package com.sagosago.sagopermission;

/**
 * Created by aram on 16-09-13.
 */
import java.util.HashMap;
import java.util.Map;
import android.app.Activity;
import android.content.Context;
import android.util.Log;
import android.view.View;

/**
 * ResourceUtil has a list of static methods that gets the relevant resource strings from resource files such as strings.xml and permissio_strings.xml.
 * Note that a lot of the parameters aren't used but simply commented out as a reference.
 */
public class ResourceUtil {
    // Configuration.xml
    // public static final String needs_sensitive_landscape_orientation = "needs_sensitive_landscape_orientation";

    // Animations
    // public static final String fade_in = "fade_in";
    // public static final String fade_out = "fade_out";
    // public static final String none = "none";

    // Layouts
    public static final String permissions_dialog = "permissions_dialog";

    // View ids
    // public static final String permissions_container = "permissions_container";
    // public static final String permissions_title = "permissions_title";
    // public static final String permissions_text = "permissions_text";
    // public static final String permissions_button_layout = "permissions_button_layout";
    // public static final String permissions_button_no = "permissions_button_no";
    // public static final String permissions_button_yes = "permissions_button_yes";
    public static final String permissions_dialog_button = "permissions_dialog_button";
    public static final String permissions_dialog_text = "permissions_dialog_text";
    public static final String permissions_dialog_title = "permissions_dialog_title";

    // Strings
    // public static final String error_title = "error.title";
    // public static final String error_neutral = "error.neutral";
    public static final String permissions_multiple_title = "permissions_multiple_title";
    public static final String permissions_multiple_permission_request = "permissions_multiple_permission_request";
    public static final String permissions_multiple_app_settings = "permissions_multiple_app_settings";
    // public static final String permission_string_next = "permission_dialog_next";
    public static final String permission_string_app_settings = "permission_dialog_app_settings";

    // Styles
    // public static final String PermissionsDialogTheme = "PermissionsDialogTheme";

    // Drawable
//    public static final String settings = "settings";

    // Dimens
    public static final String permissions_dialog_width = "permissions_dialog_width";

    public static final void LogResourceUtil(Context context){
    }

    public enum DefType {
        integer, layout, anim, id, drawable, style, string, bool, assets, raw, dimen
    }

    public enum ResourceType {
        STRING, BOOL, LAYOUT, INTEGER, ANIM, ID, DRAWABLE, STYLE
    }

    public static Integer getResourceIdentifier(Context context, String key, DefType defType){
        return context.getResources().getIdentifier(key, defType.toString(), context.getPackageName());
    }

    public static String getResourceString(Context context, String key){
        Log.d("ResourceUtil", "Getting string for key: " + key + " (" + context.getResources().getIdentifier(key, "string", context.getPackageName()) );
        return context.getResources().getString(context.getResources().getIdentifier(key, "string", context.getPackageName()));
    }

    public static String getResourceStringFormatted(Context context, String key, Object... formatArgs){
        Log.d("ResourceUtil", "Getting formatted string for key: " + key + " (" + context.getResources().getIdentifier(key, "string", context.getPackageName()) + ", args: " + arrToString(formatArgs));
        return context.getResources().getString(context.getResources().getIdentifier(key, "string", context.getPackageName()), formatArgs);
    }

    private static String arrToString(Object[] args){
        StringBuilder sb = new StringBuilder("Args ---> ");
        for(Object o : args){
            if(o instanceof String){
                sb.append((String) o + ", ");
            } else {
                sb.append(o.getClass().getSimpleName() + "->" + o.toString() + ", ");
            }
        }
        return sb.toString();
    }
    public static Boolean getResourceBoolean(Context context, String key){
        return context.getResources().getBoolean(context.getResources().getIdentifier(key, "bool", context.getPackageName()));
    }

    public static Integer getResourceInteger(Context context, String key, DefType defType){
        return context.getResources().getInteger(context.getResources().getIdentifier(key, defType.toString(), context.getPackageName()));
    }

    public static Integer getResourceDimen(Context context, String key){
        return context.getResources().getDimensionPixelSize(context.getResources().getIdentifier(key, DefType.dimen.toString(), context.getPackageName()));
    }

    public static View findViewById(Activity activity, String id){
        return activity.findViewById(getResourceIdentifier(activity, id, DefType.id));
    }

}
