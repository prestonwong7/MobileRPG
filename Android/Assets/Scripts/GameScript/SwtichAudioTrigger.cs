using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwtichAudioTrigger : MonoBehaviour 
{
    public AudioClip newTrack;
    private AudioTest theAM;

	// Use this for initialization
	void Start () 
    {
       
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "Player1")
        {
            if(newTrack != null)
            {
                theAM = FindObjectOfType<AudioTest>();
                theAM.ChangeBGM(newTrack);
            }
        }
    }
}
