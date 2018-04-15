using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource _AudioSource;

    public AudioClip _AudioClip1;
    public AudioClip _AudioClip2;

    public string test;

    // void Start()
    //{
    //    //Scene curr = SceneManager.GetActiveScene();
    //    //string sceneName = curr.name;

    //    _AudioSource.clip = _AudioClip1;

    //    _AudioSource.Play();

    //}


    //void Update()
    //{
    //    Scene curr = SceneManager.GetActiveScene();
    //    string sceneName = curr.name;

    //    if(sceneName == "Ken's Map")
    //    //if (Input.GetKeyDown(KeyCode.S))
    //    {

    //        if (_AudioSource.clip == _AudioClip1)
    //        {

    //            _AudioSource.clip = _AudioClip2;

    //            _AudioSource.Play();

    //        }

    //        else
    //        {

    //            _AudioSource.clip = _AudioClip1;

    //            _AudioSource.Play();

    //        }

    //    }

    //}

    private void Start()
    {
        _AudioSource.clip = _AudioClip1;
        _AudioSource.Play();

        // Create a temporary reference to the current scene.
        Scene currentScene = SceneManager.GetActiveScene();

        // Retrieve the name of this scene.
        string sceneName = currentScene.name;

        //if (sceneName == "Example 1")
        //{
        //    // Do something...
        //}
        //else if (sceneName == "Example 2")
        //{
        //    // Do something...
        //}

        // Retrieve the index of the scene in the project's build settings.
        int buildIndex = currentScene.buildIndex;

        // Check the scene name as a conditional.
        switch (buildIndex)
        {
            case 0:
                break;
            case 1:
                _AudioSource.clip = _AudioClip2;
                _AudioSource.Play();
                break;
        }
    }


}
