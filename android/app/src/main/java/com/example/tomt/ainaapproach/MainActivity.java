package com.example.tomt.ainaapproach;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.CountDownTimer;
import android.widget.Toast;

public class MainActivity extends Activity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        //setContentView(R.layout.activity_main);

        Toast toast = Toast.makeText(getApplicationContext(), "onCreate called successfully ", Toast.LENGTH_SHORT);
        toast.show();

        new CountDownTimer(100, 10000) {
            public void onTick(long millisUntilFinished) {
            }

            public void onFinish() {



                // Aufruf der nächsten Aktivität
                Intent intent = new Intent(MainActivity.this, SimpleInterfaceActivity.class);
                startActivity(intent);
                MainActivity.this.finish();
            }
        }.start();

    }
    
    public static void CallActivity(Activity activity)
    {
        Intent intent = new Intent (activity, MainActivity.class);
        activity.startActivity(intent);
    }
    
}
