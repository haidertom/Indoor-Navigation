using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObj : MonoBehaviour {




	// Use this for initialization
	void Start () {
      
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Camera.main.transform.position, Vector3.up, 10 * Time.deltaTime);
	}
}
