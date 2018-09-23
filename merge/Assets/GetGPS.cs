using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Net;
using System.IO.Compression;
using System.Text;

public class GetGPS : MonoBehaviour {

    private GameObject distanceTextLati;
    private GameObject distanceTextLongi;
    private GameObject distanceTextLatiStart;
    private GameObject distanceTextLongiStart;
    private GameObject angleToGO;
    private GameObject angleToMagneticNorth;
    private GameObject BearingAngle;
    private GameObject CamToObject;
    private GameObject distanceText;
    private GameObject PuiID;

    private float currentLatitude;
    private float currentLongitude;
    private float targetLongitude;
    private float targetLatitude;
    private float angle;
    private float angleMagnetic;
    private float bearingAngle;
    private float CamToObjectAngle;

    public ResponseGetRoute NavRouteXY;

    //For usage in RotateCompass.cs
    public static float bearingAngleToOtherScript;
    public static bool getNewBearing = false;

    private double distance;

    private bool setOriginalValues = true;

    //For Nav
    public GameObject PoiNamesText;
    public GameObject NavRouteText;

    String poiNames;
    String navRoute;


	// Use this for initialization
	void Start () {
        distance = 100;
        //Nav
        poiNames = "default";
        navRoute = "default";

        //PoiNamesText = GameObject.FindGameObjectWithTag("Pois");
        //PoiNamesText.GetComponent<Text>().text = poiNames;
        //NavRouteText = GameObject.FindGameObjectWithTag("NavRoute");
        //NavRouteText.GetComponent<Text>().text = navRoute;

        StartCoroutine(GetPoisAllFloor());
        StartCoroutine(GetRouteXY());
        //Nav End



        //Initialization text fields
        distanceTextLatiStart = GameObject.FindGameObjectWithTag("StartLatitude");
        distanceTextLongiStart = GameObject.FindGameObjectWithTag("StartLongitude");
        distanceTextLati = GameObject.FindGameObjectWithTag("Latitude");
        distanceTextLongi = GameObject.FindGameObjectWithTag("Longitude");
        distanceText = GameObject.FindGameObjectWithTag("distance");
        angleToGO = GameObject.FindGameObjectWithTag("AngleToGameobject");
        angleToMagneticNorth = GameObject.FindGameObjectWithTag("AngleToMagneticNorth");
        BearingAngle = GameObject.FindGameObjectWithTag("BearingAngle");
        CamToObject = GameObject.FindGameObjectWithTag("CamToObjectAngle");
        PuiID = GameObject.FindGameObjectWithTag("CurrentPoi");

        // Initialization magnetic sensor
        Input.location.Start();
        Input.compass.enabled = true;

        //while (NavRouteXY.pois.Count == 0) { }

        //Start the Coroutine - GetCoordinates
        StartCoroutine("GetCoordinates");

        ////Only activate for testing in Unity
        //originalLatitude = 49.003526f;
        //originalLongitude = 12.094494f;
        //    targetLatitude = 48.997298f;
        //targetLongitude = 12.093633f;
        //CalcBearingAngle(targetLatitude, targetLongitude, originalLatitude, originalLongitude);
        ////Testing end

	}

    IEnumerator GetCoordinates()
    {

        yield return NavRouteXY;

        int NavI = NavRouteXY.pois.Count;

        for (int i = 0; i < NavI; i++)
        {

            while (true)
            {



                if (!Input.location.isEnabledByUser) yield break;

                // Start service before querying location
                Input.location.Start(1f, .1f);

                // Wait until service initializes
                int maxWait = 20;
                while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
                {
                    yield return new WaitForSeconds(1);
                    maxWait--;
                }

                // Service didn't initialize in 20 seconds
                if (maxWait < 1)
                {
                    print("Timed out");
                    yield break;
                }

                // Connection has failed
                if (Input.location.status == LocationServiceStatus.Failed)
                {
                    print("Unable to determine device location");
                    yield break;
                }
                else
                {
                    //if (setOriginalValues)
                    //{
                     currentLatitude = Input.location.lastData.latitude;
                     currentLongitude = Input.location.lastData.longitude;

                    ////49.003553, 12.094526
                    //currentLatitude = 49.0035536f;
                    //currentLongitude = 12.094526f;

                    distanceTextLatiStart.GetComponent<Text>().text = "Latitude: " + currentLatitude;
                    distanceTextLongiStart.GetComponent<Text>().text = "Longitude: " + currentLongitude;
                    setOriginalValues = false;
                    //}

                    //currentLatitude = Input.location.lastData.latitude;
                    //currentLongitude = Input.location.lastData.longitude;

                    //Hier einlesen der Navigationskoordinaten in For schleife
                    //49.003212, 12.093448

                    targetLatitude = float.Parse(NavRouteXY.pois[i].lat);
                    targetLongitude = float.Parse(NavRouteXY.pois[i].lon);

                    PuiID.GetComponent<Text>().text = "Current Poi: " + NavRouteXY.pois[i].puid + " i: " + i ;

                    distanceTextLati.GetComponent<Text>().text = "Latitude: " + targetLatitude;
                    distanceTextLongi.GetComponent<Text>().text = "Longitude: " + targetLongitude;

                    Calc(currentLatitude, currentLongitude, targetLatitude, targetLongitude);
                    CalcBearingAngle(targetLatitude, targetLongitude, currentLatitude, currentLongitude);

                    //angleMagnetic = Quaternion.Euler(0, -Input.compass.magneticHeading, 0).y;
                    angleMagnetic = Input.compass.trueHeading;
                    angleToMagneticNorth.GetComponent<Text>().text = "Angle to magnetic north: " + angleMagnetic;



                    //Cam to Object Angle
                    CamToObjectAngle = Mathf.RoundToInt(calculateDifferenceBetweenAngles(angleMagnetic, bearingAngle));
                    CamToObject.GetComponent<Text>().text = "Angle to Gameobject with cam: " + CamToObjectAngle;
                    bearingAngleToOtherScript = CamToObjectAngle;
                }
                Input.location.Stop();

                if (distance < 4)
                {
                    break;
                }


            }
            getNewBearing = true;
        }
    }

	// Update is called once per frame
	void Update () {


	}

    private float calculateDifferenceBetweenAngles(float firstAngle, float secondAngle)
    {
           float difference =  firstAngle - secondAngle;
        while (difference < -180) difference += 360;
        while (difference > 180) difference -= 360;
        //float difference = 360 - Mathf.Abs(firstAngle - secondAngle);
      //  float difference = (firstAngle - secondAngle) + 180;
        //difference = (difference / 360.0f);
           // difference = ((difference - Mathf.RoundToInt(difference)) * 360.0f) - 180f;


        return difference;
    }

    public void Calc(float lat1, float lon1, float lat2, float lon2)
    {

        var R = 6378.137; // Radius of earth in KM
        var dLat = lat2 * Mathf.PI / 180 - lat1 * Mathf.PI / 180;
        var dLon = lon2 * Mathf.PI / 180 - lon1 * Mathf.PI / 180;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
          Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) *
          Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        distance = R * c;
        distance = distance * 1000f; // meters
                                     //set the distance text on the canvas
        distanceText.GetComponent<Text>().text = "Distance: " + distance + "m";
        angle =  Camera.main.transform.eulerAngles.y ;

        //int Delete_Nav = NavRouteXY.pois.Count;

        angleToGO.GetComponent<Text>().text = "Angle to Gameobject without cam: " + angle ;

        //convert distance from double to float
        //float distanceFloat = (float)distance;
        //set the target position of the ufo, this is where we lerp to in the update function
        //targetPosition = originalPosition - new Vector3(0, 0, distanceFloat * 12);
        //distance was multiplied by 12 so I didn't have to walk that far to get the UFO to show up closer

    }

    public void CalcBearingAngle(float lat1, float lon1, float lat2, float lon2)
    {
        float y = Mathf.Cos(lat2) * Mathf.Sin(lon2 - lon1);
        float x = (Mathf.Cos(lat1) * Mathf.Sin(lat2)) - (Mathf.Sin(lat1) * Mathf.Cos(lat2) * Mathf.Cos(lon2 - lon1));
        bearingAngle = Mathf.Atan2(y, x);
        //bearingAngle = bearingAngle * 180 / Mathf.PI;
        bearingAngle = ((bearingAngle * Mathf.Rad2Deg)+180)% 360;
        BearingAngle.GetComponent<Text>().text = "Bearing Angle: " + bearingAngle;

    }

    IEnumerator GetPoisAllFloor()
    {
        String requestUrl = "http://oth-regensburg.de/fileadmin/media/server_909_90_/st/8985642%&tz/BA%20-%20Navigation%20OTH/Code/Aktuell";
        String buid = "building_2c2afcb0-1c63-470d-8407-93bddf8247ae_1504705971600";
        String floor_number = "1";

        WWW www;
        Hashtable postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");

        RequestObject requestObject = new RequestObject
        {
            username = "username",
            password = "pass",
            buid = buid,
            floor_number = floor_number
        };

        String jsonString = JsonUtility.ToJson(requestObject);

        var formData = System.Text.Encoding.UTF8.GetBytes(jsonString);

        www = new WWW(requestUrl, formData, postHeader);

        yield return www;

        if (www != null)
        {
            Debug.Log("Web Request for POIS successfull");

            String jsonResponse = www.text;

            ResponseGetPois allPois = JsonUtility.FromJson<ResponseGetPois>(jsonResponse);

            poiNames = allPois.pois.Count + " POIS found on floor " + floor_number + ":\n ";

            for (int i = 0; i < allPois.pois.Count; i++)
            {
                poiNames = poiNames + allPois.pois[i].name + "\n";
            }

            //PoiNamesText.GetComponent<Text>().text = poiNames;

        }
        else
        {
            Debug.Log("Web Request for POIS failed");
        }
    }

    IEnumerator GetRoutePois()
    {
        String requestUrl = "http://oth-regensburg.de/fileadmin/media/server_909_90_/st/8985642%&tz/BA%20-%20Navigation%20OTH/Code/Aktuell";
        String pois_to = "poi_67f310b8-ce9d-47bc-b438-26b3373bea83";
        String pois_from = "poi_4d1f350f-3cf0-47d8-907c-b7307430d612";
        String floor_number = "1";

        WWW www;
        Hashtable postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");

        RequestObject requestObject = new RequestObject
        {
            username = "username",
            password = "pass",
            pois_from = pois_from,
            pois_to = pois_to,
            floor_number = floor_number
        };
        string jsonString = JsonUtility.ToJson(requestObject);

        var formData = System.Text.Encoding.UTF8.GetBytes(jsonString);

        www = new WWW(requestUrl, formData, postHeader);

        yield return www;

        if (www != null)
        {
           // Debug.Log("Web Request for Route successfull");

            String jsonResponse = www.text;

            ResponseGetRoute NavRoute = JsonUtility.FromJson<ResponseGetRoute>(jsonResponse);

            navRoute = "Navigation Route: \n";

            for (int i = 0; i < NavRoute.pois.Count; i++)
            {
                navRoute = navRoute + "lat: " + NavRoute.pois[i].lat.ToString() + "; lon: " + NavRoute.pois[i].lon.ToString() + "\n";
            }

           // NavRouteText.GetComponent<Text>().text = navRoute;
        }
        else
        {
            Debug.Log("Web Request for Route failed");
        }
    }

    IEnumerator GetRouteXY()
    {
        String requestUrl = "http://oth-regensburg.de/fileadmin/media/server_909_90_/st/8985642%&tz/BA%20-%20Navigation%20OTH/Code/Aktuell";
        String buid = "building_2c2afcb0-1c63-470d-8407-93bddf8247ae_1504705971600";
        String pois_to = "poi_67f310b8-ce9d-47bc-b438-26b3373bea83";
        String lat_to = "49.00350321415986";
        String lon_to = "12.094378173351288";

        String floor_number = "1";

        WWW www;
        Hashtable postHeader = new Hashtable();
        postHeader.Add("Content-Type", "application/json");

        RequestObject requestObject = new RequestObject
        {
            username = "username",
            password = "pass",
            buid = buid,
            coordinates_lat = lat_to,
            coordinates_lon = lon_to,
            pois_to = pois_to,
            floor_number = floor_number
        };
        string jsonString = JsonUtility.ToJson(requestObject);

        var formData = System.Text.Encoding.UTF8.GetBytes(jsonString);

        www = new WWW(requestUrl, formData, postHeader);

        yield return www;

        if (www != null)
        {
            Debug.Log("Web Request for Route XY successfull");

            String jsonResponse = www.text;

            NavRouteXY = JsonUtility.FromJson<ResponseGetRoute>(jsonResponse);

            navRoute = "Navigation Route: \n";

            for (int i = 0; i < NavRouteXY.pois.Count; i++)
            {
                navRoute = navRoute + "lat: " + NavRouteXY.pois[i].lat.ToString() + "; lon: " + NavRouteXY.pois[i].lon.ToString() + "\n";
            }

           // NavRouteText.GetComponent<Text>().text = navRoute;
        }
        else
        {
            Debug.Log("Web Request for Route failed");
        }
    }


}

[System.Serializable]
public class RequestObject
{
    public string username;
    public string password;
    public string pois_to;
    public string pois_from;
    public string coordinates_lat;
    public string coordinates_lon;
    public string floor_number;
    public string buid;
}

[System.Serializable]
public class ResponseGetPois_Data
{
    public string is_building_entrance;
    public string floor_number;
    public string pois_type;
    public string buid;
    public string image;
    public string coordinates_lon;
    public string url;
    public string coordinates_lat;
    public string floor_name;
    public string description;
    public string name;
    public string is_door;
    public string is_published;
    public string puid;
}

public class ResponseGetPois
{
    public List<ResponseGetPois_Data> pois;
}

[System.Serializable]
public class ResponseGetRoute_Data
{
    public string lat;
    public string lon;
    public string puid;
    public string buid;
    public string floor_number;
    public string pois_type;
}

[System.Serializable]
public class ResponseGetRoute
{
    public int num_of_pois;
    public List<ResponseGetRoute_Data> pois;
    public string status;
    public string message;
    public int status_code;
}
