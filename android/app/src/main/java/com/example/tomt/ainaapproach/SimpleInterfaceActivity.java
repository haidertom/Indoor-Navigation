package com.example.tomt.ainaapproach;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.res.AssetManager;
import android.net.wifi.ScanResult;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.telecom.Call;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;
import android.util.Log;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileOutputStream;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.MalformedURLException;
import java.net.ProtocolException;
import java.net.URISyntaxException;
import java.net.URL;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.zip.GZIPInputStream;

import javax.net.ssl.HttpsURLConnection;

import com.example.tomt.ainaapproach.Algorithms;
import com.example.tomt.ainaapproach.LocDistance;
import com.example.tomt.ainaapproach.LogRecord;
import com.example.tomt.ainaapproach.RadioMap;
import com.example.tomt.ainaapproach.SimpleWifiManager;
import com.example.tomt.ainaapproach.WifiReceiver;

public class SimpleInterfaceActivity extends Activity {

    public static ArrayList<LogRecord> latestScanList;
    public static String radioMapString;
    public static String calculatedLocation;
    public static RadioMap rm;
    public static File myRadioFile = null;

    public static Activity unityActivity;
    public static Context myContext;

    public static String receivedSignals;
    public static String createdList;
    public static SimpleWifiManager myWifiInstance;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        myContext = this.getApplicationContext();
        latestScanList = new ArrayList<LogRecord>();
        rm = new RadioMap();
        myWifiInstance = SimpleWifiManager.getInstance(getApplicationContext());

        //setContentView(R.layout.activity_simple_interface);

        CallLineToast(getApplicationContext(), "Simple Interface Activity Started");



        // userData = new AnyUserData();

        // ---------- Radio Map ----------

        // ---------- read Radio Map from Text file and save it in a File in the mobile cache ----------
        myRadioFile = new File(getCacheDir()+"/fl_1_indoor-radiomap.txt");

        //my result in a problem in unity, if the following lines are not used
        try {
            InputStream is = getAssets().open("fl_1_indoor-radiomap.txt");
            int size = is.available();
            byte[] buffer = new byte[size];
            is.read(buffer);
            is.close();

            FileOutputStream fos = new FileOutputStream(myRadioFile);
            fos.write(buffer);
            fos.close();

        }
        catch (Exception e) {
            throw new RuntimeException(e);
        }

        String pathString = myRadioFile.getAbsolutePath();



        // ---------- read Radio Map from Text file and save it in a String ----------

        setRadioMapString();
        CallLineToast(getApplicationContext(),"Cached Radio Map: \n \n "+ radioMapString.substring(0,1000));


        //  ---------- Fill the Object of the type RadioMap

        initiateRadioMap();


        // ---------- Scan Wifi ----------

        updateScanList();
        CallLineToast(myContext, "Received Wfi Signals: \n \n "+ receivedSignals);
        CallLineToast(myContext,"Created Scan List: \n \n "+ createdList);

        // ---------- positioning ----------

        updateLocation();


        CallLineToast(getApplicationContext(), "Calculated Location:\n \n" + calculatedLocation);


       finish();

    }


    public static void CallLineToast(final Context AppContext, final String toastMessage)
    {
        new CountDownTimer(1000, 10000) {
            public void onTick(long millisUntilFinished) {
            }

            public void onFinish() {

                Toast toast1 = Toast.makeText(AppContext,toastMessage, Toast.LENGTH_SHORT);
                toast1.show();

            }
        }.start();
    }

    public static String getJsonString(String buid, String floorNumber)
    {

        JSONObject j = new JSONObject();

        try {
            j.put("username", "username");
            j.put("password", "pass");
            j.put("buid", buid);
            j.put("floor", floorNumber);

        } catch (JSONException e) {
            //e.printStackTrace();
        }

        return j.toString();
    }

    public void setRadioMapString() {

        StringBuilder returnRmString = new StringBuilder();

        BufferedReader stringBuffer = null;
        try {
            stringBuffer = new BufferedReader(
                    new InputStreamReader(getAssets().open("fl_1_indoor-radiomap.txt")));

            // do reading, usually loop until end of file reading
            String mLine;
            while ((mLine = stringBuffer.readLine()) != null) {
                returnRmString.append(mLine);
            }
        } catch (IOException e) {
            //log the exception
        } finally {
            if (stringBuffer != null) {
                try {
                    stringBuffer.close();
                } catch (IOException e) {
                    //log the exception
                }
            }
        }

        radioMapString = returnRmString.toString();
    }

    public static void updateScanList(){

        myWifiInstance.startScan();
        List<ScanResult> wifiList = myWifiInstance.getScanResults();

        latestScanList.clear();

        LogRecord lr = null;

        // If we receive results, add them to latest scan list
        if (wifiList != null && !wifiList.isEmpty()) {
            for (int i = 0; i < wifiList.size(); i++) {
                lr = new LogRecord(wifiList.get(i).BSSID, wifiList.get(i).level);
                latestScanList.add(lr);
            }
        }

        receivedSignals = wifiList.toString();
        receivedSignals = receivedSignals.substring(0, 1000);

        createdList = latestScanList.toString();
        createdList = createdList.substring(0, 500);


    }



    public void initiateRadioMap(){
        rm.NaN = "-110";
        rm.MacAdressList = new ArrayList<String>();
        rm.LocationRSS_HashMap = new HashMap<String, ArrayList<String>>();
        rm.OrderList = new ArrayList<String>();

        if (!myRadioFile.exists() || !myRadioFile.canRead()) {
            CallLineToast(getApplicationContext(), "Error reading file");
        }

        rm.RadiomapMean_File = myRadioFile;
        rm.OrderList.clear();
        rm.MacAdressList.clear();
        rm.LocationRSS_HashMap.clear();

        ArrayList<String> RSS_Values = null;
        BufferedReader reader = null;
        String line = null;
        String[] temp = null;
        String key = null;

        try {

            reader = new BufferedReader(new FileReader(myRadioFile));

            // Read the first line # NaN -110
            line = reader.readLine();
            temp = line.split(" ");

            if (!temp[1].equals("NaN")){
                CallLineToast(getApplicationContext(), "Error reading file 1");
            }

            rm.NaN = temp[2];
            line = reader.readLine();

            // Must exists
            if (line == null){
                CallLineToast(getApplicationContext(), "Error reading file 2");
            }

            line = line.replace(", ", " ");
            temp = line.split(" ");

            final int startOfRSS = 4;

            // Must have more than 4 fields
            if (temp.length < startOfRSS){
                CallLineToast(getApplicationContext(), "Error reading file 3");
            }

            // Store all Mac Addresses Heading Added
            for (int i = startOfRSS; i < temp.length; ++i)
                rm.MacAdressList.add(temp[i]);

            while ((line = reader.readLine()) != null) {

                if (line.trim().equals(""))
                    continue;

                line = line.replace(", ", " ");
                temp = line.split(" ");

                if (temp.length < startOfRSS){
                    CallLineToast(getApplicationContext(), "Error reading file 4");
                }

                key = temp[0] + " " + temp[1];

                RSS_Values = new ArrayList<String>();

                for (int i = startOfRSS - 1; i < temp.length; ++i)
                    RSS_Values.add(temp[i]);

                // Equal number of MAC address and RSS Values
                if (rm.MacAdressList.size() != RSS_Values.size()){
                    CallLineToast(getApplicationContext(), "Error reading file 5");
                }

                rm.LocationRSS_HashMap.put(key, RSS_Values);

                rm.OrderList.add(key);
            }

        } catch (Exception ex) {

            CallLineToast(getApplicationContext(), "Error reading file 6");

        } finally {
            if (reader != null)
                try {
                    reader.close();
                } catch (IOException e) {

                }
        }

    }

    public static void updateLocation(){

        calculatedLocation = com.example.tomt.ainaapproach.Algorithms.ProcessingAlgorithms( 2);
    }

    public static String returnLocation(){
        return calculatedLocation;
    }



    public static void CallActivity(Activity activity)
    {
        unityActivity = activity;
        Intent intent = new Intent (activity, SimpleInterfaceActivity.class);
        activity.startActivity(intent);
    }
}
