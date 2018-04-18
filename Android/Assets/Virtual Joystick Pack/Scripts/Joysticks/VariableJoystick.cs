using UnityEngine;
using UnityEngine.EventSystems;

public class VariableJoystick : Joystick
{
    [Header("Options")]
    public bool isFixed = true;
    public Vector2 fixedScreenPosition;

    Vector2 joystickCenter = Vector2.zero;
    bool fixedLast = true;

    void Start()
    {
        if (!isFixed)
        {
            OnFloat();
        }
        else
        {
            OnFixed();
        }
    }

    void Update()
    {
        if (isFixed != fixedLast)
        {
            if (!isFixed)
            {
                OnFloat();
            }
            else
            {
                OnFixed();
            }
            fixedLast = isFixed;
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - joystickCenter;
        inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!isFixed)
        {
            background.gameObject.SetActive(true);
            background.position = eventData.position;
            handle.anchoredPosition = Vector2.zero;
            joystickCenter = eventData.position;
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!isFixed)
        {
            background.gameObject.SetActive(false);
        }
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    void OnFixed()
    {
        joystickCenter = fixedScreenPosition;
        background.gameObject.SetActive(true);
        handle.anchoredPosition = Vector2.zero;
        background.anchoredPosition = fixedScreenPosition;
    }

    void OnFloat()
    {
        handle.anchoredPosition = Vector2.zero;
        background.gameObject.SetActive(false);
    }
}