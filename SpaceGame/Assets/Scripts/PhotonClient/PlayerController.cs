using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    Rigidbody rigidBody;
    float turnSpeed = 10.0f;
    float speed = 5.0f;
    float minSpeed = 5.0f;
    float maxSpeed = 20f;

    float timer;
    float shootDelay = .25f;

	// Use this for initialization
    void OnEnable()
    {
        timer = 0f;

        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody.useGravity)
            rigidBody.useGravity = false;
    }

	void Update () 
    {
        timer += Time.deltaTime;

        if(Input.GetButton("Jump") && timer >= shootDelay)
        {
            ObjectPool pool = GetComponent<ObjectPool>();
            pool.spawn(transform.position + transform.forward * 2f, transform.rotation);

            timer = 0;
        }

        float vDir = Input.GetAxis("Vertical");
        float hDir = Input.GetAxis("Horizontal");
        
        if(vDir != 0)
        {
            if(vDir > 0)
            {
                if (speed < maxSpeed)
                    speed++;
            }
            else
            {
                if (speed > minSpeed)
                    speed--;
            }
        }

        rigidBody.velocity = transform.forward * speed;
        transform.Rotate(Vector3.up, hDir, Space.World);
	}
}
