using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolder : MonoBehaviour
{

    public string dialogue;
    private DialogueManager dialogueManage;
    private JoyButton joybutton;

    public string[] dialogueLines;

    public static bool autoPlay = true; // TRIGGER THE DIALGOUE ONCE

    // Use this for initialization
    void Start()
    {
        dialogueManage = FindObjectOfType<DialogueManager>();
        joybutton = FindObjectOfType<JoyButton>();
        


    }

    // Update is called once per frame
    void Update()
    {
        if (autoPlay)
        {

            dialogueManage.dialogueLines = dialogueLines;
            dialogueManage.currentLine = 0;
            dialogueManage.ShowDialogue();
            autoPlay = false;
    
        }
    }

    /*
     * Every moment the player stays inside the box
     * We don't use OnTriggerEnter2D because it just happens ONCE
     */
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Player1")
        {
            if (Input.GetKeyUp(KeyCode.Z) ) // joybutton.pressed
            {
                //dialogueManage.ShowBox(dialogue);

                if (!dialogueManage.dialogueActive) // Restart from the first dialogue line
                {
                    dialogueManage.dialogueLines = dialogueLines;
                    dialogueManage.currentLine = 0;
                    dialogueManage.ShowDialogue();
                }

                if (transform.parent.GetComponent<VillagerMovement>() != null) // if the villager even has a villagermovement script
                {
                    transform.parent.GetComponent<VillagerMovement>().canMove = false;
                }


            }
        }
    }
}
