using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chalktalk
{
    public class Renderer : Holojam.Tools.Trackable
    {

        // For DevDebug
        public Byte[] DataViewer;

        [SerializeField]
        public Curve curvePrefab;
        [SerializeField]
        public BindingBox bindingBox;
        [SerializeField]
        public Transform curvedParent;

        public CurvedUI.CurvedUISettings mySettings;

        [SerializeField]
        private string label = "Display";

        Chalktalk.Parser ctParser = new Chalktalk.Parser();

        public List<Curve> curves = new List<Curve>();

        public override void ResetData()
        {
            base.ResetData();
            mySettings = GetComponentInParent<CurvedUI.CurvedUISettings>();
        }

        public override string Label
        {
            get { return label; }
        }

        public override string Scope
        {
            get { return ""; }// "Chalktalk"; }
        }

        protected override void UpdateTracking()
        {
            if (this.Tracked)
            {
                DestroyCurves();
                DataViewer = data.bytes;
                Parse(data.bytes);
                Draw();
            }
        }

        private void DestroyCurves()
        {
            foreach (Curve curve in curves)
            {
                DestroyImmediate(curve.gameObject);
            }
            curves.Clear();
        }

        
        private void Parse(byte[] bytes)
        {
            ChalkTalkObj ctObj = ctParser.Parse(bytes, this);
            return;
        }


        private void Draw()
        {
            foreach (Curve curve in curves)
            {
                curve.Draw();
            }
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

        void oldparse(byte[] bytes)
        {
            //Skip the "CTDATA01" String header
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
                    print(point);
                    //point.Scale(bindingBox.transform.localScale);
                    //Move point to the bindingBox Coordinate
                    point = bindingBox.transform.rotation * point + bindingBox.transform.position;
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

                Curve curve = GameObject.Instantiate<Curve>(curvePrefab);
                curve.transform.SetParent(this.transform);

                curve.points = points;
                curve.width = width * 3;
                curve.color = isFrame ? new Color(1, 1, 1, 1) : color;
                // zhenyi: not using the chalktalk color
                curve.color = new Color(1, 1, 1, 1);
                curves.Add(curve);
            }
        }

    }
}








