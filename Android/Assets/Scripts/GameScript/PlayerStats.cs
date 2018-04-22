using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Levelling up class 
 * 
 */
public class PlayerStats : MonoBehaviour {

    public int currentLevel;
    public int currentExp;

    public int maxExp;

    public int[] ExpNeededToLevelUp;

    public int[] HPLevels;
    public int[] attackLevels;
    public int[] defenseLevels;

    public int currentHp;
    public int currentAttack;
    public int currentDefense;

    public bool dead;
    public float respawnTime;
    public float respawnTimeCounter;
    public string respawnPoint;

    private PlayerHealthManager thePlayerHealth;
    private PlayerController thePC;
    private SFXManager theSFX;
    private PlayerStartPoint thePSP;
    

	// Use this for initialization
	void Start () {
        currentHp = HPLevels[1];
        currentAttack = attackLevels[1];
        currentDefense = defenseLevels[1];

        maxExp = ExpNeededToLevelUp[1];

        thePlayerHealth = FindObjectOfType<PlayerHealthManager>();
        theSFX = FindObjectOfType<SFXManager>();
        thePSP = FindObjectOfType<PlayerStartPoint>();
        thePC = FindObjectOfType<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
        
        if (currentExp >= ExpNeededToLevelUp[currentLevel])
        {
            //currentLevel++;
            LevelUp();
            
        }

        // Respawning

        if (!dead)
        {
            if (thePlayerHealth.playerCurrentHealth <= 0)
            {
                //deadCheck = true;
                dead = true;
                respawnTimeCounter = respawnTime;

                //Destroy(gameObject);

            }
        }

        if (dead)
        {
            if (respawnTimeCounter > 0)
            {
                respawnTimeCounter -= Time.deltaTime;
            }


            if (respawnTimeCounter <= 0)
            {
                thePC.gameObject.SetActive(true);
                //thePC.transform.position = thePSP.transform.position;
                dead = false;
                thePlayerHealth.playerCurrentHealth = thePlayerHealth.playerMaxHealth;
                thePC.transform.position = Vector2.zero;
            }
        }
       


    }

    public void AddExp(int expToAdd)
    {
        currentExp += expToAdd;
    }

    public void LevelUp()
    {
        currentLevel++;
        currentExp = 0;

        thePlayerHealth.playerMaxHealth = currentHp;
        thePlayerHealth.playerCurrentHealth += currentHp - HPLevels[currentLevel - 1];

        thePlayerHealth.playerCurrentHealth = HPLevels[currentLevel - 1]; // Make player health back to full hp

        currentHp = HPLevels[currentLevel];
        currentAttack = attackLevels[currentLevel];
        currentDefense = defenseLevels[currentLevel];

        maxExp = ExpNeededToLevelUp[currentLevel];
        theSFX.playerLevelUp.Play();


    }
}
