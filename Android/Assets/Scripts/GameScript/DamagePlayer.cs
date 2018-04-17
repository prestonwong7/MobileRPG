using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour {

    public int damage; // Doesn't change the damage at Unity
    private int currentDamage; // Used to calculate the damage + defense
    public GameObject damageNumber;
    public bool ignoreOtherBoxCollider;

    private PlayerStats thePlayerStats;

    // Use this for initialization
    void Start () {
        thePlayerStats = FindObjectOfType<PlayerStats>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D other) // Used when player hits a slime, or enemy object
    {
        if (other.gameObject.name == "Player1")
        {
            currentDamage = damage - thePlayerStats.currentDefense; 
            if (currentDamage <= 0)
            {
                currentDamage = 1;
            }

            other.gameObject.GetComponent<PlayerHealthManager>().damagePlayer(currentDamage); // Gets function from PlayerHealthManager class, hurting player

            var clone = (GameObject)Instantiate(damageNumber, other.transform.position, Quaternion.Euler(Vector3.zero)); // I dont get quaternion either
            clone.GetComponent<FloatingNumbers>().damageNumber = currentDamage;
            clone.transform.position = new Vector2(other.transform.position.x, other.transform.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ignoreOtherBoxCollider)
        {
            if (other.gameObject.name == "Player1")
            {
                currentDamage = damage - thePlayerStats.currentDefense;
                if (currentDamage <= 0)
                {
                    currentDamage = 1;
                }

                other.gameObject.GetComponent<PlayerHealthManager>().damagePlayer(currentDamage); // Gets function from PlayerHealthManager class, hurting player

                var clone = (GameObject)Instantiate(damageNumber, other.transform.position, Quaternion.Euler(Vector3.zero)); // I dont get quaternion either
                clone.GetComponent<FloatingNumbers>().damageNumber = currentDamage;
                clone.transform.position = new Vector2(other.transform.position.x, other.transform.position.y);
            }
        }
    }
}
