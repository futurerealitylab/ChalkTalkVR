using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalkTalkPosScaleChange : MonoBehaviour {
    public Transform childTrans;
    public Transform selfTrans;
    public void setupNewScale(float s)
    {
        if (s > 0)
        {
            Vector3 newScale = new Vector3(s, s, s);
            selfTrans.localScale = newScale;
        }
        
    }

    public void setupNewPos (float p)
    {
        Vector3 newPos = childTrans.localPosition;
        newPos.y = p;
        childTrans.localPosition = newPos;
    }
}
