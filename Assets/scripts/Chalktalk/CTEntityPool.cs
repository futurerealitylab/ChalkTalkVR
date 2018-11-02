using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chalktalk;


public static class CTEntityPool {
  public static void Initialize() {

  }

  public static Curve getEntity(Chalktalk.ChalktalkDrawType type, Chalktalk.Renderer renderer) {
    Curve curve = GameObject.Instantiate<Curve>(renderer.curvePrefab);
    return curve;
  }
}
