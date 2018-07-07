using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chalktalk
{
    public enum ChalktalkDrawType { STROKE, FILL };



    //[RequireComponent(typeof(LineRenderer))]
    public class Curve : MonoBehaviour
    {
        public Material defaultMat;
        private LineRenderer line;

        public List<Vector3> points = new List<Vector3>();
        public Color color = Color.white;
        public float width = 0f;
        public int id = 0;
        public ChalktalkDrawType type;

        public void Draw()
        {
            switch (type)
            {
                case ChalktalkDrawType.STROKE:
                    line = this.gameObject.AddComponent<LineRenderer>();
                    line.positionCount = points.Count;
                    line.SetPositions(points.ToArray());
                    Color c = new Color(Mathf.Pow(color.r, 0.45f), Mathf.Pow(color.g, 0.45f), Mathf.Pow(color.b, 0.45f));
                    line.startColor = c;
                    line.endColor = c;
                    line.material = defaultMat;
                    line.material.color = c;
                    line.material.SetColor("_EmissionColor", c);
                    line.startWidth = width;
                    line.endWidth = width;

                    break;
                case ChalktalkDrawType.FILL:
                    GameObject go = this.gameObject;
                    Mesh shape = go.GetComponent<Mesh>();
                    MeshRenderer mr = null;
                    MeshFilter filter = null;
                    if (shape == null)
                    {
                        shape = new Mesh();
                        mr = go.AddComponent<MeshRenderer>();
                        filter = go.AddComponent<MeshFilter>();
                    }
                    else
                    {
                        filter = go.GetComponent<MeshFilter>();
                    }
                    shape.vertices = points.ToArray();

                    int countSides = points.Count;
                    int countTris = countSides - 2;
                    int[] indices = new int[countTris * 3 * 2];
                    for (int i = 0, off = 0; i < countTris; ++i, off += 6)
                    {
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
                    c = new Color(Mathf.Pow(color.r, 0.45f), Mathf.Pow(color.g, 0.45f), Mathf.Pow(color.b, 0.45f));
                    mymat.SetColor("_EmissionColor", c);
                    mr.material = mymat;
                    //mr.material.color = color;

                    //Vector3[] V = {
                    //    new Vector3(0f, 0f, 0f),
                    //    new Vector3(1f, 0f, 0f),
                    //    new Vector3(1f, 1f, 0f),
                    //    new Vector3(0f, 1f, 0f)
                    //};

                    //int[] I = {
                    //    0, 1, 2,
                    //    2, 3, 0
                    //};

                    //shape.vertices = V;
                    //shape.triangles = I;
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

        public override string ToString()
        {
            string s = "ID: " + id + " COLOR: " + color + " WIDTH: " + width + " POINTS: ";
            for (int i = 0; i < points.Count; i++)
            {
                s += " { x : " + points[i].x + " y: " + points[i].y + " z: " + points[i].z + " } ";
            }
            s += " TYPE: " + this.type;
            return s;
        }
    }
}