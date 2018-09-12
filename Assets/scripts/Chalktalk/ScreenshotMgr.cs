using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotMgr : MonoBehaviour {

    // one function for save and maintain current CurvePrefabs, and then setup or change pos of current binding box
    List<GameObject> staticCurveObjs;

    public bool curControlThumbClick, curControlOne, curControlThree;

    // Use this for initialization
    void Start () {
        staticCurveObjs = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
        curControlThumbClick = OVRInput.Get(OVRInput.Button.PrimaryThumbstick);
        curControlOne = OVRInput.Get(OVRInput.Button.One);
        curControlThree = OVRInput.Get(OVRInput.Button.Three);
        if (curControlOne || curControlThree) {
            print("thumb click");
            SaveScreenShot();
        }

    }

    public void SaveScreenShot()
    {
        // search by name I guess
        GameObject[] curveObjs;
        curveObjs = GameObject.FindGameObjectsWithTag("screenshot");
        // create by them
        foreach (GameObject go in curveObjs) {
            GameObject gocopy = GameObject.Instantiate(go, go.transform.position, go.transform.rotation, transform);
            gocopy.tag = "Untagged";
            staticCurveObjs.Add(gocopy);
            go.SetActive(false);
            //staticCurveObjs[staticCurveObjs.Count - 1].transform.parent = transform;
        }
    }
}
