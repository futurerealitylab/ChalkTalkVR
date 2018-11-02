using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace RockVR.Rift
{
    /// <summary>
    /// Setup VR tooltip attribute.
    /// </summary>
    public class RIFT_Tooltip : MonoBehaviour
    {
        [Tooltip("The text that is displayed on the tooltip.")]
        public string displayText;
        [Tooltip("The size of the text that is displayed.")]
        public int fontSize = 14;
        [Tooltip("The size of the tooltip container where `x = width` and `y = height`.")]
        public Vector2 containerSize = new Vector2(100f, 30f);
        [Tooltip("An optional transform of where to start drawing the line from. If one is not provided the centre of the tooltip is used for the initial line position.")]
        public Transform drawLineFrom;
        [Tooltip("The width of the line drawn between the tooltip and the destination transform.")]
        public float lineWidth = 0.001f;
        /// <summary>
        /// The colour to use for the text on the tooltip
        /// </summary>
        [NonSerialized]
        public Color fontColor = Color.black;
        /// <summary>
        /// The colour to use for the background container of the tooltip
        /// </summary>
        [NonSerialized]
        public Color containerColor = Color.black;
        /// <summary>
        /// The colour to use for the line drawn between the tooltip and the destination transform.
        /// </summary>
        [NonSerialized]
        public Color lineColor = Color.black;
        /// <summary>
        /// A transform of another object in the scene that a line will be drawn from the tooltip to, 
        /// this helps denote what the tooltip is in relation to. If no transform is provided and the tooltip is a child of another object, 
        /// then the parent object's transform will be used as this destination position.
        /// </summary>
        [NonSerialized]
        public Transform drawLineTo;
        private RectTransform rectTransform;
        private LineRenderer line;
        private Text tmpText;
        private GameObject tmpContainer;

        private void Awake()
        {
            if (rectTransform == null)
            {
                rectTransform = transform.Find("TooltipCanvas").GetComponent<RectTransform>();
            }
            if (tmpContainer == null)
            {
                tmpContainer = transform.Find("TooltipCanvas/UIContainer").gameObject;
            }
            if (tmpText == null)
            {
                tmpText = transform.Find("TooltipCanvas/UITextFront").GetComponent<Text>();
            }
            if (line == null)
            {
                line = transform.Find("Line").GetComponent<LineRenderer>();
            }
        }
        /// <summary>
        /// The Reset method resets the tooltip back to its initial state
        /// </summary>
        public void Reset()
        {
            SetText();
            SetLine();
        }

        public void Init()
        {
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = containerSize;
            }
            if (tmpContainer != null)
            {
                tmpContainer.GetComponent<RectTransform>().sizeDelta = containerSize;
                tmpContainer.GetComponent<Image>().color = containerColor;
            }
            if (tmpText != null)
            {
                Material newMaterial1 = new Material(Shader.Find("RockVR/UIOverlay"));
                tmpText.material = newMaterial1;
                tmpText.color = fontColor;
                tmpText.fontSize = fontSize;
            }
            if (line != null)
            {
                Material newMaterial2 = new Material(Shader.Find("RockVR/LaserPointer"));
                line.material = newMaterial2;
                line.material.color = lineColor;
                line.SetColors(lineColor, lineColor);
                line.SetWidth(lineWidth, lineWidth);
            }
        }

        private void SetText()
        {
            if (tmpText)
            {
                tmpText.text = displayText.Replace("\\n", "\n");
            }
        }

        private void SetLine()
        {
            if (drawLineFrom == null)
            {
                drawLineFrom = transform;
            }
        }

        private void DrawLine()
        {
            if (drawLineTo)
            {
                line.SetPosition(0, drawLineFrom.position);
                line.SetPosition(1, drawLineTo.position);
            }
        }

        private void Update()
        {
            DrawLine();
            if (rectTransform.GetComponent<BoxCollider>() != null)
            {
                rectTransform.GetComponent<BoxCollider>().isTrigger = true;
            }
        }
    }
}
