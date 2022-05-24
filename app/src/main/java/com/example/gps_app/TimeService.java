package com.example.gps_app;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

public class TimeService  extends BroadcastReceiver {


   @Override
    public void onReceive(Context context, Intent intent) {


       Intent i = new Intent(context, GPSTracker.class);
       context.startService(i);

    }

}