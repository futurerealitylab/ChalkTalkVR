﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DedicatedCmr : MonoBehaviour {

    public Transform vrcamera;

    public Vector3 offsetT;

	// Use this for initialization
	void Start () {
        //vrcamera = GetComponent<Camera>().transform;
        
    }
	
	// Update is called once per frame
	void LateUpdate () {
        //transform.parent.position = vrcamera.position;
        //transform.parent.rotation = vrcamera.rotation;

        transform.position = vrcamera.position + vrcamera.rotation * offsetT;
        //transform.position = vrcamera.position + offsetT;

        Vector3 eulerAngles = vrcamera.eulerAngles;
        eulerAngles.z = 0.0f;

        if (Vector3.Distance(transform.position, vrcamera.position) < 0.01f)
            transform.rotation = vrcamera.rotation;
        else
          transform.LookAt(vrcamera);



        // transform.eulerAngles = Vector3.up;
        //transform.rotation =  vrcamera.rotation;
        //transform.localPosition = new Vector3(0, 0.6f, -0.9f);
        //transform.localRotation = Quaternion.identity;
	}
}
