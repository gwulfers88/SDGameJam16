using UnityEngine;
using System.Collections;

public class Leader : Photon.MonoBehaviour
{
    string duty = "Leader";
    const float maxHealth = 150f;
    float defense = .35f;
    float health;

    // Use this for initialization
    void Start()
    {
        if (photonView.isMine)
        {
            health = maxHealth;
            GetComponent<PlayerNetwork>().SetHealth(health);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        float totalDamage = (damage - (damage * defense));
        GetComponent<PlayerNetwork>().PlayerDamage(totalDamage);
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("TEST: Got hit!");
        TakeDamage(20);
    }
}
