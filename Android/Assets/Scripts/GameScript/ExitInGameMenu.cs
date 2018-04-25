using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitInGameMenu : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    //[HideInInspector]
    //public bool settingsPressed;
    public bool settingsActive;
    private UIManager theUI;
    public GameObject settingsMenu;
    public bool exitActive;


    public void OnPointerUp(PointerEventData eventData)
    {
        //Input.GetKeyUp(pressed);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print(eventData);
        settingsActive = false;
        settingsMenu.SetActive(false);
        gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        theUI = FindObjectOfType<UIManager>();
        //settingsMenu = FindObjectOfType<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
