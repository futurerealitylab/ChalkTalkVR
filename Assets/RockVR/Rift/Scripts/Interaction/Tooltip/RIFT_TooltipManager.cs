using UnityEngine;
using System.Collections;

namespace RockVR.Rift
{
    /// <summary>
    /// Manage all tooltips.
    /// </summary>
    public class RIFT_TooltipManager : MonoBehaviour
    {
        [Tooltip("The text to display for the button one action.")]
        public string buttonOneText;
        [Tooltip("The text to display for the button two action.")]
        public string buttonTwoText;
        [Tooltip("The text to display for the thumbstick action.")]
        public string thumbstickText;
        [Tooltip("The text to display for the application menu button action.")]
        public string buttonStartText;
        [Tooltip("The text to display for the index trigger button action.")]
        public string indexTriggerText;
        [Tooltip("The text to display for the hand trigger menu button action.")]
        public string handTriggerText;
        [Tooltip("The colour to use for the tooltip background container.")]
        public Color tipBackgroundColor = Color.black;
        [Tooltip("The colour to use for the text within the tooltip.")]
        public Color tipTextColor = Color.white;
        [Tooltip("The colour to use for the line between the tooltip and the relevant controller button.")]
        public Color tipLineColor = Color.black;
        /// <summary>
        /// Judge the button one init.
        /// </summary>
        private bool buttonOneInit = false;
        /// <summary>
        /// Judge the button two init.
        /// </summary>
        private bool buttonTwoInit = false;
        /// <summary>
        /// Judge the thumbstick init.
        /// </summary>
        private bool thumbstickInit = false;
        /// <summary>
        /// Judge the applicationMenu button init.
        /// </summary>
        private bool buttonStartInit = false;
        /// <summary>
        /// Judge the index trigger button init.
        /// </summary>
        private bool indexTriggerInit = false;
        /// <summary>
        /// Judge the hand trigger button init.
        /// </summary>
        private bool handTriggerInit = false;
        /// <summary>
        /// The rift's model button transform.
        /// </summary>
        private Transform riftButtonModelTransform;

        private void Start()
        {
            buttonOneInit = false;
            buttonTwoInit = false;
            thumbstickInit = false;
            buttonStartInit = false;
            indexTriggerInit = false;
            handTriggerInit = false;
        }
        /// <summary>
        /// Init all tooltips
        /// </summary>
        void InitTips()
        {
            foreach (var tooltip in GetComponentsInChildren<RIFT_Tooltip>())
            {
                var tipText = "";
                Transform tipTransform = null;
                switch (tooltip.name.Replace("Tooltip", "").ToLower())
                {
                    case "buttonone":
                        tipText = buttonOneText;
                        tipTransform = GetTransform("b_button01");
                        if (tipTransform != null)
                        {
                            buttonOneInit = true;
                        }
                        break;
                    case "buttontwo":
                        tipText = buttonTwoText;
                        tipTransform = GetTransform("b_button02");
                        if (tipTransform != null)
                        {
                            buttonTwoInit = true;
                        }
                        break;
                    case "thumbstick":
                        tipText = thumbstickText;
                        tipTransform = GetTransform("b_stick");
                        if (tipTransform != null)
                        {
                            thumbstickInit = true;
                        }
                        break;
                    case "appmenu":
                        tipText = buttonStartText;
                        tipTransform = GetTransform("b_button03");
                        if (tipTransform != null)
                        {
                            buttonStartInit = true;
                        }
                        break;
                    case "indextrigger":
                        tipText = indexTriggerText;
                        tipTransform = GetTransform("b_trigger");
                        if (tipTransform != null)
                        {
                            indexTriggerInit = true;
                        }
                        break;
                    case "handtrigger":
                        tipText = handTriggerText;
                        tipTransform = GetTransform("b_hold");
                        if (tipTransform != null)
                        {
                            handTriggerInit = true;
                        }
                        break;
                }
                tooltip.containerColor = tipBackgroundColor;
                tooltip.fontColor = tipTextColor;
                tooltip.lineColor = tipLineColor;
                tooltip.Init();
                tooltip.displayText = tipText;
                tooltip.drawLineTo = tipTransform;
                tooltip.Reset();
            }
        }

        private void Update()
        {
            //Whether the initialization is successful. If it is not successful, re-initialize
            if (InitializeSuccess()) InitTips();
        }
        /// <summary>
        /// Whether initialize successfully
        /// </summary>
        private bool InitializeSuccess()
        {
            return !(buttonOneInit && buttonTwoInit && thumbstickInit && buttonStartInit && indexTriggerInit && handTriggerInit);
        }
        /// <summary>
        /// Searching corresponding rift trackobject
        /// </summary>
        /// <param name="findTransform"></param>
        /// <returns>The search object</returns>
        private Transform GetTransform(string findTransform)
        {
            if (transform.parent.name== "LeftHandAnchor")
            {
                riftButtonModelTransform = transform.parent.Find("LeftControllerPf/left_touch_controller_model_skel/lctrl:left_touch_controller_world");
                return riftButtonModelTransform.Find("lctrl:" + findTransform);
            }
            else if (transform.parent.name == "RightHandAnchor")
            {
                riftButtonModelTransform = transform.parent.Find("RightControllerPf/right_touch_controller_model_skel/rctrl:right_touch_controller_world");
                return riftButtonModelTransform.Find("rctrl:" + findTransform);
            }
            return transform;
        }
    }
}