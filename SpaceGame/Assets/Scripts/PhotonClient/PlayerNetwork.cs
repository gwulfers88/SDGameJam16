using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using System.Collections.Generic;

// NETWORKING FUNCTIONALITY HERE!!!
[RequireComponent(typeof(PhotonView))]
public class PlayerNetwork : Photon.MonoBehaviour
{
    float health = 100;
    bool initialLoad = true;
    private Vector3 position;
    private Quaternion rotation;
    private float smoothing = 10000f;

    public delegate void Respawn(float time);
    public event Respawn RespawnMe;
    public delegate void SendMessage(string messageOverlay);
    public event SendMessage SendNetworkMessage;

    // Use this for initialization
    void Start ()
    {
        if (photonView.isMine)
        {
            GetComponent<PlayerController>().enabled = true;

            foreach (Camera cam in GetComponentsInChildren<Camera>())
            {
                cam.enabled = true;
                Debug.Log("Got The Camera " + cam);
            }
        }
        else
        {
            GetComponent<PlayerController>().enabled = false;

            foreach (Camera cam in GetComponentsInChildren<Camera>())
            {
                cam.enabled = false;
                Debug.Log("Not my Camera " + cam);
            }
        }
	}

    IEnumerator UpdateData()
    {
        if (initialLoad)
        {
            initialLoad = false;

            transform.position = position;
            transform.rotation = rotation;
        }

        if(!photonView.isMine)//while (true)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
        }

        yield return null;
    }

    // Update is called once per frame
    void Update ()
    {
	    //if(!photonView.isMine)
     //   {
     //       transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 5f);
     //       transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5f);
     //   }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo messageInfo)
    {
        if (stream.isWriting)
        {
            Debug.Log("We own you!");
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(health);
            
        }
        else
        {
            Debug.Log("They own you!");
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
            health = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void PlayerDamage(float damage)
    {
        if (photonView.isMine)
        {
            health -= damage;
            if (health <= 0 && photonView.isMine)
            {
                if (SendNetworkMessage != null)
                {
                    SendNetworkMessage(PhotonNetwork.player.name + " was killed.");
                }
                if (RespawnMe != null)
                {
                    RespawnMe(3f);
                }

                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

}
