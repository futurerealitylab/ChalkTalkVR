using UnityEngine;
using System.Collections;
using System;

namespace RockVR.Rift
{
    /// <summary>
    /// The camera follow function
    /// </summary>
    public class RIFT_FollowCamera : MonoBehaviour
    {
        /// <summary>
        /// The follow camera.
        /// </summary>
        [NonSerialized]
        public Camera followCamera;
        /// <summary>
        /// The facing target.
        /// </summary>
        public Transform target = null;
        /// <summary>
        /// The min keep distance
        /// </summary>
        public float keepDistance;
        /// <summary>
        /// The rate of smooth
        /// </summary>
        public float smooth = 0.5f;
        /// <summary>
        /// Real-time spacing
        /// </summary>
        private float distance;
        private const float CAMERA_TARGET_DISTANCE_Y = 2f;
        /// <summary>
        /// The camera original position
        /// </summary>
        private Vector3 oldDistance;

        void Start()
        {
            oldDistance = target.transform.position - transform.position;
            if (keepDistance <= 0)
                keepDistance = Vector3.Distance(this.transform.position, target.transform.position);
        }

        void FixedUpdate()
        {
            distance = Vector3.Distance(this.transform.position, target.transform.position);
        }

        private void Update()
        {
            LookAtTarget();
        }

        void LateUpdate()
        {
            if (distance > keepDistance)
            {
                transform.position =
                    Vector3.Lerp(
                    transform.position,
                    (target.transform.position - oldDistance),
                    Time.deltaTime * smooth);
            }
            if (target.transform.position.y > transform.position.y &&
                !(transform.position.y >= (target.transform.position.y + CAMERA_TARGET_DISTANCE_Y)))
            {
                transform.position =
                    Vector3.Lerp(transform.position,
                                 new Vector3(
                                     transform.position.x,
                                     target.transform.position.y + CAMERA_TARGET_DISTANCE_Y,
                                     transform.position.z),
                                 Time.deltaTime * smooth);
            }
        }

        public void OnCameraPointChange()
        {
            if (followCamera != null)
            {
                followCamera.transform.SetParent(this.transform, false);
                followCamera.transform.localPosition = Vector3.zero;
                followCamera.transform.position = transform.position;
                followCamera.transform.SetParent(this.transform, false);
                followCamera.transform.localPosition = Vector3.zero;
                followCamera.GetComponent<BoxCollider>().enabled = false;
            }
        }

        public void LookAtTarget()
        {
            if (target && followCamera != null)
            {
                followCamera.transform.LookAt(target.transform);
            }
        }
    }
}
