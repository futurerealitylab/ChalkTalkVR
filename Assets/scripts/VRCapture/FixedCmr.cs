using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCmr : MonoBehaviour {

    Transform vrcamera;

    public Vector3 offsetT;

	// Use this for initialization
	void Start () {
        //vrcamera = Camera.main.transform;
        
    }
	
	// Update is called once per frame
	void Update () {
        //transform.parent.position = vrcamera.position;
        //transform.parent.rotation = vrcamera.rotation;

        transform.position = offsetT;
        transform.rotation = Quaternion.identity;

        //transform.localPosition = new Vector3(0, 0.6f, -0.9f);
        //transform.localRotation = Quaternion.identity;
	}
}
