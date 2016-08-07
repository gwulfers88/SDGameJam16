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

    public int GetPhotonID()
    {
        return PhotonNetwork.player.ID;
    }

    // Update is called once per frame
    void Update ()
    {
        if (photonView.isMine)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player.GetInstanceID() != this.GetInstanceID())
                {
                    Debug.Log("Player Found: " + player.name);
                    float dist = Vector3.Distance(player.transform.position, transform.position);
                    if(dist > 100.0f)
                    {
                        GameObject.Find("NetworkManager").GetPhotonView().RPC("AddMessage_RPC", PhotonNetwork.player.Get(player.GetComponent<PlayerNetwork>().GetPhotonID()), "WARNING: You are leaveing you team!!");
                    }
                }
            }
        }

	    //if(!photonView.isMine)
     //   {
     //       transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 5f);
     //       transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5f);
     //   }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo messageInfo)
    {
        int type = (int)GetComponent<PlayerController>().GetPlayerType();

        if (stream.isWriting)
        {
            Debug.Log("We own you!");
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(health);
            stream.SendNext(GameObject.Find("NetworkManager").GetComponent<NetworkManager>().txtTeam[type - 1].text);
        }
        else if (stream.isReading)
        {
            Debug.Log("They own you!");
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
            health = (float)stream.ReceiveNext();
            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().txtTeam[type - 1].text = (string)stream.ReceiveNext();

            SendNetworkMessage(health.ToString() + " " + type);

            //if (GameObject.Find("NetworkManager").GetComponent<NetworkManager>().txtTeam[type - 1].enabled == false)
            //{
            //    GameObject.Find("NetworkManager").GetComponent<NetworkManager>().txtTeam[type - 1].enabled = true;    
            //}
            //else
            //{
            //    GameObject.Find("NetworkManager").GetComponent<NetworkManager>().txtTeam[type - 1].text = PhotonNetwork.player.name + " " + health;
            //}
        }
    }

    public void SetHealth(float health)
    {
        this.health = health;
    }

    public float GetHealth()
    {
        return health;
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

            SendNetworkMessage("BATTLELOG: " + PhotonNetwork.player.name + " took " + damage + " damage!");
        }
    }

}
