package com.unity3d.player;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;

import com.sagosago.permissiontest.R;

public class UnityPlayerActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_unity_player_native);
        Log.d("PERMISSIONTEST", "Started UnityPlayerActivity");
    }
}
