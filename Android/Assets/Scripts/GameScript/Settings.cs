using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Settings : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
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
        settingsActive = true;
        settingsMenu.SetActive(true);
        if (exitActive)
        {
            Destroy(gameObject);
        }
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
