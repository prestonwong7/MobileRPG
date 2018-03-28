using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour {

    public int playerMaxHealth;
    public int playerCurrentHealth;

	// Use this for initialization
	void Start () {
        playerCurrentHealth = playerMaxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		 if (playerCurrentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
	}

    public void damagePlayer(int damage)
    {
        playerCurrentHealth -= damage;
    }

    public void setMaxHealth()
    {
        playerCurrentHealth = playerMaxHealth;
    }
}
