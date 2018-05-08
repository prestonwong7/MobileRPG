using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestObject : MonoBehaviour
{

    public int questNumber;
    public int questExp;

    public QuestManager theQuestManager;
    private JoyButton joybutton;
    private PlayerStats thePlayerStats;
    private DialogueManager theDialogueManager;

    private UIManager theUI;
    public string questName;
    public Text questText;

    public string startText;
    public string endText;

    public bool isEnemyQuest; //Hunting enemy quest
    public string targetEnemy;
    public int enemiesToKill;
    public int enemyKillCount;

    public bool playAfterQuest;
    public int questNumberAfterQuest;
    public string questNameNumber;

    //Enemy kill count needs to start 0 instead of 1

    // Use this for initialization
    void Start()
    {
        joybutton = FindObjectOfType<JoyButton>();
        thePlayerStats = FindObjectOfType<PlayerStats>();
        theDialogueManager = FindObjectOfType<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (isEnemyQuest)
        {
            if (theQuestManager.enemyKilled == targetEnemy)
            {
                theQuestManager.enemyKilled = null;
                enemyKillCount++;

            }
            if (enemyKillCount >= enemiesToKill)
            {
                enemyKillCount = 0;
                EndQuest();

                {
                    if ((Input.GetKeyUp(KeyCode.Space) || joybutton.pressed))

                        gameObject.SetActive(false);
                }
            }
            // for UI
            questText.text = questName + ": " + enemyKillCount + "/" + enemiesToKill;
        }
        //if (playAfterQuest)
        //{
        //    if (theQuestManager.questCompleted[questNumberAfterQuest])
        //    {
        //        //gameObject.transform.Find(questNameNumber).setActive(true);
        //        StartQuest();
        //    }
        //}


    }

    public void StartQuest()
    {
        theQuestManager.ShowQuestText(startText);


    }

    public void EndQuest()
    {
        thePlayerStats.AddExp(questExp);
        theQuestManager.ShowQuestText(endText);
        theQuestManager.questCompleted[questNumber] = true;


    }
}
