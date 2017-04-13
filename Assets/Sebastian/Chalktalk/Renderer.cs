using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chalktalk {
  public class Renderer : Holojam.Tools.Trackable {

    [SerializeField]
    private Curve curvePrefab;
    [SerializeField]
    private BindingBox bindingBox;

    [SerializeField]
    private string label = "Display";

    private List<Curve> curves = new List<Curve>();

    public override string Label {
      get { return label; }
    }

    public override string Scope {
      get { return ""; }// "Chalktalk"; }
    }

    protected override void UpdateTracking() {
      if (this.Tracked) {
        DestroyCurves();
        Parse(data.bytes);
        Draw();
      }
    }

    private void DestroyCurves() {
      foreach (Curve curve in curves) {
        DestroyImmediate(curve.gameObject);
      }
      curves.Clear();
    }

    private void Parse(byte[] bytes) {
      int cursor = 8;
      int curveCount = Utility.ParsetoInt16(bytes, cursor);
      cursor += 2;

      for (int i = 0; i < curveCount; i++) {
        int length = Utility.ParsetoInt16(bytes, cursor);
        cursor += 2;

        Color color = Utility.ParsetoColor(bytes, cursor);
        cursor += 4;

        float width = Utility.ParsetoFloat(Utility.ParsetoInt16(bytes, cursor));
        cursor += 2;

        List<Vector3> points = new List<Vector3>();
        for (int j = 0; j < length; j++) {
          Vector3 point = Utility.ParsetoVector3(bytes, cursor, 1);
          //point.Scale(bindingBox.transform.localScale);
          points.Add(bindingBox.transform.rotation * point + bindingBox.transform.position);
          cursor += 6;
        }

        Curve curve = GameObject.Instantiate<Curve>(curvePrefab);
        curve.transform.SetParent(this.transform);

        curve.points = points;
        curve.width = width;
        curve.color = color;
        curves.Add(curve);
      }
    }


    private void Draw() {
      foreach (Curve curve in curves) {
        curve.Draw();
      }
    }
  }
}








