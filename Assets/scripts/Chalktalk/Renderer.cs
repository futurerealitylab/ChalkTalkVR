//#define DEBUG_PRINT
//#define FAIL_FAST
//#define BEFORE_POOL


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FRL.Network;
using FRL.Utility;
using UnityEngine.UI;
using UnityEditor;

namespace Chalktalk {

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

        public DisplayObj(string l) {
            label = l;
            bytes = new byte[0];
        }
    }

    public class Renderer : MonoBehaviour {
        
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

        //public List<string> trackedLabels = new List<string>();

        // entities are lines, fills, text, ...
        public CTEntityPool entityPool;




        private void Awake() {
            this.entityPool = new CTEntityPool();
            this.entityPool.Init(curvePrefab.gameObject, curvePrefab.gameObject, curvePrefab.gameObject, 32, 16, 16);
        }

        void Start() {
            displayObj = new DisplayObj(label);
            //trackedLabels.Add(label);
            // To karl: create or assign label to DisplayObj when you need to. And then retrieve bytes through public members.
        }


        public class BatchData {
            public int byteLength = 0;
            public ulong slicesArrivedBitset = 0;
            public byte[][] slices = null;
            public ulong timestampKey = 0;

            public static int timestampMostRecentlyCompleted = 0;
            public static int timestampOverflowMostRecentlyCompleted = 0;
            public static ulong timestampKeyMostRecentlyCompleted = 0;

            public BatchData() { }
            public BatchData(int byteLength, int sliceCount, ulong timestampKey) {
                this.byteLength = byteLength;
                this.slices = new byte[sliceCount][];
                this.timestampKey = timestampKey;

                this.InitSliceBitset();
            }


            public void InitSliceBitset() {
                // 1 == has not arrived, 0 == arrived, when all arrived, should be a value of 0
                this.slicesArrivedBitset = (0xFFFFFFFFFFFFFFFF >> (64 - this.slices.Length));
                //Debug.Log(this.ToString());
            }

            public void MarkSliceAsArrived(int idx) {
                this.slicesArrivedBitset ^= (uint)(1 << idx);

                //Debug.Log("MARK " + Convert.ToString((long)this.slicesArrivedBitset, 2).PadLeft(64, '0'));
            }

            public bool AllSlicesArrived() {
                return this.slicesArrivedBitset == 0;
            }

            public override string ToString() {
                return "{KEY: " + timestampKey + ", ARRIVED: " + Convert.ToString((long)this.slicesArrivedBitset, 2).PadLeft(64, '0') + ", SLICE COUNT: " + slices.Length + "}";
            }
        }


        public int sliceCount = 1;
        public byte[] mergedBytes = null;

        public Dictionary<ulong, BatchData> timestampMap = new Dictionary<ulong, BatchData>();

        public BatchData completeBatchData = new BatchData(); // sentinel data to avoid need for null checks


        private int MergeBytes(BatchData data) {
            if (data.slices.Length == 1) {
                mergedBytes = data.slices[0];
                return mergedBytes.Length;
            }

            mergedBytes = new byte[data.byteLength];

            int ptr = 0;
            {
                byte[][] slices = data.slices;
                int length = slices.Length;
                Debug.Log(length);
                for (int i = 0; i < length; ++i) {
                    byte[] byteSlice = slices[i];

                    Buffer.BlockCopy(byteSlice, 0, mergedBytes, ptr, byteSlice.Length);
                    ptr += byteSlice.Length;
                }
            }

            return ptr;
        }

        public void DiscardOldEntriesFromMap() {
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


        public bool PollForData(int labelCount) {
            bool readyToDraw = false;

            for (int i = 0; i < labelCount; ++i) {
                // try to update tracking on label (i + 1)
                if (!displayObj.UpdateTracking("Display" + (i + 1))) {
#if DEBUG_PRINT
                    Debug.Log("Display" + (i + 1) + " did not arrive yet: frame: " + Time.frameCount);
#endif 
                    continue;
                }

                //Debug.Log(displayObj.batchTimestamp);


                // ignore data belonging to old batches
                if ((ulong)displayObj.batchTimestamp < BatchData.timestampKeyMostRecentlyCompleted) {
#if DEBUG_PRINT
                    Debug.Log("OUTDATED DATA ARRIVED: frame: " + Time.frameCount);
#endif
                    continue;
                }

                // retrieve or create batch data
                BatchData batch;
                // if the timestamp has not been registered, create new batch data
                if (!timestampMap.TryGetValue((ulong)displayObj.batchTimestamp, out batch)) {
                    batch = new BatchData(
                        byteLength : displayObj.batchByteLength,
                        sliceCount : displayObj.sliceCount,
                        timestampKey : (ulong)displayObj.batchTimestamp
                    );
                    timestampMap[(ulong)displayObj.batchTimestamp] = batch;
                }

                // store byte data as a slice
                batch.slices[i] = displayObj.bytes;

                batch.MarkSliceAsArrived(i);

                // if slices arrived matches the expected number of slices for this batch,
                // mark as complete and for drawing
                if (batch.AllSlicesArrived() &&
                    batch.timestampKey > completeBatchData.timestampKey) {
                    completeBatchData = batch;
                    BatchData.timestampKeyMostRecentlyCompleted = batch.timestampKey;
                    readyToDraw = true;
                }
            }

            return readyToDraw;
        }

        protected void Update() {
            int labelCount = XRNetworkClient.LabelCount();

            bool readyToDraw = PollForData(labelCount);
            if (readyToDraw) {
               
                // merge all bytes into one flat array (possibly unnecessary)
                int endPtr = MergeBytes(completeBatchData);
                if (completeBatchData.byteLength != endPtr) {
                    Debug.LogWarningFormat("batch byte length " + completeBatchData.byteLength + " does not equal total copied byte length " + endPtr);
#if FAIL_FAST
                    EditorApplication.isPlaying = false;
#endif
                    return;
                }

                DiscardOldEntriesFromMap();

                DataViewer = mergedBytes;

#if BEFORE_POOL
                DestroyCurves();
#endif

                ctParser.Parse(mergedBytes, this);

#if BEFORE_POOL
                Draw();
#endif
                // this will disable all unused entities
                entityPool.FinalizeFrameData();


            }
        }

        private void DestroyCurves() {
            for (int i = 0; i < curves.Count; i += 1) {
                DestroyImmediate(curves[i].gameObject);
            }
            curves.Clear();
        }


        private void Parse(byte[] bytes) {
            ChalkTalkObj ctObj = ctParser.Parse(bytes, this);
            return;
        }


        private void Draw() {
            for (int i = 0; i < curves.Count; i += 1) {
                curves[i].Draw();
            }
        }

        private bool boldenFrame(List<Vector3> points) {
            if (points.Count == 5) {
                for (int i = 0; i < points.Count; i++) {
                    if ((points[i].z < 1.0f) && (points[i].z > -1.0f)) {
                        break;
                    }
                }
                return true;
            }

            return false;
        }

    }
}








