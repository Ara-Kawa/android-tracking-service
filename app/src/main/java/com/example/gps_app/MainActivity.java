package com.example.gps_app;

import android.Manifest;
import android.app.ActivityManager;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.StrictMode;
import android.util.Log;
import android.widget.Button;

import androidx.appcompat.app.AppCompatActivity;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;

public class MainActivity extends AppCompatActivity {
Button btn;
    GPSTracker gpsTracker;
    PrintWriter pw;
    Intent serviceIntent;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        serviceIntent = new Intent(this, GPSTracker.class);
        setContentView(R.layout.activity_main);



        StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
        StrictMode.setThreadPolicy(policy);


        Thread myThread = new Thread(new server());
        myThread.start();
        if (checkSelfPermission( Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED && checkSelfPermission( Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {

            requestPermissions(new String[]{Manifest.permission.ACCESS_FINE_LOCATION}, 111);


        }
        else {
            isMyServiceRunning(GPSTracker.class);

        }






    }
    private void isMyServiceRunning(Class<?> service_class) {
        ActivityManager manager = (ActivityManager) getSystemService(Context.ACTIVITY_SERVICE);
        for (ActivityManager.RunningServiceInfo service : manager.getRunningServices(Integer.MAX_VALUE)) {
            if (service_class.getName().equals(service.service.getClassName())) {

                finish();
            }
        }
        startService(serviceIntent);  finish();
    }

    class server implements  Runnable{

        Socket s;
        ServerSocket ss;
        InputStreamReader isr;
        BufferedReader br;
        String message;
        Handler h = new Handler();
        Runnable run;
        String latitude;
        String longitude;
        String str = "";

        @Override
        public void run() {
            try {
                Log.e("fd", "dsfds");
                ss = new ServerSocket(8700);




                while (true) {

                    Socket socket = ss.accept();
                    BufferedReader in = new BufferedReader(
                            new InputStreamReader(socket.getInputStream()));

                            str = in.readLine();


                    Log.i("received response from server", str);
                   in.close();
                  socket.close();
                    if(str.equalsIgnoreCase("get location")){
                        try {
 run = new Runnable() {
    @Override
    public void run() {
        try {
            gpsTracker = new GPSTracker(getApplicationContext());
            latitude = String.valueOf(gpsTracker.getLatitude());
            longitude = String.valueOf(gpsTracker.getLongitude());
            s = new Socket("192.168.1.50", 8700);
            pw = new PrintWriter(s.getOutputStream());
            pw.write(latitude + "," + longitude);
            pw.flush();
            pw.close();
            s.close();
            h.postDelayed(this, 5000);
        }
        catch (Exception ex){

        }
    }
};
h.postDelayed(run, 0);



                        } catch (Exception e) {
                            e.printStackTrace();
                        }
                   }
                    else{
                        h.removeCallbacks(run);
                        s = new Socket("192.168.1.50", 8700);
                        pw = new PrintWriter(s.getOutputStream());
                        pw.write("Stopped");
                        pw.flush();
                        pw.close();
                        s.close();
                    }




                }
            } catch (IOException e) {

            } catch (Exception e) {

            }
        }

    }



}