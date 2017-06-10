using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.IO;
using System;

public class ChalkTalkStylus : MonoBehaviour, IGlobalApplicationMenuPressDownHandler , IGlobalTriggerPressUpHandler   {


  //public void OnGlobalTriggerPress(BaseEventData eventData) {
  //  if (onPlane)
  //    CTSender.sendMouseMove(1, hitPoint);
  //}
  public void OnGlobalApplicationMenuPressDown(VREventData eventData) {
    print("click");
    //CTSender.sendMouseDown(1, hitPoint);

    //CTSender.sendMouseUp(1, hitPoint);
    if (onPlane)
      StartCoroutine(Click(hitPoint));
  }

  bool isClick = false;

  IEnumerator Click(Vector3 h) {
    Vector3 A = new Vector3();
    A = h;
    isClick = true;
    CTSender.sendMouseDown(1, A);
    yield return new WaitForSeconds(0.3f);
    CTSender.sendMouseUp(1, A);
    isClick = false;
  }

  //public void OnGlobalTriggerClick(BaseEventData eventData) {
  //  print("click " + isFired);
  //  if (onPlane) {
  //    if (isFired) {
  //      print("Up");
  //      CTSender.sendMouseUp(1, hitPoint);
  //    } else {
  //      print("Down");
  //      CTSender.sendMouseDown(1, hitPoint);
  //    }
  //    isFired = !isFired;
  //  }   
  //    }

  Vector2 pointScale = new Vector2(1f/5f ,1f/5f);
  public Transform plane;

  public void OnGlobalTriggerPressUp(VREventData eventData) {
    // print(hitPoint);
    //Vector2 v = new Vector2(1440, 900);
    // print("hit!!!! " + hitPoint);
    //if (onPlane)
    //CTSender.sendMouseDown(1, hitPoint);
    // CTSender.sendMouseDown(1, v);
    if (onPlane) {
      if (isFired) {
        print("Up");
        CTSender.sendMouseUp(1, hitPoint);
      } else {
        print("Down");
        CTSender.sendMouseDown(1, hitPoint);
      }
      isFired = !isFired;
    }
  }



  public void RegisterButton(int i) {
    index = i;
    print(index);
  }
  private Vector2 pointonscreen(Vector3 p) {
    p -= plane.position;
    p.Scale(pointScale);    
    Vector2 rst = new Vector2((p.x + 1) / 4 * 2880, (0.6f - p.y) / 2.4f * 1800);
    
    //Vector2 rst = new Vector2(((p.x / 5f) + 1f) / 4f * 2560, (-p.y / 5f + 1f) / 4f * 1600);
    //Vector2 rst = new Vector2(((p.x / 5f)) , (-p.y / 5f));
   // Debug.Log ("rst : "+rst);
    return rst;
  }

  public Transform Dot;
  public ChalkTalkSender CTSender;
  private RaycastHit hit;
  private Ray ray;
  private LineRenderer line;
  [SerializeField] private bool onPlane;
  [SerializeField] private Vector2 hitPoint;
  private int index;
  private bool isFired;
  // Use this for initialization
  void Start() {
    line =GetComponent<LineRenderer>();
    isFired = false;
  }

  // Update is called once per frame
  void Update() {

    if (Physics.Raycast(transform.position, transform.forward, out hit)) {
      //print(hit.transform.gameObject.name);
      if (hit.transform.gameObject.name == "Plane") {
        hitPoint = pointonscreen(hit.point);
        //        print(hit.point);
        if (!isClick)
        CTSender.sendMouseMove(1, hitPoint);
        onPlane = true;
        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hit.point);
        Dot.gameObject.SetActive(true);
        Dot.position = hit.point;
      } else
        onPlane = false;
    } else {
      onPlane = false;
      line.enabled = false;
      Dot.gameObject.SetActive(false);
      hitPoint = new Vector2(-50f, -50f);
    }
  }


}
