using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class Line3D : MonoBehaviour {

    public int lineAmt;
    public Color32[] colorOptions;
    // testing, creating multiple lines and each line has four points

    public List<Vector3> linePoints;
    List<Color32> lineColors;
    VectorLine line;
    public Transform customTransform;

    public Texture2D capLineTex;
    public Texture2D capTex;

    VectorLine vText;
    public float textSize;
    public float capLength;
    public GameObject ori;
    public GameObject CenterCameraEye;

    void CreateOneLine(float range, Color32 lineColor)
    {
        // first point
        Vector3 curPoint = new Vector3(Random.Range(range, range + 3.0f) / 10.0f, Random.Range(0.0f, 10.0f) / 10.0f, Random.Range(range, range + 3.0f) / 10.0f);// - Camera.main.transform.forward * 10.0f;
        linePoints.Add(curPoint);
        lineColors.Add(lineColor);
        for (int i = 0; i < 3; i++)
        {
            curPoint = new Vector3(Random.Range(range, range + 3.0f) / 10.0f, Random.Range(0.0f, 10.0f) / 10.0f, Random.Range(range, range + 3.0f) / 10.0f);
            linePoints.Add(curPoint);
            lineColors.Add(lineColor);
            linePoints.Add(curPoint);
        }
        curPoint = new Vector3(Random.Range(range, range + 3.0f) / 10.0f, Random.Range(0.0f, 10.0f) / 10.0f, Random.Range(range, range + 3.0f) / 10.0f);
        linePoints.Add(curPoint);
        // continuous result with type discrete
    }

    void configEndCap()
    {
        // name any kind of end cap here, with certain amount of texture
        VectorLine.SetEndCap("RoundCap", EndCap.Mirror, capLineTex, capTex);
    }

    // Use this for initialization
    void Start () {
        // config VectorLine
        VectorLine.SetCamera3D(CenterCameraEye);

        configEndCap();

        // Make a Vector2 list; in this case we just use 2 elements...
        linePoints = new List<Vector3>();
        lineColors = new List<Color32>();
        for (int j = 0; j < lineAmt; j++)
        {
            // Make a VectorLine object using the above points, with a width of 2 pixels
            CreateOneLine(j * 3.0f, colorOptions[j]);
        }
        line = new VectorLine("3DLine", linePoints, 6.0f, LineType.Discrete, Joins.Weld);
        //line = new VectorLine("3DLine", linePoints, capLineTex, capLength, LineType.Discrete);
        line.SetColors(lineColors);
        line.drawTransform = customTransform;
        //line.capLength = 10.0f;
        line.lineWidth = 10.0f;
        line.endCap = "RoundCap";
        //line.drawStart = 0;
        //line.drawEnd = 3;
        
        //line.joins = Joins.Weld;
        // Draw the line
        line.Draw3D();
        //line.StopDrawing3DAuto();

        

        vText = new VectorLine("3DText", new List<Vector3>(), 1.0f);
        vText.color = Color.yellow;
        vText.drawTransform = customTransform;
        vText.MakeText("Vectrosity!", new Vector3(0f,0.5f,0f), textSize);
        vText.Draw3D();
    }
	
	// Update is called once per frame
	void Update () {
        //for (int i = 0; i < lineAmt; i++)
        //{
        //    linePoints[i] = new Vector3(Random.Range(0, 3), Random.Range(0, 3), Random.Range(0, 3));
        //}
        //line.Draw3D();

        //VectorLine.SetCamera3D(ori);
        //line.capLength = capLength;
        line.drawTransform = customTransform;
        //line.drawStart = 0;
        //line.drawEnd = 3;
        //line.Draw3D();

        //line.drawStart = 4;
        //line.drawEnd = 7;
        line.Draw3D();

        vText.drawTransform = customTransform;
        vText.MakeText("Vectrosity!", new Vector3(0.5f, 0.5f, 0.5f), textSize);
        vText.Draw3D();
    }
}
