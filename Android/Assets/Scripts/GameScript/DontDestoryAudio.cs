using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryAudio : MonoBehaviour
{
    static DontDestoryAudio instance = null;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
  
}
