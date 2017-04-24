using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalkTalkController : Holojam.Network.Controller
{

    // Try adding two of these components, one sending and one not

    // Expose sending to the inspector
    [SerializeField]
    bool sending = true;

    // Controller property overrides

    public override string Label
    {
        get { return "Stylus"; } // The unique identifier for this piece of data
    }

    public override string Scope
    {
        get { return ""; }
    }

    public override bool Sending
    {
        get { return sending; }
    }

    public override bool Deaf
    {
        get { return true; }
    }

    protected override ProcessDelegate Process
    {
        get { return PrintData; }
    }



    public int Data
    {
        get
        {
            return data.ints[0];
        }
        set
        {
            data.ints[0] = value;
        }
    }

    public Vector3 Pos
    {
        set
        {
            data.vector3s[0] = value;
        }
    }

    public Vector3 Rot
    {
        set
        {
            data.vector3s[1] = value;
        }
    }

    public float[] touchpad
    {
        set
        {
            data.floats = value;
        }
    }
    // Called in Update()
    void PrintData()
    {
        // Controller

        /*
        Debug.Log(
          "There are " + Holojam.Network.Controller.All<ControllerTemplate>().Count
          + " ControllersTemplate(s) in the scene"
        );
        */

        //Debug.Log(
        //  "This ControllerTemplate is " + (Sending ? "sending" : "not sending")
        //  + " and " + (Tracked ? "tracked" : "not tracked") + "\n"
        //  + "It has the brand " + Brand
        //  + (Sending ? "" : " and is coming from " + Source),
        //  this
        //);

        // FlakeComponent

        /*
        if (!Sending) {
          Debug.Log(
            "This ControllerTemplate was last updated at " + Timestamp
            + " (delta = " + DeltaTime() + "ms)",
            this
          );
        }
        */

        //byte[] myBools = Data;

        //Debug.Log(
        //  " MyBools = (" + myBools[0] + ", " + myBools[1] + ")",
        //  this
        //);
    }

    // Controller method overrides

    // If you need to do something on enable
    protected override void OnEnable()
    {
        base.OnEnable();
        Debug.Log("ControllerTemplate enabled", this);
    }

    // If you need to do something on disable
    protected override void OnDisable()
    {
        base.OnDisable();
        Debug.Log("ControllerTemplate disabled", this);
    }

    // Don't do this unless you really need to!
    protected override void Update()
    {
        if (!Application.isPlaying) return;

        //Debug.Log("ControllerTemplate updating", this);
        //Debug.Log(Data);
        Process(); // Mandatory call
    }

    // FlakeComponent method overrides

    // If you need to do something on Awake()
    protected override void Awake()
    {
        Debug.Log(
          "ControllerTemplate data before allocation: "
          + data, this
        );

        ResetData();

        Debug.Log(
          "ControllerTemplate data after allocation: "
          + data, this
        );
    }

    // You need to reset (allocate) this Controller's data before you can use it
    // Awake() calls ResetData() by default
    public override void ResetData()
    {
        data = new Holojam.Network.Flake(
          2, 0, 2, 1, 0, false
        );
    }
}

