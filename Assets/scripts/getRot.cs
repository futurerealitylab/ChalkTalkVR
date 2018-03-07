using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getRot : MonoBehaviour {

    [SerializeField] Vector3 eulerangle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        eulerangle = transform.rotation.eulerAngles;

    }
}
