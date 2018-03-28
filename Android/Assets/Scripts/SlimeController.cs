using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlimeController : MonoBehaviour {

    public float moveSpeed;
    public float timeBetweenMove; // How long it stays still
    public float timeToMove; // How long it moves
    public float respawnTime;

    private bool reloading;
    private Vector3 moveDirection;
    private float timeBetweenMoveCounter;
    private float timeToMoveCounter;
    private bool moving; 
    private Rigidbody2D myRigidBody;
    private GameObject thePlayer; 

	// Use this for initialization
	void Start () {
        myRigidBody = GetComponent<Rigidbody2D>(); // Needed for RigidBody2D

        //timeBetweenMoveCounter = timeBetweenMove;
        //timeToMoveCounter = timeToMove;

        timeBetweenMoveCounter = Random.Range(timeBetweenMove * 0.75f, timeBetweenMove * 1.25f); // Minimum, Max Values, Adjustable
        timeToMoveCounter = Random.Range(timeToMove * 0.75f, timeToMove * 1.25f); // Minimum, Max Values, Adjustable

    }
	
	// Update is called once per frame
	void Update () {
		if (moving == true)
        {
            timeToMoveCounter -= Time.deltaTime; //Time delta time is approx 0.02 I think
            myRigidBody.velocity = moveDirection;

            if (timeToMoveCounter < 0f) // Counts down to 0, when hitting 0, makes moving false
            {
                moving = false;
                //timeBetweenMoveCounter = timeBetweenMove;
                timeBetweenMoveCounter = Random.Range(timeBetweenMove * 0.75f, timeBetweenMove * 1.25f); // Minimum, Max Values, Adjustable
            }
        }
        else
        {
            timeBetweenMoveCounter -= Time.deltaTime;
            myRigidBody.velocity = Vector2.zero; 
            
            if (timeBetweenMoveCounter < 0f) // Counts down to 0, when hitting 0, makes moving true
            {
                moving = true;
                //timeToMoveCounter = timeToMove;
                timeToMoveCounter = Random.Range(timeToMove * 0.75f, timeToMove * 1.25f); // Minimum, Max Values, Adjustable

                moveDirection = new Vector3(Random.Range(-1f, 1f) * moveSpeed, Random.Range(-1f, 1f) * moveSpeed, 0f);
            }
        }

        if (reloading == true) // if (reloading)
        {
            respawnTime -= Time.deltaTime;
            if (respawnTime < 0f)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                thePlayer.SetActive(true);
            }
        }
	}

    void OnCollisionEnter2D(Collision2D other) // Used when player hits a slime, or enemy object
    {
        //if (other.gameObject.name == "Player1")
        //{
        //    //Destroy(other.gameObject) //KILLS PLAYER but camera wont follow
        //    other.gameObject.SetActive(false);
        //    reloading = true;
        //    thePlayer = other.gameObject;
        //}
    }
}
