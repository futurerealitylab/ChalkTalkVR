using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testVT : MonoBehaviour {

    public Transform source;
    public Vector3 offset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 sourcePosition = new Vector3(-source.position.x, source.position.y, source.position.z);
        Quaternion sourceRotation = new Quaternion(source.rotation.x, source.rotation.y, source.rotation.z, source.rotation.w);
        sourcePosition += sourceRotation * offset;
        transform.position = sourcePosition;
    }
}
