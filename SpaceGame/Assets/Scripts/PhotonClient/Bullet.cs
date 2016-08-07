using UnityEngine;
using System.Collections;

public class Bullet : Photon.MonoBehaviour
{
    Vector3 position;
    Quaternion rotation;

    float timer;
    float deathDelay = 5f;
    float speed = 100f;
    float damage;
    string owner;

	// Use this for initialization
	void OnEnable ()
    {
        if (photonView.isMine)
        {
            position = transform.position;
            rotation = transform.rotation;

            timer = 0;
        }
	}
	
    void OnDisable()
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
        timer = 0;
    }

    public void SetOwner(string newOwner)
    {
        owner = newOwner;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo messageInfo)
    {
        if (stream.isWriting)
        {
            Debug.Log("My bullet");
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if(stream.isReading)
        {
            Debug.Log("Their bullet");
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {
            position += transform.forward * speed * Time.deltaTime;
            transform.position = position;

            timer += Time.deltaTime;

            if (timer >= deathDelay)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    void Destroy()
    {
        this.gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.collider.CompareTag("Enemy"))
        {
            Destroy();
            col.collider.gameObject.GetComponent<EnemyHealth>().Explode();
        }
    }
}
