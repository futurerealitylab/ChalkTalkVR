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
        transform.position = vrcamera.position;
        transform.rotation = vrcamera.rotation;
	}
}
