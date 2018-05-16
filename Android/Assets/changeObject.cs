using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeObject : MonoBehaviour {
    public GameObject male;
    public GameObject female;
    public Dropdown myDropdown;

	// Use this for initialization
	void Start () {
        male.SetActive(true);
        female.SetActive(false);
        myDropdown = FindObjectOfType<Dropdown>();
	}
	
	// Update is called once per frame
	void Update () {
        
		if(myDropdown.value == 0)
        {
            male.SetActive(true);
            female.SetActive(false);
            DontDestroyOnLoad(male);
            Debug.Log("Male working");
        }
        if(myDropdown.value == 1)
        {
            male.SetActive(false);
            female.SetActive(true);
            DontDestroyOnLoad(female);
            Debug.Log("female working");
        }
	}
}
