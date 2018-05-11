package com.sagosago.sagopermission;

import android.Manifest;
import android.content.Context;
import android.os.Parcel;
import android.os.Parcelable;

/**
 * Permission is a parcelable class which represents an Android permission along with its status (isGranted)
 * along with UI strings for the permission description.
 */
public class Permission implements Parcelable {
    private final String name;
    private final String title;
    private final String text;
    private final String appSettingsText;
    private final String androidName;
    private boolean granted;

    public static Permission from(Context context, String androidName, Boolean granted){
        return new Permission(context, androidName, granted);
    }

    private Permission(Context context, String androidName, Boolean granted) {
        super();
        this.androidName = androidName;
        final String prefix = getResourceStringPrefix();
        this.name = ResourceUtil.getResourceString(context, prefix + ".name");
        this.title = ResourceUtil.getResourceString(context, prefix + ".title");
        this.text = ResourceUtil.getResourceString(context, prefix + ".text");
        this.appSettingsText = ResourceUtil.getResourceString(context, prefix + ".app_settings_text");
        this.granted = granted;
    }

    public String getName() {
        return name;
    }

    public String getTitle() {
        return title;
    }

    public String getText() { return text; }

    public String getAndroidName() {
        return androidName;
    }

    public void setGranted(boolean granted){
        this.granted = granted;
    }

    /**
     * Gets whether the permission is granted or not.
     * @return boolean
     */
    public boolean isGranted() {
        return granted;
    }

    /**
     * Gets the app settings text
     * @return String
     */
    public String getAppSettingsText() { return appSettingsText; }

    /**
     * NOT USED! getPermissionGroup Gets the permission group from the androidName
     * @return String representing manifest's permission string literal.
     */
    @Deprecated
    public String getPermissionGroup(){
        switch (androidName){
            case Manifest.permission.READ_CALENDAR:
            case Manifest.permission.WRITE_CALENDAR:
                return Manifest.permission_group.CALENDAR;
            case Manifest.permission.CAMERA:
                return Manifest.permission_group.CAMERA;
            case Manifest.permission.READ_CONTACTS:
            case Manifest.permission.WRITE_CONTACTS:
                return Manifest.permission_group.CONTACTS;
            case Manifest.permission.ACCESS_FINE_LOCATION:
            case Manifest.permission.ACCESS_COARSE_LOCATION:
                return Manifest.permission_group.LOCATION;
            case Manifest.permission.RECORD_AUDIO:
                return Manifest.permission_group.MICROPHONE;
            case Manifest.permission.READ_PHONE_STATE:
            case Manifest.permission.CALL_PHONE:
            case Manifest.permission.READ_CALL_LOG:
            case Manifest.permission.WRITE_CALL_LOG:
            case Manifest.permission.ADD_VOICEMAIL:
            case Manifest.permission.USE_SIP:
            case Manifest.permission.PROCESS_OUTGOING_CALLS:
                return Manifest.permission_group.PHONE;
            case Manifest.permission.BODY_SENSORS:
                return Manifest.permission_group.SENSORS;
            case Manifest.permission.SEND_SMS:
            case Manifest.permission.RECEIVE_SMS:
            case Manifest.permission.READ_SMS:
            case Manifest.permission.RECEIVE_WAP_PUSH:
            case Manifest.permission.RECEIVE_MMS:
                return Manifest.permission_group.SMS;
            case Manifest.permission.READ_EXTERNAL_STORAGE:
            case Manifest.permission.WRITE_EXTERNAL_STORAGE:
                return Manifest.permission_group.STORAGE;
            default:
                return "";
        }
    }

    /**
     * Gets the resource prefix for the current permission.
     * The string prefix should exist in resources (permission_strings.xml)
     * @return String
     */
    public String getResourceStringPrefix(){

        switch (androidName){
            case Manifest.permission.WRITE_EXTERNAL_STORAGE:
                return "permission_write_ext_storage";
            case Manifest.permission.READ_EXTERNAL_STORAGE:
                return "permission_read_ext_storage";
            case Manifest.permission.CAMERA:
                return "permission_camera";
            case Manifest.permission.READ_PHONE_STATE:
                return "permission_read_phone_state";
            case Manifest.permission.ACCESS_FINE_LOCATION:
                return "permission_access_fine_location";
            case Manifest.permission.RECORD_AUDIO:
                return "permission_record_audio";
            default:
                return "";
        }
    }

    @Override
    public String toString() {
        return "Permission{" +
                "name='" + name + '\'' +
                ", title='" + title + '\'' +
                ", text='" + text + '\'' +
                ", appSettingsText='" + appSettingsText + '\'' +
                ", androidName='" + androidName + '\'' +
                '}';
    }

    /* PARCELABLE INTERFACE */
    @Override
    public int describeContents(){
        return 0;
    }

    @Override
    public void writeToParcel(Parcel out, int flags){
        out.writeString(name);
        out.writeString(title);
        out.writeString(text);
        out.writeString(androidName);
        out.writeString(appSettingsText);
        out.writeByte((byte) (granted ? 1 : 0));
    }

    private Permission(Parcel in) {
        name = in.readString();
        title = in.readString();
        text = in.readString();
        androidName = in.readString();
        appSettingsText = in.readString();
        granted = in.readByte() != 0;
    }

    public static final Parcelable.Creator<Permission> CREATOR = new Parcelable.Creator<Permission>() {
        public Permission createFromParcel(Parcel in) {
            return new Permission(in);
        }

        public Permission[] newArray(int size) {
            return new Permission[size];
        }
    };

}
