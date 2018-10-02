using UnityEngine;
using UnityEngine.UI;

namespace RockVR.Rift
{
    /// <summary>
    /// Set to a dynamic circle arc.
    /// </summary>
    [ExecuteInEditMode]
    public class RIFT_GraphicCircle : Graphic
    {
        /// <summary>
        /// The number of fill percent
        /// </summary>
        [Range(0, 100)]
        public int fillPercent;
        /// <summary>
        /// Whether to fill
        /// </summary>
        public bool fill = true;
        /// <summary>
        /// The number of thickness
        /// </summary>
        public int thickness = 5;
        /// <summary>
        /// The number of segments
        /// </summary>
        [Range(0, 360)]
        public int segments = 360;
        /// <summary>
        /// The circle texture
        /// </summary>
        [SerializeField]
        private Texture circleTexture;

        public override Texture mainTexture
        {
            get
            {
                return (circleTexture == null ? s_WhiteTexture : circleTexture);
            }
        }
        // Texture to be used.
        public Texture texture
        {
            get
            {
                return circleTexture;
            }
            set
            {
                if (circleTexture == value)
                {
                    return;
                }
                circleTexture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }
        /// <summary>
        /// Draw the mesh
        /// </summary>
        /// <param name="toFill">The number of fill</param>
        [System.Obsolete("Use OnPopulateMesh(VertexHelper vh) instead.")]
        protected override void OnPopulateMesh(Mesh toFill)
        {
            float outer = -rectTransform.pivot.x * rectTransform.rect.width;
            float inner = -rectTransform.pivot.x * rectTransform.rect.width + thickness;
            toFill.Clear();
            var vbo = new VertexHelper(toFill);
            Vector2 prevX = Vector2.zero;
            Vector2 prevY = Vector2.zero;
            Vector2 uv0 = new Vector2(0, 0);
            Vector2 uv1 = new Vector2(0, 1);
            Vector2 uv2 = new Vector2(1, 1);
            Vector2 uv3 = new Vector2(1, 0);
            Vector2 pos0;
            Vector2 pos1;
            Vector2 pos2;
            Vector2 pos3;
            float f = (fillPercent / 100f);
            float degrees = 360f / segments;
            int fa = (int)((segments + 1) * f);
            for (int i = -1 - (fa / 2); i < fa / 2 + 1; i++)
            {
                float rad = Mathf.Deg2Rad * (i * degrees);
                float c = Mathf.Cos(rad);
                float s = Mathf.Sin(rad);
                uv0 = new Vector2(0, 1);
                uv1 = new Vector2(1, 1);
                uv2 = new Vector2(1, 0);
                uv3 = new Vector2(0, 0);
                pos0 = prevX;
                pos1 = new Vector2(outer * c, outer * s);
                if (fill)
                {
                    pos2 = Vector2.zero;
                    pos3 = Vector2.zero;
                }
                else
                {
                    pos2 = new Vector2(inner * c, inner * s);
                    pos3 = prevY;
                }
                prevX = pos1;
                prevY = pos2;
                vbo.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }));
            }
            if (vbo.currentVertCount > 3)
            {
                vbo.FillMesh(toFill);
            }
        }

        private void Update()
        {
            thickness = (int)Mathf.Clamp(thickness, 0, rectTransform.rect.width / 2);
        }
    }
}
