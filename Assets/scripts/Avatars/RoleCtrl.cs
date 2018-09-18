﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleCtrl : MonoBehaviour {

    public enum Role { Presentor, Audience};
    public Role role;

    Transform world, mirrorWorld;
    public Transform Chalktalkboard, mycmr, localAvatar;
    public Transform [] remoteAvatars;
    public string localLabel;
    public string [] remoteLabels;

    void unused()
    {
        switch (role) {
            case Role.Presentor:
                // presentor will see himself mirrored, so only remoteAvatar in world
                world.Find("RemoteAvatar").gameObject.SetActive(true);
                world.Find("LocalAvatar").gameObject.SetActive(false);
                world.Find("Chalktalk").gameObject.SetActive(false);
                world.Find("OVRCameraRig").gameObject.SetActive(false);

                mirrorWorld.Find("RemoteAvatar").gameObject.SetActive(false);
                mirrorWorld.Find("LocalAvatar").gameObject.SetActive(true);
                mirrorWorld.Find("Chalktalk").gameObject.SetActive(true);
                mirrorWorld.Find("OVRCameraRig").gameObject.SetActive(true);
                break;
            case Role.Audience:
                // audience will see presenter mirrored
                world.Find("RemoteAvatar").gameObject.SetActive(false);
                world.Find("LocalAvatar").gameObject.SetActive(true);
                world.Find("Chalktalk").gameObject.SetActive(true);
                world.Find("OVRCameraRig").gameObject.SetActive(true);

                mirrorWorld.Find("RemoteAvatar").gameObject.SetActive(true);
                mirrorWorld.Find("LocalAvatar").gameObject.SetActive(false);
                mirrorWorld.Find("Chalktalk").gameObject.SetActive(false);
                mirrorWorld.Find("OVRCameraRig").gameObject.SetActive(false);
                //LocalAvatar
                break;
            default:
                break;
        }
    }

    void tryint()
    {
        switch (role) {
            case Role.Audience:
                localAvatar.localRotation = Quaternion.Euler(0, 180, 0);
                //localAvatar.localScale = Vector3.back;
                mycmr.localRotation = Quaternion.Euler(0, 180, 0);
                //mycmr.localScale = Vector3.back;
                //Chalktalkboard.localRotation = Quaternion.Euler(0, 180, 0);
                //Chalktalkboard.localScale = Vector3.back;
                break;
            case Role.Presentor:
                foreach(Transform remoteAvatar in remoteAvatars)
                    remoteAvatar.localRotation = Quaternion.Euler(0, 180, 0);
                //remoteAvatar.localScale = Vector3.back;

                break;
            default:
                break;
        }
    }

    // Use this for initialization
    void Start () {
        tryint();
        localAvatar.GetComponent<AvatarManager>().label = localLabel;
        for(int i = 0; i < remoteLabels.Length; i++)
            remoteAvatars[i].GetComponent<AvatarManager>().label = remoteLabels[i];
        Chalktalkboard.parent.GetComponent<Chalktalk.Renderer>().facingDirection = (role == Role.Audience) ? 270 : 90;
        print("Chalktalkboard.parent.GetComponent<Chalktalk.Renderer>().facingDirection:" + Chalktalkboard.parent.GetComponent<Chalktalk.Renderer>().facingDirection);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
