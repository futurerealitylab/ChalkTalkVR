using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLootat : MonoBehaviour {
    public GameObject p;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Transform trans = GetComponent<Transform>();
        Vector3 myPos = trans.position;
        myPos.x = p.transform.position.x;
        myPos.z = p.transform.position.z;
        trans.LookAt(myPos);
	}
}
