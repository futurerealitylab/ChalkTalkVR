using UnityEngine;
using RockVR.Video;
using UnityEngine.UI;

namespace RockVR.Rift.Demo
{
    public class CameraSetUpCtrl : MonoBehaviour
    {
        public GameObject cameraScreen;
        public GameObject cameraSphere;
        public GameObject captureText;
        public Transform handParent;
        public Vector3 screenPosition = new Vector3(-0.3f, 0f, 0.1f);
        private bool enableCamera = false;

        private void Awake()
        {
            if (cameraScreen == null)
            {
                throw new MissingComponentException("CameraScreen not attached!");
            }
            if (cameraSphere == null)
            {
                throw new MissingComponentException("CameraSphere not attached!");
            }
            if (captureText == null)
            {
                throw new MissingComponentException("CaptureText not attached!");
            }
        }

        public void EnableCamera()
        {
            cameraScreen.SetActive(true);
            cameraSphere.SetActive(false);
            captureText.SetActive(false);
            enableCamera = true;
        }

        private void Update()
        {
            if (!enableCamera) return;
            if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.NOT_START ||
                     VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.FINISH)
            {
                captureText.SetActive(false);
            }
            else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.STARTED)
            {
                captureText.SetActive(true);
                captureText.GetComponentInChildren<Text>().text = RecordTime.SetRecordTime();
            }
            else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.STOPPED)
            {
                RecordTime.recordTime = 0;
                captureText.GetComponentInChildren<Text>().text = "Processing";
            }
        }

        public void SetCameraScreen()
        {
            cameraScreen.transform.parent = handParent;
            cameraScreen.transform.localPosition = screenPosition;
            cameraScreen.transform.localRotation = Quaternion.Euler(new Vector3(30, 0, 0));
        }
    }
}
