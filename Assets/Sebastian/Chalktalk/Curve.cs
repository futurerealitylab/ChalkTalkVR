using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chalktalk {
  [RequireComponent(typeof(LineRenderer))]
  public class Curve : MonoBehaviour {

    private LineRenderer line;

    public List<Vector3> points = new List<Vector3>();
    public Color color = Color.white;
    public float width = 0f;
    public int id = 0;

    private void Awake() {
      line = GetComponent<LineRenderer>();
    }

    public void Draw() {
      line.numPositions = points.Count;
      line.SetPositions(points.ToArray());
      line.startColor = color;
      line.endColor = color;
      line.startWidth = width;
      line.endWidth = width;
    }

    public override string ToString() {
      string s = "ID: " + id + " COLOR: " + color + " WIDTH: " + width + " POINTS: ";
      for (int i = 0; i < points.Count; i++) {
        s += " { x : " + points[i].x + " y: " + points[i].y + " z: " + points[i].z + " } ";
      }
      return s;
    }
  }
}
