using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTestCheck : MonoBehaviour 
{

    public GameObject audioMan;


	// Use this for initialization
	void Start ()
    {
        if(FindObjectOfType<AudioTest>())
        {
            return;
        }
        else
        {
            Instantiate(audioMan, transform.position, transform.rotation);
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}
}
