using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public GameObject dialogueBox;
    public Text dialogueText;

    public bool dialogueActive;

    public string[] dialogueLines;
    public int currentLine;

    private PlayerController thePlayer;
    //private Joystick joystick;
    private JoyButton joybutton;

	// Use this for initialization
	void Start () {
        thePlayer = FindObjectOfType<PlayerController>();
        //joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<JoyButton>();
    }
	
	// Update is called once per frame
	void Update () {
        if (dialogueActive && (Input.GetKeyDown(KeyCode.Space) || joybutton.pressed ))
        {
            //dialogueBox.SetActive(false);
            //dialogueActive = false;


            currentLine++;
        }

        if (currentLine >= dialogueLines.Length)
        {
            dialogueBox.SetActive(false);
            dialogueActive = false;

            currentLine = 0; // Reset dialogue to 0 so other NPCs can talk
            thePlayer.canMove = true; // Player can move after dialogue is set active
        }

        //dialogueText.text = dialogueLines[currentLine]; // Read the max value currentLine, so it displays something

    }

    public void ShowBox(string dialogue)
    {
        dialogueActive = true;
        dialogueBox.SetActive(true);
        dialogueText.text = dialogue;
        
    }

    public void ShowDialogue()
    {
        dialogueActive = true;
        dialogueBox.SetActive(true);
        thePlayer.canMove = false; // Player can't move while dialogue is occuring


    }
}
