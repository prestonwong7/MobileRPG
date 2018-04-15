using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour {

    public AudioSource playerHurt; // Reference to the audio
    public AudioSource playerDead;
    public AudioSource playerAttack;

    private static bool sfManExists;

	// Use this for initialization
	void Start () {
		if (!sfManExists)
        {
            sfManExists = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
