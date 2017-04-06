﻿// Tracker.cs
// Created by Holojam Inc. on 11.11.16

using UnityEngine;

namespace Holojam.Martian {

  /// <summary>
  /// Unity representation of a tracking camera, used for placing a Martian Actor in the space.
  /// Receives (extra) data from Holoscope.
  /// </summary>
  public sealed class Tracker : Network.Controller {

    /// <summary>
    /// Position of the camera in the scene.
    /// </summary>
    public Vector2 origin;

    /// <summary>
    /// Height of the camera in meters.
    /// </summary>
    public float height = 1;

    /// <summary>
    /// Up/down angle of the camera in degrees.
    /// </summary>
    [Range(-90, 90)] public float angle = 0; // Degrees

    protected override ProcessDelegate Process { get { return UpdateFrustum; } }

    /// <summary>
    /// Holoscope raw input float.
    /// </summary>
    public float Stem { get { return data.floats[0]; } }

    /// <summary>"ExtraData"</summary>
    public override string Label {get { return "ExtraData"; } }
    /// <summary>"Holoscope"</summary>
    public override string Scope { get { return "Holoscope"; } }

    /// <summary>
    /// Input-only.
    /// </summary>
    public override bool Sending { get { return false; } }

    /// <summary>
    /// Updates position and rotation in scene.
    /// </summary>
    void UpdateFrustum() {
      transform.position = new Vector3(origin.x, height, origin.y);
      transform.rotation = Quaternion.AngleAxis(-angle, Vector3.right);
    }

    /// <summary>
    /// Localize an input position within this reference frame.
    /// </summary>
    public Vector3 Localize(Vector3 position) {
      return transform.TransformPoint(position);
    }

    /// <summary>
    /// Localize an input rotation within this reference frame.
    /// </summary>
    public Quaternion Localize(Quaternion rotation) {
      return Quaternion.Inverse(transform.rotation) * rotation;
    }
  }
}
