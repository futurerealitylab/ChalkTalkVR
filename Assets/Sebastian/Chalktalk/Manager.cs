using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holojam.Utility;
using FRL.IO;
using System;

namespace Chalktalk {
  public class Manager : Global<Manager>, IGlobalTriggerPressSetHandler, IGlobalApplicationMenuPressDownHandler, IGlobalTouchpadTouchHandler {

    public BindingBox bindingBox;
    public Transform cursor;
 
    private MouseEvent mouseEvent;
    private KeyEvent keyEvent;
    private BaseInputModule module;
    private Receiver receiver;

    void Awake() {
      receiver = GetComponent<Receiver>();
    }

    void Start() {
      InitializeEventHandlers();
    }

    private void Update() {
      if (cursor) {
        cursor.transform.position = bindingBox.GetBoundPosition(receiver.module.transform.position, BindingBox.Plane.Z, true);
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
      Debug.Log("Firing MouseDown Event at position: " + pos);
      mouseEvent.FireMouseDown(pos);
    }

    void FireMouseMoveEvent(Vector3 position) {
      Vector2 pos = bindingBox.GetBoundValue(position, BindingBox.Plane.Z, true);
      Debug.Log("Firing MouseMove Event at position: " + pos);
      mouseEvent.FireMouseMove(pos);
    }

    void FireMouseUpEvent(Vector3 position) {
      Vector2 pos = bindingBox.GetBoundValue(position, BindingBox.Plane.Z, true);
      Debug.Log("Firing MouseUp Event at position: " + pos);
      mouseEvent.FireMouseUp(pos);
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
        FireMouseMoveEvent(module.transform.position);
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

    void IGlobalTouchpadTouchHandler.OnGlobalTouchpadTouch(VREventData eventData) {

      float dp = Vector2.Dot(Vector2.up, eventData.touchpadAxis);
      Debug.Log(Mathf.Rad2Deg * Mathf.PI * dp);
    }
  }
}

