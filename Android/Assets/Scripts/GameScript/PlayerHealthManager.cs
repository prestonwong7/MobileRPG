using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour {

    public int playerMaxHealth;
    public int playerCurrentHealth;

    private bool flashActive; // if player gets damaged, player flashes
    public float flashLength;
    private float flashCounter;

    private SpriteRenderer playerSprite;

	// Use this for initialization
	void Start () {
        playerCurrentHealth = playerMaxHealth;

        playerSprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (playerCurrentHealth <= 0)
        {
            gameObject.SetActive(false);
        }

        if (flashActive)
        {
            if (flashCounter > (flashLength * .6))
            {
                playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 0f); // Invisible
            } else if (flashCounter >= (flashLength * .3))
            {
                playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 1f); // Visible
            } else if (flashCounter >= 0)
            {
                playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 0f); // Invisible
            } else 
            {
                playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 1f); // Change alpha color in sprite renderer to 1 , visible
                flashActive = false;
            }
        }

        flashCounter -=  Time.deltaTime;
	}

    public void damagePlayer(int damage)
    {
        playerCurrentHealth -= damage;

        flashActive = true;
        flashCounter = flashLength;
    }

    public void setMaxHealth()
    {
        playerCurrentHealth = playerMaxHealth;
    }
}
