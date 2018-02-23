//// ChalkTalk Renderer in Unity
//// Created by Wenbo Lan on 21.02.17
//using UnityEngine;
//using System;
//using System.Collections.Generic;

//internal class Convert {


//}


////internal class Curve  {
////	private Color _color;
////	private float _width;
////	private Vector3[] _points;
////	private int _id;

////	//Accessors
////	public Color color {
////		get { return _color; }
////		set { _color = value; }
////	}

////	public float width {
////		get { return _width; }
////		set{ _width = value; }
////	}

////	public Vector3[] points{
////		get { return _points;}
////		set { _points = value;}
////	}

////	public Curve(){
////		_color = new Color();
////		_points = new Vector3[0];
////		_width = 0f;
////		_id = 0;
////	}

////	public int Size{get {return _points.Length;}}

////	public Curve(List<Vector3> p){
////		_color = new Color();
////		_width = 0f;
////		_id = 0;
////		Vector3[] tmp = new Vector3[p.Count];
////		int i = 0;
////		foreach (Vector3 v in p) {
////			tmp [i++] = v;
////		}
////		_points = tmp;
////	}

////	public Curve(List<Vector3> p, float w, Color c)
////	{
////		_id = 0;
////		Vector3[] tmp = new Vector3[p.Count];
////		int i = 0;
////		foreach (Vector3 v in p) {
////			tmp [i++] = v;
////		}
////		_points = tmp;
////		_width = w;
////		_color = c;
////	}
  
////	public string ToString{
////		get{
////			string s = "";
////			for (int i = 0; i < _points.Length; i++)
////				s += "{x " + _points [i].x + ", y " + _points [i].y + ", z " + _points [i].z + "}";
////			return s;
////		}
////	}

////	public static void DrawCurve (GameObject p, GameObject m, Curve Cu){
////		GameObject g = (GameObject)MonoBehaviour.Instantiate(p);
////		g.transform.parent = m.transform;
////		g.GetComponent<LineRenderer> ().numPositions = Cu.Size;
////		for (int i = 0; i < Cu.Size; i++)
////			g.GetComponent<LineRenderer> ().SetPosition (i, Cu.points [i]);
////		g.GetComponent<LineRenderer> ().startColor  = Cu.color;
////		g.GetComponent<LineRenderer> ().endColor  = Cu.color;
////		g.GetComponent<LineRenderer> ().startWidth = Cu.width;
////		g.GetComponent<LineRenderer> ().endWidth = Cu.width;
////	}

////	public static void DrawCurves(GameObject p, GameObject m, List<Curve> Cs)
////	{
////		foreach (Curve c in Cs)
////			Curve.DrawCurve (p, m, c);
////	}
////}

//public class CTRenderer : Holojam.Tools.Synchronizable
//{

//	[SerializeField] string label = "Display";
//	[SerializeField] string scope = "ChalkTalk";

//  Vector3 lineTrans = new Vector3(0,0,0);
//  Vector3 lineScale = new Vector3(5, 5, 5);
//  public Transform plane;

//  public GameObject prefab;
//	public GameObject empty;
//	public byte[] hehe;

//	private int cursor;
//	private GameObject master;
//  [SerializeField] private List<Curve> curves;
  
//  public List<Vector3> testPoints;
//  public override string Label { get { return label; } }

//	public override string Scope { get { return scope; } }
//	// Use this for initialization
//	public override void ResetData ()
//	{
//		//data = new Holojam.Network.Flake(2, 1);
//		data = new Holojam.Network.Flake ();
//		curves = new List<Curve>();
//    lineTrans = plane.position;

//  }

//	void parseCurve (int index, int size)
//	{
//		Color co = new Color (data.vector4s [index].x / 256f,
//			           data.vector4s [index].y / 256f,
//			           data.vector4s [index].z / 256f,
//			           data.vector4s [index].w / 256f);
//		//Debug.Log (data.vector4s[index]);
//		GameObject g = (GameObject)Instantiate (prefab);
//		g.transform.parent = master.transform;
//		LineRenderer line = g.GetComponent<LineRenderer> ();
//		Vector3[] points = new Vector3[size];
//		//Debug.Log (points.Length);
//		for (int j = 0; j < size; j++) {
//			if (j + index + 1 > data.vector4s.Length)
//				Debug.Log ("index" + index + "j " +j+"size"+size);
//			else {
//				points [j] = new Vector3 (data.vector4s [j + index + 1].x,
//					data.vector4s [j + index + 1].y,
//					0.1f);
//				points [j] *= 5;
//			}

//		}
//		line.numPositions = points.Length;
//		int i = 0;
//		foreach (Vector3 v in points) {
//			line.SetPosition (i++, v);
//		}
//		//line.startColor = co;
//		//line.startWidth = data.vector4s [index + 1].w;
//		//line.endWidth = data.vector4s [index + size].w;
//	}
//	void parse(){
//		cursor = 10;
//		while (cursor < data.bytes.Length) {
//			int size = (Convert.ParsetoInt16 (data.bytes, cursor) - 3)/3;
//			cursor += 2;
//			Color c = Convert.ParsetoColor (data.bytes, cursor);
//			cursor += 4;
//			float w = Convert.ParsetoFloat (Convert.ParsetoInt16 (data.bytes, cursor));
//			cursor += 2;
//			curves.Add(new Curve(Convert.ParsetoVector3s(data.bytes,cursor,size),w,c));
//			cursor += size * 3 * 2;
//		}
//	}

//  void heheparse() {
//    cursor = 8;
//    int curveCnt = Convert.ParsetoInt16(data.bytes, cursor);
//    cursor += 2;
    
//    for (int i = 0; i < curveCnt; i++) {
//      int curveLength = Convert.ParsetoInt16(data.bytes, cursor);
//      cursor += 2;
      
//      Color c = Convert.ParsetoColor(data.bytes, cursor);
//      cursor += 4;

//      int iw = Convert.ParsetoInt16(data.bytes, cursor);
//      float w = Convert.ParsetoFloat(Convert.ParsetoInt16(data.bytes, cursor));
//      cursor += 2;

//      List<Vector3> points = new List<Vector3>();
//      testPoints = new List<Vector3>();

//      for (int j = 0; j < curveLength; j++) {
//        // parse the point
//        Vector3 p = Convert.ParsetoVector3(data.bytes, cursor, 1);
//        // do scale for point
//        p.Scale(lineScale);
//        // do translation for point
//        p += lineTrans;
//        // add to list
//        points.Add(p);
//        testPoints.Add(Convert.ParsetoVector3(data.bytes, cursor, 3f));
//        cursor += 6;
//      }
//      curves.Add(new Curve(points, w * 5.0f, c));
//      //curves.Add(new Curve(testPoints, w * 5.0f, c));

//    }
//  }

//  void draw(){
//		Curve.DrawCurves (prefab, master, curves);
//	}
//	// Update is called once per frame
//	protected override void Sync ()
//	{
//    curves.Clear();
//    if (this.Tracked) {
//      int count = 0;
//      cursor = 0;
//      hehe = data.bytes;
//      //Debug.Log("the bytes size is " + data.bytes.Length);
//      if (master != null)
//        Destroy(master);
//        //if(master == null)
//            master = (GameObject)Instantiate(empty);
//      //parse();
//      heheparse();
//    }
//	}

//	void LateUpdate()
//	{
//		draw ();
//	}


//	void print (Quaternion[] v)
//	{
//		for (int k = 0; k < v.Length; k++)
//			Debug.Log (v [k]);
//	}

//}
