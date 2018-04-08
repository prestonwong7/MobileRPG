using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueHolder : MonoBehaviour {

    public string dialogue;
    private DialogueManager dialogueManage;

	// Use this for initialization
	void Start () {
        dialogueManage = FindObjectOfType<DialogueManager>();
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /*
     * Every moment the player stays inside the box
     * We don't use OnTriggerEnter2D because it just happens ONCE
     */
    private void OnTriggerStay2D(Collider2D other) 
    { 
        if (other.gameObject.name == "Player1")
        {
            if (Input.GetKeyUp(KeyCode.Z))
            {
                dialogueManage.ShowBox(dialogue);
            }
        }
    }
}
