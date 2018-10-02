using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RockVR.Rift
{
    /// <summary>
    /// This adds a UI element into the world space that can be dropped into a Controller object and used to create and use Radial Menus from the touchpad.
    /// </summary>
    [ExecuteInEditMode]
    public class RIFT_RadialMenu : MonoBehaviour
    {
        public List<RadialMenuButton> positionButton;
        /// <summary>
        /// Menu option key
        /// </summary>
        public GameObject positionPrefab;
        /// <summary>
        /// Menu picture is rotated
        /// </summary>
        public bool rotateIcons;
        public bool shown;
        /// <summary>
        /// Whether or not to hide
        /// </summary>
        public bool hideOnRelease;
        /// <summary>
        /// Do not click the display menu
        /// </summary>
        public bool executeOnUnclick;
        /// <summary>
        /// RadialMenu Color
        /// </summary>
        public Color buttonColor = Color.white;
        [Range(0, 359)]
        [Tooltip("Adjust the size of the rotation offset")]
        public float offsetRotation;
        [Range(0f, 1f)]
        [Tooltip("Thickness of ring menu")]
        public float buttonThickness = 0.5f;
        public List<GameObject> menuButtons;
        private int buttonIndex = -1;
        private int currentTouch = -1;

        private void Awake()
        {
            if (Application.isPlaying)
            {
                if (!shown)
                {
                    transform.localScale = Vector3.zero;
                }
                if (menuButtons.Count == 0)
                {
                    RegenerateButtons();
                }
            }
        }

        void Update()
        {
            if (currentTouch != -1)
            {
                positionButton[currentTouch].OnHold.Invoke();
            }
        }
        /// <summary>
        /// Turns and Angle and Event type into a button action
        /// </summary>
        /// <param name="angle">Offset the touch coordinate </param>
        /// <param name="buttonEvent">The event of button status</param>
        public void InteractButton(float angle, ButtonEvent buttonEvent)
        {
            float buttonAngle = 360f / positionButton.Count;
            angle = Mod((angle + offsetRotation), 360);
            int buttonID = (int)Mod(((angle + (buttonAngle / 2f)) / buttonAngle), positionButton.Count);
            var pointer = new PointerEventData(EventSystem.current);
            if (buttonIndex != buttonID && buttonIndex != -1)
            {
                ExecuteEvents.Execute(menuButtons[buttonIndex], pointer, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(menuButtons[buttonIndex], pointer, ExecuteEvents.pointerExitHandler);
                positionButton[buttonIndex].OnHoverExit.Invoke();

                if (executeOnUnclick && currentTouch != -1)
                {
                    ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerDownHandler);
                }
            }
            if (buttonEvent == ButtonEvent.click)
            {
                ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerDownHandler);
                currentTouch = buttonID;
                if (!executeOnUnclick)
                {
                    positionButton[buttonID].OnClick.Invoke();
                }
            }
            else if (buttonEvent == ButtonEvent.unclick)
            {
                ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerUpHandler);
                currentTouch = -1;

                if (executeOnUnclick)
                {
                    positionButton[buttonID].OnClick.Invoke();
                }
            }
            else if (buttonEvent == ButtonEvent.hoverOn && buttonIndex != buttonID)
            {
                ExecuteEvents.Execute(menuButtons[buttonID], pointer, ExecuteEvents.pointerEnterHandler);
                positionButton[buttonID].OnHoverEnter.Invoke();
            }
            buttonIndex = buttonID;
        }

        private float Mod(float a, float b)
        {
            return a - b * Mathf.Floor(a / b);
        }
        /// <summary>
        /// Stop touch radial menu
        /// </summary>
        public void StopTouching()
        {
            if (buttonIndex != -1)
            {
                var pointer = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(menuButtons[buttonIndex], pointer, ExecuteEvents.pointerExitHandler);
                positionButton[buttonIndex].OnHoverExit.Invoke();
                buttonIndex = -1;
            }
        }
        /// <summary>
        /// Show menu
        /// </summary>
        public void EnableMenu()
        {
            if (!shown)
            {
                shown = true;
                StopCoroutine("TweenMenuScale");
                StartCoroutine("TweenMenuScale", shown);
            }
        }
        /// <summary>
        /// Hide menu
        /// </summary>
        /// <param name="Close"> Judge is close menu</param>
        public void DisableMenu(bool Close)
        {
            if (shown && (hideOnRelease || Close))
            {
                shown = false;
                StopCoroutine("TweenMenuScale");
                StartCoroutine("TweenMenuScale", shown);
            }
        }
        /// <summary>
        /// Set the display of the ring menu
        /// </summary>
        /// <param name="show">Judge is show menu</param>
        private IEnumerator TweenMenuScale(bool show)
        {
            float targetScale = 0;
            Vector3 dir = -1 * Vector3.one;
            if (show)
            {
                targetScale = 1;
                dir = Vector3.one;
            }
            int i = 0;
            while (i < 250 && ((show && transform.localScale.x < targetScale) || (!show && transform.localScale.x > targetScale)))
            {
                transform.localScale += dir * Time.deltaTime * 4f;
                yield return true;
                i++;
            }
            transform.localScale = dir * targetScale;
            StopCoroutine("TweenMenuScale");
        }
        /// <summary>
        /// Creates all the button Arcs and populates them with desired icons
        /// </summary>
        public void RegenerateButtons()
        {
            RemoveAllButtons();
            for (int i = 0; i < positionButton.Count; i++)
            {
                GameObject newButton = Instantiate(positionPrefab);
                newButton.transform.SetParent(transform);
                newButton.transform.localScale = Vector3.one;
                newButton.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                newButton.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                RIFT_GraphicCircle circle = newButton.GetComponent<RIFT_GraphicCircle>();

                if (buttonThickness == 1f)
                {
                    circle.fill = true;
                }
                else
                {
                    circle.thickness = (int)(buttonThickness * (GetComponent<RectTransform>().rect.width / 2f));
                }
                int fillPerc = (int)(100f / positionButton.Count);
                circle.fillPercent = fillPerc;
                circle.color = buttonColor;

                float angle = ((360 / positionButton.Count) * i) + offsetRotation;
                newButton.transform.localEulerAngles = new Vector3(0, 0, angle);
                newButton.layer = 4;
                newButton.transform.localPosition = Vector3.zero;
                if (circle.fillPercent < 55)
                {
                    float angleRad = (angle * Mathf.PI) / 180f;
                    Vector2 angleVector = new Vector2(-Mathf.Cos(angleRad), -Mathf.Sin(angleRad));
                    newButton.transform.localPosition += (Vector3)angleVector;
                }
                GameObject buttonIcon = newButton.GetComponentInChildren<Image>().gameObject;
                if (positionButton[i].ButtonIcon == null)
                {
                    buttonIcon.SetActive(true);
                }
                else
                {
                    buttonIcon.GetComponent<Image>().sprite = positionButton[i].ButtonIcon;
                    buttonIcon.transform.localPosition = new Vector2(-1 * ((newButton.GetComponent<RectTransform>().rect.width / 2f) - (circle.thickness / 2f)), 0);
                    float scale1 = Mathf.Abs(circle.thickness);
                    float R = Mathf.Abs(buttonIcon.transform.localPosition.x);
                    float bAngle = (359f * circle.fillPercent * 0.01f * Mathf.PI) / 180f;
                    float scale2 = (R * 2 * Mathf.Sin(bAngle / 2f));
                    if (circle.fillPercent > 24)
                    {
                        scale2 = float.MaxValue;
                    }
                    float iconScale = Mathf.Min(scale1, scale2);
                    buttonIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(iconScale, iconScale);
                    if (!rotateIcons)
                    {
                        buttonIcon.transform.eulerAngles = GetComponentInParent<Canvas>().transform.eulerAngles;
                    }
                }
                menuButtons.Add(newButton);
            }
        }
        /// <summary>
        /// Clear all buttons
        /// </summary>
        private void RemoveAllButtons()
        {
            if (menuButtons == null)
            {
                menuButtons = new List<GameObject>();
            }
            for (int i = 0; i < menuButtons.Count; i++)
            {
                if (menuButtons[i] != null)
                {
                    DestroyImmediate(menuButtons[i], true);
                }
            }
            menuButtons = new List<GameObject>();
        }
    }

    [System.Serializable]
    public class RadialMenuButton
    {
        public Sprite ButtonIcon;

        public UnityEvent OnClick;
        public UnityEvent OnHold;
        public UnityEvent OnHoverEnter;
        public UnityEvent OnHoverExit;
    }

    public enum ButtonEvent
    {
        hoverOn,
        hoverOff,
        click,
        unclick
    }
}