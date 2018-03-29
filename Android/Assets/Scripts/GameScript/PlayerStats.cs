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

    public int[] ExpNeededToLevelUp;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        if (currentExp >= ExpNeededToLevelUp[currentLevel])
        {
            currentLevel++;
            
        }
	}

    public void AddExp(int expToAdd)
    {
        currentExp += expToAdd;
    }
}
