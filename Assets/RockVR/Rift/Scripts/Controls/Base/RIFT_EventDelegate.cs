namespace RockVR.Rift
{
    /// <summary>
    /// The Rift button event delegate class
    /// </summary>
    public class RIFT_EventDelegate
    {
        public delegate void PressButtonOne();
        public delegate void PressButtonOneDown();
        public delegate void PressButtonOneUp();
        public delegate void PressButtonTwo();
        public delegate void PressButtonTwoDown();
        public delegate void PressButtonTwoUp();
        public delegate void PressPrimaryThumbstick();
        public delegate void PressPrimaryThumbstickDown();
        public delegate void PressPrimaryThumbstickUp();
        public delegate void PressButtonStart();
        public delegate void PressButtonStartDown();
        public delegate void PressButtonStartUp();
        public delegate void PressButtonPrimaryIndexTrigger();
        public delegate void PressButtonPrimaryIndexTriggerDown();
        public delegate void PressButtonPrimaryIndexTriggerUp();
        public delegate void PressButtonPrimaryHandTrigger();
        public delegate void PressButtonPrimaryHandTriggerDown();
        public delegate void PressButtonPrimaryHandTriggerUp();
        public delegate void TouchPrimaryThumbRest();
        public delegate void TouchPrimaryThumbRestDown();
        public delegate void TouchPrimaryThumbRestUp();
        public delegate void TouchPrimaryThumbstick();
        public delegate void TouchPrimaryThumbstickDown();
        public delegate void TouchPrimaryThumbstickUp();
        public delegate void PressReserved();
        public delegate void Axis2DPrimaryThumbstick();
        /// <summary>
        ///  The triggering event when the one button is pressed
        /// </summary>
        public PressButtonOne OnPressButtonOne;
        /// <summary>
        /// The triggering event when the one button is pressed down
        /// </summary>
        public PressButtonOneDown OnPressButtonOneDown;
        /// <summary>
        /// The triggering event when the one button is pressed up
        /// </summary>
        public PressButtonOneUp OnPressButtonOneUp;
        /// <summary>
        ///  The triggering event when the two button is pressed
        /// </summary>
        public PressButtonTwo OnPressButtonTwo;
        /// <summary>
        /// The triggering event when the two button is pressed down
        /// </summary>
        public PressButtonTwoDown OnPressButtonTwoDown;
        /// <summary>
        /// The triggering event when the two button is pressed up
        /// </summary>
        public PressButtonTwoUp OnPressButtonTwoUp;
        /// <summary>
        /// The triggering event when the primary thumbstick button is pressed
        /// </summary>
        public PressPrimaryThumbstick OnPressPrimaryThumbstick;
        /// <summary>
        /// The triggering event when the primary thumbstick button is pressed down
        /// </summary>
        public PressPrimaryThumbstickDown OnPressPrimaryThumbstickDown;
        /// <summary>
        /// The triggering event when the primary thumbstick button is pressed up
        /// </summary>
        public PressPrimaryThumbstickUp OnPressPrimaryThumbstickUp;
        /// <summary>
        /// The triggering event when the start button is pressed
        /// </summary>
        public PressButtonStart OnPressButtonStart;
        /// <summary>
        /// The triggering event when the start button is pressed down
        /// </summary>
        public PressButtonStartDown OnPressButtonStartDown;
        /// <summary>
        /// The triggering event when the start button is pressed up
        /// </summary>
        public PressButtonStartUp OnPressButtonStartUp;
        /// <summary>
        /// The triggering event when the primary index trigger is pressed
        /// </summary>
        public PressButtonPrimaryIndexTrigger OnPressButtonPrimaryIndexTrigger;
        /// <summary>
        /// The triggering event when the primary index trigger is pressed down
        /// </summary>
        public PressButtonPrimaryIndexTriggerDown OnPressButtonPrimaryIndexTriggerDown;
        /// <summary>
        /// The triggering event when the primary index trigger is pressed up
        /// </summary>
        public PressButtonPrimaryIndexTriggerUp OnPressButtonPrimaryIndexTriggerUp;
        /// <summary>
        /// The triggering event when the primary hand trigger is pressed
        /// </summary>
        public PressButtonPrimaryHandTrigger OnPressButtonPrimaryHandTrigger;
        /// <summary>
        /// The triggering event when the primary hand trigger is pressed down
        /// </summary>
        public PressButtonPrimaryIndexTriggerDown OnPressButtonPrimaryHandTriggerDown;
        /// <summary>
        /// The triggering event when the primary hand trigger is pressed up
        /// </summary>
        public PressButtonPrimaryIndexTriggerUp OnPressButtonPrimaryHandTriggerUp;
        /// <summary>
        /// The triggering event when the primary thumb rest button is touched
        /// </summary>
        public TouchPrimaryThumbRest OnTouchPrimaryThumbRest;
        /// <summary>
        /// The triggering event when the primary thumb rest button is touched up
        /// </summary>
        public TouchPrimaryThumbRestUp OnTouchPrimaryThumbRestUp;
        /// <summary>
        /// The triggering event when the primary thumb rest button is touched down
        /// </summary>
        public TouchPrimaryThumbRestDown OnTouchPrimaryThumbRestDown;
        /// <summary>
        /// The triggering event when the primary thumbstick button is touched
        /// </summary>
        public TouchPrimaryThumbstick OnTouchPrimaryThumbstick;
        /// <summary>
        /// The triggering event when the primary thumbstick button is touched up
        /// </summary>
        public TouchPrimaryThumbstickUp OnTouchPrimaryThumbstickUp;
        /// <summary>
        /// The triggering event when the primary thumbstick button is touched down
        /// </summary>
        public TouchPrimaryThumbstickDown OnTouchPrimaryThumbstickDown;
        /// <summary>
        /// The triggering event when the reserved button is pressed
        /// </summary>
        public PressReserved OnPressReserved;
        /// <summary>
        /// The triggering event when the primary thumbstick is turned
        /// </summary>
        public Axis2DPrimaryThumbstick OnAxis2DPrimaryThumbstick;
    }
}