using UnityEngine;
using System;

namespace RockVR.Rift
{
    public class RIFT_EventCtrl : MonoBehaviour
    {
        public OVRInput.Controller riftController;
        public RIFT_EventDelegate eventDelegate = new RIFT_EventDelegate();
        /// <summary>
        /// The rift's thumbstick device axis
        /// </summary>
        [NonSerialized]
        public Vector2 deviceAxis;
        /// <summary>
        /// The rift's thumbstick axis angle
        /// </summary>
        [NonSerialized]
        public float axisAngle;
        void FixedUpdate()
        {
            this.transform.localPosition = OVRInput.GetLocalControllerPosition(riftController);
            this.transform.localRotation = OVRInput.GetLocalControllerRotation(riftController);
        }

        // Update is called once per frame
        void Update()
        {
            if (OVRInput.Get(OVRInput.Button.One, riftController))
            {
                if (eventDelegate.OnPressButtonOne != null)
                {
                    eventDelegate.OnPressButtonOne();
                }
            }
            if (OVRInput.GetDown(OVRInput.Button.One, riftController))
            {
                if (eventDelegate.OnPressButtonOneDown != null)
                {
                    eventDelegate.OnPressButtonOneDown();
                }
            }
            if (OVRInput.GetUp(OVRInput.Button.One, riftController))
            {
                if (eventDelegate.OnPressButtonOneUp != null)
                {
                    eventDelegate.OnPressButtonOneUp();
                }
            }
            if (OVRInput.Get(OVRInput.Button.Two, riftController))
            {
                if (eventDelegate.OnPressButtonTwo != null)
                {
                    eventDelegate.OnPressButtonTwo();
                }
            }
            if (OVRInput.GetDown(OVRInput.Button.Two, riftController))
            {
                if (eventDelegate.OnPressButtonTwoDown != null)
                {
                    eventDelegate.OnPressButtonTwoDown();
                }
            }
            if (OVRInput.GetUp(OVRInput.Button.Two, riftController))
            {
                if (eventDelegate.OnPressButtonTwoUp != null)
                {
                    eventDelegate.OnPressButtonTwoUp();
                }
            }
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick, riftController))
            {
                if (eventDelegate.OnPressPrimaryThumbstick != null)
                {
                    eventDelegate.OnPressPrimaryThumbstick();
                }
            }
            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, riftController))
            {
                if (eventDelegate.OnPressPrimaryThumbstickDown != null)
                {
                    eventDelegate.OnPressPrimaryThumbstickDown();
                }
            }
            if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, riftController))
            {
                if (eventDelegate.OnPressPrimaryThumbstickUp != null)
                {
                    eventDelegate.OnPressPrimaryThumbstickUp();
                }
            }
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, riftController))
            {
                if (eventDelegate.OnPressButtonPrimaryHandTrigger != null)
                {
                    eventDelegate.OnPressButtonPrimaryHandTrigger();
                }
            }
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, riftController))
            {
                if (eventDelegate.OnPressButtonPrimaryHandTriggerDown != null)
                {
                    eventDelegate.OnPressButtonPrimaryHandTriggerDown();
                }
            }
            if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, riftController))
            {
                if (eventDelegate.OnPressButtonPrimaryHandTriggerUp != null)
                {
                    eventDelegate.OnPressButtonPrimaryHandTriggerUp();
                }
            }
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, riftController))
            {
                if (eventDelegate.OnPressButtonPrimaryIndexTrigger != null)
                {
                    eventDelegate.OnPressButtonPrimaryIndexTrigger();
                }
            }
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, riftController))
            {
                if (eventDelegate.OnPressButtonPrimaryIndexTriggerDown != null)
                {
                    eventDelegate.OnPressButtonPrimaryIndexTriggerDown();
                }
            }
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, riftController))
            {
                if (eventDelegate.OnPressButtonPrimaryIndexTriggerUp != null)
                {
                    eventDelegate.OnPressButtonPrimaryIndexTriggerUp();
                }
            }
            if (OVRInput.Get(OVRInput.Button.Start, riftController))
            {
                if (eventDelegate.OnPressButtonStart != null)
                {
                    eventDelegate.OnPressButtonStart();
                }
            }
            if (OVRInput.GetDown(OVRInput.Button.Start, riftController))
            {
                if (eventDelegate.OnPressButtonStartDown != null)
                {
                    eventDelegate.OnPressButtonStartDown();
                }
            }
            if (OVRInput.GetUp(OVRInput.Button.Start, riftController))
            {
                if (eventDelegate.OnPressButtonStartUp != null)
                {
                    eventDelegate.OnPressButtonStartUp();
                }
            }
            if (OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, riftController))
            {
                if (eventDelegate.OnTouchPrimaryThumbRest != null)
                {
                    eventDelegate.OnTouchPrimaryThumbRest();
                }
            }
            if (OVRInput.GetDown(OVRInput.Touch.PrimaryThumbRest, riftController))
            {
                if (eventDelegate.OnTouchPrimaryThumbRestDown != null)
                {
                    eventDelegate.OnTouchPrimaryThumbRestDown();
                }
            }
            if (OVRInput.GetUp(OVRInput.Touch.PrimaryThumbRest, riftController))
            {
                if (eventDelegate.OnTouchPrimaryThumbRestUp != null)
                {
                    eventDelegate.OnTouchPrimaryThumbRestUp();
                }
            }
            if (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, riftController))
            {
                if (eventDelegate.OnTouchPrimaryThumbstick != null)
                {
                    eventDelegate.OnTouchPrimaryThumbstick();
                }
            }
            if (OVRInput.GetDown(OVRInput.Touch.PrimaryThumbstick, riftController))
            {
                if (eventDelegate.OnTouchPrimaryThumbstickDown != null)
                {
                    eventDelegate.OnTouchPrimaryThumbstickDown();
                }
            }
            if (OVRInput.GetUp(OVRInput.Touch.PrimaryThumbstick, riftController))
            {
                if (eventDelegate.OnTouchPrimaryThumbstickUp != null)
                {
                    eventDelegate.OnTouchPrimaryThumbstickUp();
                }
            }
            deviceAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, riftController);
        }

        private void LateUpdate()
        {
            axisAngle = ChangeTouchpadAxisAngle(deviceAxis);
        }

        /// <summary>
        /// Judgment of sliding angle method
        /// </summary>
        /// <param name="axis"></param>
        /// <returns>The number of angle</returns>
        private float ChangeTouchpadAxisAngle(Vector2 axis)
        {
            float angle = Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg;
            angle = 90.0f - angle;
            if (angle < 0)
            {
                angle += 360.0f;
            }
            return 360 - angle;
        }
    }
}