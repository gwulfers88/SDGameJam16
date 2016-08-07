using UnityEngine;
using System.Collections;

public class Healer : Photon.MonoBehaviour
{
    string duty = "Healer";
    const float maxHealth = 100f;
    float defense = .25f;
    float health;

	// Use this for initialization
	void Start ()
    {
        if(photonView.isMine)
        {
            health = maxHealth;
            GetComponent<PlayerNetwork>().SetHealth(health);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    public void TakeDamage(float damage)
    {
        float totalDamage = (damage - (damage * defense));
        GetComponent<PlayerNetwork>().PlayerDamage(totalDamage);
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Got hit!");
        TakeDamage(20);
    }
}
