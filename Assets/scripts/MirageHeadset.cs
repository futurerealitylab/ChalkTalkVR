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

    private Vector3 prevPosition = Vector3.zero;
    public Quaternion prevRotation = Quaternion.identity;
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
        prevPosition = transform.position;
        prevRotation = transform.rotation;
    }

    protected override void Update()
    {

        //base.Update();
		UpdateTracking();
    }

    Quaternion IMURot;
    protected override void UpdateTracking()
    {
        base.UpdateTracking();

        this.smoother = FRL.Utility.Smoother.GetSmoother(smoothing);

        //IMURot = imuObj.GetComponent<SyncIMU>().imuRotation;
		// for test
		IMURot = imuObj.transform.localRotation;
		//print ("imu rotation:" + imuObj.transform.rotation.ToString ("F3"));
		print ("imu local rotation:" + imuObj.transform.localRotation.ToString ("F3"));
		if (IMURot == Quaternion.identity)
			return;
        var newIMU = Quaternion.Slerp(lastIMURot, IMURot, lowPassFactor);
        lastIMURot = IMURot;
        IMURot = newIMU;

        velocity = (transform.position - prevPosition) / Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxMagnitude);

        //imuOnly = !Tracked;

        if (cur_mode != Mode.IMU) transform.position = GetCurrentPosition();
        if (cur_mode != Mode.GVR) transform.rotation = GetCurrentRotation();
		print ("transform.rotation:" + transform.rotation.eulerAngles.ToString ("F3"));
        prevPosition = transform.position;
        prevRotation = transform.rotation;
		print ("prevRotation:" + prevRotation.eulerAngles.ToString ("F3"));
        lastTracked = Tracked;
        if (lastTracked) lastTrackedTime = Time.time;
        lastVelocity = velocity;
    }
    public Quaternion sourceRotation;
    private Quaternion GetCurrentRotation()
    {
		print ("------\nRawRotation:" + RawRotation.eulerAngles.ToString("F3"));
        sourceRotation = RawRotation * Quaternion.Euler(offsetRot);
		print ("sourceRotation:" + sourceRotation.eulerAngles.ToString("F3"));
        Quaternion inv = Quaternion.Inverse(IMURot);
		print ("IMURot:" + IMURot.ToString("F3"));
		print ("inv:" + inv.eulerAngles.ToString("F3"));
		print ("last tracked:" + lastTracked);

        //If we're using sensor fusion, Camera rotation should be IMU rotation.
        //if (cur_mode == Mode.FUSION) transform.localRotation = IMURot;
        //else transform.localRotation = Quaternion.identity;

        if (Tracked)
        {
            if (cur_mode == Mode.FUSION )
            {
				//Sensor fusion. Calculate the y-rotation difference, and return.
				sourceRotation *= inv;
				print ("sourceRotation after /imu:" + sourceRotation.eulerAngles.ToString ("F3"));
				if (lastTracked) {
					print ("prevRotation:" + prevRotation.eulerAngles.ToString ("F3"));
					if (smoother != null)
						sourceRotation = smoother.Smooth (sourceRotation, ref prevRotation, Time.deltaTime);
					print ("prevRotation:" + prevRotation.eulerAngles.ToString ("F3"));
					print ("sourceRotation after smooth:" + sourceRotation.eulerAngles.ToString ("F3"));
					return Quaternion.AngleAxis (sourceRotation.eulerAngles.y, Vector3.up);
				} else {
					return Quaternion.AngleAxis (sourceRotation.eulerAngles.y, Vector3.up);
				}
            }
            else
            {
                //No sensor fusion. Smooth based on smoother.
                if (smoother != null) sourceRotation = smoother.Smooth(sourceRotation, ref prevRotation, Time.deltaTime);
				return Quaternion.AngleAxis(sourceRotation.eulerAngles.y, Vector3.up);
            }
        }
        else
        {
            //Untracked. Return last rotation.
			return Quaternion.AngleAxis(prevRotation.eulerAngles.y, Vector3.up);
        }
    }

    private Vector3 GetCurrentPosition()
    {
        var sourcePosition = RawPosition;

        if (Tracked)
        {
            if (lastTracked && smoother != null)
                return smoother.Smooth(sourcePosition, ref prevPosition, Time.deltaTime);
            else
                return sourcePosition;
        }
        else
            return prevPosition;
    }
}
