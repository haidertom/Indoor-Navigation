using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Net;
using System.IO.Compression;
using System.Text;


public class RotateCompass : MonoBehaviour
{
    Quaternion initialRotationQ;

    private GameObject BearingGyroObject;
    private GameObject GyroRotZ;
    public static float bearingAngleFromOtherScript = 0;

    public Transform targetQ;

    int iteration = 0;

    private bool RotationBoolean = false;

    // Use this for initialization
    void Start()
    {
        BearingGyroObject = GameObject.FindGameObjectWithTag("BearingGyro");
        GyroRotZ = GameObject.FindGameObjectWithTag("GyroRotZ");
        initialRotationQ = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z));
        StartCoroutine("GetInitAngle");
    }


    public IEnumerator GetInitAngle(){
        

        yield return new WaitForSeconds(2);
        //RotationBoolean = true;
        bearingAngleFromOtherScript = GetGPS.bearingAngleToOtherScript;

        iteration++;

    }
    // Update is called once per frame
    void Update()
    {

        if (GetGPS.getNewBearing == true) {
            Input.ResetInputAxes();
            initialRotationQ = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z));
            StartCoroutine("GetInitAngle");
            GetGPS.getNewBearing = false;
        }
       // if (RotationBoolean == true)
        transform.localRotation = Quaternion.Euler(new Vector3(initialRotationQ.x, initialRotationQ.y, bearingAngleFromOtherScript + Input.gyro.rotationRate.z));
        // 
        GyroRotZ.GetComponent<Text>().text = "GyroRotZ: " + Input.gyro.rotationRate.z;
        BearingGyroObject.GetComponent<Text>().text = "Bearing + (Gyro:) " + bearingAngleFromOtherScript + "it: " + iteration;
        // transform.rotation = bearingAngleFromOtherScript;
    }
}
