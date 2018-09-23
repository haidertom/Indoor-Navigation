using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour {

    public string package;
    public Text T;
    public string Location;

    public AndroidJavaClass unityClass, androidClass;
    public AndroidJavaObject unityInstance;

	// Use this for initialization
	void Start () {

        package = "com.example.tomt.ainaapproach.SimpleInterfaceActivity";
        Location = "not initialized";

        string errorString;

        try
        {
            StartCoroutine(StartAndroidActivity());
        }
        catch (Exception ex)
        {
            errorString = ex.ToString();
        }

	}
	


    IEnumerator StartAndroidActivity()
    {
        unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");              // Unity Klasse erstellen
        unityInstance = unityClass.GetStatic<AndroidJavaObject>("currentActivity");       // Unity Instanz erzeugen

        androidClass = new AndroidJavaClass(package);                                     // Android Klasse erstellen // 

        androidClass.CallStatic("CallActivity", unityInstance);   // statische Funktion lässt sich ohne Instanz aufrufen


        Debug.Log("before yield");
        yield return new WaitForSeconds(2);

        T.text = Location;
        Debug.Log("after yield");



        //androidClass.CallStatic("closeActivity");
    }

    void FixedUpdate()
    {
        Debug.Log("Update " + Time.deltaTime);
        androidClass.CallStatic("updateScanList"); //Funktionen von Statisch zu nicht-statisch ändern
        androidClass.CallStatic("updateLocation");

        Location = androidClass.CallStatic<string>("returnLocation");
        T.text = Location;
    }
}
