using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RockVR.Rift
{
    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="isActive">The state of whether the UI Pointer is currently active or not.</param>
    /// <param name="currentTarget">The current UI element that the pointer is colliding with.</param>
    /// <param name="previousTarget">The previous UI element that the pointer was colliding with.</param>
    public struct RIFT_PointerEventArgs
    {
        public bool isActive;
        public GameObject currentTarget;
        public GameObject previousTarget;
    }

    public delegate void UIPointerEventHandler(object sender, RIFT_PointerEventArgs e);
    /// <summary>
    /// The UI Pointer provides a mechanism for interacting with Unity UI elements on a world canvas. 
    /// The UI Pointer can be attached to any game object the same way in which a Base Pointer can be 
    /// and the UI Pointer also requires a controller to initiate the pointer activation and pointer click states.
    /// </summary>
    public class RIFT_Pointer : MonoBehaviour
    {
        /// <summary>
        /// Methods of activation.
        /// </summary>
        /// <param name="HoldButton">Only activates the UI Pointer when the Pointer button on the controller is pressed and held down.</param>
        /// <param name="ToggleButton">Activates the UI Pointer on the first click of the Pointer button on the controller and it stays active until the Pointer button is clicked again.</param>
        /// <param name="AlwaysOn">The UI Pointer is always active regardless of whether the Pointer button on the controller is pressed or not.</param>
        public enum ActivationMethods
        {
            HoldButton,
            ToggleButton,
            AlwaysOn
        }

        public ActivationMethods activationMode = ActivationMethods.HoldButton;
        [HideInInspector]
        public PointerEventData pointerEventData;
        [HideInInspector]
        public GameObject hoveringElement;
        /// <summary>
        /// Emitted when the UI Pointer is colliding with a valid UI element.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementEnter;
        /// <summary>
        /// Emitted when the UI Pointer is no longer colliding with any valid UI elements.
        /// </summary>
        public event UIPointerEventHandler UIPointerElementExit;
        private bool pointerClicked = false;
        private bool beamEnabledState = false;
        private bool lastPointerPressState = false;

        public virtual void OnUIPointerElementEnter(RIFT_PointerEventArgs e)
        {
            if (UIPointerElementEnter != null)
            {
                UIPointerElementEnter(this, e);
            }
        }

        public virtual void OnUIPointerElementExit(RIFT_PointerEventArgs e)
        {
            if (UIPointerElementExit != null)
            {
                UIPointerElementExit(this, e);

                if (!e.isActive && e.previousTarget)
                {
                    pointerEventData.pointerPress = e.previousTarget;
                }
            }
        }

        public virtual bool ButtonClick()
        {
            return false;
        }

        public RIFT_PointerEventArgs SetUIPointerEvent(GameObject currentTarget, GameObject lastTarget = null)
        {
            RIFT_PointerEventArgs e;
            e.isActive = PointerActive();
            e.currentTarget = currentTarget;
            e.previousTarget = lastTarget;
            return e;
        }
        /// <summary>
        /// The SetEventSystem method is used to set up the global Unity event system for the UI pointer. 
        /// It also handles disabling the existing Standalone Input Module that exists on the EventSystem 
        /// and adds a custom VRTK Event System VR Input component that is required for interacting with the UI with VR inputs.
        /// </summary>
        /// <param name="eventSystem">The global Unity event system to be used by the UI pointers.</param>
        public RIFT_UIEventInputModule SetEventSystem(EventSystem eventSystem)
        {
            if (!eventSystem)
            {
                Debug.LogError("A VRUIPoint requires an EventSystem");
                return null;
            }
            var standaloneInputModule = eventSystem.gameObject.GetComponent<StandaloneInputModule>();
            if (standaloneInputModule.enabled)
            {
                standaloneInputModule.enabled = false;
            }
            var eventSystemInput = eventSystem.GetComponent<RIFT_UIEventInputModule>();
            if (!eventSystemInput)
            {
                eventSystemInput = eventSystem.gameObject.AddComponent<RIFT_UIEventInputModule>();
                eventSystemInput.Initialise();
            }

            return eventSystemInput;
        }

        public void SetWorldCanvas(Canvas canvas)
        {
            var defaultRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
            var customRaycaster = canvas.gameObject.GetComponent<RIFT_GraphicRaycaster>();
            if (!customRaycaster)
            {
                customRaycaster = canvas.gameObject.AddComponent<RIFT_GraphicRaycaster>();
            }
            if (defaultRaycaster && defaultRaycaster.enabled)
            {
                customRaycaster.ignoreReversedGraphics = defaultRaycaster.ignoreReversedGraphics;
                customRaycaster.blockingObjects = defaultRaycaster.blockingObjects;
                defaultRaycaster.enabled = false;
            }
            var canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
            if (!canvas.gameObject.GetComponent<BoxCollider>())
            {
                var canvasBoxCollider = canvas.gameObject.AddComponent<BoxCollider>();
                canvasBoxCollider.size = new Vector3(canvasSize.x, canvasSize.y, 0.01f);
                canvasBoxCollider.center = new Vector3(0f, 0f, -0.005f);
            }
            if (!canvas.gameObject.GetComponent<Image>())
            {
                canvas.gameObject.AddComponent<Image>().color = Color.clear;
            }
        }

        public bool PointerActive()
        {
            if (activationMode == ActivationMethods.AlwaysOn)
            {
                return true;
            }
            else if (activationMode == ActivationMethods.HoldButton)
            {
                return true;
            }
            else
            {
                pointerClicked = false;
                if (ButtonClick() && !lastPointerPressState)
                {
                    pointerClicked = true;
                }
                lastPointerPressState = ButtonClick();
                if (pointerClicked)
                {
                    beamEnabledState = !beamEnabledState;
                }
                return beamEnabledState;
            }
        }


        public virtual void Awake()
        {
            ConfigureEventSystem();
            ConfigureWorldCanvases();
            pointerClicked = false;
            lastPointerPressState = false;
            beamEnabledState = false;
        }
        /// <summary>
        /// Configure eventsystem property
        /// </summary>
        private void ConfigureEventSystem()
        {
            var eventSystem = FindObjectOfType<EventSystem>();
            var eventSystemInput = SetEventSystem(eventSystem);
            pointerEventData = new PointerEventData(eventSystem);
            eventSystemInput.pointers.Add(this);
        }

        private void ConfigureWorldCanvases()
        {
            foreach (var canvas in FindObjectsOfType<Canvas>())
            {
                SetWorldCanvas(canvas);
            }
        }
    }
}