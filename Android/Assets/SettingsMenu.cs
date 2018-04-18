using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    public GameObject audioManager;
    public Slider volumeSlider;

	public void SetVolume()
    {
        //shit doesn't work fam
        //audioManager.GetComponent<AudioSource>().volume = volumeSlider.value;
    }
}
