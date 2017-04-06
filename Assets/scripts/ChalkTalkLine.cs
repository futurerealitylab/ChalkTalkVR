using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalkTalkLine : MonoBehaviour {

	LineRenderer line;
	Vector3[] points;
	// Use this for initialization
	void Start () {
		line = this.GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetUpPoints (Vector3[] p){
		points = p;
	}

	public void ApplyPoints(){
		line.SetPositions (points);
	}
}