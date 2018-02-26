using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncViveTracker : Holojam.Tools.Synchronizable
{
    // Try switching between Master Client and regular client on the BuildManager to
    // experiment with auto hosting.
    
    // Expose auto hosting to the inspector
    [SerializeField] bool autoHost;

    public string label;

    public override string Label
    {
        get { return label; } // The unique identifier for this piece of data
    }

    public override bool Host
    {
        get { return false; }
    }

    public override bool AutoHost
    {
        get { return autoHost; }
    }
    
    
    // You need to reset (allocate) this Controller's data before you can use it
    // Awake() calls ResetData() by default
    public override void ResetData()
    {
        // Allocate one int and three floats
        data = new Holojam.Network.Flake(
          1, 1
        );
        
    }

    // Core method in Synchronizable
    protected override void Sync()
    {
        // If this synchronizable is hosting data on the Label
//         if (Sending)
//         {
            // Set the outgoing data
            data.vector3s[0] = Input.acceleration;
            data.vector4s[0] = Input.gyro.attitude;
            Debug.Log("SynchronizableTemplate: sending data on " + Brand);
//         }
// 
//         // If this synchronizable is listening for data on the Label
//         else
//         {
//             if (Tracked)
//             { // Do something with the incoming data if it's tracked
//                 Debug.Log(
//                   "SynchronizableTemplate: data is coming in on " + Brand
//                   + " from " + Source
//                   + " (MyInt = " +  ")",
//                   this
//                 );
//                 
//             }
// 
//             // Not tracked--either nobody is hosting on the Label, or this client
//             // is not connected to the network
//             else
//             {
//                 Debug.Log(
//                   "SynchronizableTemplate: no data coming in on " + Brand,
//                   this
//                 );
//             }
//         }
    }
}
