﻿//#define DEBUG_PRINT
//#define BEFORE_POOL



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chalktalk {
    public enum ChalktalkDrawType { STROKE, FILL, TEXT };



    //[RequireComponent(typeof(LineRenderer))]
    public class Curve : MonoBehaviour {

        public List<Vector3> points;
        public Color color = Color.white;
        public float width = 0f;
        public int id = 0;

        public Material defaultMat;
        public LineRenderer line;

        public Material myMat;
        public Mesh shape;
        public MeshFilter meshFilter;
        public MeshRenderer mr;




        public ChalktalkDrawType type;

        public void Init() {

        }
        public void Start() {
        }


        public void InitWithLines(List<Vector3> points, Color color, float width) {
            this.type = ChalktalkDrawType.STROKE;
            this.line.positionCount = points.Count;
            this.line.SetPositions(points.ToArray());
            this.line.startColor = color;
            this.line.endColor = color;
            this.line.material.color = color;
            this.line.startWidth = width;
            this.line.endWidth = width;
        }

        public void InitWithFill(List<Vector3> points, Color color) {
            this.type = ChalktalkDrawType.FILL;
            Mesh shape = this.shape;
            shape.vertices = points.ToArray();

            MeshRenderer mr = this.mr;
            MeshFilter filter = this.meshFilter;



            int countSides = points.Count;
            int countTris = countSides - 2;
            int[] indices = new int[countTris * 3 * 2];
            for (int i = 0, off = 0; i < countTris; ++i, off += 6) {
                indices[off] = 0;
                indices[off + 1] = i + 1;
                indices[off + 2] = i + 2;
                indices[off + 3] = 0;
                indices[off + 4] = i + 2;
                indices[off + 5] = i + 1;
            }
            shape.triangles = indices;
            Material mymat = new Material(defaultMat);
            // similar to what chalktalk do to the color TODO check if shader material is the same as what already exists (in which case, don't modify)
            Color c = new Color(Mathf.Pow(color.r, 0.45f), Mathf.Pow(color.g, 0.45f), Mathf.Pow(color.b, 0.45f));

            mymat.SetColor("_EmissionColor", c);
            mr.material = mymat;

            shape.RecalculateBounds();
            shape.RecalculateNormals();

            filter.mesh = shape;
        }



        public void Draw() {
            switch (type) {
                case ChalktalkDrawType.STROKE:
                    line = this.gameObject.AddComponent<LineRenderer>();
                    line.positionCount = points.Count;
                    line.SetPositions(points.ToArray());
                    line.startColor = color;
                    line.endColor = color;
                    line.material.color = color;
                    line.startWidth = width;
                    line.endWidth = width;

                    break;
                case ChalktalkDrawType.FILL:
                    GameObject go = this.gameObject;
                    Mesh shape = go.GetComponent<Mesh>();
                    MeshRenderer mr = null;
                    MeshFilter filter = null;
                    if (shape == null) {
                        shape = new Mesh();
                        mr = go.AddComponent<MeshRenderer>();
                        filter = go.AddComponent<MeshFilter>();
                    } else {
                        filter = go.GetComponent<MeshFilter>();
                    }
                    shape.vertices = points.ToArray();

                    int countSides = points.Count;
                    int countTris = countSides - 2;
                    int[] indices = new int[countTris * 3 * 2];
                    for (int i = 0, off = 0; i < countTris; ++i, off += 6) {
                        indices[off] = 0;
                        indices[off + 1] = i + 1;
                        indices[off + 2] = i + 2;
                        indices[off + 3] = 0;
                        indices[off + 4] = i + 2;
                        indices[off + 5] = i + 1;
                    }
                    shape.triangles = indices;
                    Material mymat = new Material(defaultMat);
                    // similar to what chalktalk do to the color
                    Color c = new Color(Mathf.Pow(color.r, 0.45f), Mathf.Pow(color.g, 0.45f), Mathf.Pow(color.b, 0.45f));
                    mymat.SetColor("_EmissionColor", c);
                    mr.material = mymat;

                    shape.RecalculateBounds();
                    shape.RecalculateNormals();

                    filter.mesh = shape;

                    break;
                default:
                    //goto case ChalktalkDrawType.STROKE;
                    break;
            }


#if DEBUG_PRINT
            Debug.Log(this);
#endif
        }

        public override string ToString() {
            string s = "ID: " + id + " COLOR: " + color + " WIDTH: " + width + " POINTS: ";
            for (int i = 0; i < points.Count; i++) {
                s += " { x : " + points[i].x + " y: " + points[i].y + " z: " + points[i].z + " } ";
            }
            s += " TYPE: " + this.type;
            return s;
        }
    }
}
