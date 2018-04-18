using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    [Header("Fixed Joystick")]
    

    Vector2 joystickPosition = Vector2.zero;
    Vector2 direction = Vector2.zero;
    private Camera cam = new Camera();
    private Joystick joyDimensions;
    private GameObject joyDimension;
    private RectTransform joyStick;

    void Start()
    {
        joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        joyDimensions = FindObjectOfType<Joystick>();
        
        joyStick = (RectTransform)joyDimensions.transform;
        float width = joyStick.boundsd;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        //joyDimension.bounds.size = 5;
        //789/2 - 373 = 
        //145
        print("X: " + direction.x + ", Y: " + direction.y); //not original
        print(eventData.position);
        print("Joystick Pos: " +joystickPosition);
        //print("Joystick" + joystickPosition);
        //Vector2 direction = eventData.position - joystickPosition;
        //inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        //handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;

        //Vector2 direction = (eventData.position * () - joystickPosition;
        direction.x = eventData.position.x - joystickPosition.x;
        direction.y = eventData.position.y - joystickPosition.y;
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
        handle.anchoredPosition = joystickPosition;
    }
}