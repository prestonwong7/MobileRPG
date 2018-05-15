using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour {

    public AudioSource playerHurt; // Reference to the audio
    public AudioSource playerDead;
    public AudioSource playerAttack;
    public AudioSource playerLevelUp;

    
    private static bool sfManExists;

	// Use this for initialization
	void Start () {
		//if (!sfManExists)
        //{
            //sfManExists = true;
            DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void mute()
    {
        playerHurt.mute = !playerHurt.mute;
        playerDead.mute = !playerDead.mute;
        playerAttack.mute = !playerAttack.mute;
        playerLevelUp.mute = !playerLevelUp.mute;
    }
}
