using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour {

    public int enemyMaxHealth;
    public int enemyCurrentHealth;

    public int enemyExp;

    public string enemyQuestName;
    private QuestManager theQuestManager;

    private PlayerStats thePlayerStats;

    // Use this for initialization
    void Start()
    {
        theQuestManager = FindObjectOfType<QuestManager>();
        enemyCurrentHealth = enemyMaxHealth;

        thePlayerStats = FindObjectOfType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCurrentHealth <= 0)
        {
            theQuestManager.enemyKilled = enemyQuestName;
            gameObject.SetActive(false);

            thePlayerStats.AddExp(enemyExp);
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
