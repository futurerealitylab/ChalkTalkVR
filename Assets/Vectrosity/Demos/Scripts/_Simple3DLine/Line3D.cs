using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class Line3D : MonoBehaviour {

    public int lineAmt, batchAmt;
    public Color32[] colorOptions;
    // testing, creating multiple lines and each line has four points

    List<Vector3> linePoints;
    List<Color32> lineColors;
    VectorLine line;

    void CreateOneLine(int range, Color32 lineColor)
    {
        // first point
        Vector3 curPoint = new Vector3(Random.Range(range, range+3), Random.Range(range, range+3), Random.Range(range, range+3));
        linePoints.Add(curPoint);
        lineColors.Add(lineColor);
        for (int i = 0; i < 3; i++)
        {
            curPoint = new Vector3(Random.Range(range, range + 3), Random.Range(range, range + 3), Random.Range(range, range + 3));
            linePoints.Add(curPoint);
            lineColors.Add(lineColor);
            linePoints.Add(curPoint);
        }
        curPoint = new Vector3(Random.Range(range, range + 3), Random.Range(range, range + 3), Random.Range(range, range + 3));
        linePoints.Add(curPoint);
        // continuous result with type discrete
    }

    // Use this for initialization
    void Start () {
        // Make a Vector2 list; in this case we just use 2 elements...
        linePoints = new List<Vector3>();
        lineColors = new List<Color32>();
        for (int j = 0; j < lineAmt; j++)
        {
            // Make a VectorLine object using the above points, with a width of 2 pixels
            CreateOneLine(j * 3, colorOptions[j]);
        }
        line = new VectorLine("3DLine", linePoints, 2.0f, LineType.Discrete);
        line.SetColors(lineColors);
        // Draw the line
        line.Draw3D();
    }
	
	// Update is called once per frame
	void Update () {
        //for (int i = 0; i < lineAmt; i++)
        //{
        //    linePoints[i] = new Vector3(Random.Range(0, 3), Random.Range(0, 3), Random.Range(0, 3));
        //}
        //line.Draw3D();
    }
}
