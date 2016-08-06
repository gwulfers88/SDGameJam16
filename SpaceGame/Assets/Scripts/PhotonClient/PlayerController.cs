using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Vector3 forwardV;
    [SerializeField]
    Vector3 position;
    [SerializeField]
    Vector3 velocity;
    [SerializeField, Range(10f, 20f)]
    float maxVelocity = 20f;
    [SerializeField]
    Vector3 accel;
    
	// Use this for initialization
	void Start () 
    {
        forwardV = this.transform.forward;
        position = this.transform.position;
        velocity = Vector3.zero;
        accel = Vector3.zero;
	}
	
    float Square(float value)
    {
        return value * value;
    }
	
	void Update () 
    {

        float hDir = Input.GetAxis("Horizontal");
        float vDir = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up, hDir * 5f);
        forwardV = transform.forward;

        position += forwardV * Time.deltaTime * (vDir * 10f);
        transform.position = position;

        //float dt = Time.deltaTime;
        //forwardV = this.transform.forward;

        //accel = Vector3.zero;

        //if(Input.GetKey(KeyCode.W))
        //{
        //    accel = this.transform.forward;
        //}
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    accel = -this.transform.forward;
        //}

        //float hDir = Input.GetAxis("Horizontal") * 10f;
        
        //if (hDir != 0)
        //{
        //    transform.Rotate(Vector3.up, hDir);
        //    accel = this.transform.forward;
        //}

        //accel *= 10f;

        //position = ((accel * 0.5f) * Square(dt)) + (velocity * dt) + position;
        //velocity = accel * dt + velocity;

        ////velocity.z = Mathf.Clamp(velocity.z, maxVelocity * 0.5f, maxVelocity);
        ////velocity.x = Mathf.Clamp(velocity.x, maxVelocity * 0.5f, maxVelocity);

        //this.transform.position = position;

        Debug.DrawLine(position, position + forwardV * 10f, Color.red);
	}
}
