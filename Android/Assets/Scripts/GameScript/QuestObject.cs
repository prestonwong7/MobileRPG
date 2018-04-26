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

    private UIManager theUI;
    public string questName;
    public Text questText;

    public string startText;
    public string endText;

    public bool isEnemyQuest; //Hunting enemy quest
    public string targetEnemy;
    public int enemiesToKill;
    public int enemyKillCount;

    //Enemy kill count needs to start 0 instead of 1

    // Use this for initialization
    void Start()
    {
        joybutton = FindObjectOfType<JoyButton>();
        thePlayerStats = FindObjectOfType<PlayerStats>();
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
                EndQuest();
                if (joybutton.pressed)
                {
                    gameObject.SetActive(false);
                }
            }
            // for UI
            questText.text = questName + ": " + enemyKillCount + "/" + enemiesToKill;
        }



    }

    public void StartQuest()
    {
        theQuestManager.ShowQuestText(startText);


    }

    public void EndQuest()
    {
        theQuestManager.ShowQuestText(endText);
        theQuestManager.questCompleted[questNumber] = true;
        thePlayerStats.AddExp(questExp);

    }
}
