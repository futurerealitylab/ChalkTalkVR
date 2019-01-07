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
		RETINA_1440,
        BENQ,
        RUNTIME
		// etc
	}

	public enum ActiveController{
		LEFT,
		RIGHT
	}

    public enum DeviceType
    {
        OCULUS_RIFT,
        OCULUS_GO
    }

	public ResolutionType resolutionType;
    public short width;
    public short height;

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
            case ResolutionType.BENQ:
                return new ResolutionFormat(2560, 1440);
            case ResolutionType.RUNTIME:
                return new ResolutionFormat(width, height);
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

	//public ActiveController ac;
	public OvrAvatarHand[] oculusCtrls;
	//private OvrAvatarHand oculusCtrl;

    public DeviceType deviceType;
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
		//oculusCtrl = oculusCtrls [(int)ac];

	}

    public bool isPresenter = false;
    void ToggleChalkTalkSend()
    {
        if (!isPresenter)
            return;
        switch (deviceType)
        {
            case DeviceType.OCULUS_GO:
                if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
                    ctc.sendsetter = !ctc.sendsetter;
                break;
            case DeviceType.OCULUS_RIFT:
                if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
                    ctc.sendsetter = !ctc.sendsetter;
                break;
            default:
                break;
        }
    }

//     Vector3 CyllinderToCanvas(Vector3 pos)
//     {
//         float theta = (pos.x / renderer.mySettings.savedRectSize.x) * renderer.mySettings.Angle * Mathf.Deg2Rad;
//         pos.x = Mathf.Sin(theta) * (renderer.mySettings.SavedRadius + pos.z);
//         pos.z += Mathf.Cos(theta) * (renderer.mySettings.SavedRadius + pos.z) - (renderer.mySettings.SavedRadius + pos.z);
// 
//         return pos;
//     }

//     public Vector3 ReverseCurveTransformation(Vector3 p, Renderer renderer)
//     {
//         if (renderer.mySettings != null)
//         {
//             Vector3 positionInCanvasSpace = renderer.mySettings.transform.worldToLocalMatrix.MultiplyPoint3x4(p);
//             p = renderer.mySettings.CanvasToCurvedCanvas(positionInCanvasSpace);
//             //transform.rotation = Quaternion.LookRotation(renderer.mySettings.CanvasToCurvedCanvasNormal(transform.parent.localPosition), transform.parent.up);
//         }
//         return p;
//     }

    void MapOculusInput()
    {
        switch (deviceType)
        {
            case DeviceType.OCULUS_GO:
                {
                    OVRInput.Controller activeController = OVRInput.GetActiveController();
                    if (activeController == OVRInput.Controller.None)
                        return;

                    // reverse the curved transformation from curved panel to world coordinate system

                    // then go back to local position of binding box


                    break;
                }
            case DeviceType.OCULUS_RIFT:
                {
                    if (oculusCtrls.Length == 0)
                        break;
                    OVRInput.Controller activeController = OVRInput.GetActiveController();
                    if (activeController != OVRInput.Controller.LTouch && activeController != OVRInput.Controller.RTouch)
                        return;

                    int ctrlIdx = activeController - OVRInput.Controller.LTouch;
                    cursor.transform.position = bindingBox.GetBoundPosition(oculusCtrls[ctrlIdx].gameObject.transform.position, 
                        BindingBox.Plane.Z, true);

                    curControlPress = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);

                    if (curControlPress > 0.9 && prevControlPress <= 0.9)
                    {
                        //print ("PrimaryIndexTrigger state:" + OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch + (int)ac));
                        //FireMouseDownEvent (cursor.transform.localPosition);
                        print("Mouse Down");
                        ctc.Data = 0;

                    }
                    else if (curControlPress < 0.1 && prevControlPress >= 0.1)
                    {
                        //print ("PrimaryIndexTrigger state:" + OVRInput.Get (OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch + (int)ac));
                        //FireMouseUpEvent (cursor.transform.localPosition);
                        print("Mouse Up");
                        ctc.Data = 2;
                    }
                    else
                    {
                        if (sendMouseMove && Holojam.Tools.BuildManager.BUILD_INDEX == 1)
                        {
                            //print ("Mouse Move");
                            //FireMouseMoveEvent (cursor.transform.localPosition);
                            ctc.Data = 1;
                        }
                    }
                    break;
                }
            default:
                break;
        }
    }

    float curControlPress = 0;
    float prevControlPress = 0;
    public Vector3 pos;
    public RoleCtrl.Configuration MRConfig;
	private void Update() {
        //OVRInput.Update ();

        //print(OVRInput.Get(OVRInput.Button.PrimaryHandTrigger));
        ToggleChalkTalkSend();

        if (cursor) {
            MapOculusInput();

            pos = cursor.transform.localPosition;
            //Vector3 pos = cursor.transform.position;
            // 			pos.y = -pos.y + (float)GetResolution (resolutionType).height / (float)GetResolution (resolutionType).width * 5f/*width of the plane*/ - 1;
            // 			pos.y /= ((float)GetResolution (resolutionType).height / (float)GetResolution (resolutionType).width * 5f);
            // 			pos.z = -pos.z + 5f / 2f;
            // 			pos.z /= 5f;
            float scale = (float)GetResolution(resolutionType).width / (float)GetResolution(resolutionType).height;
            Vector3 newpos = pos;
            // because bindingbox is rotated by 90 degree in y axis, so x->-z z->x
            newpos.y = (-(pos.y-bindingBox.transform.position.y) / bindingBox.transform.localScale.y / 2 * scale + 0.5f);// / scale;
            newpos.z = -(pos.z - bindingBox.transform.position.z) /bindingBox.transform.localScale.x/2 + 0.5f;
            if (MRConfig == RoleCtrl.Configuration.eyesfree)
            {
                newpos.y = (-(pos.x - bindingBox.transform.position.x) / bindingBox.transform.localScale.x / 2 * scale + 0.5f);// / scale;
                newpos.z = -(pos.z - bindingBox.transform.position.z) / bindingBox.transform.localScale.x / 2 + 0.5f;
            }
            print(cursor.transform.localPosition.ToString("F3") + "\t" + newpos.ToString("F3"));
            ctc.Pos = newpos;
			ctc.Rot = cursor.transform.eulerAngles;
            
			prevControlPress = curControlPress;
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

