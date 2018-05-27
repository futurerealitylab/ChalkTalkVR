#define DEBUG_PRINT


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.Network;
using FRL.Utility;
using UnityEngine.UI;

namespace Chalktalk
{

    public class DisplayObj
    {
        public string label;
        public bool isTracked;
        public byte[] bytes;
        public int batchByteLength;
        public int sliceCount;
        public int sliceId;
        public int batchTimestamp;

        public bool UpdateTracking()
        {
            isTracked = XRNetworkClient.IsTracked(label);
            if (isTracked) {
                bytes = XRNetworkClient.GetBytes(label);
                batchByteLength = XRNetworkClient.GetInt(label, 0);
                sliceCount = XRNetworkClient.GetInt(label, 1);
                sliceId = XRNetworkClient.GetInt(label, 2);
                batchTimestamp = XRNetworkClient.GetInt(label, 3);
                return true;
            }
            return false;
        }

        public bool UpdateTracking(string l) {
            isTracked = XRNetworkClient.IsTracked(l);
            if (isTracked) {
                bytes = XRNetworkClient.GetBytes(l);
                batchByteLength = XRNetworkClient.GetInt(l, 0);
                sliceCount = XRNetworkClient.GetInt(l, 1);
                sliceId = XRNetworkClient.GetInt(l, 2);
                batchTimestamp = XRNetworkClient.GetInt(l, 3);
                return true;
            }
            return false;
        }

        public override string ToString() {
            return "BATCH BYTE LENGTH: " + batchByteLength + ", BYTE LENGTH: " + bytes.Length + ", SLICE COUNT: " + sliceCount + ", SLICE ID: " + sliceId + ", BATCH TIMESTAMP: " + batchTimestamp;
        }

        public DisplayObj()
        {
            label = "label";
            bytes = new byte[0];
        }

        public DisplayObj(string l)
        {
            label = l;
            bytes = new byte[0];
        }
    }

    public class Renderer : MonoBehaviour
    {

        // For DevDebug
        public Byte[] DataViewer;

        [SerializeField]
        public Curve curvePrefab;
        [SerializeField]
        public BindingBox bindingBox;

        [SerializeField]
        private string label = "Display1";

        DisplayObj displayObj;

        Chalktalk.Parser ctParser = new Chalktalk.Parser();

        public List<Curve> curves = new List<Curve>();

        public List<string> trackedLabels = new List<string>();

        void Start()
        {
            displayObj = new DisplayObj(label);
            trackedLabels.Add(label);
            // To karl: create or assign label to DisplayObj when you need to. And then retrieve bytes through public members.
        }

        public int batchLen = 0;
        public int sliceCount = 1;
        public bool[] sliceArrived = null;
        public int slicesArrived = 0;
        public byte[] mergedBytes = null;
        bool isIdle = true;
        int timeStamp = -1;

        public byte[][] allBytes = null;

        protected void Update()
        {
            for (int i = 0; i < sliceCount; ++i) {
                if (!isIdle && sliceArrived[i]) {
                    continue;
                }
                // get bytes by label
                if (displayObj.UpdateTracking("Display" + (i + 1))) {
                    if (isIdle) {
                        isIdle = false;

                        sliceCount  = displayObj.sliceCount;
                        allBytes    = new byte[sliceCount][];
                        sliceArrived = new bool[sliceCount];
                        batchLen = displayObj.batchByteLength;

                        timeStamp = displayObj.batchTimestamp;

                    }


                    DataViewer = displayObj.bytes;
#if DEBUG_PRINT             
                    Debug.Log("Display" + (i + 1) + " arrived");
                    Debug.Log(displayObj);
#endif
                    allBytes[i] = displayObj.bytes;
                    ++slicesArrived;
                    sliceArrived[i] = true;

                } else {
#if DEBUG_PRINT             
                    Debug.Log("Display" + (i + 1) + " did not arrive yet");
#endif
                }
            }

            if (!isIdle && slicesArrived == sliceCount) {
                slicesArrived = 0;
                //sliceCount = 1;
                isIdle = true;

                DestroyCurves();


                mergedBytes = new byte[batchLen];
                // copy header from first slice
                int ptr = 0;
                Buffer.BlockCopy(allBytes[0], 0, mergedBytes, ptr, 8);
                //for (; ptr < 8; ++ptr) {
                //    mergedBytes[ptr] = allBytes[0][ptr];
                //}
                ptr += 8;

                // copy the rest of the byte arrays ( TODO send slices from client without duplicate headers per slice)
                for (uint i = 0; i < allBytes.Length; ++i) {
                    byte[] byteSlice = allBytes[i];
                    Buffer.BlockCopy(byteSlice, 8, mergedBytes, ptr, byteSlice.Length - 8);
                    //for (uint j = 8; j < byteSlice.Length; ++j) {
                    //    mergedBytes[ptr] = byteSlice[j];
                    //    ++ptr;
                    //}
                    ptr += (byteSlice.Length - 8);
                }

                if (batchLen != ptr) {
                    Debug.LogWarningFormat("batch byte length " + batchLen + " does not equal total copied byte length " + ptr);
                }

                //string S = "{";
                //for (int i = 0; i < mergedBytes.Length; ++i) {
                //    S += mergedBytes[i] + ", ";
                //}
                //S += "}";

                ctParser.Parse(mergedBytes, this);

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








