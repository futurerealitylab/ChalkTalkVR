// Use this method if you need more control than you get with Vector.SetLine
using UnityEngine;
using System.Collections.Generic;
using Vectrosity;

public class Line : MonoBehaviour {

    public Texture2D capLineTex;
    public Texture2D capTex;

    void Start () {
		// Make a Vector2 list; in this case we just use 2 elements...
		var linePoints = new List<Vector2>();
		linePoints.Add (new Vector2(0, Random.Range(0, Screen.height)));				// ...one on the left side of the screen somewhere
		linePoints.Add (new Vector2(Screen.width-1, Random.Range(0, Screen.height)));   // ...and one on the right

        // Make a VectorLine object using the above points, with a width of 2 pixels
        //var line = new VectorLine("Line", linePoints, 2.0f);
        // zhenyi
        Texture2D tex;
        float useLineWidth;
        float capLineWidth = 20.0f;
        VectorLine.SetEndCap("RoundCap", EndCap.Mirror, capLineTex, capTex);
        tex = capLineTex;
        useLineWidth = capLineWidth;

        var line = new VectorLine("Line", linePoints, tex, useLineWidth, LineType.Continuous, Joins.Weld);
        line.endCap = "RoundCap";
        //line.capLength = 10.0f;
        
        // Draw the line
        line.Draw();
	}
}