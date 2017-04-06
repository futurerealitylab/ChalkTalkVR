using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myCalib : MonoBehaviour {

  public GameObject vivecontroller;

  public GameObject reference;

  public Vector3 deltaPos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    deltaPos = vivecontroller.transform.position - reference.transform.position;
    print(deltaPos);
  }
}
