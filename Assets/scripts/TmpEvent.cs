using Holojam.Utility;
using FRL.IO;
using System;
using UnityEngine.UI;
using Chalktalk;
using Holojam.Network;
using UnityEngine;

public class TmpEvent :  EventPusher{


    public override string Label { get { return "save"; } }
    public override void ResetData() { data = new Flake(0, 0, 0, 0); }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.Push();
        }
    }

}