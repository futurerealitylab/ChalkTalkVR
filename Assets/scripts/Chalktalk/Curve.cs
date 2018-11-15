using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;


namespace Chalktalk
{
    public enum ChalktalkDrawType {STROKE = 0, FILL = 1, TEXT = 2};



    //[RequireComponent(typeof(LineRenderer))]
    public class Curve : MonoBehaviour
    {
        public Vector3[] points;
        public Color color = Color.black;
        public float width = 0.0f;

        public Material defaultMat;
        public Material mat2;
        public LineRenderer line;

        //public Material myMat;
        public Mesh shape;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public static float CT_TEXT_SCALE_FACTOR = 0.638f * 0.855f;
        public TextMesh textMesh;
        public Vector3 textPos = Vector3.zero;
        public float textScale;
        public float facingDirection;
        public Font myfont;
        public Material fontMat;
        public string text;

        public int id = 0;
        public ChalktalkDrawType type;

        // Vectrosity related
        VectorLine vectrosityLine;
        VectorLine vText;
        public Transform forDrawTransform;

        public MaterialPropertyBlock materialPropertyBlock;

        public static Material mainMaterial;
        public static int colorPropID;

        public static IDictionary<Color, KeyValuePair<Material, Color>> colorToMaterialInfoMap = new Dictionary<Color, KeyValuePair<Material, Color>>();

        private void Awake()
        {
            if (Curve.mainMaterial == null)
            {
                Curve.mainMaterial = new Material(mat2);

            };
        }

        public void CacheShaderPropID()
        {
            colorPropID = Shader.PropertyToID("_Color");
        }

#if !BEFORE_POOL

        public void InitWithLines(Vector3[] points, Color color, float width)
        {
#if USE_VECTROSITY
            DrawVectrosityLine(points, color, width);
#else
            DrawLineRendererLine(points, color, width);
#endif
        }

        void DrawLineRendererLine(Vector3[] points, Color color, float width)
        {
            this.line.positionCount = points.Length;
            this.line.SetPositions(points);

            // do not replace the material if nothing has changed
            if (color == this.color)
            {
                return;
            }
            else
            {
                this.color = color;
            }

            KeyValuePair<Material, Color> materialInfo;
            Color matColor;
            if (colorToMaterialInfoMap.TryGetValue(color, out materialInfo))
            {
                line.sharedMaterial = materialInfo.Key;
                matColor = materialInfo.Value;

                //Debug.Log("Reusing a color");
            }
            else
            {
                matColor = new Color(Mathf.Pow(color.r, 0.45f), Mathf.Pow(color.g, 0.45f), Mathf.Pow(color.b, 0.45f));
                Material mat = new Material(defaultMat);
                mat.SetColor("_Color", matColor);
                line.sharedMaterial = mat;

                colorToMaterialInfoMap.Add(color, new KeyValuePair<Material, Color>(mat, matColor));

                //Debug.Log("Adding a color");
            }

            line.startColor = matColor;
            line.endColor = matColor;




            // NEW
            //this.materialPropertyBlock.SetColor(colorPropID, c);
            //this.line.SetPropertyBlock(this.materialPropertyBlock);

            this.line.startWidth = width;
            this.line.endWidth = width;
        }

        void DrawVectrosityLine(Vector3[] points, Color color, float width)
        {
            // TODO: VectorLine only takes List
            List<Vector3> vPoints = new List<Vector3>(points);
            if (vectrosityLine == null)
            {
                vectrosityLine = new VectorLine("haha", vPoints, width*1080.0f/3.0f, LineType.Continuous);
            }
            else
            {
                vectrosityLine.points3 = vPoints;
            }
            //vectrosityLine.material = defaultMat;
            Color c = new Color(Mathf.Pow(color.r, 0.45f), Mathf.Pow(color.g, 0.45f), Mathf.Pow(color.b, 0.45f));
            //vectrosityLine.material.color = c;
            //vectrosityLine.material.SetColor("_EmissionColor", c);

            vectrosityLine.SetColor(c);
            vectrosityLine.Draw3D();
        }


        public void InitWithFill(Vector3[] points, Color color)
        {
            Mesh shape = this.shape;
            shape.vertices = points;

            MeshRenderer mr = this.meshRenderer;
            MeshFilter filter = this.meshFilter;

            int countSides = points.Length;
            int countTris = countSides - 2;
            int[] indices = new int[countTris * 3 * 2];
            for (int i = 0, off = 0; i < countTris; ++i, off += 6) {
                indices[off] = 0;
                indices[off + 1] = i + 1;
                indices[off + 2] = i + 2;
                indices[off + 3] = 0;
                indices[off + 4] = i + 2;
                indices[off + 5] = i + 1;
            }
            shape.triangles = indices;
            Material mymat = new Material(defaultMat);
            // similar to what chalktalk do to the color TODO check if shader material is the same as what already exists (in which case, don't modify)
            Color c = new Color(Mathf.Pow(color.r, 0.45f), Mathf.Pow(color.g, 0.45f), Mathf.Pow(color.b, 0.45f));

            mymat.SetColor("_Color", c);
            mr.material = mymat;

            shape.RecalculateBounds();
            shape.RecalculateNormals();

            filter.mesh = shape;
        }

        public void InitWithText(string textStr, Vector3 textPos, float scale, float facingDirection, Color color)
        {
#if USE_VECTROSITY
            DrawVectrosityText(textStr, textPos, scale, facingDirection, color);
#else
            InitTextMeshText(textStr, textPos, scale, facingDirection, color);
#endif
        }

        void InitTextMeshText(string textStr, Vector3 textPos, float scale, float facingDirection, Color color)
        {
            gameObject.SetActive(true);

            // don't really need to save these to the object
            this.text = textStr;
            this.facingDirection = facingDirection;
            this.textPos = textPos;
            this.textScale = scale;
            this.color = color;

            this.textMesh.anchor = TextAnchor.MiddleCenter;

            // reorient to face towards you
            transform.localRotation = Quaternion.Euler(0, this.facingDirection, 0);
            //transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

            //                    textMesh.fontSize = 3;
            //textMesh.font = Resources.Load("Nevis") as Font;
            textMesh.text = text;
            //textMesh.font = myfont;
            //textMesh.font.material = fontMat;
            textMesh.fontSize = 355;
            textMesh.characterSize = 0.1f;
            textMesh.color = color;
            if (!float.IsNaN(textPos.x))
            {
                transform.localPosition = textPos;
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }


            transform.localScale = new Vector3(
            textScale * CT_TEXT_SCALE_FACTOR,
            textScale * CT_TEXT_SCALE_FACTOR, 1.0f);
        }

        void DrawVectrosityText(string textStr, Vector3 textPos, float textScale, float facingDirection, Color color)
        {
            if (vText == null)
            {
                vText = new VectorLine("3DText-" + textStr, new List<Vector3>(), 1.0f);
            }

            vText.color = color;
            //vText.drawTransform.localRotation = Quaternion.Euler(0, facingDirection, 0);

            vText.MakeText(textStr, Vector3.zero, textScale);
            forDrawTransform.localPosition = textPos;
            forDrawTransform.localRotation = Quaternion.Euler(0, facingDirection, 0);

            vText.drawTransform = forDrawTransform;


            //vText.MakeText(text, textPos, textScale);

            vText.Draw3D();
        }

#endif


            // BEFORE POOL
            public void Draw()
        {
            switch (type)
            {
                case ChalktalkDrawType.STROKE:
                    line = this.gameObject.AddComponent<LineRenderer>();
                    line.positionCount = points.Length;
                    line.SetPositions(points);
                    Color c = new Color(Mathf.Pow(color.r, 0.45f), Mathf.Pow(color.g, 0.45f), Mathf.Pow(color.b, 0.45f));
                    line.startColor = c;
                    line.endColor = c;
                    line.numCornerVertices = 20;
                    line.numCapVertices = 20;
                    line.material = defaultMat;
                    line.material.color = c;
                    line.material.SetColor("_EmissionColor", c);
                    line.startWidth = width;
                    line.endWidth = width;

                    break;
                case ChalktalkDrawType.FILL:
                    GameObject go = this.gameObject;
                    Mesh shape = go.GetComponent<Mesh>();
                    MeshRenderer mr = null;
                    MeshFilter filter = null;
                    if (shape == null)
                    {
                        shape = new Mesh();
                        mr = go.AddComponent<MeshRenderer>();
                        filter = go.AddComponent<MeshFilter>();
                    }
                    else
                    {
                        filter = go.GetComponent<MeshFilter>();
                    }
                    shape.vertices = points;

                    int countSides = points.Length;
                    int countTris = countSides - 2;
                    int[] indices = new int[countTris * 3 * 2];
                    for (int i = 0, off = 0; i < countTris; ++i, off += 6)
                    {
                        indices[off] = 0;
                        indices[off + 1] = i + 1;
                        indices[off + 2] = i + 2;
                        indices[off + 3] = 0;
                        indices[off + 4] = i + 2;
                        indices[off + 5] = i + 1;
                    }
                    shape.triangles = indices;
                    //
                    //string outputPoints = "FILLED SHAPE points:\t";
                    //foreach(Vector3 p in points)
                        //outputPoints  += p.ToString("F3") + "\t";
                    //Debug.Log(outputPoints);
                   // string outputIndices = "FILLED SHAPE indices:\t";
                   // foreach (int index in indices)
                       // outputIndices += index.ToString("F3") + "\t";
                    //Debug.Log(outputIndices);
                    Material mymat = new Material(defaultMat);
                    // similar to what chalktalk do to the color
                    c = new Color(Mathf.Pow(color.r, 0.45f), Mathf.Pow(color.g, 0.45f), Mathf.Pow(color.b, 0.45f));
                    mymat.SetColor("_EmissionColor", c);
                    mymat.color = c;
                    mr.material = mymat;
                    //mr.material.color = color;

                    //Vector3[] V = {
                    //    new Vector3(0f, 0f, 0f),
                    //    new Vector3(1f, 0f, 0f),
                    //    new Vector3(1f, 1f, 0f),
                    //    new Vector3(0f, 1f, 0f)
                    //};

                    //int[] I = {
                    //    0, 1, 2,
                    //    2, 3, 0
                    //};

                    //shape.vertices = V;
                    //shape.triangles = I;
                    shape.RecalculateBounds();
                    shape.RecalculateNormals();


                    filter.mesh = shape;

                    break;
                case ChalktalkDrawType.TEXT:


                   // goto default;

                    textMesh = gameObject.AddComponent<TextMesh>();
                    textMesh.anchor = TextAnchor.MiddleCenter;

                    // reorient to face towards you
                    transform.localRotation = Quaternion.Euler(0, facingDirection, 0);
                    //transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

//                    textMesh.fontSize = 3;
                    //textMesh.font = Resources.Load("Nevis") as Font;
                    textMesh.text = text;
                    //textMesh.font = myfont;
                    //textMesh.font.material = fontMat;
                    textMesh.fontSize = 355;
                    textMesh.characterSize = 0.1f;
                    textMesh.color = color;
                    if (!float.IsNaN(textPos.x))
                    {
                        transform.localPosition = textPos;
                    }
                    else
                    {
                        transform.localPosition = Vector3.zero;
                    }
                    

                    transform.localScale = new Vector3(
                    textScale* CT_TEXT_SCALE_FACTOR,
                    textScale *CT_TEXT_SCALE_FACTOR, 1.0f);

                    break;
                default:
                    break;
            }
        }

        public override string ToString()
        {
            string s = "ID: " + id + " COLOR: " + color + " WIDTH: " + width + " POINTS: ";
            for (int i = 0; i < points.Length; i++)
            {
                s += " { x : " + points[i].x + " y: " + points[i].y + " z: " + points[i].z + " } ";
            }
            s += " TYPE: " + this.type;
            return s;
        }
    }
}