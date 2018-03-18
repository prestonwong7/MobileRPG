using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour {

    public float moveSpeed;
    public float timeBetweenMove;
    public float timeToMove;

    private Vector3 moveDirection;
    private float timeBetweenMoveCounter;
    private float timeToMoveCounter;
    private bool moving;
    private Rigidbody2D myRigidBody;

	// Use this for initialization
	void Start () {
        myRigidBody = GetComponent<Rigidbody2D>();

        timeBetweenMoveCounter = timeBetweenMove;
        timeToMoveCounter = timeToMove;
	}
	
	// Update is called once per frame
	void Update () {
		if (moving == true)
        {
            timeToMoveCounter -= Time.deltaTime;
            myRigidBody.velocity = moveDirection;

            if (timeToMoveCounter < 0f)
            {
                moving = false;
                timeBetweenMoveCounter = timeBetweenMove;
            }
        }
        else
        {
            timeBetweenMoveCounter -= Time.deltaTime;
            myRigidBody.velocity = Vector2.zero; 
            
            if (timeBetweenMoveCounter < 0f)
            {
                moving = true;
                timeToMoveCounter = timeToMove;

                moveDirection = new Vector3(Random.Range(-1f, 1f) * moveSpeed, Random.Range(-1f, 1f) * moveSpeed, 0f);
            }
        }
	}
}
