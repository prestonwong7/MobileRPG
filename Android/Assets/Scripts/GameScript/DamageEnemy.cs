using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemy : MonoBehaviour {

    public int damage;
    public GameObject damageParticle;
    public Transform pointOfImpact;
    public GameObject damageNumber;

    private FloatingNumbers theFloatingNumber;

	// Use this for initialization
	void Start () {
        theFloatingNumber = GetComponent<FloatingNumbers>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (other.gameObject != null)
            {
                other.gameObject.GetComponent<EnemyHealthManager>().damageEnemy(damage);
                Instantiate(damageParticle, pointOfImpact.position, pointOfImpact.rotation);

                var clone = (GameObject) Instantiate(damageNumber, pointOfImpact.position, Quaternion.Euler(Vector3.zero)); // I dont get quaternion either
                clone.GetComponent<FloatingNumbers>().damageNumber = damage;
                clone.transform.position = new Vector2(pointOfImpact.position.x, pointOfImpact.position.y);
                //print("point " + pointOfImpact.position);
                //print("Clone is" + clone.transform.position);

            }
        }
    }

}
