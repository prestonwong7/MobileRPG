using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour {

    public int damage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D other) // Used when player hits a slime, or enemy object
    {
        if (other.gameObject.name == "Player1")
        {
            other.gameObject.GetComponent<PlayerHealthManager>().damagePlayer(damage); // Gets function from PlayerHealthManager class, hurting player
           
        }
    }
}
