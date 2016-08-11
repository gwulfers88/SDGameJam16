using UnityEngine;
using System.Collections;

public class EnemyBullet : Photon.MonoBehaviour
{

    Vector3 position;
    Quaternion rotation;

    float timer;
    float deathDelay = 5f;
    float speed = 100f;

    // Use this for initialization
    void OnEnable()
    {
        //if (photonView.isMine)
        //{
        position = transform.position;
        rotation = transform.rotation;

        timer = 0;
        //}
    }

    void OnDisable()
    {

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo messageInfo)
    {
        if (stream.isWriting)
        {
            Debug.Log("My bullet");
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.isReading)
        {
            Debug.Log("Their bullet");
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (photonView.isMine)
        //{
        position += transform.forward * speed * Time.deltaTime;
        transform.position = position;

        timer += Time.deltaTime;

        if (timer >= deathDelay)
        {
            this.gameObject.SetActive(false);
        }
        //}
    }
}
