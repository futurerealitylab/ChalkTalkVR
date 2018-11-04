﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chalktalk;

// every frame we need to start with a "free" list,
// using entries as needed and incrementing the count for every entry we get,
// at the end of the frame, the count is reset so the entries can be reinitialized for the new frame,
// unused entries are disabled
public class SubList<T> {
    public List<T> buffer;
    public int countElementsInUse;
    public int prevCountElementsInUse;

    public SubList(int capInit = 100) {
        this.buffer = new List<T>(capInit);
        this.countElementsInUse = 0;
        this.prevCountElementsInUse = 0;
    }
}

// TODO for later if we need pools for other uses
public class EntityPool<T> {
    public SubList<T> data;
    public GameObject prefab;

    public EntityPool(GameObject prefab, int capInit = 100) {
        this.data = new SubList<T>(capInit);
        this.prefab = prefab;
    }
}

public class CTEntityPool {



    public SubList<Curve> withLinesList, withFillList, withTextList;
    public GameObject linePrefab, fillPrefab, textPrefab;

    public void Init(GameObject linePrefab, GameObject fillPrefab, GameObject textPrefab, int nLines = 100, int nFill = 100, int nText = 100) {
        this.linePrefab = linePrefab;
        this.fillPrefab = fillPrefab;
        this.textPrefab = textPrefab;

        withLinesList = new SubList<Curve>(nLines);
        withFillList = new SubList<Curve>(nFill);
        withTextList = new SubList<Curve>(nText);

        // pre-allocate
        AllocateAndInitLines(linePrefab, nLines, withLinesList.buffer);
        for (int i = 0; i < withLinesList.buffer.Count; i += 1) {
            withLinesList.buffer[i].enabled = false;
            withLinesList.buffer[i].line.enabled = false;
            //withLinesList.buffer[i].gameObject.SetActive(false);
        }

        AllocateAndInitFills(fillPrefab, nFill, withFillList.buffer);
        for (int i = 0; i < withLinesList.buffer.Count; i += 1) {
            withFillList.buffer[i].enabled = false;
        }

        AllocateAndInitText(textPrefab, nText, withTextList.buffer);
        for (int i = 0; i < withLinesList.buffer.Count; i += 1) {
            withTextList.buffer[i].enabled = false;
        }

    }


    // temporarily use old system
    public Curve GetEntity(Chalktalk.ChalktalkDrawType type, Chalktalk.Renderer renderer) {
        Curve curve = GameObject.Instantiate<Curve>(renderer.curvePrefab);
        return curve;
    }

    public static int ID = 0;
    // pre-allocate buffer for lines
    private static void AllocateAndInitLines(GameObject prefab, int count, List<Curve> list) {
        for (; count > 0; count -= 1) {
            GameObject go = GameObject.Instantiate(prefab);
            go.name = "L:" + ID; ID += 1;

            Curve c = go.GetComponent<Curve>();
            c.line = c.gameObject.AddComponent<LineRenderer>();
            c.line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            c.line.receiveShadows = false;

            c.enabled = false;
            c.line.enabled = false;

            list.Add(c);

            //go.SetActive(false);
        }
    }

    // pre-allocate buffer for fills
    private static void AllocateAndInitFills(GameObject prefab, int count, List<Curve> list) {
        for (; count > 0; count -= 1) {
            GameObject go = GameObject.Instantiate(prefab);
            go.name = "F:" + ID; ID += 1;
            Curve c = go.GetComponent<Curve>();

            c.enabled = false;

            list.Add(c);
        }
    }

    // pre-allocate buffer for text
    private static void AllocateAndInitText(GameObject prefab, int count, List<Curve> list) {
        for (; count > 0; count -= 1) {
            GameObject go = GameObject.Instantiate(prefab);
            go.name = "T" + ID; ID += 1;
            Curve c = go.GetComponent<Curve>();
            list.Add(c);
        }
    }


    // TODO not sure whether this should be done externally ... when to initialize? when called or later?
    public Curve GetCTEntity(Chalktalk.ChalktalkDrawType type) {
        Curve c = null;
        switch (type) {
            case ChalktalkDrawType.STROKE: {
                    c = this.GetCTEntityLine();
                    break;
                }
            case ChalktalkDrawType.FILL: {
                    c = this.GetCTEntityFill();
                    break;
                }
            case ChalktalkDrawType.TEXT: {
                    c = this.GetCTEntityText();
                    break;
                }
            default: {
                    c = null;
                    break;
                }

        }

        return c;
    }


    public Curve GetCTEntityLine() {
        // grow if all space used
        if (withLinesList.countElementsInUse == withLinesList.buffer.Count) {
            AllocateAndInitLines(linePrefab, 100, withLinesList.buffer);
        }

        // get the curve 
        Curve c = withLinesList.buffer[withLinesList.countElementsInUse];
        if (withLinesList.prevCountElementsInUse <= withLinesList.countElementsInUse) {
            c.enabled = true;
            c.line.enabled = true;
            //c.gameObject.SetActive(true);
        }

        // now one more element is in use this frame
        withLinesList.countElementsInUse += 1;
        return c;
    }

    public Curve GetCTEntityFill() {
        // grow if all space used
        if (withFillList.countElementsInUse == withFillList.buffer.Count) {
            AllocateAndInitFills(fillPrefab, 100, withFillList.buffer);
        }

        // get the curve 
        Curve c = withFillList.buffer[withFillList.countElementsInUse];
        if (withFillList.prevCountElementsInUse <= withFillList.countElementsInUse) {
            c.enabled = true;
            c.line.enabled = true;

        }

        // now one more element is in use this frame
        withFillList.countElementsInUse += 1;
        return c;
    }

    public Curve GetCTEntityText() {
       // Debug.LogWarning("UNFINISHED PROCEDURE");
        withTextList.countElementsInUse += 1;
        return null;
    }

    // disable un-needed entities
    public void DisableUnusedEntitiesLines() {
        SubList<Curve> list = withLinesList;
        List<Curve> buff = list.buffer;

        int bound = Mathf.Min(
            Mathf.Max(list.countElementsInUse, list.prevCountElementsInUse), 
            buff.Count
        );

        for (int i = list.countElementsInUse; i < bound;  i += 1) {
            buff[i].enabled = false;
            buff[i].line.enabled = false;
            //buff[i].gameObject.SetActive(false);
        }
        list.prevCountElementsInUse = list.countElementsInUse;
    }
    public void DisableUnusedEntitiesFill() {
        SubList<Curve> list = withFillList;
        List<Curve> buff = list.buffer;

        int bound = Mathf.Min(
            Mathf.Max(list.countElementsInUse, list.prevCountElementsInUse),
            buff.Count
        );


        for (int i = list.countElementsInUse; i < bound; i += 1) {
            buff[i].enabled = false;
        }
        list.prevCountElementsInUse = list.countElementsInUse;
    }
    public static void DisableUnusedEntitiesText() {
        //Debug.LogWarning("UNFINISHED PROCEUDRE");
    }

    // reset buffers to position 0
    public void RewindBuffers() {
        withLinesList.countElementsInUse = 0;
        withFillList.countElementsInUse  = 0;
        withTextList.countElementsInUse  = 0;
    }

    // call at the end of the frame before rendering
    public void FinalizeFrameData() {
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
