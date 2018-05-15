using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chalktalk
{
    public enum ChalkTalkType { CURVE, PROCEDURE};
    public class ChalkTalkObj
    {
        public ChalkTalkType type;
        public ChalkTalkObj()
        {
            type = ChalkTalkType.CURVE;
        }
    }
    public class Parser
    {
        [SerializeField]
        private Curve curvePrefab;

        public ChalkTalkObj Parse(byte[] bytes, Renderer renderer)
        {
            ChalkTalkObj ctobj = new ChalkTalkObj();

            // Check the header
            string header = Utility.ParsetoString(bytes, 0, 8);

            if(header == "CTdata01")
            {
                
                ctobj.type = ChalkTalkType.CURVE;
                ParseStroke(bytes, ref ctobj, renderer);
                return ctobj;
            }
            else
            {
                ctobj.type = ChalkTalkType.PROCEDURE;
                ParseProcedureAnimation(bytes, ref ctobj, renderer);
                return ctobj;
            }
            
        }

        public void ParseStroke(byte[] bytes, ref ChalkTalkObj ctobj, Renderer renderer)
        {
            int cursor = 8;

            // The total number of words in this packet, then get the size of the bytes size
            int curveCount = Utility.ParsetoInt16(bytes, cursor);
            cursor += 2;

            for (; cursor < bytes.Length;)
            {
                Debug.Log("Current Cursor: " + cursor);
                //The length of the current line
                int length = Utility.ParsetoInt16(bytes, cursor);
                cursor += 2;

                // if the line data is less than 12, we skip this one curve
                // TODO: implement the new curve module so that we can keep the curve with the same id
                if (length < 12)
                    continue;


                // The ID of current line
                int ID = Utility.ParsetoInt16(bytes, cursor);
                cursor += 2;

                //Parse the color of the line
                Color color = Utility.ParsetoColor(bytes, cursor);
                cursor += 4;

                //float width = Utility.ParsetoFloat(Utility.ParsetoInt16(bytes, cursor));
                //cursor += 2;

                //Parse the Transform of this Curve
                Vector3 translation = Utility.ParsetoVector3(bytes, cursor, 1);
                cursor += 6;
                Quaternion rotation = Utility.ParsetoQuaternion(bytes, cursor, 1);
                cursor += 6;
                float scale = Utility.ParsetoFloat(Utility.ParsetoInt16(bytes, cursor));
                cursor += 2;

                //Parse the type of the stroke
                int type = Utility.ParsetoInt16(bytes, cursor);
                cursor += 2;


                //Parse the width of the line
                float width = 0;

                List<Vector3> points = new List<Vector3>();
                Debug.Log("Current Line's points count: " + (length - 12) / 4);
                Debug.Log("Current Cursor before read the points :" + cursor);
                for (int j = 0; j < (length - 12) / 4; j++)
                {
                    Vector3 point = Utility.ParsetoVector3(bytes, cursor, 1);
                    //point.Scale(bindingBox.transform.localScale);
                    //Move point to the bindingBox Coordinate
                    point = renderer.bindingBox.transform.rotation * point + renderer.bindingBox.transform.position;
                    //Apply the point transform for each point
                    points.Add(point);
                    //points.Add((rotation * point + translation) * scale);
                    cursor += 6;
                    width = Utility.ParsetoFloat(Utility.ParsetoInt16(bytes, cursor));
                    cursor += 2;
                }

                // bold the framework
                bool isFrame = boldenFrame(points);
                // width *= (isFrame) ? 20.0f : 1.0f;


                Curve curve = GameObject.Instantiate<Curve>(renderer.curvePrefab);
                curve.transform.SetParent(renderer.transform);

                curve.points = points;
                curve.width = width * 3;
                // curve.color = isFrame ? new Color(1, 1, 1, 1) : color;
                curve.color = color;
                // zhenyi: not using the chalktalk color
                //curve.color = new Color(1, 1, 1, 1);
                curve.type = (ChalktalkDrawType)type;
                renderer.curves.Add(curve);
            }
        }

        public void ParseProcedureAnimation(byte[] bytes, ref ChalkTalkObj ctobj, Renderer renderer)
        {

        }

        private bool boldenFrame(List<Vector3> points)
        {
            if (points.Count == 5)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if ((points[i].z < 1.0f) && (points[i].z > -1.0f))
                    {
                        break;
                    }
                }
                return true;
            }

            return false;
        }
    }

}