using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    Vector3 position;
    float timer;
    float deathDelay = 5f;
    float speed = 100f;

	// Use this for initialization
	void OnEnable ()
    {
        position = transform.position;
        timer = 0;
	}
	
    void OnDisable()
    {

    }

	// Update is called once per frame
	void Update ()
    {
        position += transform.forward * speed * Time.deltaTime;
        transform.position = position;

        timer += Time.deltaTime;

        if(timer >= deathDelay)
        {
            this.gameObject.SetActive(false);
        }
	}
}
