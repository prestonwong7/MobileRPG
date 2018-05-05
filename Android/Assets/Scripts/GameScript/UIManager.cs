using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public Slider healthBar;
    public Text HPText;
    public PlayerHealthManager playerHealth;

    public EnemyHealthManager enemyHealth;
    public bool enemy;

    public Slider expBar;
    public Text expText;


    //public PlayerStats playerCurrentExp;

    //public string questName;
    //public Text questText;
    //private QuestObject theQO;


    private PlayerStats thePlayerStat;
    public Text levelText;


    private static bool UIExists;

    // Use this for initialization
    void Start()
    {
        if (!UIExists)
        {
            UIExists = true;
            DontDestroyOnLoad(transform.gameObject);
        }
        //else
        //{
        //    Destroy(gameObject);
        //}

        thePlayerStat = GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy)
        {
            healthBar.maxValue = enemyHealth.enemyMaxHealth;
            healthBar.value = enemyHealth.enemyCurrentHealth;
            return;
        }
        healthBar.maxValue = playerHealth.playerMaxHealth;
        healthBar.value = playerHealth.playerCurrentHealth;
        HPText.text = "HP: " + playerHealth.playerCurrentHealth + "/" + playerHealth.playerMaxHealth;

        levelText.text = "LVL: " + thePlayerStat.currentLevel;

        expBar.maxValue = thePlayerStat.maxExp;
        expBar.value = thePlayerStat.currentExp;
        expText.text = "EXP: " + thePlayerStat.currentExp + "/" + thePlayerStat.maxExp;

        //if (theQO.isEnemyQuest) {
        //    questText.text = questName + ": " + theQO.enemyKillCount + "/" + theQO.enemiesToKill;
        //}
    }
}
