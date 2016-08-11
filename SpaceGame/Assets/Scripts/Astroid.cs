using UnityEngine;
using System.Collections;

public class Astroid : MonoBehaviour
{
    EnemyHealth _enemyHealth;
    void OnCollisionEnter(Collision other)
    {
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemyHealth.Explode();
        Debug.Log("Exploded!");
        Debug.Log("Asteroid hit");
        if(other.collider.tag == "Bullet")
        {
            _enemyHealth = GetComponent<EnemyHealth>();
            _enemyHealth.Explode();
            Debug.Log("Exploded!");
            Debug.Log("Asteroid hit");
        }
    }
}
