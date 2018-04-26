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
    private QuestTrigger endQuest;

	// Use this for initialization
	void Start () {
        thePlayer = FindObjectOfType<PlayerController>();
        //joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<JoyButton>();
        endQuest = FindObjectOfType<QuestTrigger>();
    }
	
	// Update is called once per frame
	void Update () {
        //print("Dialogue active: " + dialogueActive);
        //print(joybutton.pressed);
        //print(currentLine);
        //print(dialogueLines.Length - 1);

        

        if (dialogueActive && (Input.GetKeyDown(KeyCode.Space) || joybutton.pressed ))
        {
            //dialogueBox.SetActive(false);
            //dialogueActive = false;
            //print("Dialogue active: " + dialogueActive);
            //print("Joybutton Status: " + joybutton.pressed);
            //print("Current Line: " + currentLine);
            //print("Total Lines: " + dialogueLines.Length);
            //print("Length: " + dialogueLines.Length);
            
            currentLine++;
            
            joybutton.pressed = false; // Can't hold button down
            print("Dialgoue active: " + dialogueActive);
            print("joybutton: " + joybutton.pressed);
            print("EndQuest " + endQuest);
        }
        //print("current" + currentLine);
        //print("dialogue lines" + dialogueLines.Length);

        if (currentLine >= dialogueLines.Length - 1 )
        {
            dialogueBox.SetActive(false);
            dialogueActive = false;

            //print("Current Line: " + currentLine);
            currentLine = 0; // Reset dialogue to 0 so other NPCs can talk

            thePlayer.canMove = true;
            //joybutton.pressed = false; // Used so that you can't hold button down!
        }
        

        dialogueText.text = dialogueLines[currentLine]; // Read the max value currentLine, so it displays something
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
