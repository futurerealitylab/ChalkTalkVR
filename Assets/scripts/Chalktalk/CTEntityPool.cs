using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chalktalk;


public static class CTEntityPool {

    // every frame we need to start with a "free" list,
    // using entries as needed and incrementing the count for every entry we get,
    // at the end of the frame, the count is reset so the entries can be reinitialized for the new frame,
    // unused entries are disabled
    public struct SubList<T> {
        public List<T> buffer;
        public int countElementsInUse;

        public SubList(int capInit) {
            this.buffer = new List<T>(capInit);
            this.countElementsInUse = 0;
        }
    }

    public static SubList<Curve> withLinesList, withFillList, withTextList;
    public static GameObject linePrefab, fillPrefab, textPrefab;

    public static void Init(GameObject linePrefab, GameObject fillPrefab, GameObject textPrefab, int nLines = 100, int nFill = 100, int nText = 100) {
        CTEntityPool.linePrefab = linePrefab;
        CTEntityPool.fillPrefab = fillPrefab;
        CTEntityPool.textPrefab = textPrefab;

        withLinesList = new SubList<Curve>(nLines);
        withFillList = new SubList<Curve>(nFill);
        withTextList = new SubList<Curve>(nText);

        // pre-allocate
        AllocateCTEntitiesLine(linePrefab, withLinesList.buffer.Capacity, withLinesList.buffer);
        AllocateCTEntitiesFill(fillPrefab, withFillList.buffer.Capacity, withFillList.buffer);
        AllocateCTEntitiesText(textPrefab, withTextList.buffer.Capacity, withTextList.buffer);
    }

    // temporarily use old system
    public static Curve GetEntity(Chalktalk.ChalktalkDrawType type, Chalktalk.Renderer renderer) {
        Curve curve = GameObject.Instantiate<Curve>(renderer.curvePrefab);
        return curve;
    }


    // pre-allocate buffer for lines
    private static void AllocateCTEntitiesLine(GameObject prefab, int count, List<Curve> list) {
        for (; count > 0; count -= 1) {
            GameObject go = GameObject.Instantiate(prefab);

            Curve c = go.GetComponent<Curve>();
            c.line = c.gameObject.AddComponent<LineRenderer>();

            list.Add(c);
        }
    }

    // pre-allocate buffer for fills
    private static void AllocateCTEntitiesFill(GameObject prefab, int count, List<Curve> list) {
        for (; count > 0; count -= 1) {
            GameObject go = GameObject.Instantiate(prefab);
            Curve c = go.GetComponent<Curve>();
            list.Add(c);
        }
    }

    // pre-allocate buffer for text
    private static void AllocateCTEntitiesText(GameObject prefab, int count, List<Curve> list) {
        for (; count > 0; count -= 1) {
            GameObject go = GameObject.Instantiate(prefab);
            Curve c = go.GetComponent<Curve>();
            list.Add(c);
        }
    }


    // TODO not sure whether this should be done externally ... when to initialize? when called or later?
    public static Curve GetCTEntity(Chalktalk.ChalktalkDrawType type) {
        Curve c = null;
        switch (type) {
            case ChalktalkDrawType.STROKE: {
                    c = GetCTEntityLine();
                    break;
                }
            case ChalktalkDrawType.FILL: {
                    c = GetCTEntityFill();
                    break;
                }
            case ChalktalkDrawType.TEXT: {
                    c = GetCTEntityText();
                    break;
                }
            default: {
                    c = null;
                    break;
                }

        }

        return c;
    }


    public static Curve GetCTEntityLine() {
        // grow if all space used
        if (withLinesList.countElementsInUse == withLinesList.buffer.Capacity) {
            AllocateCTEntitiesLine(linePrefab, 100, withLinesList.buffer);
        }

        // get the curve 
        Curve c = withLinesList.buffer[withLinesList.countElementsInUse];

        // now one more element is in use this frame
        withLinesList.countElementsInUse += 1;
        return c;
    }

    public static Curve GetCTEntityFill() {
        // grow if all space used
        if (withFillList.countElementsInUse == withFillList.buffer.Capacity) {
            AllocateCTEntitiesFill(fillPrefab, 100, withFillList.buffer);
        }

        // get the curve 
        Curve c = withFillList.buffer[withFillList.countElementsInUse];

        // now one more element is in use this frame
        withFillList.countElementsInUse += 1;
        return c;
    }

    public static Curve GetCTEntityText() {
        Debug.LogWarning("UNFINISHED PROCEDURE");
        withTextList.countElementsInUse += 1;
        return null;
    }

    // disable un-needed entities
    public static void DisableUnusedEntitiesLines() {
        List<Curve> buff = withLinesList.buffer;
        for (int i = withLinesList.countElementsInUse; i < buff.Capacity; i += 1) {
            buff[i].enabled = false;
        }
    }
    public static void DisableUnusedEntitiesFill() {
        List<Curve> buff = withFillList.buffer;
        for (int i = withLinesList.countElementsInUse; i < buff.Capacity; i += 1) {
            buff[i].enabled = false;
        }
    }
    public static void DisableUnusedEntitiesText() {
        Debug.LogWarning("UNFINISHED PROCEUDRE");
    }

    // reset buffers to position 0
    public static void RewindBuffers() {
        withLinesList.countElementsInUse = 0;
        withFillList.countElementsInUse  = 0;
        withTextList.countElementsInUse  = 0;
    }

    // call at the end of the frame before rendering
    public static void FinalizeFrameData() {
        DisableUnusedEntitiesLines();
        DisableUnusedEntitiesFill();
        DisableUnusedEntitiesText();
        RewindBuffers();
    }


    //public static void AddEntityWithStroke(List<GameObject> entities) {

    //    while (lineRenderers.Count < c.Count) {
    //        lineRenderers.Add(UnityEngine.Object.Instantiate(lineRendererPrefab));
    //    }
    //    while (lineRenderers.Count > c.Count) {
    //        LineRenderer rend = lineRenderers[lineRenderers.Count - 1];
    //        lineRenderers.RemoveAt(lineRenderers.Count - 1);
    //        UnityEngine.Object.Destroy(rend.gameObject);
    //    }
    //    for (int k = 0; k < c.Count; k++) {
    //        List<Vector3> curve = c[k];
    //        lineRenderers[k].positionCount = curve.Count;

    //        Vector3[] arr = curve.ToArray();
    //        for (int i = 0; i < curve.Count; i++) {
    //            arr[i] = transform.TransformPoint(arr[i]);
    //        }
    //        lineRenderers[k].SetPositions(arr);
    //    }
    //}
}
