using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalkTalkChalkMaker : MonoBehaviour {
	RaycastHit hit;
	List<Vector3> line = new List<Vector3> ();
	List<Vector3> renderLine = new List<Vector3>();
	LineRenderer render;
	bool clicked;
	float xlo,xhi,ylo,yhi;
	ChalkTalkSender cts;

	public GameObject prefab;
	// Use this for initialization
	void Start () {
		line.Clear ();
		cts = this.GetComponent<ChalkTalkSender> ();
		xlo = GameObject.Find ("Plane").GetComponent<MeshRenderer> ().bounds.center.x - GameObject.Find ("Plane").GetComponent<MeshRenderer> ().bounds.size.x / 2f;
		xhi = GameObject.Find ("Plane").GetComponent<MeshRenderer> ().bounds.center.x + GameObject.Find ("Plane").GetComponent<MeshRenderer> ().bounds.size.x / 2f;
		ylo = GameObject.Find ("Plane").GetComponent<MeshRenderer> ().bounds.center.y - GameObject.Find ("Plane").GetComponent<MeshRenderer> ().bounds.size.y / 2f;
		yhi = GameObject.Find ("Plane").GetComponent<MeshRenderer> ().bounds.center.y + GameObject.Find ("Plane").GetComponent<MeshRenderer> ().bounds.size.y / 2f;
	}

	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Input.GetKeyDown (KeyCode.Space)) {
			line.Clear ();
		}
		if(Input.GetMouseButtonDown(0)){
			//renderLine.Clear ();
			//GameObject g = (GameObject)Instantiate (prefab);
			//render = g.GetComponent<LineRenderer> ();
			if(Physics.Raycast(ray, out hit))
			{
				clicked = true;
				if (hit.collider.name == "Plane") {
					//line.Add (hit.point);
					//renderLine.Add (hit.point);
					//draw ();
					cts.sendMouseDown (1, pointonscreen (hit.point));
				}
			}
		}
		if (Input.GetMouseButton(0)){
			if(Physics.Raycast(ray, out hit))
			{
				if (hit.collider.name == "Plane") {
				//	line.Add (hit.point);
				//	renderLine.Add (hit.point);
					//draw ();
					cts.sendMouseMove (1, pointonscreen (hit.point));
				}
			}
				
		}
		if (Input.GetMouseButtonUp (0)) {
			cts.sendMouseUp (1, pointonscreen (hit.point));
			clicked = false;
		}
	}
	void draw(){
		render.numPositions = renderLine.Count;
		int i = 0;
		foreach (Vector3 v in renderLine) {
			render.SetPosition (i++, v);
		}
	}

	Vector2 pointonscreen (Vector3 p)
	{
		//Vector2 rst = new Vector2( ((p.x - xlo)/(xhi - xlo)) ,((p.y)/(yhi - ylo)));
		Vector2 rst = new Vector2(((p.x/5f)+1f)/4f * 2560,(-p.y/5f+1f)/4f *1600) ;
		//Debug.Log (rst);
		return rst;
	}
	
}
