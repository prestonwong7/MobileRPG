using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class is used for creates/making the damage number rise up 
 */
public class FloatingNumbers : MonoBehaviour {

    public float floatSpeed;
    public int damageNumber;
    public Text displayNumber;
    public Text displayText;

	// Use this for initialization
	void Start () {
        displayNumber.transform.position = new Vector3(transform.position.x, (transform.position.y + (floatSpeed * Time.deltaTime)), transform.position.z);

    }
	
	// Update is called once per frame
	void Update () {
        displayNumber.text = "" + damageNumber ;
        displayNumber.transform.position = new Vector3(displayNumber.transform.position.x, (displayNumber.transform.position.y + (floatSpeed * Time.deltaTime)), displayNumber.transform.position.z);
        //displayText.transform.position = new Vector3(displayText.transform.position.x, (displayText.transform.position.y + (floatSpeed * Time.deltaTime)), displayText.transform.position.z);

    }
}
