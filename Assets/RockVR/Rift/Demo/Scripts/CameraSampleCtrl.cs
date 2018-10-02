using UnityEngine;
using RockVR.Video;

namespace RockVR.Rift.Demo
{
    enum CameraState
    {
        Normal,
        Touched,
        Picked
    }
    public class CameraSampleCtrl : MonoBehaviour
    {
        public ControllerState controllerState = ControllerState.Normal;
        public CameraSetUpCtrl cameraSetUpCtrl;
        public RIFT_FollowCamera[] followCameras;
        public GameObject oneButtonTooltip;
        public GameObject twoButtonTooltip;
        public GameObject indexTriggerTooltip;
        public GameObject hangTriggerTooltip;
        public GameObject thumbstickTooltip;
        public GameObject startButtonTooltip;
        private GameObject cameraObject;
        private CameraState cameraState = CameraState.Normal;
        private bool enableRadialMenu = false;
        private RIFT_Interaction vrIteraction;
        private RIFT_EventCtrl eventCtrl;
        private RIFT_TooltipManager tooltipController;
        private RIFT_Teleport teleport;
        protected RIFT_RadialMenu radiaMenu;

        void Awake()
        {
            vrIteraction = this.transform.GetComponent<RIFT_Interaction>();
            eventCtrl = this.GetComponent<RIFT_EventCtrl>();
            radiaMenu = this.transform.GetComponentInChildren<RIFT_RadialMenu>();
            tooltipController = this.GetComponentInChildren<RIFT_TooltipManager>();
            teleport = this.GetComponent<RIFT_Teleport>();
        }

        private void Start()
        {
            if (controllerState == ControllerState.Ray)
            {
                tooltipController.handTriggerText = "Show Ray";
                tooltipController.indexTriggerText = "Pick Camera";
                tooltipController.thumbstickText = "Swicth Camera";
                tooltipController.buttonOneText = "Start/Stop Capture";
            }
            else if (controllerState == ControllerState.Touch)
            {
                tooltipController.indexTriggerText = "Grab Camera";
                tooltipController.thumbstickText = "Teleport";
                tooltipController.buttonOneText = "Start/Stop Capture";
                hangTriggerTooltip.SetActive(false);
            }
            startButtonTooltip.SetActive(false);
            twoButtonTooltip.SetActive(false);
        }

        void OnEnable()
        {
            if (eventCtrl != null)
            {
                eventCtrl.eventDelegate.OnPressButtonPrimaryHandTrigger += OnPressButtonPrimaryHandTrigger;
                eventCtrl.eventDelegate.OnPressButtonPrimaryHandTriggerUp += OnPressButtonPrimaryHandTriggerUp;
                eventCtrl.eventDelegate.OnPressButtonOneDown += OnPressButtonOneDown;
                eventCtrl.eventDelegate.OnPressButtonPrimaryIndexTrigger += OnPressButtonPrimaryIndexTrigger;
                eventCtrl.eventDelegate.OnPressButtonPrimaryIndexTriggerUp += OnPressButtonPrimaryIndexTriggerUp;
                eventCtrl.eventDelegate.OnTouchPrimaryThumbstick += OnTouchPrimaryThumbstick;
                eventCtrl.eventDelegate.OnTouchPrimaryThumbstickUp += OnTouchPrimaryThumbstickUp;
                eventCtrl.eventDelegate.OnPressPrimaryThumbstick += OnPressPrimaryThumbstick;
                eventCtrl.eventDelegate.OnPressPrimaryThumbstickDown += OnPressPrimaryThumbstickDown;
                eventCtrl.eventDelegate.OnPressPrimaryThumbstickUp += OnPressPrimaryThumbstickUp;
            }
        }

        void OnDisable()
        {
            if (eventCtrl != null)
            {
                eventCtrl.eventDelegate.OnPressButtonPrimaryHandTrigger -= OnPressButtonPrimaryHandTrigger;
                eventCtrl.eventDelegate.OnPressButtonPrimaryHandTriggerUp -= OnPressButtonPrimaryHandTriggerUp;
                eventCtrl.eventDelegate.OnPressButtonOneDown -= OnPressButtonOneDown;
                eventCtrl.eventDelegate.OnPressButtonPrimaryIndexTrigger -= OnPressButtonPrimaryIndexTrigger;
                eventCtrl.eventDelegate.OnPressButtonPrimaryIndexTriggerUp -= OnPressButtonPrimaryIndexTriggerUp;
                eventCtrl.eventDelegate.OnTouchPrimaryThumbstick -= OnTouchPrimaryThumbstick;
                eventCtrl.eventDelegate.OnTouchPrimaryThumbstickUp -= OnTouchPrimaryThumbstickUp;
                eventCtrl.eventDelegate.OnPressPrimaryThumbstick -= OnPressPrimaryThumbstick;
                eventCtrl.eventDelegate.OnPressPrimaryThumbstickDown -= OnPressPrimaryThumbstickDown;
                eventCtrl.eventDelegate.OnPressPrimaryThumbstickUp -= OnPressPrimaryThumbstickUp;
            }
        }

        private void OnPressButtonPrimaryHandTriggerUp()
        {
            if (controllerState == ControllerState.Ray)
            {
                vrIteraction.show = false;
                hangTriggerTooltip.SetActive(false);
            }
        }

        private void OnPressButtonPrimaryHandTrigger()
        {
            if (controllerState == ControllerState.Ray)
            {
                vrIteraction.show = true;
            }
        }

        private void OnPressPrimaryThumbstick()
        {
            if (controllerState == ControllerState.Touch)
            {
                if (teleport != null)
                {
                    teleport.SearchDropPoint();
                }
                thumbstickTooltip.SetActive(false);
            }
        }

        private void OnPressPrimaryThumbstickDown()
        {
            if (controllerState == ControllerState.Ray)
            {
                if (!radiaMenu) return;
                if (eventCtrl.axisAngle != 0)
                {
                    radiaMenu.InteractButton(eventCtrl.axisAngle, ButtonEvent.click);
                }
            }
        }

        private void OnTouchPrimaryThumbstickUp()
        {
            if (controllerState == ControllerState.Ray)
            {
                radiaMenu.StopTouching();
                radiaMenu.DisableMenu(false);
            }
            else if (controllerState == ControllerState.Touch)
            {
                if (teleport != null)
                {
                    teleport.ConfirmDropPoint();
                }
            }
        }

        private void OnTouchPrimaryThumbstick()
        {
            if (controllerState == ControllerState.Ray)
            {
                if (cameraState == CameraState.Picked && enableRadialMenu)
                {
                    radiaMenu.EnableMenu();
                    if (eventCtrl.axisAngle != 0)
                    {
                        radiaMenu.InteractButton(eventCtrl.axisAngle, ButtonEvent.hoverOn);
                    }
                    thumbstickTooltip.SetActive(false);
                }
            }
        }

        private void OnPressPrimaryThumbstickUp()
        {
            if (controllerState == ControllerState.Ray)
            {
                if (eventCtrl.axisAngle != 0)
                {
                    radiaMenu.InteractButton(eventCtrl.axisAngle, ButtonEvent.unclick);
                }
                enableRadialMenu = false;
            }
        }

        private void OnPressButtonOneDown()
        {
            if (cameraState == CameraState.Picked || controllerState == ControllerState.Touch)
            {
                if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.NOT_START ||
                    VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.FINISH)
                {
                    VideoCaptureCtrl.instance.StartCapture();
                    oneButtonTooltip.SetActive(false);
                }
                else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.STARTED)
                {
                    VideoCaptureCtrl.instance.StopCapture();
                }
                else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.STOPPED)
                {
                    return;
                }
            }
        }

        private void OnPressButtonPrimaryIndexTrigger()
        {
            if (vrIteraction.selectedObject != null)
            {
                if (vrIteraction.selectedObject.GetComponent<CameraSetUpCtrl>() != null)
                {
                    cameraSetUpCtrl.EnableCamera();
                    if (controllerState == ControllerState.Ray)
                    {
                        foreach (var followCamera in followCameras)
                        {
                            followCamera.followCamera = cameraSetUpCtrl.GetComponent<Camera>();
                        }
                        followCameras[1].OnCameraPointChange();
                        cameraSetUpCtrl.SetCameraScreen();
                        cameraState = CameraState.Picked;
                        enableRadialMenu = true;
                    }
                    else if (controllerState == ControllerState.Touch)
                    {
                        cameraObject = vrIteraction.selectedObject;
                        cameraObject.transform.parent = this.transform;
                        cameraState = CameraState.Touched;
                    }
                    indexTriggerTooltip.SetActive(false);
                }
            }
        }

        private void OnPressButtonPrimaryIndexTriggerUp()
        {
            if (controllerState == ControllerState.Touch)
            {
                if (cameraObject != null)
                {
                    cameraObject.transform.parent = null;
                }
            }
        }
    }
}