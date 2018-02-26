using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMUVis : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = Input.acceleration;
        transform.localRotation = Input.gyro.attitude;
    }
}
