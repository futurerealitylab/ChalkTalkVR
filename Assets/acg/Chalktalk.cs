////Chalktalk.cs
////Created by Aaron C Gaudette on 17.07.16
//
////WIP code
//
////the more lines you have, the slower it runs
////weird lag, not like "normal" lag, likely multiple problems at once
////the slowdown has nothing to do with the rendering
////must be the server, the amount of data sent over the sockets, or the boxing done here
////probably something obvious
//
////you'll notice the framerate in unity (and also the memory I think) is fine.
////just the more lines you add, the more this client lags behind the other one
////also when the other (native) client isn't open, unity updates at a much slower rate
////this is probably all "intended" behavior with the chalktalk server at some level
//
////ken says it's the buffer
//
//using UnityEngine;
//using WebSocketSharp;
//using System.Collections.Generic;
//
//public class Chalktalk : MonoBehaviour{
//	public string url = "ws://localhost:22346";
//	public float scale = 1;
//	public float widthScale = 8;
//	
//	//
//	public LineRenderer renderer;
//	
//	public List<float[]> curves = new List<float[]>();
//	public float[][] cache = new float[0][];
//	
//	//
//	List<Color> colors = new List<Color>();
//	Color[] colorCache = new Color[0];
//	
//	WebSocket ws;
//	
//	void Awake(){
//		ws=new WebSocket(url);
//		
//		ws.OnOpen += (sender,e) => Debug.Log("Chalktalk: Opened socket");
//		ws.OnError += (sender,e) => Debug.LogError("Chalktalk: "+e.Message);
//		ws.OnClose += (sender,e) => Debug.Log("Chalktalk: Socket closed");
//		
//		//
//		ws.OnMessage += (sender,e) => {
//			JSONObject o = new JSONObject(e.Data);
//			
//			//
//			JSONObject l = o["display"];
//			string label = l!=null && l.type==JSONObject.Type.STRING?
//				l.str:"null";
//			
//			if(label=="path"){
//				List<JSONObject> rawPoints = o["path"].list;
//				float[] points = new float[rawPoints.Count];
//				
//				for(int i=0;i<rawPoints.Count;++i)
//					points[i]=rawPoints[i].n*scale;
//				
//				curves.Add(points);
//				
//				List<JSONObject> rawColorData = o["rgba"].list;
//				float[] colorData = new float[rawColorData.Count];
//				
//				for(int i=0;i<rawColorData.Count;++i)
//					colorData[i]=rawColorData[i].n;
//				
//				colors.Add(new Color(colorData[0],colorData[1],colorData[2],colorData[3]));
//				//backwards?
//				//colors.Insert(0,new Color(colorData[0],colorData[1],colorData[2],colorData[3]));
//			}
//			else if(label=="flush"){
//				Debug.Log("Chalktalk: Flushed");
//				
//				//important that the cache is a copy, not a reference
//				//chalktalk's update is not in sync with Unity's update
//				//need to cache between each rebuild/flush
//				cache=new float[curves.Count][];
//				for(int i=0;i<cache.Length;++i)
//					cache[i]=curves[i];
//				
//				curves.Clear();
//				
//				colorCache=new Color[colors.Count];
//				for(int i=0;i<colorCache.Length;++i)
//					colorCache[i]=colors[i];
//				
//				colors.Clear();
//			}
//			//else print(e.Data);
//		};
//		
//		ws.Connect();
//		
//		//set the global flag
//		ws.Send("{ \"global\": \"displayListener\", \"value\": true }");
//	}
//	
//	void OnApplicationQuit(){
//		ws.Send("{ \"global\": \"displayListener\", \"value\": false }");
//	}
//	
//	/*
//	
//	//needs to be attached to a camera
//	void OnPostRender(){
//		debugMaterial.SetPass(0);
//		foreach(float[] ff in cache){
//			GL.Begin(GL.LINES);
//			GL.Color(Color.white); //
//			for(int i=0;i<ff.Length && ff.Length>3;i+=4){
//				if(i>0 && i<ff.Length) //tmp
//					GL.Vertex(new Vector3(ff[i],ff[i+1],ff[i+2]));
//				GL.Vertex(new Vector3(ff[i],ff[i+1],ff[i+2]));
//			}
//			GL.End();
//		}
//	}
//	
//	//shader in resources folder
//	private static Material mat;
//	static Material debugMaterial{
//		get{
//			Shader shader = Shader.Find("Custom/Debug");
//			if(mat==null || mat.shader!=shader){
//				DestroyImmediate(mat);
//				mat=new Material(shader);
//				mat.hideFlags=HideFlags.DontSave;
//			}
//			return mat;
//		}
//	}
//	void OnDisable(){DestroyImmediate(mat);}
//	
//	*/
//	
//	void Update(){
//		
//		//fill in these values for the vive
//		//do a simple projection from 3d to 2d using the controller's position
//		//the trick is going to be matching what the server outputs to the input position
//			//if you always know where the reference window is in 3d space (you should) this becomes trivial
//		
//		float x = Input.mousePosition.x;
//		float y = Input.mousePosition.y; //NOTE for mouse this should be inverted, but those values aren't showing up properly
//		
//		//excessive lag on input
//		
//		if(Input.GetKeyDown(KeyCode.Mouse0)){
//			Debug.Log("Chalktalk: Mouse down");
//			ws.Send("{\"eventType\": \"onmousedown\", \"event\": { \"button\": 0, \"clientX\": "+x+", \"clientY\": "+y+" } }");
//		}
//		if(Input.GetKeyUp(KeyCode.Mouse0)){
//			Debug.Log("Chalktalk: Mouse up");
//			ws.Send("{\"eventType\": \"onmouseup\", \"event\": { \"button\": 0, \"clientX\": "+x+", \"clientY\": "+y+" } }");
//		}
//		if(Input.GetKey(KeyCode.Mouse0))
//			ws.Send("{\"eventType\": \"onmousemove\", \"event\": { \"button\": 0, \"clientX\": "+x+", \"clientY\": "+y+" } }");
//		
//		//add keyboard events here if you need them
//		//this code doesn't work (?) for some reason
//		//may be the code is not in ascii
//		if(Input.GetKeyDown(KeyCode.Backspace)){
//			Debug.Log("Chalktalk: Delete");
//			ws.Send("{\"eventType\": \"onkeypress\", \"event\": { \"keyCode\": 8 } }");
//		}
//		if(Input.GetKeyUp(KeyCode.Backspace))
//			ws.Send("{\"eventType\": \"onkeyrelease\", \"event\": { \"keyCode\": 8 } }");
//		
//		// fix null
//		//f(cache!=null)foreach(float[] ff in cache)DrawCurve(ff);
//		
//		for(int i=0;i<transform.childCount || i<cache.Length;++i){
//			if(i<cache.Length)
//				DrawLine(cache[i],colorCache[i],i);
//			else ClearLine(i);
//		}
//	}
//	void DrawLine(float[] points, Color color, int index){
//		LineRenderer r = null;
//		if(transform.childCount<=index){
//			r=Instantiate(renderer,transform.position,transform.rotation) as LineRenderer;
//			r.transform.parent=transform;
//			r.gameObject.name="Line "+index;
//		}
//		else r=transform.GetChild(index).GetComponent<LineRenderer>();
//		
//		r.useWorldSpace=false;
//		
//		//can optimize this data later, depends on how we end up using it
//		Vector3[] positions = new Vector3[points.Length/4];
//		int pi = 0;
//		for(int i=0;i<points.Length;i+=4)
//			positions[pi++]=new Vector3(points[i],points[i+1],points[i+2]);
//		
//		r.SetVertexCount(points.Length/4);
//		r.SetPositions(positions);
//		
//		//hack
//		r.SetWidth(points[3]*widthScale,points[3]*widthScale);
//		r.SetColors(color,color);
//		
//	}
//	void ClearLine(int index){
//		LineRenderer r = transform.GetChild(index).GetComponent<LineRenderer>();
//		
//		r.SetVertexCount(0);
//		//r.SetWidth(0,0);
//	}
//	
//	void DrawCurve(float[] points){
//		//
//		for(int i=0;i<points.Length-4;i+=4)
//			Debug.DrawLine(
//				new Vector3(points[i],points[i+1],points[i+2]),
//				new Vector3(points[i+4],points[i+5],points[i+6]),
//				Color.white
//			);
//	}
//}
//
////need to fail gracefully (connection loss)