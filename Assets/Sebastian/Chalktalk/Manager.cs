using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holojam.Utility;
using FRL.IO;
using System;
using UnityEngine.UI;

namespace Chalktalk {
  public class Manager : Global<Manager>, IGlobalTriggerPressSetHandler, IGlobalApplicationMenuPressDownHandler, IGlobalTouchpadTouchHandler, IGlobalTouchpadPressDownHandler, IGlobalTouchpadPressUpHandler, IGlobalTouchpadTouchUpHandler {

    public BindingBox bindingBox;
    public Transform cursor;

    public Text keyText; 

    private MouseEvent mouseEvent;
    private KeyEvent keyEvent;
    private BaseInputModule module;
    private Receiver receiver;

    private bool isKeyDown = false;
    private int currentKey = -1;

    public bool sendMouseMove = false;

    void Awake() {

      receiver = GetComponent<Receiver>();
    }

    void Start() {
      QualitySettings.vSyncCount = 0;
      Application.targetFrameRate = 90;
      InitializeEventHandlers();
    }



    private void Update() {
      if (cursor && receiver.module.gameObject.activeInHierarchy) {
        cursor.transform.position = bindingBox.GetBoundPosition(receiver.module.transform.position, BindingBox.Plane.Z, true);
        if (sendMouseMove && Holojam.Tools.BuildManager.BUILD_INDEX == 1)
          FireMouseMoveEvent(cursor.transform.position);
      }

      if (keyText) {
        keyText.text = (currentKey == -1 ? "" : currentKey.ToString());
      }
    }

    private void InitializeEventHandlers() {
      GameObject eventGO = new GameObject("EventHandler");
      eventGO.transform.SetParent(transform);
      mouseEvent = eventGO.AddComponent<MouseEvent>();
      keyEvent = eventGO.AddComponent<KeyEvent>();
    }



    void FireMouseDownEvent(Vector3 position) {
      Vector2 pos = bindingBox.GetBoundValue(position, BindingBox.Plane.Z, true);
      //Debug.Log("Firing MouseDown Event at position: " + pos);
      mouseEvent.FireMouseDown(pos);
    }

    void FireMouseMoveEvent(Vector3 position) {
      Vector2 pos = bindingBox.GetBoundValue(position, BindingBox.Plane.Z, true);
      //Debug.Log("Firing MouseMove Event at position: " + pos);
      mouseEvent.FireMouseMove(pos);
    }

    void FireMouseUpEvent(Vector3 position) {
      Vector2 pos = bindingBox.GetBoundValue(position, BindingBox.Plane.Z, true);
      //Debug.Log("Firing MouseUp Event at position: " + pos);
      mouseEvent.FireMouseUp(pos);
    }

    void FireKeyDownEvent(int key) {
      
      keyEvent.FireKeyDown(key);
    }

    void FireKeyUpEvent(int key) {
      keyEvent.FireKeyUp(key);
    }


    private bool isClicking = false;
    IEnumerator ClickRoutine(Vector3 position) {
      if (isClicking) yield break;
      FireMouseDownEvent(position);
      yield return new WaitForSeconds(0.3f);
      FireMouseUpEvent(position);
    }

    void IGlobalTriggerPressDownHandler.OnGlobalTriggerPressDown(VREventData eventData) {
      if (module == null) { // && bindingBox.Contains(eventData.module.transform.position)) {
        module = eventData.module;
        FireMouseDownEvent(module.transform.position);
      }
    }

    void IGlobalTriggerPressHandler.OnGlobalTriggerPress(VREventData eventData) {
      if (module == eventData.module) {
        //FireMouseMoveEvent(module.transform.position);
      }
    }

    void IGlobalTriggerPressUpHandler.OnGlobalTriggerPressUp(VREventData eventData) {
      if (module == eventData.module) {
        FireMouseUpEvent(module.transform.position);
        module = null;
      }
    }

    void IGlobalApplicationMenuPressDownHandler.OnGlobalApplicationMenuPressDown(VREventData eventData) {
      StartCoroutine(ClickRoutine(eventData.module.transform.position));
    }

    void IGlobalTouchpadPressDownHandler.OnGlobalTouchpadPressDown(VREventData eventData) {
      isKeyDown = true;
      FireKeyDownEvent(currentKey);
    }

    void IGlobalTouchpadTouchHandler.OnGlobalTouchpadTouch(VREventData eventData) {
      if (!isKeyDown) {
        Vector2 v1 = eventData.touchpadAxis.normalized;
        Vector2 v2 = Vector2.right.normalized;

        if (eventData.touchpadAxis.magnitude < 0.25f) {
          currentKey = 8;
          return;
        }


        float angle = Mathf.Atan2(v1.y, v1.x) - Mathf.Atan2(v2.y, v2.x);

        if (v1.y < v2.y) {
          angle += Mathf.PI * 2;
        }
        angle /= (Mathf.PI * 2);
        //At this point, the value of angle is 0 to 1.
        //angle = (angle + 1) % 8 ;
        //Debug.Log(currentKey);

        currentKey = (int)(angle * 8);
        currentKey = (currentKey + 1) % 8;
        Debug.Log(currentKey);
      }
    }

    void IGlobalTouchpadTouchUpHandler.OnGlobalTouchpadTouchUp(VREventData eventData) {
      currentKey = -1;
    }

    void IGlobalTouchpadPressUpHandler.OnGlobalTouchpadPressUp(VREventData eventData) {
      isKeyDown = false;
      FireKeyUpEvent(currentKey);
    }
  }
}

