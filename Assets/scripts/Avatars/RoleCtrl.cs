using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Avatar;
using Oculus.Platform;
using Oculus.Platform.Models;
using System.Collections;


public class RoleCtrl : MonoBehaviour {

    public enum Configuration { sidebyside, mirror, eyesfree};
    public Configuration MRConfig;

    public enum Role { Presentor, Audience};
    public Role role;

    Transform world, mirrorWorld;
    public Transform Chalktalkboard, mycmr, localAvatar;
    [SerializeField]
    private Transform [] remoteAvatars;
    public string localLabel;
    public string [] remoteLabels;

    public GameObject remoteAvatarPrefab;

    public GameObject emptyHeadPrefab;

    public GameObject dataCollection;

    public OvrAvatar myAvatar;

    void Awake()
    {
        Oculus.Platform.Core.Initialize();
        Oculus.Platform.Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
        Oculus.Platform.Request.RunCallbacks();  //avoids race condition with OvrAvatar.cs Start().
    }

    private void GetLoggedInUserCallback(Message<User> message)
    {
        if (!message.IsError)
        {
            myAvatar.oculusUserID = message.Data.ID.ToString();
        }
    }

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
        switch (MRConfig)
        {
            case Configuration.mirror:
                mirrorRoleCtrl();
                break;
            case Configuration.sidebyside:
                break;
            case Configuration.eyesfree:
                break;
            default:
                break;

        }
        
    }

    void mirrorRoleCtrl()
    {
        switch (role)
        {
            case Role.Audience:

                //    localAvatar.localRotation = Quaternion.Euler(0, 180, 0);
                //    //localAvatar.localScale = Vector3.back;
                //    mycmr.localRotation = Quaternion.Euler(0, 180, 0);
                //    //mycmr.localScale = Vector3.back;
                //    //Chalktalkboard.localRotation = Quaternion.Euler(0, 180, 0);
                //    //Chalktalkboard.localScale = Vector3.back;

                foreach (Transform remoteAvatar in remoteAvatars)
                    //remoteAvatar.localRotation = Quaternion.Euler(0, 180, 0);
                    remoteAvatar.localScale = new Vector3(-1, 1, 1);

                // if i am the audience, sending my ovrcamera
                GameObject go = Instantiate(emptyHeadPrefab);
                go.GetComponent<HeadFlake>().isPresenter = false;
                go.GetComponent<HeadFlake>().label = localLabel + "head";
                //

                // zhenyi
                //foreach (Transform remoteAvatar in remoteAvatars) {
                //    GameObject remPar = new GameObject();
                //    remoteAvatar.transform.parent = remPar.transform;
                //    remPar.transform.localScale = new Vector3(-1, 1, 1);
                //}

                dataCollection.SetActive(false);
                break;
            case Role.Presentor:
                foreach (Transform remoteAvatar in remoteAvatars)
                    //remoteAvatar.localRotation = Quaternion.Euler(0, 180, 0);
                    remoteAvatar.localScale = new Vector3(-1, 1, 1);

                // if i am the presenter, receiving from audience about ovrcamera
                for (int i = 0; i < remoteLabels.Length; i++)
                {
                    GameObject go2 = Instantiate(emptyHeadPrefab);
                    go2.GetComponent<HeadFlake>().isPresenter = true;
                    go2.GetComponent<HeadFlake>().label = remoteLabels[i] + "head";
                    go2.transform.localScale = new Vector3(-1, 1, 1);
                }

                dataCollection.SetActive(true);
                break;
            default:
                break;
        }
    }

    void sidebysideRoleCtrl()
    {
        switch (role)
        {
            case Role.Audience:
                // if i am the audience, sending my ovrcamera
                GameObject go = Instantiate(emptyHeadPrefab);
                go.GetComponent<HeadFlake>().isPresenter = false;
                go.GetComponent<HeadFlake>().label = localLabel + "head";
                //
                dataCollection.SetActive(false);
                break;
            case Role.Presentor:
                // if i am the presenter, receiving from audience about ovrcamera
                for (int i = 0; i < remoteLabels.Length; i++)
                {
                    GameObject go2 = Instantiate(emptyHeadPrefab);
                    go2.GetComponent<HeadFlake>().isPresenter = true;
                    go2.GetComponent<HeadFlake>().label = remoteLabels[i] + "head";
                    go2.transform.localScale = new Vector3(-1, 1, 1);
                }

                dataCollection.SetActive(true);
                break;
            default:
                break;
        }
    }

    Dictionary<string, string> mapLabelUserID;
    // Use this for initialization
    void Start () {
        mapLabelUserID = new Dictionary<string, string>();
        mapLabelUserID.Add("zhenyi", "1682533711857130");
        mapLabelUserID.Add("A1", "2182355055148104");
        mapLabelUserID.Add("A2", "2347129931970208");
        localAvatar.GetComponent<AvatarManager>().label = localLabel;
        remoteAvatars = new Transform[remoteLabels.Length];
        for (int i = 0; i < remoteLabels.Length; i++) {
            GameObject go = Instantiate(remoteAvatarPrefab, localAvatar.parent);
            AvatarManager am = go.GetComponent<AvatarManager>();
            am.label = remoteLabels[i];
            remoteAvatars[i] = go.transform;
            go.name = am.label;
            go.GetComponent<OvrAvatar>().oculusUserID = mapLabelUserID[go.name];
            //
        }
        tryint();
        Chalktalkboard.parent.GetComponent<Chalktalk.Renderer>().facingDirection = 90;// (role == Role.Audience) ? 270 : 90;
        Chalktalkboard.parent.GetComponent<OculusMgr>().isPresenter = role == Role.Presentor;
        print("Chalktalkboard.parent.GetComponent<Chalktalk.Renderer>().facingDirection:" + Chalktalkboard.parent.GetComponent<Chalktalk.Renderer>().facingDirection);
    }

    Transform bodyRenderPart2, localAvatarBody;
	// Update is called once per frame
	void Update () {
        // set layer of render_body_3 to triangle so that fps cannot see it.
        if(bodyRenderPart2 == null)
        {
            localAvatarBody = localAvatar.Find("body");
            bodyRenderPart2 = localAvatarBody.Find("body_renderPart_2");
        }
        if (bodyRenderPart2 != null)
        {
            if (!bodyRenderPart2.parent.parent.name.Equals("LocalAvatar"))
                bodyRenderPart2 = null;
        }
        if (bodyRenderPart2 != null)
        {
            bodyRenderPart2.gameObject.layer = LayerMask.NameToLayer("first");
            localAvatarBody.Find("body_renderPart_1").gameObject.layer = LayerMask.NameToLayer("first");
            localAvatarBody.Find("body_renderPart_0").gameObject.layer = LayerMask.NameToLayer("first");
        }
            
    }
}
