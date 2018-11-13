//#define USE_TEST_MESH_FOR_TEXT

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

        // Vectrosity related
        public Transform forDrawTransform;

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

        public Vector3 ApplyCurveTransformation(Vector3 p, Renderer renderer)
        {
            if(renderer.mySettings != null)
            {
                Vector3 positionInCanvasSpace = renderer.mySettings.transform.worldToLocalMatrix.MultiplyPoint3x4(p);
                p = renderer.mySettings.CanvasToCurvedCanvas(positionInCanvasSpace);
                //transform.rotation = Quaternion.LookRotation(renderer.mySettings.CanvasToCurvedCanvasNormal(transform.parent.localPosition), transform.parent.up);
            }
            return p;
        }

        public void ParseStroke(byte[] bytes, ref ChalkTalkObj ctobj, Renderer renderer)
        {
            // data byte cursor (skip the 8-byte header)
            int cursor = 8;

            // The total number of words in this packet, then get the size of the bytes size
            int curveCount = Utility.ParsetoInt16(bytes, cursor);

            //Debug.Log("CURVE COUNT: " + curveCount);
            cursor += 2;

            for (; cursor < bytes.Length;)
            {
                //The length of the current line
                int length = Utility.ParsetoInt16(bytes, cursor);
                cursor += 2;

                // if the line data is less than 12, we skip this one curve
                // TODO: implement the new curve module so that we can keep the curve with the same id (???)
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
                // new version, use real float instead of fake float
                Vector3 translation = Utility.ParsetoRealVector3(bytes, cursor, 1);
                cursor += 6*2;
                //Debug.Log("translation:" + translation);
                Quaternion rotation = Utility.ParsetoRealQuaternion(bytes, cursor, 1);
                cursor += 6*2;
                //Debug.Log("rotation:" + rotation);
                float scale = Utility.ParsetoRealFloat(bytes, cursor);
                cursor += 2*2;
                //Debug.Log("scale:" + scale);

                //Debug.Log("header transformation:" + translation.ToString("F3") +"\t"+ scale.ToString("F3"));
                //Parse the type of the stroke
                int type = Utility.ParsetoInt16(bytes, cursor);
                cursor += 2;
                //Debug.Log("CT type:" + type);

                //Parse the width of the line
                float width = 0;

                //Debug.Log("Current Line's points count: " + (length - 12) / 4);
                //Debug.Log("Current Cursor before read the points :" + cursor);

                //Debug.Log("LENGTH: " + length);

#if BEFORE_POOL
                // handle text
                if ((ChalktalkDrawType)type == ChalktalkDrawType.TEXT) {
                    //cursor += (length-12)*2;    // text display with format, header (12 bytes) + n * 2 * 8-bit-char
                    string textStr = "";
                    for(int j = 0; j < (length-12); j++)
                    {
                        int curInt = Utility.ParsetoInt16(bytes, cursor);
                        int res1 = curInt >> 8;
                        int res2 = curInt - (res1 << 8);
                        textStr += ((char)res1).ToString() + ((char)res2).ToString();
                        cursor += 2;
                    }
                    if (textStr.Length >= 0) {
                        Curve curve = GameObject.Instantiate<Curve>(renderer.curvePrefab);
                        //curve.transform.SetParent(renderer.transform);
                        curve.transform.SetParent(renderer.curvedParent);
                        curve.type = (ChalktalkDrawType)type;
                        curve.text = textStr;
                        renderer.curves.Add(curve);
                        // TODO
                        curve.facingDirection = renderer.facingDirection;
                        //Debug.Log("curve.facingDirection = renderer.facingDirection: " + curve.facingDirection);
                        // translation.y = (-1 + (2 * translation.y)) * (1080.0f / 1920.0f);

                        translation = Vector3.Scale(translation, renderer.bindingBox.transform.localScale);

                        //translation.y -= renderer.bindingBox.transform.position.y;
                        //translation.y *= 1080.0f / 1920.0f;
                        //translation.y += renderer.bindingBox.transform.position.y;



                        //translation.y -= 0.638f / 2;
                        //translation = Vector3.Scale(translation, new Vector3(scale*0.638f,scale*0.638f,1));
                        //translation = Vector3.Scale(translation, new Vector3(scale*0.698f,scale*0.698f,1));


                        translation = renderer.bindingBox.transform.rotation * translation + renderer.bindingBox.transform.position;
                        //translation = ApplyCurveTransformation(translation, renderer);
                        //Debug.Log("translation in text:" + translation);
                        curve.textPos = translation; //+ new Vector3(0.0f, -TEMP_TEX_Y_OFF, 0.0f);
                        curve.textScale = scale;

                        curve.color = color;


                        //Debug.Log(translation);
#if USE_TEST_MESH_FOR_TEXT
                        curve.testMesh = GameObject.CreatePrimitive(PrimitiveType.Quad);
                        curve.transform.SetParent(renderer.curvedParent);
                        curve.testMesh.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        //curve.testMesh.transform.localPosition = new Vector3(0.0f, 0.0f, translation.z);
                        curve.testMesh.transform.localPosition = translation;

                        curve.testMesh.transform.localScale = // new Vector3((Mathf.Sin(Time.time) + 1.0f) / 2.0f, (Mathf.Sin(Time.time) + 1.0f) / 2.0f, (Mathf.Sin(Time.time) + 1.0f) / 2.0f);
                            new Vector3(scale, scale, scale);

                        curve.testMesh.transform.Translate(new Vector3(0.0f, -TEMP_TEX_Y_OFF, 0.0f));
#endif

                        //
                    }
                } else {

                    Vector3[] points = new Vector3[(length - 12) / 4];

                    // handle stroke or fill
                    for (int j = 0; j < (length - 12) / 4; j++) {
                        Vector3 point = Utility.ParsetoRealVector3(bytes, cursor, 1);
                        point = Vector3.Scale(point, renderer.bindingBox.transform.localScale);
                        //Move point to the bindingBox Coordinate
                        //Debug.Log("bf:" + point);
                        point = renderer.bindingBox.transform.rotation * point + renderer.bindingBox.transform.position;
                        //Debug.Log("mid:" + point);
                        point = ApplyCurveTransformation(point, renderer);
                        
                        //Debug.Log("af:" + point);
                        //Apply the point transform for each point
                        points[j] = point;
                        //points.Add((rotation * point + translation) * scale);
                        cursor += 6*2;
                        width = Utility.ParsetoRealFloat(bytes, cursor);
                        cursor += 2*2;
                    }

                    // bold the framework
                    bool isFrame = boldenFrame(points);
                    // width *= (isFrame) ? 20.0f : 1.0f;


                    Curve curve = GameObject.Instantiate<Curve>(renderer.curvePrefab);

                    curve.points = points;
                    curve.width = width * 3;
                    curve.color = isFrame ? new Color(1, 1, 1, 1) : color;
                    //curve.color = color;
                    // zhenyi: not using the chalktalk color
                    //curve.color = new Color(1, 1, 1, 1);
                    curve.type = (ChalktalkDrawType)type;
                    renderer.curves.Add(curve);
                }
#else

                CTEntityPool pool = renderer.entityPool;
                ChalktalkDrawType ctType = (ChalktalkDrawType)type;
                // parse text
                if (ctType == ChalktalkDrawType.TEXT)
                {
                    string textStr = "";
                    for (int j = 0; j < (length - 12); j++)
                    {
                        int curInt = Utility.ParsetoInt16(bytes, cursor);
                        int res1 = curInt >> 8;
                        int res2 = curInt - (res1 << 8);
                        textStr += ((char)res1).ToString() + ((char)res2).ToString();
                        cursor += 2;
                    }
                    if (textStr.Length >= 0)
                    {
                        Curve curve = pool.GetCTEntityText();


                        //curve.transform.SetParent(renderer.transform);
                        //curve.transform.SetParent(renderer.curvedParent);
                        //curve.type = (ChalktalkDrawType)type;
                        //curve.text = textStr;
                        // TODO
                        //curve.facingDirection = renderer.facingDirection;
                        //Debug.Log("curve.facingDirection = renderer.facingDirection: " + curve.facingDirection);
                        // translation.y = (-1 + (2 * translation.y)) * (1080.0f / 1920.0f);

                        translation = Vector3.Scale(translation, renderer.bindingBox.transform.localScale);

                        //translation.y -= renderer.bindingBox.transform.position.y;
                        //translation.y *= 1080.0f / 1920.0f;
                        //translation.y += renderer.bindingBox.transform.position.y;



                        //translation.y -= 0.638f / 2;
                        //translation = Vector3.Scale(translation, new Vector3(scale*0.638f,scale*0.638f,1));
                        //translation = Vector3.Scale(translation, new Vector3(scale*0.698f,scale*0.698f,1));


                        translation = renderer.bindingBox.transform.rotation * translation + renderer.bindingBox.transform.position;
                        //translation = ApplyCurveTransformation(translation, renderer);
                        //Debug.Log("translation in text:" + translation);
                        //curve.textPos = translation; //+ new Vector3(0.0f, -TEMP_TEX_Y_OFF, 0.0f);
                        //curve.textScale = scale;

                        //curve.color = color;

                        Debug.Log(textStr);
                        // Vectrosity related
                        curve.forDrawTransform = forDrawTransform;
                        curve.InitWithText(textStr, translation, scale, renderer.facingDirection, color);
#if CT_DEBUG
                        renderer.curves.Add(curve);
#endif
                    }
                    continue;
                }
                // otherwise parse stroke or fill

                int pointCount = (length - 12) / 4;
                Vector3[] points = new Vector3[pointCount];
                for (int j = 0; j < pointCount; j += 1)
                {
                    Vector3 point = Utility.ParsetoRealVector3(bytes, cursor, 1);
                    point = Vector3.Scale(point, renderer.bindingBox.transform.localScale);
                    //Move point to the bindingBox Coordinate
                    //Debug.Log("bf:" + point);
                    point = renderer.bindingBox.transform.rotation * point + renderer.bindingBox.transform.position;
                    //Debug.Log("mid:" + point);
                    //point = ApplyCurveTransformation(point, renderer);

                    //Debug.Log("af:" + point);
                    //Apply the point transform for each point
                    points[j] = point;
                    //points.Add((rotation * point + translation) * scale);
                    cursor += 6 * 2;
                    width = Utility.ParsetoRealFloat(bytes, cursor);
                    cursor += 2 * 2;
                }

                // bold the framework
                //bool isFrame = boldenFrame(points);


                switch ((ChalktalkDrawType)type)
                {
                    case ChalktalkDrawType.STROKE:
                        {
                            Curve curve = pool.GetCTEntityLine();
                            curve.InitWithLines(points, /*isFrame ? new Color(1, 1, 1, 1) : */ color, width * 3);
#if CT_DEBUG
                            renderer.curves.Add(curve);
#endif
                            break;
                        }
                    case ChalktalkDrawType.FILL:
                        {
                            Curve curve = pool.GetCTEntityFill();
                            curve.InitWithFill(points, /*isFrame ? new Color(1, 1, 1, 1) : */ color);
#if CT_DEBUG
                            renderer.curves.Add(curve);
#endif
                            break;
                        }
                    default: 
                        {
                            break;
                        }
                }

#endif
                            }
        }

        public void ParseProcedureAnimation(byte[] bytes, ref ChalkTalkObj ctobj, Renderer renderer)
        {

        }

        private bool boldenFrame(Vector3[] points)
        {
            if (points.Length == 5)
            {
                for (int i = 0; i < points.Length; i++)
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