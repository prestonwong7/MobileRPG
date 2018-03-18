using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject followTarget;
    private Vector3 targetPos;
    public float moveSpeed;

    private static bool cameraExists;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(transform.gameObject); //Doesnt destroy camera when switching scenes
        
        if (!cameraExists)
        {
            cameraExists = true;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject); // Destroy
        }

    }

    // Update is called once per frame
    void Update () {
        targetPos = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, transform.position.z); // Follows player
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed*Time.deltaTime); // Follows player
	}
}
