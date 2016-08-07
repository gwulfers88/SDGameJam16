using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class PlayerController : Photon.MonoBehaviour
{
    public enum Type { None = 0, Leader, Healer, Defender, Heavy };

    Vector3 position;
    Quaternion rotation;
    public Type playerType;

    Rigidbody rigidBody;
    float turnSpeed = 10.0f;
    float speed = 5.0f;
    float minSpeed = 5.0f;
    float maxSpeed = 20f;

    float timer;
    float shootDelay = .25f;
    ObjectPool pool;
    GameObject debugSphere;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        pool = GetComponent<ObjectPool>();

        if (photonView.isMine)
        {
            timer = 0f;
            turnSpeed = 5.0f;
            speed = 5.0f;
            minSpeed = 5.0f;
            maxSpeed = 20.0f;
        }

        if (rigidBody.useGravity)
            rigidBody.useGravity = false;

        //debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //debugSphere.transform.localScale = new Vector3(10f, 10f, 10f);
        //debugSphere.transform.position = this.transform.position;
        //debugSphere.transform.SetParent(this.transform);
        //debugSphere.GetComponent<SphereCollider>().isTrigger = true;
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void Update()
    {
        if (photonView.isMine)
        {
            switch (playerType)
            {
                case Type.None:
                    Debug.Log("You are nothing yet");
                    break;

                case Type.Leader:
                    Debug.Log("You are The leader");
                    break;

                case Type.Healer:
                    Debug.Log("You are The Healer");
                    break;

                case Type.Heavy:
                    Debug.Log("You are Heavy");
                    break;

                case Type.Defender:
                    Debug.Log("You are the Defender");
                    break;
            }
                

            if (PhotonNetwork.connected)
                Debug.Log("Connected!");
            else
                Debug.Log("Not Connected!");

            timer += Time.deltaTime;

            if (Input.GetButton("Jump") && timer >= shootDelay)
            {
                Debug.Log("Player with ID " + PhotonNetwork.player.ID);
                pool.spawn(transform.position + transform.forward * 2f, transform.rotation);

                timer = 0;
            }

            float vDir = Input.GetAxis("Vertical");
            float hDir = Input.GetAxis("Horizontal");

            float mouseVert = Input.GetAxis("Mouse Y");
            float mouseHor = Input.GetAxis("Mouse X");

            if (vDir != 0)
            {
                if (vDir > 0)
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
            transform.Rotate(Vector3.right, mouseVert, Space.Self);
            transform.Rotate(Vector3.forward, -mouseHor, Space.Self);
        }
        
    }
    
    public Type GetPlayerType()
    {
        return playerType;
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(transform.position, 100.0f);
    //}
}
