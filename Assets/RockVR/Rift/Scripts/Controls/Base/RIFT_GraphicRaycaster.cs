using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RockVR.Rift
{
    /// <summary>
    /// Inherited from GraphicRaycaster, used to detect collision of UGUI object for laser input interaction. 
    /// Please read https://docs.unity3d.com/Manual/script-GraphicRaycaster.html for reference.
    /// </summary>
    public class RIFT_GraphicRaycaster : GraphicRaycaster
    {
        public GameObject raycastSource;
        /// <summary>
        /// Create Vive Graphic struct
        /// </summary>
        private struct RIFT_Graphic
        {
            public Graphic graphic;
            /// <summary>
            /// Distance to the hit.
            /// </summary>
            public float distance;
            /// <summary>
            /// The world position of the where the raycast has hit.
            /// </summary>
            public Vector3 position;
            /// <summary>
            /// The ui canvas position from which the raycast was generated.
            /// </summary>
            public Vector2 pointerPosition;
        }

        private Canvas uiCanvas;
        /// <summary>
        /// Last pointer position.
        /// </summary>
        private Vector2 lastPointerPosition;
        private const float UI_CONTROL_OFFSET = 0.00001f;
        [NonSerialized]
        private List<RIFT_Graphic> vrRaycastResults = new List<RIFT_Graphic>();
        [NonSerialized]
        // Use a static to prevent list reallocation. We only need one of these globally (single main thread), and only to hold temporary data
        private static readonly List<RIFT_Graphic> sortedGraphics = new List<RIFT_Graphic>();

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (UICanvas == null)
            {
                return;
            }
            vrRaycastResults.Clear();
            var ray = new Ray(eventData.pointerCurrentRaycast.worldPosition, eventData.pointerCurrentRaycast.worldNormal);
            Raycast(UICanvas, eventCamera, ray, vrRaycastResults);
            SetNearestRaycast(ref eventData, resultAppendList);
        }

        private void SetNearestRaycast(ref PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            RaycastResult? nearestRaycast = null;
            for (var index = 0; index < vrRaycastResults.Count; index++)
            {
                RaycastResult castResult = new RaycastResult();
                castResult.gameObject = vrRaycastResults[index].graphic.gameObject;
                castResult.module = this;
                castResult.distance = vrRaycastResults[index].distance;
                castResult.screenPosition = vrRaycastResults[index].pointerPosition;
                castResult.worldPosition = vrRaycastResults[index].position;
                castResult.index = resultAppendList.Count;
                castResult.depth = vrRaycastResults[index].graphic.depth;
                castResult.sortingLayer = UICanvas.sortingLayerID;
                castResult.sortingOrder = UICanvas.sortingOrder;
                if (!nearestRaycast.HasValue || castResult.distance < nearestRaycast.Value.distance)
                {
                    nearestRaycast = castResult;
                }
                resultAppendList.Add(castResult);
            }
            if (nearestRaycast.HasValue)
            {
                eventData.position = nearestRaycast.Value.screenPosition;
                eventData.delta = eventData.position - lastPointerPosition;
                lastPointerPosition = eventData.position;
                eventData.pointerCurrentRaycast = nearestRaycast.Value;
            }
        }

        private float GetHitDistance(Ray ray)
        {
            var hitDistance = float.MaxValue;

            if (UICanvas.renderMode != RenderMode.ScreenSpaceOverlay && blockingObjects != BlockingObjects.None)
            {
                var maxDistance = Vector3.Distance(ray.origin, UICanvas.transform.position);
                if (blockingObjects == BlockingObjects.ThreeD || blockingObjects == BlockingObjects.All)
                {
                    RaycastHit hit;
                    Physics.Raycast(ray, out hit, maxDistance);
                    if (hit.collider)
                    {
                        hitDistance = hit.distance;
                    }
                }
                if (blockingObjects == BlockingObjects.TwoD || blockingObjects == BlockingObjects.All)
                {
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxDistance);

                    if (hit.collider != null)
                    {
                        hitDistance = hit.fraction * maxDistance;
                    }
                }
            }
            return hitDistance;
        }

        private void Raycast(Canvas canvas, Camera eventCamera, Ray ray, List<RIFT_Graphic> results)
        {
            var hitDistance = GetHitDistance(ray);
            var canvasGraphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
            for (int i = 0; i < canvasGraphics.Count; ++i)
            {
                var graphic = canvasGraphics[i];
                if (graphic.depth == -1 || !graphic.raycastTarget)
                {
                    continue;
                }
                var graphicTransform = graphic.transform;
                Vector3 graphicFormward = graphicTransform.forward;
                float distance = (Vector3.Dot(graphicFormward, graphicTransform.position - ray.origin) / Vector3.Dot(graphicFormward, ray.direction));
                if (distance < 0)
                {
                    continue;
                }
                if ((distance - UI_CONTROL_OFFSET) > hitDistance)
                {
                    continue;
                }
                Vector3 position = ray.GetPoint(distance);
                Vector2 pointerPosition = eventCamera.WorldToScreenPoint(position);
                if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera))
                {
                    continue;
                }
                if (graphic.Raycast(pointerPosition, eventCamera))
                {
                    var vrGraphic = new RIFT_Graphic();
                    vrGraphic.graphic = graphic;
                    vrGraphic.position = position;
                    vrGraphic.distance = distance;
                    vrGraphic.pointerPosition = pointerPosition;
                    sortedGraphics.Add(vrGraphic);
                }
            }
            sortedGraphics.Sort((g1, g2) => g2.graphic.depth.CompareTo(g1.graphic.depth));
            for (int i = 0; i < sortedGraphics.Count; ++i)
            {
                results.Add(sortedGraphics[i]);
            }

            sortedGraphics.Clear();
        }

        private Canvas UICanvas
        {
            get
            {
                if (uiCanvas != null)
                {
                    return uiCanvas;
                }
                uiCanvas = gameObject.GetComponent<Canvas>();
                return uiCanvas;
            }
        }
    }
}