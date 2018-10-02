using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DedicatedCmr : MonoBehaviour {

    Transform vrcamera;

	// Use this for initialization
	void Start () {
        vrcamera = Camera.main.transform;
        
    }
	
	// Update is called once per frame
	void Update () {
        //transform.parent.position = vrcamera.position;
        //transform.parent.rotation = vrcamera.rotation;

        transform.position = vrcamera.position;
        transform.rotation = vrcamera.rotation;

        //transform.localPosition = new Vector3(0, 0.6f, -0.9f);
        //transform.localRotation = Quaternion.identity;
	}
}
