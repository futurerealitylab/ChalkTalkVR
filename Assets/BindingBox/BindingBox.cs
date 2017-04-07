using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindingBox : MonoBehaviour {

  public enum Plane { X, Y, Z, NONE }

  public Transform testTransform;

  private Collider collider;

  void Awake() {
    collider = GetComponent<Collider>();
  }

  void OnDrawGizmos() {
    if (testTransform) {
      //Gizmos.color = Color.red;
      //Gizmos.DrawSphere(GetBoundPosition(testTransform.position, Plane.X, true), 0.05f);
      //Gizmos.color = Color.green;
      //Gizmos.DrawSphere(GetBoundPosition(testTransform.position, Plane.Y, true), 0.05f);
      Gizmos.color = Color.blue;
      Gizmos.DrawSphere(GetBoundPosition(testTransform.position, Plane.Z, true), 0.05f);
      //Gizmos.color = Color.white;
      //Gizmos.DrawSphere(GetBoundPosition(testTransform.position, Plane.NONE, true), 0.05f);

      Vector2 val = GetBoundValue(testTransform.position, Plane.Z);
      //Debug.Log("GetBoundValue for Zs: " + val);

      Gizmos.DrawSphere(GetPositionOnPlane(val, Plane.Z, false),0.05f);
    }
  }

  public bool Contains(Vector3 position) {
    return collider.bounds.Contains(position);
  }

  public Vector3 GetBoundPosition(Vector3 position, Plane b = Plane.X, bool clamp = false) {
    Quaternion q = Quaternion.Inverse(transform.rotation);
    position -= transform.position;
    position = q * position;

    switch (b) {
      case Plane.X:
        position.x = 0f;
        break;
      case Plane.Y:
        position.y = 0f;
        break;
      case Plane.Z:
        position.z = 0f;
        break;
      default:
        break;
    }

    if (clamp) {
      position.x = Mathf.Clamp(position.x,transform.localScale.x * -0.5f, transform.localScale.x * 0.5f);
      position.y = Mathf.Clamp(position.y, transform.localScale.y * -0.5f, transform.localScale.y * 0.5f);
      position.z = Mathf.Clamp(position.z, transform.localScale.z * -0.5f, transform.localScale.z * 0.5f);
    }

    position = transform.position + transform.rotation * position;
    return position;
  }

  public Vector2 GetBoundValue(Vector3 position, Plane b = Plane.X, bool clamp = false) {
    if (b == Plane.NONE) {
      throw new Exception("Cannot get bound value if no plane is provided.");
    }

    Quaternion q = Quaternion.Inverse(transform.rotation);
    position -= transform.position;
    position = q * position;

    Vector2 v = new Vector2();

    switch (b) {
      case Plane.X:
        v = new Vector2(position.z/transform.localScale.z, position.y/transform.localScale.y);
        break;
      case Plane.Y:
        v = new Vector2(position.x/transform.localScale.x, position.z/transform.localScale.z);
        break;
      case Plane.Z:
        v = new Vector2(position.x / transform.localScale.x, position.y / transform.localScale.y);
        break;
    }

    v *= 2f;

    if (clamp) {
      v.x = Mathf.Clamp(v.x, -1f, 1f);
      v.y = Mathf.Clamp(v.y, -1f, 1f);
    }
    return v;
  }

  public Vector3 GetPositionOnPlane(Vector2 value, Plane b = Plane.X, bool clamp = false) {
    if (b == Plane.NONE) {
      throw new Exception("Cannot get position on plane if no plane is provided.");
    }

    Vector3 v = new Vector3();

    switch (b) {
      case Plane.X:
        v = new Vector3(0f, value.x * transform.localScale.z, value.y * transform.localScale.y);
        break;
      case Plane.Y:
        v = new Vector3(value.x * transform.localScale.x, 0f, value.y * transform.localScale.z);
        break;
      case Plane.Z:
        v = new Vector3(value.x * transform.localScale.x, value.y * transform.localScale.y, 0f);
        break;
    }
    v /= 2f;

    if (clamp) {
      v.x = Mathf.Clamp(v.x, transform.localScale.x * -0.5f, transform.localScale.x * 0.5f);
      v.y = Mathf.Clamp(v.y, transform.localScale.y * -0.5f, transform.localScale.y * 0.5f);
      v.z = Mathf.Clamp(v.z, transform.localScale.z * -0.5f, transform.localScale.z * 0.5f);
    }

    return transform.position + transform.rotation * v;
  }
}
