using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemy : MonoBehaviour {

    public int damage;
    public GameObject damageParticle;
    public Transform pointOfImpact;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyHealthManager>().damageEnemy(damage);
            Instantiate(damageParticle, pointOfImpact.position, pointOfImpact.rotation);
        }
    }

}
