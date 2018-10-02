using UnityEngine;
using System.Collections.Generic;

namespace RockVR.Rift
{
    /// <summary>
    /// This script implemented functionality of teleport in VR scene.
    /// </summary>
    public class RIFT_Teleport : MonoBehaviour
    {
        /// <summary>
        /// Can load on flat
        /// </summary>
        public bool canLoadOnFlat = true;
        /// <summary>
        /// Can load max angle
        /// </summary>
        [Range(0, 60)]
        public float maxLoadAngle = 30f;
        /// <summary>
        /// The line childrenPosition max distance
        /// </summary>
        public float maxDistance = 5f;
        /// <summary>
        /// Linerenderer width
        /// </summary>
        public float lineTelepontWidth = 0.05f;
        /// <summary>
        /// The color of can teleport
        /// </summary>
        public Color canTeleportColor = Color.blue;
        /// <summary>
        /// The color of can't teleport
        /// </summary>
        public Color unTeleportColor = Color.red;
        /// <summary>
        /// The prefab of teleport point
        /// </summary>
        public GameObject vrTeleportSimple;
        /// <summary>
        /// The prefab of player area
        /// </summary>
        public GameObject vrPlayAreaSimple;
        /// <summary>
        /// The vive's camera
        /// </summary>
        private Transform vrCamera;
        /// <summary>
        /// The area of player
        /// </summary>
        private Transform vrPlayArea;
        /// <summary>
        /// The line of teleport
        /// </summary>
        private LineRenderer vrTeleportLine;
        /// <summary>
        /// The teleport point position
        /// </summary>
        private Vector3 teleportPoint;
        /// <summary>
        /// Whether can teleport
        /// </summary>
        private bool canTeleport = false;
        private bool teleportActive;
        private GameObject vrTeleportItem;
        private GameObject vrPlayerAreaItem;

        void Awake()
        {
            vrCamera = Camera.main.transform;
            if (vrCamera == null)
            {
                Debug.LogError("vrCamera is null");
                enabled = false;
                return;
            }
            vrPlayArea = transform.parent.parent;
            if (vrPlayArea == null)
            {
                Debug.LogError("vrplayarea is null");
                enabled = false;
                return;
            }
        }

        void Start()
        {
            InitTeleporter();
        }
        /// <summary>
        /// Init teleporter Object
        /// </summary>
        void InitTeleporter()
        {
            GameObject vrTeleportObject = new GameObject(string.Format("[{0}]VrTeleport", gameObject.name));
            vrTeleportObject.transform.localScale = vrPlayArea.localScale;
            GameObject vrTeleportLineObject = new GameObject(string.Format("[{0}]VrTeleportLin", gameObject.name));
            vrTeleportLineObject.transform.SetParent(vrTeleportObject.transform);
            vrTeleportLine = vrTeleportLineObject.AddComponent<LineRenderer>();
            vrTeleportLine.SetWidth(lineTelepontWidth * vrPlayArea.localScale.magnitude, lineTelepontWidth * vrPlayArea.localScale.magnitude);
            vrTeleportLine.SetColors(canTeleportColor, canTeleportColor);
            vrTeleportLine.material = new Material(Shader.Find("Sprites/Default"));
            vrTeleportLine.enabled = false;
            if (vrPlayAreaSimple != null)
            {
                vrPlayerAreaItem = Instantiate(vrPlayAreaSimple, Vector3.zero, Quaternion.identity) as GameObject;
                vrPlayerAreaItem.transform.SetParent(vrTeleportObject.transform);
                vrPlayerAreaItem.transform.rotation = vrPlayArea.rotation;
                vrPlayerAreaItem.SetActive(false);
            }
            if (vrTeleportSimple != null)
            {
                vrTeleportItem = Instantiate(vrTeleportSimple, Vector3.zero, Quaternion.identity) as GameObject;
                vrTeleportItem.transform.SetParent(vrTeleportObject.transform);
                vrTeleportItem.SetActive(false);
            }
        }
        /// <summary>
        /// Find the drop point
        /// </summary>
        public void SearchDropPoint()
        {
            vrTeleportLine.enabled = true;
            teleportActive = true;
            if (teleportActive && canTeleport)
            {
                if (vrTeleportItem != null)
                    vrTeleportItem.SetActive(true);
                if (vrPlayerAreaItem != null)
                    vrPlayerAreaItem.SetActive(true);
            }
        }
        /// <summary>
        /// Sure the drop point
        /// </summary>
        public void ConfirmDropPoint()
        {
            vrTeleportLine.enabled = false;
            if (vrPlayerAreaItem != null)
                vrPlayerAreaItem.SetActive(false);
            if (vrTeleportItem != null)
                vrTeleportItem.SetActive(false);
            if (teleportActive && canTeleport)
            {
                Vector3 camSpot = new Vector3(vrCamera.position.x, 0, vrCamera.position.z);
                Vector3 roomSpot = new Vector3(vrPlayArea.position.x, 0, vrPlayArea.position.z);
                Vector3 offset = roomSpot - camSpot;
                vrPlayArea.position = teleportPoint + offset;
            }
            teleportActive = false;
        }

        void Update()
        {
            if (!teleportActive)
                return;
            UpdateTeleportLine();
        }
        /// <summary>
        /// Update Line
        /// </summary>
        void UpdateTeleportLine()
        {
            List<Vector3> positions = new List<Vector3>();
            Quaternion currentRotation = transform.rotation;
            Vector3 currentPosition = transform.position;
            positions.Add(currentPosition);
            Vector3 lastPostion = transform.position - transform.forward;
            Vector3 currentDirection = transform.forward;
            Vector3 downForward = new Vector3(transform.forward.x * 0.01f, -1, transform.forward.z * 0.01f);
            RaycastHit hit = new RaycastHit();
            float allDistance = 0;
            bool isFirstArray = true;
            int i = 0;
            while (i < 500)
            {
                i++;
                Quaternion forDownRotate = Quaternion.LookRotation(downForward);
                currentRotation = Quaternion.RotateTowards(currentRotation, forDownRotate, 1f);
                Ray ray = new Ray(currentPosition, currentPosition - lastPostion);
                float rayLenght = (maxDistance * 0.05f) * vrPlayArea.localScale.magnitude;
                if (currentRotation == forDownRotate)
                {
                    isFirstArray = false;
                }
                bool hitObject = false;
                hitObject = Physics.Raycast(ray, out hit, rayLenght);
                if (hitObject)
                {
                    if (isFirstArray)
                    {
                        allDistance += (currentPosition - hit.point).magnitude;
                        positions.Add(hit.point);
                    }
                    break;
                }
                currentDirection = currentRotation * Vector3.forward;
                lastPostion = currentPosition;
                currentPosition += currentDirection * rayLenght;
                if (isFirstArray)
                {
                    allDistance += rayLenght;
                    positions.Add(currentPosition);
                }
            }
            if (isFirstArray)
            {
                teleportPoint = positions[positions.Count - 1];
            }
            canTeleport = CanTeleporter(hit);
            if (canTeleport)
            {
                vrTeleportLine.SetColors(canTeleportColor, canTeleportColor);
                if (vrPlayerAreaItem != null)
                {
                    vrPlayerAreaItem.SetActive(true);
                    Vector3 camPoint = new Vector3(vrCamera.position.x, 0, vrCamera.position.z);
                    Vector3 playAreaPoint = new Vector3(vrPlayArea.position.x, 0, vrPlayArea.position.z);
                    Vector3 offset = playAreaPoint - camPoint;
                    vrPlayerAreaItem.transform.position = (teleportPoint + offset) + hit.normal * 0.05f;
                }
            }
            else
            {
                vrTeleportLine.SetColors(unTeleportColor, unTeleportColor);
                if (vrPlayerAreaItem != null)
                {
                    vrPlayerAreaItem.SetActive(false);
                }
                if (vrTeleportItem != null)
                {
                    vrTeleportItem.SetActive(false);
                }
            }
            if (vrTeleportItem != null)
            {
                vrTeleportItem.transform.position = teleportPoint + (hit.normal * 0.05f);
                if (hit.normal == Vector3.zero)
                {
                    vrTeleportItem.transform.rotation = Quaternion.identity;
                }
                else
                {
                    vrTeleportItem.transform.rotation = Quaternion.LookRotation(hit.normal);
                }
            }
            vrTeleportLine.SetVertexCount(positions.Count);
            vrTeleportLine.SetPositions(positions.ToArray());
        }
        /// <summary>
        /// Determine whether can transfer
        /// </summary>
        /// <param name="hit">The hit ray</param>
        /// <returns>Whether can teleport</returns>
        private bool CanTeleporter(RaycastHit hit)
        {
            if (hit.transform == null) return false;
            if (canLoadOnFlat)
            {
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                if (angle > maxLoadAngle)
                    return false;
            }
            return true;
        }
    }

}
