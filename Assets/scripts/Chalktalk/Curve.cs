#define DEBUG_PRINT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chalktalk
{
    public enum ChalktalkDrawType { STROKE, FILL };

    [RequireComponent(typeof(LineRenderer))]
    public class Curve : MonoBehaviour
    {

        private LineRenderer line;

        public List<Vector3> points = new List<Vector3>();
        public Color color = Color.white;
        public float width = 0f;
        public int id = 0;
        public ChalktalkDrawType type;
        public GameObject go;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
        }

        public void Draw()
        {
            switch (type) {
                case ChalktalkDrawType.STROKE:
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
                    MeshFilter filter = null;
                    if (shape == null) {
                        shape = new Mesh();
                        go.AddComponent<MeshRenderer>();
                        filter = go.AddComponent<MeshFilter>();
                    } else {
                        filter = go.GetComponent<MeshFilter>();
                    }
                    shape.vertices = points.ToArray();

                    int countSides = points.Count;
                    int countTris = countSides - 2;
                    int[] indices = new int[countTris * 3];
                    for (int i = 2; i < countSides; ++i) {
                        indices[i - 2] = 0;
                        indices[i - 1] = i - 1;
                        indices[i] = i;
                    }
                    shape.triangles = indices;
                    

                    filter.mesh = shape;

                    break;
                default:
                    goto case ChalktalkDrawType.STROKE;
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
