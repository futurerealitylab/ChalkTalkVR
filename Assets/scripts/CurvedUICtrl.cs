using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurvedUICtrl : MonoBehaviour {

    CurvedUI.CurvedUISettings setting;

    public Text t;

    // Use this for initialization
    void Start () {
        setting = GetComponent<CurvedUI.CurvedUISettings>();

    }
	
	// Update is called once per frame
	void Update () {
        //         if (OVRInput.GetDown(OVRInput.Button.DpadDown))
        //         {
        //             setting.angle += 5;
        //         }
        //        if (OVRInput.GetDown(OVRInput.Button.DpadUp))
        //         {
        //             setting.angle -= 5;
        //         }
        if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
        {
            Vector2 pos = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
            if(pos.y < -0.5f)
                setting.angle -= 5;
            if(pos.y > 0.5f)
                setting.angle += 5;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            setting.angle -= 5;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            setting.angle += 5;
        }
        t.text = setting.angle.ToString();
    }
}
