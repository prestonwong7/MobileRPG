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
	}
	
	// Update is called once per frame
	void Update (Dropdown target) {
        
		if(target.value == 0)
        {
            male.SetActive(true);
            female.SetActive(false);
        }
        if(target.value == 1)
        {
            male.SetActive(false);
            female.SetActive(true);
        }
	}
}
