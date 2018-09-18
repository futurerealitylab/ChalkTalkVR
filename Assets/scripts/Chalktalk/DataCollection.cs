using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollection : MonoBehaviour {

    // collect eye contact data

    public Transform localAvatar, remoteAvatar;
    public Transform localEye, remoteEye, localEyeRoot, remoteEyeRoot;
    int layerMask = 1 << 8;
    GameObject go1;
    private void initilaize()
    {
        // find the body and assign layer, 
        localEye = localAvatar.Find("body_renderPart_2");
        if (localEye != null) {
            localEye.gameObject.layer = 8;
            //MeshCollider bc = localEye.gameObject.AddComponent<MeshCollider>();
            SkinnedMeshRenderer smr = localEye.GetComponent<SkinnedMeshRenderer>();
            Mesh m = smr.sharedMesh;
            
            go1 = new GameObject("head4raycast");
            go1.transform.position = m.vertices[0];
            //go1.transform.position = bc.transform.position+ bc.center;
            //go1.transform.rotation = bc.transform.rotation;
            //localEyeRoot = go1.transform;// localEye.Find("root");
        }


        //remoteEye = remoteAvatar.Find("body_renderPart_2");
        //if (remoteEye != null) {
        //    remoteEye.gameObject.layer = 8;
        //    BoxCollider bc = remoteEye.gameObject.AddComponent<BoxCollider>();
        //    GameObject go = new GameObject("head4raycast");
        //    go.transform.position = bc.transform.position+ bc.center;
        //    go.transform.rotation = bc.transform.rotation;
        //    remoteEyeRoot = go.transform;//remoteEye.Find("root");
        //}
    }

    // Use this for initialization
    void Start () {
        
        
    }
	
	// Update is called once per frame
	void Update () {
        layerMask = ~layerMask;
        RaycastHit hit;
        

        // Does the ray intersect any objects excluding the player layer
        if (localEye != null) {

            //BoxCollider bc = localEye.gameObject.GetComponent<BoxCollider>();
            //if(bc != null) {
            //    go1.transform.position = bc.transform.position + bc.center;
            //    go1.transform.rotation = bc.transform.rotation;
            //}
            SkinnedMeshRenderer smr = localEye.GetComponent<SkinnedMeshRenderer>();
            Mesh m = smr.sharedMesh;
            go1.transform.position = m.vertices[0];
            print("mesh 0:" + m.vertices[0]);

            if (Physics.Raycast(localEyeRoot.position, localEyeRoot.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)) {
                Debug.DrawRay(localEyeRoot.position, localEyeRoot.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("localAvatar Did Hit");
            } else {
                Debug.DrawRay(localEyeRoot.position, localEyeRoot.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log(" localAvatarDid not Hit");
            }
        } else {
            initilaize();
        }

        if (remoteEye != null) {
            if (Physics.Raycast(remoteEyeRoot.position, remoteEyeRoot.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)) {
                Debug.DrawRay(remoteEyeRoot.position, remoteEyeRoot.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
                Debug.Log("remoteAvatar Did Hit");
            } else {
                Debug.DrawRay(remoteEyeRoot.position, remoteEyeRoot.TransformDirection(Vector3.forward) * 1000, Color.red);
                Debug.Log("remoteAvatar Did not Hit");
            }
        } 
    }
}
