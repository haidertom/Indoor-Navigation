using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateComp : MonoBehaviour {
    Quaternion initialRotation;
    Vector3 relativePositionQ;
    Quaternion targetRotationQ;

    public static float bearingAngleFromOtherScript = 0;

    public Transform targetQ;

     

	// Use this for initialization
	void Start () {
        initialRotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z));

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
       // relativePositionQ = targetQ.position - transform.position;
       // targetRotationQ = Quaternion.LookRotation(relativePositionQ);

       // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationQ, Time.time * 1f);
       // transform.localRotation = Quaternion.Euler(new Vector3(initialRotation.x, transform.localRotation.y, initialRotation.z));
       
        //var targetPosLocal = Camera.transform.InverseTransformPoint(targetObjectPosition);
        //var targetAngle = -Mathf.Atan2(targetPosLocal.x, targetPosLocal.y) * Mathf.Rad2Deg - 90;
        //ArrowUIObject.eulerAngles = new Vector3(0, 0, targetAngle);

        bearingAngleFromOtherScript = GetGPS.bearingAngleToOtherScript;
      //  transform.eulerAngles = new Vector3(90, 0 , bearingAngleFromOtherScript);

        transform.localRotation = Quaternion.Euler(new Vector3(initialRotation.x, bearingAngleFromOtherScript, initialRotation.z));
       // transform.rotation = bearingAngleFromOtherScript;
	}
}
