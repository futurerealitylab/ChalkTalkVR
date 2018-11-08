using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeTriangle : MonoBehaviour {

    public Color c = Color.blue;

    public Material mat;

    GameObject view;

    BoxCollider plane;

    Mesh tri;
    MeshFilter filter;
    MeshRenderer rend;

    void Start () {
        view = new GameObject("TriangularGaze");
        view.layer = LayerMask.NameToLayer("triangle");

        this.plane = GameObject.Find("BindingBox").GetComponent<BoxCollider>();

        this.tri = new Mesh();
        this.filter = this.view.AddComponent<MeshFilter>();
        this.rend = this.view.AddComponent<MeshRenderer>();

        Material mymat = new Material(mat);
        mymat.SetColor("_EmissionColor", c);
        mymat.color = c;
        rend.material = mymat;

        this.tri.vertices = new Vector3[]{Vector3.zero, Vector3.zero, Vector3.zero};
        this.tri.triangles = new int[]{0, 1, 2};
        this.tri.RecalculateBounds();
        this.tri.RecalculateNormals();

        this.filter.mesh = this.tri;
    }

    void Update () {
        Vector3 center = this.transform.position;

        // angle of separation between forward gaze
        float angle = 15.0f;
        
        // raycast angle towards side a and side b 
        Ray a = new Ray(center, Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward * 20.0f);
        Ray b = new Ray(center, Quaternion.AngleAxis(-angle, Vector3.up) * this.transform.forward * 20.0f);

        RaycastHit hitA, hitB;
        if (this.plane.Raycast(a, out hitA, Mathf.Infinity) && 
            this.plane.Raycast(b, out hitB, Mathf.Infinity))
        {
            this.rend.enabled = true;

            float da = Vector3.Distance(center, hitA.point);
            float db = Vector3.Distance(center, hitB.point);
            if (da > db)
            {
                Vector3 dir = b.direction;
                Vector3 pt = this.transform.position + b.direction * da;

                this.filter.mesh.vertices = new Vector3[] {
                    this.gameObject.transform.position,
                    pt,
                    hitA.point
                };
            } else
            {
                Vector3 dir = a.direction;
                Vector3 pt = this.transform.position + a.direction * db;

                this.filter.mesh.vertices = new Vector3[] {
                    this.gameObject.transform.position,
                    hitB.point,
                    pt
                };
            }
            this.filter.mesh.RecalculateBounds();
            this.filter.mesh.RecalculateNormals();

        } else {
            this.rend.enabled = false;
        }

        //Debug.DrawLine(this.filter.mesh.vertices[0], this.filter.mesh.vertices[1], Color.green);
        //Debug.DrawLine(this.filter.mesh.vertices[0], this.filter.mesh.vertices[2], Color.green);
    }
}
