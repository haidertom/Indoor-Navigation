using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Net;
using System.IO.Compression;
using System.Text;

public class NewBehaviourScript : MonoBehaviour
{

    public GameObject PoiNamesText;
    public GameObject NavRouteText;

    String poiNames;
    String navRoute;

    public void Start()
    {
        poiNames = "default";
        navRoute = "default";

        PoiNamesText = GameObject.FindGameObjectWithTag("Pois");
        PoiNamesText.GetComponent<Text>().text = poiNames;
        NavRouteText = GameObject.FindGameObjectWithTag("NavRoute");
        NavRouteText.GetComponent<Text>().text = navRoute;

        StartCoroutine(GetPoisAllFloor());
        StartCoroutine(GetRouteXY());
    }

    // Update is called once per frame
    void Update()
    {


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

            poiNames = allPois.pois.Count +  " POIS found on floor " + floor_number +":\n ";

            for (int i = 0; i < allPois.pois.Count; i++)
            {
                poiNames = poiNames + allPois.pois[i].name + "\n";
            }

            PoiNamesText.GetComponent<Text>().text = poiNames;

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
            Debug.Log("Web Request for Route successfull");

            String jsonResponse = www.text;

            ResponseGetRoute NavRoute = JsonUtility.FromJson<ResponseGetRoute>(jsonResponse);

            navRoute = "Navigation Route: \n";

            for (int i = 0; i < NavRoute.pois.Count; i++)
            {
                navRoute = navRoute + "lat: " + NavRoute.pois[i].lat.ToString() + "; lon: " + NavRoute.pois[i].lon.ToString() + "\n";
            }

            NavRouteText.GetComponent<Text>().text = navRoute;
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
        String pois_to = "poi_0d531bdd-036a-46bc-a62f-b879a89fd603";
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

            ResponseGetRoute NavRouteXY = JsonUtility.FromJson<ResponseGetRoute>(jsonResponse);

            navRoute = "Navigation Route: \n";

            for (int i = 0; i < NavRouteXY.pois.Count; i++)
            {
                navRoute = navRoute + "lat: " + NavRouteXY.pois[i].lat.ToString() + "; lon: " + NavRouteXY.pois[i].lon.ToString() + "\n";
            }

            NavRouteText.GetComponent<Text>().text = jsonResponse;
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
