using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncIMU : Holojam.Tools.SynchronizableTrackable
{

    [SerializeField] Vector3 myacceleration;
    [SerializeField] Quaternion mygyroattitude;

    // As an example, expose all the Synchronizable properties in the inspector.
    // In practice, you probably want to control some or all of these manually in code.

    [SerializeField] string label = "imu";
    [SerializeField] string scope = "";

    [SerializeField] bool host = true;
    [SerializeField] bool autoHost = false;

    public Quaternion imuRotation;

    // As an example, allow all the Synchronizable properties to be publicly settable
    // In practice, you probably want to control some or all of these manually in code.

    public void SetLabel(string label) { this.label = label; }
    public void SetScope(string scope) { this.scope = scope; }

    public void SetHost(bool host) { this.host = host; }
    public void SetAutoHost(bool autoHost) { this.autoHost = autoHost; }

    // Point the property overrides to the public inspector fields

    public override string Label { get { return label; } }
    public override string Scope { get { return scope; } }

    public override bool Host { get { return host; } }
    public override bool AutoHost { get { return autoHost; } }

    // Add the scale vector to Trackable, which by default only contains position/rotation
    public override void ResetData()
    {


        if (!Application.platform.ToString().Contains("Windows"))
            host = true;
        else
            host = false;
        data = new Holojam.Network.Flake(1, 1);

        Input.gyro.enabled = true;
        imuRotation = Quaternion.identity;
        //imutrans = new Vector3(-180, 0, 90);
        //rhs2lhs = new Vector4(-1, 1, -1, 1);
    }

    public Vector3 imutrans;
    public Vector4 rhs2lhs;
    // Override Sync() to include the scale vector
    protected override void Sync()
    {
        //base.Sync();

        if (Sending)
        {
            //data.vector3s[0] = Input.acceleration;
            data.vector4s[0] = Input.gyro.attitude;
            
            //data.vector3s[0] = imu.ToEulerAngles();
        }
//         else
//         {
            //transform.localPosition = data.vector3s[0];
            Quaternion imu = data.vector4s[0];
            //             imu = Quaternion(imu.x * rhs2lhs.x,
            //                 -imu.y * rhs2lhs.y,
            //                 -imu.w * rhs2lhs.z,
            //                 -imu.z * rhs2lhs.w);
//             imu = Quaternion(imu.x /** rhs2lhs.x*/,
//                 -imu.y /** rhs2lhs.y*/,
//                 -imu.w /** rhs2lhs.z*/,
//                 -imu.z/* * rhs2lhs.w*/);
            imu.x = data.vector4s[0].x * rhs2lhs.x;
            imu.y = data.vector4s[0].z * rhs2lhs.y;
            imu.z = data.vector4s[0].y * rhs2lhs.z;
            imu.w = data.vector4s[0].w * rhs2lhs.w;
        imuRotation = Quaternion.Euler(imutrans) * imu;
            transform.rotation = imuRotation;

            
       // }
    }

    protected override void Update()
    {
        if (autoHost) host = Sending; // Lock host flag
        base.Update();
    }
}
