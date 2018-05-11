package com.sagosago.thales;

import android.app.Activity;
import android.content.Context;
import android.util.Log;

import com.thalesifec.framework.android.ThalesApplicationContextType;
import com.thalesifec.framework.coreservice.CoreManager;
import com.thalesifec.sdkproduct.android.*;

/**
 * Created by aram on 16-07-28.
 */

public class Thales {

    public void Initialize(Activity activity) {
        Log.d("SAGO_THALES","Initializing thales");
        ThalesSDKContext mContext = ThalesSDKContext.getInstance(activity);

        mContext.setSDKConnectionCallback(ThalesApplicationContextType.CORE_MANAGER, new NotifyMe());
//        CoreManager sdsService = (CoreManager) mContext.getSystemService(ThalesApplicationContextType.CORE_MANAGER);
//        if( (sdsService != null) && (sdsService.getLocalSeatInfo() != null) ) {
//            String mySeatName = sdsService.getLocalSeatInfo().getSeatName();
//        }

    }



    public class NotifyMe implements ThalesSDKConnection {
        public void onServiceConnected(String name, Object handle) {
            if (name.equalsIgnoreCase(ThalesApplicationContextType.CORE_MANAGER)) {
//                Note: needs extra libraries added, therefore will only be added if the feature is going to be used.
//                sdsService = (CoreManager) handle;
//                String mySeatName = sdsService.getLocalSeatInfo().getSeatName();
            }
        }

        public void onServiceDisconnected(String arg0) {
        }
    }
}
