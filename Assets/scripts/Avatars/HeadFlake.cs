using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFlake : Holojam.Network.Controller {
    public override string Label {
        get { return label; }
    }

    public override bool Sending {
        get { return !isPresenter; }
    }

    public bool isPresenter;

    [SerializeField]
    public string label = "A1Head";

    public override void ResetData()
    {
        data = new Holojam.Network.Flake(1, 1);
    }

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    protected override void Update () {
        if (isPresenter) {
            // recv the data
            transform.position = new Vector3(-data.vector3s[0].x, data.vector3s[0].y, data.vector3s[0].z);
            //transform.rotation = data.vector4s[0];
            Quaternion oneeighty = Quaternion.AngleAxis(180, new Vector3(0, 1, 0));
            transform.rotation = data.vector4s[0] * oneeighty;
        } else {
            // send the data
            data.vector3s[0] = Camera.main.transform.position;
            data.vector4s[0] = Camera.main.transform.rotation;
        }
	}
}
