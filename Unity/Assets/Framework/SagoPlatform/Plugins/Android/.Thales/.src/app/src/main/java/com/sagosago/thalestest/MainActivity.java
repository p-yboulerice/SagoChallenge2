package com.sagosago.thalestest;

//import android.support.v7.app.AppCompatActivity;
import android.app.Activity;
import android.os.Bundle;
import android.view.*;
import com.sagosago.thales.*;

public class MainActivity extends Activity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


    }

    public void initThales(View view) {
        Thales libTest = new Thales();
        libTest.Initialize(this);

    }
}
