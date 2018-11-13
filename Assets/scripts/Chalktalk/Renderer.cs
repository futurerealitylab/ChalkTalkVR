//#define DEBUG_PRINT
#define FAIL_FAST


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.Network;
using FRL.Utility;
using UnityEngine.UI;
using UnityEditor;
using Vectrosity;

namespace Chalktalk
{

    public class DisplayObj {
        public string label;
        public bool isTracked;
        public byte[] bytes;
        public int batchByteLength;
        public int sliceCount;
        public int sliceId;


        public int batchTimestamp;
        public int batchTimestampOverflow;

        public bool UpdateTracking() {
            isTracked = XRNetworkClient.IsTracked(label);
            if (isTracked) {
                bytes = XRNetworkClient.GetBytes(label);
                batchByteLength = XRNetworkClient.GetInt(label, 0);
                sliceCount = XRNetworkClient.GetInt(label, 1);
                sliceId = XRNetworkClient.GetInt(label, 2);

                batchTimestamp = XRNetworkClient.GetInt(label, 3);
                batchTimestampOverflow = XRNetworkClient.GetInt(label, 4);
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
                batchTimestampOverflow = XRNetworkClient.GetInt(label, 4); // TODO handle very, VERY long runs of Chalktalk lasting beyond 414 days?
                return true;
            }
            return false;
        }

        public override string ToString() {
            return 
                "{BATCH BYTE LENGTH: " + batchByteLength + 
                ", BYTE LENGTH: " + bytes.Length + 
                ", SLICE COUNT: " + sliceCount + 
                ", SLICE ID: " + sliceId + 
                ", BATCH TIMESTAMP: " + batchTimestamp + ", BATCH TIMESTAMP OVERFLOW: " + batchTimestampOverflow + "}";
        }

        public DisplayObj() {
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

        // for mirror
        public float facingDirection;

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
        private string label = "Display1";

        DisplayObj displayObj;

        Chalktalk.Parser ctParser = new Chalktalk.Parser();

        public List<Curve> curves = new List<Curve>();

        public Texture2D capLineTex;
        public Texture2D capTex;

        //public override void ResetData()
        //{
        //    base.ResetData();
        //    mySettings = GetComponentInParent<CurvedUI.CurvedUISettings>();
        //}

        //public override string Label
        //promotion_video

        public List<string> trackedLabels = new List<string>();

        void configEndCap()
        {
            // name any kind of end cap here, with certain amount of texture
            VectorLine.SetEndCap("RoundCap", EndCap.Mirror, capLineTex, capTex);
        }
        // Vectrosity related
        Transform forDrawTransform;
        void Start()
        {
            // Vectrosity related
            forDrawTransform = GameObject.Find("drawTransform").transform;
            ctParser.forDrawTransform = forDrawTransform;
            displayObj = new DisplayObj(label);
            trackedLabels.Add(label);
            configEndCap();
            // To karl: create or assign label to DisplayObj when you need to. And then retrieve bytes through public members.
        }

        public class BatchData {
            public int byteLength     = 0;
            public int slicesArrived  = 0;
            public byte[][] slices    = null;
            public ulong timestampKey = 0;

            public static int timestampMostRecentlyCompleted         = 0;
            public static int timestampOverflowMostRecentlyCompleted = 0;
            public static ulong timestampKeyMostRecentlyCompleted    = 0;

            public override string ToString() {
                return "{KEY: " + timestampKey + ", ARRIVED: " + slicesArrived + ", SLICE COUNT: " + slices.Length + "}";
            }
        }


        public int sliceCount = 1;
        public byte[] mergedBytes = null;
        bool readyToDraw = false;

        public Dictionary<ulong, BatchData> timestampMap = new Dictionary<ulong, BatchData>();

        public BatchData completeBatchData = new BatchData(); // sentinel data to avoid need for null checks

        protected void Update()
        {
            int labelCount = XRNetworkClient.LabelCount();
            Debug.Log("LABEL COUNT: " + labelCount);
            for (int i = 0; i < labelCount; ++i) {
                // try to update tracking on label (i + 1)
                if (!displayObj.UpdateTracking("Display" + (i + 1))) {
#if DEBUG_PRINT
                    Debug.Log("Display" + (i + 1) + " did not arrive yet: frame: " + Time.frameCount);
#endif
                    continue;
                }
                // ignore data belonging to old batches
                if ((ulong)displayObj.batchTimestamp < BatchData.timestampKeyMostRecentlyCompleted) {
#if DEBUG_PRINT
                    Debug.Log("OUTDATED DATA ARRIVED: frame: " + Time.frameCount);
#endif
                    continue;
                }

#if DEBUG_PRINT             
                Debug.Log("Display" + (i + 1) + " arrived: frame: " + Time.frameCount);
                //Debug.Log(displayObj);
#endif

                // retrieve or create batch data
                BatchData batch;
                // if the timestamp has not been registered, create new batch data
                if (!timestampMap.TryGetValue((ulong)displayObj.batchTimestamp, out batch)) {
                    batch = new BatchData {
                        byteLength = displayObj.batchByteLength,
                        slices = new byte[displayObj.sliceCount][],
                        timestampKey = (ulong)displayObj.batchTimestamp,
                    };
                    timestampMap[(ulong)displayObj.batchTimestamp] = batch;
#if DEBUG_PRINT
                    Debug.Log("BATCH DOES NOT EXIST YET: frame: " + Time.frameCount + " IDX: " + i);
                } else {
                    Debug.Log("BATCH EXISTS: frame: " + Time.frameCount);
#endif
                }

                // store byte data as a slice
                batch.slices[i] = displayObj.bytes;

                ++batch.slicesArrived;

                // if slices arrived matches the expected number of slices for this batch,
                // mark as complete and for drawing
#if DEBUG_PRINT
                Debug.Log(batch + ": " + Time.frameCount);

                Debug.Log(
                    "CHECKING FOR COMPLETED BATCH: " + 
                    (batch.slicesArrived == batch.slices.Length &&
                    batch.timestampKey > completeBatchData.timestampKey) + ": frame: " + Time.frameCount
                );
#endif

                if (batch.slicesArrived == batch.slices.Length &&
                    batch.timestampKey > completeBatchData.timestampKey) 
                {
                    completeBatchData = batch;
                    BatchData.timestampKeyMostRecentlyCompleted = batch.timestampKey;
                    readyToDraw = true;
                }
            }

            if (readyToDraw) {
                readyToDraw = false;
#if DEBUG_PRINT
                Debug.Log("DRAWING: frame: " + Time.frameCount);
#endif

                mergedBytes = new byte[completeBatchData.byteLength];

                int ptr = 0;
                {
                    byte[][] slices = completeBatchData.slices;
                    int length = slices.Length;
                    for (int i = 0; i < length; ++i) {
                        byte[] byteSlice = slices[i];

                        Buffer.BlockCopy(byteSlice, 0, mergedBytes, ptr, byteSlice.Length);
                        ptr += byteSlice.Length;
                    }
                }
                

                Debug.AssertFormat(completeBatchData.byteLength == ptr, "batch byte length " + completeBatchData.byteLength + " does not equal total copied byte length " + ptr);

                if (completeBatchData.byteLength != ptr) {
#if FAIL_FAST
                    EditorApplication.isPlaying = false;
#else
                    return;
#endif
                }

                DataViewer = mergedBytes;

                DestroyCurves();

                ctParser.Parse(mergedBytes, this);

                Draw();

#if DEBUG_PRINT
                Debug.Log("TIMESTAMP MAP COUNT BEFORE REMOVE OPERATIONS: " + timestampMap.Count + " frame: " + Time.frameCount);
#endif
                List<ulong> toRemove = new List<ulong>();
                foreach (KeyValuePair<ulong, BatchData> entry in timestampMap) {
                    if (entry.Key < BatchData.timestampKeyMostRecentlyCompleted) {
                        toRemove.Add(entry.Key);
                    }
                }
                foreach (ulong key in toRemove) {
                    timestampMap.Remove(key);
                }
            }
        }

        private void DestroyCurves()
        {
            foreach (Curve curve in curves)
            {
                //if (curve.testMesh)
                //{
                   //DestroyImmediate(curve.testMesh);
                //}
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
                curve.facingDirection = facingDirection;
                print("curve.facingDirection = facingDirection: " + curve.facingDirection);
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








