using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    [Header("Fixed Joystick")]

    RectTransform rectTransform;
    Vector2 joystickPosition = Vector2.zero;
    Vector2 direction = Vector2.zero;
    private Camera cam = new Camera();


    void Start()
    {
        joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
      
    }

    public override void OnDrag(PointerEventData eventData)
    {
        //joyDimension.bounds.size = 5;
        //789/2 - 373 = 
        //145
        //print("X: " + direction.x + ", Y: " + direction.y); //not original
        //print(eventData.position);
        //print("Joystick Pos: " +joystickPosition);
        //print("Joystick" + joystickPosition);
        //Vector2 direction = eventData.position - joystickPosition;
        //inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        //handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;

        //Vector2 direction = (eventData.position * () - joystickPosition;
        direction.x = eventData.position.x - (handle.position.x);
        direction.y = eventData.position.y - (handle.position.y);
        inputVector = (direction.magnitude > background.sizeDelta.x/2f ) ? direction.normalized : direction / (background.sizeDelta.x /2f);
        handle.anchoredPosition = (inputVector * background.sizeDelta.x ) * handleLimit;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}