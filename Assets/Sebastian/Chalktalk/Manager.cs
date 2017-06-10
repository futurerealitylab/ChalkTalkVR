using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holojam.Utility;
using FRL.IO;
using System;
using UnityEngine.UI;

namespace Chalktalk {
  public class Manager : Global<Manager>, IGlobalTriggerPressSetHandler, IGlobalApplicationMenuPressDownHandler, IGlobalApplicationMenuPressUpHandler, IGlobalTouchpadTouchHandler, IGlobalTouchpadPressDownHandler, IGlobalTouchpadPressUpHandler, IGlobalTouchpadTouchUpHandler {

    public BindingBox bindingBox;
    public Transform cursor;

    public Text keyText; 

    private MouseEvent mouseEvent;
    private KeyEvent keyEvent;
    private BaseInputModule module;
    private Receiver receiver;

    private bool isKeyDown = false;
    private int currentKey = -1;
        private ChalkTalkController ctc;

        //Send Bytes for ChalkTalk
        [SerializeField]
        private byte button = new byte();
    private byte[] Buttons = new byte[9];
    private byte[] Data = new byte[33];
    public bool sendMouseMove = false;

    void Awake() {

      receiver = GetComponent<Receiver>();
      ctc = gameObject.AddComponent<ChalkTalkController>();
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
//            UpdateByte(receiver.module.transform.position, receiver.module.transform.rotation);

    }
		/*
    private void UpdateByte(Vector3 v, Quaternion q)
    {
            //Array.Copy(BitConverter.GetBytes(v.x), 0, Data, 0, 4);
            // Array.Copy(BitConverter.GetBytes(v.y), 0, Data, 4, 4);
            // Array.Copy(BitConverter.GetBytes(v.z), 0, Data, 8, 4);
            //Array.Copy(BitConverter.GetBytes(q.x), 0, Data, 12, 4);
            // Array.Copy(BitConverter.GetBytes(q.y), 0, Data, 16, 4);
            // Array.Copy(BitConverter.GetBytes(q.z), 0, Data, 20, 4);
            //Array.Copy(b, 0, Data, 24, 9);
            ctc.Pos = v;
            ctc.Rot = new Vector3(q.x, q.y, q.z);
            ctc.Data = button;
            Debug.Log(button);
          
     }

    private void UpdateButtonByte(float[] xy,byte b)
        {
            //Array.Copy(b, 0, Data, 24, 9);
            ctc.touchpad = xy;
            ctc.Data = b;
            Debug.Log(b);
        }

        private void UpdateButtonByte( byte b)
        {
            //Array.Copy(b, 0, Data, 24, 9);
            //ctc.touchpad = xy;
            ctc.Data = b;
            Debug.Log(b);
        }
*/

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
				/*
                //For Sending Bytes
                Buttons[8] = (byte)(Buttons[8] | 0x80);
                button = Buttons[8];
                UpdateButtonByte(button);
				*/
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
				/*
                //For Sending Bytes
                Buttons[8] = (byte)(Buttons[8] & 0x7f);
                button = Buttons[8];
                UpdateButtonByte(button);
				*/
            }
    }

    void IGlobalApplicationMenuPressDownHandler.OnGlobalApplicationMenuPressDown(VREventData eventData) {
      StartCoroutine(ClickRoutine(eventData.module.transform.position));
			/*
            Buttons[8] = (byte)(Buttons[8] | 0x10);
            button = Buttons[8];
            UpdateButtonByte(button);
            */
        }
        void IGlobalApplicationMenuPressUpHandler.OnGlobalApplicationMenuPressUp(VREventData eventData)
        {
			/*
            Buttons[8] = (byte)(Buttons[8] & 0xef);
            button = Buttons[8];
            UpdateButtonByte(button);
            */
        }
        void IGlobalTouchpadPressDownHandler.OnGlobalTouchpadPressDown(VREventData eventData) {
      isKeyDown = true;
      FireKeyDownEvent(currentKey);
			/*
            //For Bytes Sending
            Array.Copy(BitConverter.GetBytes(eventData.touchpadAxis.x), 0, Buttons, 0, 4);
            Array.Copy(BitConverter.GetBytes(eventData.touchpadAxis.x), 0, Buttons, 4, 4);
            float[] tmp = new float[2];
            tmp[0] = eventData.touchpadAxis.x;
            tmp[1] = eventData.touchpadAxis.y;
            //byte b = 0x40 ;
            Buttons[8] = (byte) (Buttons[8] | 0x20);
            button = Buttons[8];
            UpdateButtonByte(tmp,button);
			*/
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
       // Debug.Log(currentKey);
         
      }
			/*
            //For Bytes Sending
            Array.Copy(BitConverter.GetBytes(eventData.touchpadAxis.x), 0, Buttons, 0, 4);
            Array.Copy(BitConverter.GetBytes(eventData.touchpadAxis.x), 0, Buttons, 4, 4);
            //byte b = 0x40 ;
            float[] tmp = new float[2];
            tmp[0] = eventData.touchpadAxis.x;
            tmp[1] = eventData.touchpadAxis.y;
            Buttons[8] = (byte)(Buttons[8] | 0x40);
            button = Buttons[8];
            UpdateButtonByte(tmp, button);
			*/
        }

    void IGlobalTouchpadTouchUpHandler.OnGlobalTouchpadTouchUp(VREventData eventData) {
      currentKey = -1;

			/*
            //For Bytes Sending
            Array.Copy(BitConverter.GetBytes(eventData.touchpadAxis.x), 0, Buttons, 0, 4);
            Array.Copy(BitConverter.GetBytes(eventData.touchpadAxis.x), 0, Buttons, 4, 4);
            float[] tmp = new float[2];
            tmp[0] = eventData.touchpadAxis.x;
            tmp[1] = eventData.touchpadAxis.y;
            //byte b = 0x40 ;
            Buttons[8] = (byte)(Buttons[8] & 0xbf);
            button = Buttons[8];
            UpdateButtonByte(tmp, button);
			*/
        }

    void IGlobalTouchpadPressUpHandler.OnGlobalTouchpadPressUp(VREventData eventData) {
      isKeyDown = false;

			/*
            Array.Copy(BitConverter.GetBytes(eventData.touchpadAxis.x), 0, Buttons, 0, 4);
            Array.Copy(BitConverter.GetBytes(eventData.touchpadAxis.x), 0, Buttons, 4, 4);
            //byte b = 0x40 ;
            float[] tmp = new float[2];
            tmp[0] = eventData.touchpadAxis.x;
            tmp[1] = eventData.touchpadAxis.y;
            Buttons[8] = (byte)(Buttons[8] & 0xdf);
            button = Buttons[8];
            UpdateButtonByte(tmp, button);
            */
        }


    }
  }


