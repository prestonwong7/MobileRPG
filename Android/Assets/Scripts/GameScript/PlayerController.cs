using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;
    private float currentMoveSpeed;
    public float diagnoalMoveModifier;

    private Animator anim;
    private Rigidbody2D myRigidBody;

    private bool playerMoving;
    private Vector2 lastMove;

    private static bool playerExists;

    private bool attacking;
    public float attackTime;
    private float attackTimeCounter;

    public string startPoint;

    public bool canMove;


    private SFXManager theSFXManager;

    private Joystick joystick;
    private JoyButton joybutton;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>();
        theSFXManager = FindObjectOfType<SFXManager>();
        joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<JoyButton>();

        if (!playerExists)
        {
            playerExists = true;
            DontDestroyOnLoad(transform.gameObject); // Won't destroy player when switching scenes
        }
        else
        {
            Destroy(gameObject);
        }

        canMove = true;

        lastMove = new Vector2(0, -1f); // Beginning of game, player faces down
        
	}
	
	// Update is called once per frame
	void Update () {

        playerMoving = false;

        if (!canMove)
        {
            myRigidBody.velocity = Vector2.zero;
            return; // No other code will run in this function
        }

        if (!attacking)
        {
            if (Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f) // Horizontal movement
            {
                //transform.Translate(new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime, 0f, 0f));
                myRigidBody.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * currentMoveSpeed, myRigidBody.velocity.y);
                
                playerMoving = true;
                lastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0f); // Gets last move for animation
            }
           
            if (Input.GetAxisRaw("Vertical") > 0.5f || Input.GetAxisRaw("Vertical") < -0.5f) // Vertical movement
            {
                //transform.Translate(new Vector3(0f, Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime, 0f));
                myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, Input.GetAxisRaw("Vertical") * currentMoveSpeed);
                playerMoving = true;
                lastMove = new Vector2(0f, Input.GetAxisRaw("Vertical")); // Last move for animation

            }

            if (Input.GetAxisRaw("Horizontal") < 0.5f && Input.GetAxisRaw("Horizontal") > -0.5f) // Make player stop moving when moving horizontally, basically at 0
            {
                myRigidBody.velocity = new Vector2(0f, myRigidBody.velocity.y); // Move speed horizontal
            }

            if (Input.GetAxisRaw("Vertical") < 0.5f && Input.GetAxisRaw("Vertical") > -0.5f) // Basically at 0
            {
                myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, 0f); // MOve speed vertically
                
            }
           
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f && Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.5f) // Moving diagonally
            {
                currentMoveSpeed = moveSpeed * diagnoalMoveModifier;
            }
            else
            {
                currentMoveSpeed = moveSpeed;
            }

            // FOR JOYSTICKS
            if (joystick.Horizontal > 0.5f)
            {
                //myRigidBody.velocity = new Vector2(1 * currentMoveSpeed, myRigidBody.velocity.y);
                transform.Translate(new Vector3(currentMoveSpeed * Time.deltaTime, 0f, 0f));
                playerMoving = true;
                anim.SetFloat("MoveX", 1f);
                lastMove = new Vector2(1f, 0f); // Gets last move for animation
            } else {
                anim.SetFloat("MoveX", 0);
            }

            if (joystick.Horizontal < -0.5f)
            {
                //myRigidBody.velocity = new Vector2(1 * currentMoveSpeed, myRigidBody.velocity.y);
                transform.Translate(new Vector3(-currentMoveSpeed * Time.deltaTime, 0f, 0f));
                playerMoving = true;
                anim.SetFloat("MoveX", -1f);
                lastMove = new Vector2(-1f, 0f); // Gets last move for animation
            }  else {
                
            }

            if (joystick.Vertical > 0.5f)
            {
                //myRigidBody.velocity = new Vector2(1 * currentMoveSpeed, myRigidBody.velocity.y);
                transform.Translate(new Vector3(0f, currentMoveSpeed * Time.deltaTime, 0f));
                playerMoving = true;
                anim.SetFloat("MoveY", 1f);
                lastMove = new Vector2(0f, 1f); // Gets last move for animation
            }
            else
            {
                anim.SetFloat("MoveY", 0f);
            }

            if (joystick.Vertical < -0.5f)
            {
                //myRigidBody.velocity = new Vector2(1 * currentMoveSpeed, myRigidBody.velocity.y);
                transform.Translate(new Vector3(0f, -currentMoveSpeed * Time.deltaTime , 0f));
                playerMoving = true;
                anim.SetFloat("MoveY", -1f);
                lastMove = new Vector2(0f, -1f); // Gets last move for animation
            } else {
                
            }
            if (Mathf.Abs(joystick.Horizontal)> 0.5f && Mathf.Abs(joystick.Vertical) > 0.5f) // Moving diagonally
            {
                currentMoveSpeed = moveSpeed * diagnoalMoveModifier;
            }
            else
            {
                currentMoveSpeed = moveSpeed;
            }
        }
        if (Input.GetKeyDown(KeyCode.Z) || joybutton.pressed)
        {
            attackTimeCounter = attackTime;
            attacking = true;
            myRigidBody.velocity = Vector2.zero;
            anim.SetBool("Attack", true);
            theSFXManager.playerAttack.Play(); // Play sound effect
        }
        if (attackTimeCounter > 0)
        {
            attackTimeCounter -= Time.deltaTime;
            
        }
        if (attackTimeCounter <= 0)
        {
            attacking = false;
            anim.SetBool("Attack", false);
        }


        anim.SetFloat("MoveX", Input.GetAxisRaw("Horizontal")); // Used for animating
        anim.SetFloat("MoveY", Input.GetAxisRaw("Vertical")); // Comment this line and the previous line for joystick animation
        anim.SetBool("PlayerMoving", playerMoving);
        anim.SetFloat("LastMoveX", lastMove.x); // LastMove used for direction of player facing
        anim.SetFloat("LastMoveY", lastMove.y);


    }
}
