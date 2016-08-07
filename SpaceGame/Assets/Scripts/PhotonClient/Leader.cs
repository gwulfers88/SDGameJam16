using UnityEngine;
using System.Collections;

public class Leader : Photon.MonoBehaviour
{
    string duty = "Leader";
    const float maxHealth = 150f;
    float defense = .35f;
    float health;
    float attack;

    // Use this for initialization
    void Start()
    {
        if (photonView.isMine)
        {
            health = maxHealth;
            attack = 35;
            GetComponent<PlayerNetwork>().SetHealth(health);
            GetComponent<PlayerNetwork>().SetAttack(attack);
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

    public float DoDamage()
    {
        return attack;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.collider.CompareTag("Enemy"))
        {
            Debug.Log("TEST: Got hit!");
        }
    }
}
