using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holojam.Network;
using Holojam.Tools;

public class DebugNetwork : Trackable
{
    public TextMesh tm;

    public Holojam.Network.Client client;

    public Trackable debugLabel;

    string label;
    public string scope = "";

    bool isShow;

    public override string Label { get { return label; } }
    public override string Scope { get { return scope; } }

    protected override void Awake()
    {
        base.Awake();
        isShow = true;
        label = debugLabel.Label;
    }

    //Update late to catch local space updates
    protected override void UpdateTracking()
    {
        //base.UpdateTracking();
        if (isShow) {
            tm.text = data.vector3s[0].ToString();
            tm.text += "\n" + data.vector4s[0].ToString();
            tm.text += "\nLatency:" + client.gameObject.GetComponent<Holojam.Network.Ping>().CorrectedLatency + "ms";
            tm.text += "\nDwPort:" + client.DownstreamPort;
        }
        else
        {
            tm.text = "";
        }
        
    }

    public void SwitchText(){
        isShow = !isShow;
        
    }
}
