using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holojam.Network;
using Holojam.Tools;
using System;

public class MirageHeadset : Trackable
{
    [SerializeField] public Vector3 offset;
    [SerializeField] public Vector3 offsetRot;

    public enum Mode { IMU, GVR, FUSION }
    public Mode cur_mode;

    public string label = "Viewer";
    public string scope = "Vive";

    public Transform imuObj;

    public FRL.Utility.SmoothingType smoothing;

    private Vector3 lastPosition = Vector3.zero;
    private Quaternion lastRotation = Quaternion.identity;
    private bool lastTracked = false;
    private Quaternion lastIMURot;
    private FRL.Utility.Smoother smoother;

    private Vector3 velocity;
    private Vector3 lastVelocity;
    private float maxMagnitude = 1f;

    private float lastTrackedTime = 0f;
    public bool imuOnly;

    [HideInInspector]
    public float lowPassFactor = 1f;

    public override string Label { get { return label; } }
    public override string Scope { get { return scope; } }

    protected override void Awake()
    {
        base.Awake();
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    protected override void Update()
    {

        base.Update();
    }

    Quaternion IMURot;
    protected override void UpdateTracking()
    {
        base.UpdateTracking();

        this.smoother = FRL.Utility.Smoother.GetSmoother(smoothing);

        IMURot = imuObj.GetComponent<SyncIMU>().imuRotation;
        var newIMU = Quaternion.Slerp(lastIMURot, IMURot, lowPassFactor);
        lastIMURot = IMURot;
        IMURot = newIMU;

        velocity = (transform.position - lastPosition) / Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxMagnitude);

        imuOnly = !Tracked;

        if (cur_mode != Mode.IMU) transform.position = GetCurrentPosition();
        if (cur_mode != Mode.GVR) transform.rotation = GetCurrentRotation();

        lastPosition = transform.position;
        lastRotation = transform.rotation;
        lastTracked = Tracked;
        if (lastTracked) lastTrackedTime = Time.time;
        lastVelocity = velocity;
    }

    private Quaternion GetCurrentRotation()
    {
        var sourceRotation = RawRotation * Quaternion.Euler(offsetRot);
        Quaternion inv = Quaternion.Inverse(IMURot);

        //If we're using sensor fusion, Camera rotation should be IMU rotation.
        //if (cur_mode == Mode.FUSION) transform.localRotation = IMURot;
        //else transform.localRotation = Quaternion.identity;

        if (Tracked)
        {
            if (cur_mode == Mode.FUSION)
            {
                //Sensor fusion. Calculate the y-rotation difference, and return.
                sourceRotation *= inv;
                if (smoother != null) sourceRotation = smoother.Smooth(sourceRotation, ref lastRotation, Time.deltaTime);
                return Quaternion.AngleAxis(sourceRotation.eulerAngles.y, Vector3.up);
            }
            else
            {
                //No sensor fusion. Smooth based on smoother.
                if (smoother != null) sourceRotation = smoother.Smooth(sourceRotation, ref lastRotation, Time.deltaTime);
                return sourceRotation;
            }
        }
        else
        {
            //Untracked. Return last rotation.
            return lastRotation;
        }
    }

    private Vector3 GetCurrentPosition()
    {
        var sourcePosition = RawPosition;

        if (Tracked)
        {
            if (lastTracked && smoother != null)
                return smoother.Smooth(sourcePosition, ref lastPosition, Time.deltaTime);
            else
                return sourcePosition;
        }
        else
            return lastPosition;
    }
}
