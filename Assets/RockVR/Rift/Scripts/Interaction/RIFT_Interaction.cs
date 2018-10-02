using System;
using UnityEngine;

namespace RockVR.Rift
{
    /// <summary>
    /// The interaction mode state
    /// </summary>
    public enum ControllerState
    {
        Ray,
        Touch,
        Normal
    }
    /// <summary>
    /// Configure interaction mode.
    /// </summary>
    public class RIFT_Interaction : MonoBehaviour
    {
        [Tooltip("Laser Thickness")]
        public float laserThickness = 0.002f;
        [Tooltip("Laser HitScale")]
        public float laserHitScale = 0.02f;
        [Tooltip("Max Hit Distance")]
        public float maxDistance = 100.0f;
        /// <summary>
        /// The ray color.
        /// </summary>
        public Color color;
        /// <summary>
        /// The controller state
        /// </summary>
        public ControllerState controllerState = ControllerState.Normal;
        /// <summary>
        /// The selected object
        /// </summary>
        [NonSerialized]
        public GameObject selectedObject;
        /// <summary>
        /// Set the ray show or not show
        /// </summary>
        public bool show = true;
        /// <summary>
        /// Ray and object encounter point
        /// </summary>
        private GameObject hitPoint;
        /// <summary>
        /// The ray hit point
        /// </summary>
        private GameObject pointer;
        /// <summary>
        /// The Rigidbody of object
        /// </summary>
        private GameObject controllerRigidbodyObject;
        /// <summary>
        /// The limit distance
        /// </summary>
        private float distanceLimit;
        private OVRInput.Controller riftController;

        private void Awake()
        {
            riftController = this.GetComponent<RIFT_EventCtrl>().riftController;
        }

        void Start()
        {
            CreatRay();
            CreateControllerRigidBody();
        }

        void Update()
        {
            SetBoxColliderActive();
            if (show)
            {
                RayInteraction();
            }
            if (controllerState == ControllerState.Ray)
            {
                pointer.gameObject.GetComponent<MeshRenderer>().enabled = show;
            }
            else
            {
                if (hitPoint.activeSelf)
                {
                    hitPoint.SetActive(false);
                }
            }
        }
        /// <summary>
        /// Creat ray and hit point
        /// </summary>
        void CreatRay()
        {
            pointer = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pointer.transform.SetParent(transform, false);
            pointer.transform.localScale = new Vector3(laserThickness, laserThickness, 100.0f);
            pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50.0f);
            pointer.SetActive(false);
            hitPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hitPoint.transform.SetParent(transform, false);
            hitPoint.transform.localScale = new Vector3(laserHitScale, laserHitScale, laserHitScale);
            hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 100.0f);
            hitPoint.SetActive(false);
            DestroyImmediate(hitPoint.GetComponent<SphereCollider>());
            DestroyImmediate(pointer.GetComponent<CapsuleCollider>());
            Material newMaterial = new Material(Shader.Find("RockVR/LaserPointer"));
            newMaterial.SetColor("_Color", color);
            pointer.GetComponent<MeshRenderer>().material = newMaterial;
            hitPoint.GetComponent<MeshRenderer>().material = newMaterial;
        }
        /// <summary>
        /// The ray interaction function
        /// </summary>
        void RayInteraction()
        {
            if (controllerState == ControllerState.Ray)
            {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hitInfo;
                bool bHit = Physics.Raycast(ray, out hitInfo);

                float distance = maxDistance;
                if (bHit)
                {
                    distance = hitInfo.distance;
                    selectedObject = hitInfo.collider.gameObject;
                }
                else
                {
                    selectedObject = null;
                }
                if (distanceLimit > 0.0f)
                {
                    distance = Mathf.Min(distance, distanceLimit);
                    bHit = true;
                }

                pointer.transform.localScale = new Vector3(laserThickness, laserThickness, distance);
                pointer.transform.localPosition = new Vector3(0.0f, 0.0f, distance * 0.5f);

                if (bHit)
                {
                    hitPoint.SetActive(true);
                    hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, distance);
                }
                else
                {
                    hitPoint.SetActive(false);
                }

                distanceLimit = -1.0f;
            }
        }
        /// <summary>
        /// Great model collider
        /// </summary>
        private void CreateControllerRigidBody()
        {
            controllerRigidbodyObject = this.gameObject;
            controllerRigidbodyObject.transform.localPosition = Vector3.zero;
            if (riftController==OVRInput.Controller.LTouch)
            {
                CreateBoxCollider(controllerRigidbodyObject, new Vector3(-0.01f, -0.03f, 0f), new Vector3(0.1f, 0.1f, 0.05f));
                CreateBoxCollider(controllerRigidbodyObject, new Vector3(-0.01f, -0.03f, -0.05f), new Vector3(0.05f, 0.05f, 0.05f));
            }
            else if (riftController == OVRInput.Controller.RTouch)
            {
                CreateBoxCollider(controllerRigidbodyObject, new Vector3(0.01f, -0.03f, 0f), new Vector3(0.1f, 0.1f, 0.05f));
                CreateBoxCollider(controllerRigidbodyObject, new Vector3(0.01f, -0.03f, -0.05f), new Vector3(0.05f, 0.05f, 0.05f));
            }
            var createRB = controllerRigidbodyObject.AddComponent<Rigidbody>();
            createRB.mass = 100f;
            var controllerRB = controllerRigidbodyObject.GetComponent<Rigidbody>();
            controllerRB.useGravity = false;
            controllerRB.isKinematic = false;
        }
        /// <summary>
        /// Create box collider
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="center">Set the boxcollider center point</param>
        /// <param name="size">The boxCollictor size</param>
        private void CreateBoxCollider(GameObject obj, Vector3 center, Vector3 size)
        {
            BoxCollider bc = obj.AddComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.size = size;
            bc.center = center;
            bc.enabled = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other != null && controllerState == ControllerState.Touch)
            {
                selectedObject = other.gameObject;
            }
        }

        void SetBoxColliderActive()
        {
            if (controllerState == ControllerState.Ray)
            {
                pointer.SetActive(true);
                foreach (var item in this.gameObject.GetComponents<BoxCollider>())
                {
                    item.enabled = false;
                }
            }
            if (controllerState == ControllerState.Touch)
            {
                pointer.SetActive(false);
                foreach (var item in this.gameObject.GetComponents<BoxCollider>())
                {
                    item.enabled = true;
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            selectedObject = null;
        }
    }
}

