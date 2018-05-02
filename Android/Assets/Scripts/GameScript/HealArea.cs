using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealArea : MonoBehaviour {

    private PlayerHealthManager thePHM;

	// Use this for initialization
	void Start () {
        thePHM = FindObjectOfType<PlayerHealthManager>();
	}

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            thePHM.playerCurrentHealth = thePHM.playerMaxHealth;
        }
    }
}
