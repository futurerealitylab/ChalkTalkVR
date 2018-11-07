using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPosition : MonoBehaviour {

    public Transform fixedCmr;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = fixedCmr.position;// Vector3.zero;
        transform.rotation = fixedCmr.rotation;// Quaternion.identity;
	}
}
