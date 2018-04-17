using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHealthManager : MonoBehaviour {

    public int enemyMaxHealth;
    public int enemyCurrentHealth;

    public int enemyExp;

    public string enemyQuestName;
    private QuestManager theQuestManager;

    private PlayerStats thePlayerStats;

    public bool bossFightEndScene;
    public string sceneNameToSwitch;
    public string exitPoint;
    //private LoadNewArea theLNA;
    private PlayerController thePC;

    // Use this for initialization
    void Start()
    {
        theQuestManager = FindObjectOfType<QuestManager>();
        enemyCurrentHealth = enemyMaxHealth;

        thePlayerStats = FindObjectOfType<PlayerStats>();
        //theLNA = FindObjectOfType<LoadNewArea>();
        thePC = FindObjectOfType<PlayerController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCurrentHealth <= 0)
        {
            theQuestManager.enemyKilled = enemyQuestName;
            gameObject.SetActive(false);

            thePlayerStats.AddExp(enemyExp);
            if (bossFightEndScene)
            {
                SceneManager.LoadScene(sceneNameToSwitch);
                thePC.startPoint = exitPoint;
              
            }
        }
        
    }

    public void damageEnemy(int damage)
    {
        enemyCurrentHealth -= damage;
    }

    public void setMaxHealth()
    {
        enemyCurrentHealth = enemyMaxHealth;
    }
}
