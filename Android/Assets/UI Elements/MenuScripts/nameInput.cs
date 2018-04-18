using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class nameInput : MonoBehaviour {

    private InputField input;

    void Awake()
    {
        input = GameObject.Find("getName").GetComponent<InputField>();
    }

    public void GetInput(string name)
    {
        input.text = "";
    }
}
