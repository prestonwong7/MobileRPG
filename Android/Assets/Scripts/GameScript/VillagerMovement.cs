using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Used for NPCs to move
 * */
public class VillagerMovement : MonoBehaviour {

    public float moveSpeed;
    public bool isWalking;

    public float walkTime;
    private float walkCounter;
    public float waitTime;
    private float waitCounter;
    
    private Vector2 minWalkPoint; // So they can't walk outside the bounds
    private Vector2 maxWalkPoint;
    private bool hasWalkZone;


    private int walkDirection;

    private Rigidbody2D myRigidBody;

    public Animator anim;
    public Vector2 facing;

    public Collider2D walkZone;

    public bool canMove;
    private DialogueManager theDialogueManager;

    public bool oneDirection;

	// Use this for initialization
	void Start () {
        myRigidBody = GetComponent<Rigidbody2D>();
        theDialogueManager = FindObjectOfType<DialogueManager>();

        waitCounter = waitTime;
        walkCounter = walkTime;

        anim = GetComponent<Animator>();

        chooseDirection();

        if (walkZone != null)
        {
            minWalkPoint = walkZone.bounds.min;
            maxWalkPoint = walkZone.bounds.max; // Gets the max bounds
            hasWalkZone = true;
        }

        canMove = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (oneDirection)
        {
            myRigidBody.velocity = new Vector2(0, -moveSpeed);
            return;
        }

        if (!theDialogueManager.dialogueActive) // If there is no dialouge, villager can move
        {
            canMove = true;
        }

        if (!canMove) // Check
        {
            myRigidBody.velocity = Vector2.zero;
            return; // Stops the rest of the update function from running
        }

		if (isWalking) // Set when to walk
        {
            walkCounter -= Time.deltaTime;


            switch(walkDirection)
            {
                case 0:
                    myRigidBody.velocity = new Vector2(0, moveSpeed); //Move up
                    facing.y = 1;
                    facing.x = 0;
                    if (hasWalkZone &&  transform.position.y > maxWalkPoint.y)
                    {
                        isWalking = false; // Cant move when reaching max point
                        waitCounter = waitTime;
                    }
                    break;
                case 1:
                    myRigidBody.velocity = new Vector2(moveSpeed, 0 ); //Move right
                    facing.y = 0;
                    facing.x = 1;
                    if (hasWalkZone && transform.position.x > maxWalkPoint.x)
                    {
                        isWalking = false; // Cant move when reaching max point
                        waitCounter = waitTime;
                    }
                    //transform.localScale = new Vector3(6f, 6f, 1f);
                    break;
                case 2:
                    myRigidBody.velocity = new Vector2(0,  -moveSpeed); //Move down
                    facing.y = -1;
                    facing.x = 0;
                    if (hasWalkZone && transform.position.y > minWalkPoint.y)
                    {
                        isWalking = false; // Cant move when reaching max point
                        waitCounter = waitTime;
                    }
                    break;
                case 3:
                    myRigidBody.velocity = new Vector2( -moveSpeed, 0); //Move left
                    facing.y = 0;
                    facing.x = -1;
                    if (hasWalkZone && transform.position.x < minWalkPoint.x)
                    {
                        isWalking = false; // Cant move when reaching max point
                        waitCounter = waitTime;
                    }
                    //transform.localScale = new Vector3(-6f, 6f, 1f);
                    break;
            }

            if (walkCounter <= 0)
            {
                isWalking = false;
                waitCounter = waitTime;
            }
        }
        else
        {
            waitCounter -= Time.deltaTime;
            myRigidBody.velocity = Vector2.zero; // NPC CANT MOVE
            if (waitCounter <= 0)
            {
                chooseDirection();
            }
        }

        //anim.SetFloat("YourFloat", facing.x);
        //anim.SetFloat("YourFloat", facing.y);
        //anim.SetBool("YourBool", isWalking);
    }

    public void chooseDirection()
    {
        walkDirection = Random.Range(0, 4); // 0 = UP, 1 = RIGHT, 2 = DOWN, 3 = LEFT
        isWalking = true;
        walkCounter = walkTime;
    }
}
