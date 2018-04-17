using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwtichMusicOnLoad : MonoBehaviour {

    public AudioClip newTrack;
    private AudioTest theAM;

    // Use this for initialization
    void Start()
    {
        theAM = FindObjectOfType<AudioTest>();

        if (newTrack != null)
        {
            theAM.ChangeBGM(newTrack);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
