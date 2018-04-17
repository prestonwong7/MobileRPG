using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject followTarget;
    private Vector3 targetPos;
    public float moveSpeed;

    private static bool cameraExists;

    public BoxCollider2D boundBox; // Used for sticking the camera inside the map
    private Vector3 minBounds;
    private Vector3 maxBounds;

    private Camera theCamera;
    private float halfHeight; // Unity calculates the vertical camera size divided by 2, not the full size
    private float halfWidth;

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

        minBounds = boundBox.bounds.min; // Sets the x min position for boxCollider of bounds
        maxBounds = boundBox.bounds.max;

        theCamera = GetComponent<Camera>();
        halfHeight = theCamera.orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height;

    }

    // Update is called once per frame
    void Update () {
        targetPos = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, transform.position.z); // Follows player
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed*Time.deltaTime); // Follows player

        if (boundBox != null)
        {
            float clampedX = Mathf.Clamp(transform.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth); // Clamps make it so a value doesnt go past its min/max
            float clampedY = Mathf.Clamp(transform.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }

        if (boundBox == null)
        {
            boundBox = FindObjectOfType<Bounds>().GetComponent<BoxCollider2D>();
            minBounds = boundBox.bounds.min;
            maxBounds = boundBox.bounds.max;

        }
       
    }

    public void SetBounds(BoxCollider2D newBounds)
    {
        boundBox = newBounds;

        minBounds = boundBox.bounds.min; // Sets the x min position for boxCollider of bounds
        maxBounds = boundBox.bounds.max;
    }
}
