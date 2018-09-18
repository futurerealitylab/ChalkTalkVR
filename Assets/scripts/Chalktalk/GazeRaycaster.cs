using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeRaycaster : MonoBehaviour {

    //public GameObject source;

    public LayerMask[] targetLayers;
    //private LayerMask mask;

    GameObject go;
    private void Awake()
    {
        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.GetComponent<Renderer>().material.color = Color.red;
        go.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
    }

    void Start()
    {
    }


    void Update()
    {
        for (uint i = 0; i < this.targetLayers.Length; i += 1)
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, Mathf.Infinity, this.targetLayers[i]))
            {
                Debug.DrawLine(this.transform.position, hit.point, Color.blue);

                go.transform.position = hit.point;
            }
        }
    }
}
