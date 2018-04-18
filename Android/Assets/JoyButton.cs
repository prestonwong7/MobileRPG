using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

    [HideInInspector]
    public bool pressed;

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false; 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
