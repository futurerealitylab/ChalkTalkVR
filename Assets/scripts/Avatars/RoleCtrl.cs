using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleCtrl : MonoBehaviour {

    public enum Role { Presentor, Audience};
    public Role role;

    public Transform world, mirrorWorld;

    // Use this for initialization
    void Start () {
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
	
	// Update is called once per frame
	void Update () {
		
	}
}
