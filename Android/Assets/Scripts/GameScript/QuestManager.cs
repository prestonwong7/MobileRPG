using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuestManager : MonoBehaviour {

    public QuestObject[] quests;
    public bool[] questCompleted;

    public DialogueManager theDialogueManager;

    public string enemyKilled;

    

	// Use this for initialization
	void Start () {
        questCompleted = new bool[quests.Length];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowQuestText(string questText)
    {
        theDialogueManager.dialogueLines = new string[1];
        theDialogueManager.dialogueLines[0] = questText;

        theDialogueManager.currentLine = 0;
        theDialogueManager.ShowDialogue();
    }


}
