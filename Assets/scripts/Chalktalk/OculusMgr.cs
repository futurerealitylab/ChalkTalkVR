using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holojam.Utility;
using FRL.IO;
using System;
using UnityEngine.UI;

public class OculusMgr : MonoBehaviour {

	public enum ResolutionType
	{
		HD_1080,
		HD_720,
		RETINA_2560,
		RETINA_1440
		// etc
	}

	public enum ActiveController{
		LEFT,
		RIGHT
	}

	public ResolutionType resolutionType;

	//public static int HEIGHT { get { return global.GetResolution(global.resolutionType).height; } }
	//public static int WIDTH { get { return global.GetResolution(global.resolutionType).width; } }

	struct ResolutionFormat {
		public short width, height;
		public ResolutionFormat(short width, short height) {
			this.width = width;
			this.height = height;
		}
	}

	ResolutionFormat GetResolution(ResolutionType type) {
		switch (type) {
		case ResolutionType.HD_1080:
			return new ResolutionFormat (1920, 1080);
		case ResolutionType.HD_720:
			return new ResolutionFormat (1280, 720);
		case ResolutionType.RETINA_2560:
			return new ResolutionFormat (2560, 1600);
		case ResolutionType.RETINA_1440:
			return new ResolutionFormat (1440, 900);
		default:
			return GetResolution (ResolutionType.HD_1080);
		}
	}

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

	public ActiveController ac;
	public OvrAvatarHand[] oculusCtrls;
	private OvrAvatarHand oculusCtrl;
	/*
        //Send Bytes for ChalkTalk
	[SerializeField]
	private byte button = new byte();
	private byte[] Buttons = new byte[9];
	private byte[] Data = new byte[33];
	*/
	public bool sendMouseMove = false;

	void Awake() {

		receiver = GetComponent<Receiver>();
		if(gameObject.GetComponent<ChalkTalkController>() == null)
			ctc = gameObject.AddComponent<ChalkTalkController>();
		else
			ctc = gameObject.GetComponent<ChalkTalkController>();
		print ("add ctc");
	}

	void Start() {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 90;
		InitializeEventHandlers();
		oculusCtrl = oculusCtrls [(int)ac];

	}



	private void Update() {
		//OVRInput.Update ();

		if (cursor) {
			
			//Vector3 touchpos = OVRInput.GetControllerPositionTracked (oculusCtrl);
			//print (touchpos.ToString("F3"));
			cursor.transform.position = bindingBox.GetBoundPosition(oculusCtrl.gameObject.transform.position, BindingBox.Plane.Z, true);


			if (OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch + (int)ac) > 0.9) {
				print ("PrimaryIndexTrigger state:" + OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch + (int)ac));
				FireMouseDownEvent (cursor.transform.position);
				ctc.Data = 0;
				ctc.Pos = cursor.transform.position;
				ctc.Rot = cursor.transform.rotation.eulerAngles;
			} else if (OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch + (int)ac) < 0.1) {
				print ("PrimaryIndexTrigger state:" + OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch + (int)ac));
				FireMouseUpEvent (cursor.transform.position);
				ctc.Data = 2;
				ctc.Pos = cursor.transform.position;
				ctc.Rot = cursor.transform.rotation.eulerAngles;
			} else {
				if (sendMouseMove && Holojam.Tools.BuildManager.BUILD_INDEX == 1) {
					FireMouseMoveEvent (cursor.transform.position);
					ctc.Data = 1;
					ctc.Pos = cursor.transform.position;
					ctc.Rot = cursor.transform.rotation.eulerAngles;
				}
			}
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


}

