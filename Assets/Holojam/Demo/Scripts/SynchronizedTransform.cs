// SynchronizedTransform.cs
// Created by Holojam Inc. on 26.01.17
// Example Synchronizable

using UnityEngine;

public class SynchronizedTransform : Holojam.Tools.Synchronizable {

  [SerializeField] string label = "Example";
  [SerializeField] string scope = "ChalkTalk";

  public override string Label { get { return label; } }
  public override string Scope { get { return scope; } }

  // Proxies
  public Vector3 Position {
    get { return data.vector3s[0]; }
    set { data.vector3s[0] = value; }
  }
  public Quaternion Rotation {
    get { return data.vector4s[0]; }
    set { data.vector4s[0] = value; }
  }
  public Vector3 Scale{
    get { return data.vector3s[1]; }
    set { data.vector3s[1] = value; }
  }

  public override void ResetData() {
    //data = new Holojam.Network.Flake(2, 1);
		data = new Holojam.Network.Flake();
  }

  protected override void Sync() {
    if (Sending) {
      Position = transform.position;
      Rotation = transform.rotation;
      Scale = transform.localScale;
    } else {
			Debug.Log (data.vector3s.Length);
			Debug.Log (data.vector3s [3]);
      //transform.position = Position;
      //transform.rotation = Rotation;
      //transform.localScale = Scale;
    }
  }
}
