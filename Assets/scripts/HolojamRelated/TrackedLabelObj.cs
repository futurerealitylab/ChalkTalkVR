using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.Network;
using FRL.Utility;
using UnityEngine.UI;

public class TrackedLabelObj : MonoBehaviour {

    public string label;

    public bool isTracked = false;

    public byte[] b;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.isTracked = XRNetworkClient.IsTracked(label);

         b = XRNetworkClient.GetBytes(label);
    }
}
