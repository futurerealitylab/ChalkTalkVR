using System;
using System.Collections;
using System.Collections.Generic;
using FRL.IO;
using UnityEngine;

public class ViveCalibHandler : MonoBehaviour, IGlobalTriggerPressDownHandler, IGlobalTriggerPressHandler, IGlobalTriggerPressUpHandler, IGlobalGripPressDownHandler, IGlobalGripPressUpHandler {

  public bool bPressDown;

  /// <summary>
  /// A transform representing the tracking bounds, e.g. the CameraRig prefab.
  /// </summary>
  public Transform centroid;

  public Transform referController;

  public Transform optiObj;

  private Vector3 cachedPosition = Vector3.zero;

  public void OnGlobalTriggerPress(BaseEventData eventData) {
    // do the calibration
    //bPressDown = true;
    //print("right trigger down");
  }

  public void OnGlobalTriggerPressDown(BaseEventData eventData) {
    // do the calibration
    //bPressDown = true;
    //print("right trigger down");
  }

  public void OnGlobalTriggerPressUp(BaseEventData eventData) {
    //bPressDown = false;
    //print("right trigger up");
  }


  // Use this for initialization
  void Start() {
    bPressDown = false;
    if (centroid)
      cachedPosition = centroid.position;
  }

  // Update is called once per frame
  void Update() {
    if (bPressDown) {
      Calibrate(optiObj.position - referController.position);
    }
  }

  /// <summary>
  /// Offset the centroid by its difference to the absolute center.
  /// </summary>
  void Calibrate(Vector3 center) {
    centroid.position = cachedPosition + new Vector3(center.x, center.y + 0.25f, center.z);
    cachedPosition = centroid.position;
  }

  public void OnGlobalGripPressDown(BaseEventData eventData) {
    bPressDown = true;
    print("right trigger down");
  }

  public void OnGlobalGripPressUp(BaseEventData eventData) {
    bPressDown = false;
    print("right trigger up");
  }
}
