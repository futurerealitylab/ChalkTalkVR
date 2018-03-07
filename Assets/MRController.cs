using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class MRController : MonoBehaviour {
    public Camera FirstPersonCamera;
    public GameObject TrackedPlanePrefab;
    public GameObject ChalkTalkPrefab;
    public GameObject SearchingForPlaneUI;

    private List<TrackedPlane> m_NewPlanes = new List<TrackedPlane>();
    private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();

    private bool m_IsQuitting = false;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
